using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Ganyu.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Ganyu.Scripts.Utils;

public static class GanyuElementUtils
{
    public static PlayerChoiceContext? CurrentContext;
    public static int ReactionsThisTurn = 0;
    public static int TotalReactionsThisCombat = 0;
    public static int SwirlsThisTurn = 0;// 新增：追踪本回合触发的扩散次数
    public static bool HasRetainElement = false;
    private static async Task<decimal> GetReactionMultiplier(Creature source)
    {
        decimal multiplier = 1m;

        // 1. 检查永久能力：元素溢出 (每层增加 100% 基础效果)
        if (source.GetPower<ElementalOverflowPower>() is { } overflow && overflow.Amount > 0)
        {
            multiplier += overflow.Amount;
        }

        // 2. 检查消耗性Buff：反应预载 (Overload Prep)
        // 注意：如果两者同时存在，此处逻辑为在溢出基础上再翻倍
        if (source.GetPower<OverloadPrepPower>() is { } prep && prep.Amount > 0)
        {
            await PowerCmd.TickDownDuration(prep);
            multiplier *= 2m;
        }

        return multiplier;
    }

    public static async Task ExecuteReaction(PlayerChoiceContext context, Func<Task> action)
    {
        CurrentContext = context;
        try
        {
            await action();
        }
        finally
        {
            CurrentContext = null;
        }
    }

    public static void ResetReactionCount()
    {
        TotalReactionsThisCombat = 0;
        SwirlsThisTurn = 0; // 同时重置
        HasRetainElement = false;
    }
    public static void ResetTurnCount()
    {
        SwirlsThisTurn = 0;
        ReactionsThisTurn = 0; // 新增：重置本回合的反应次数
    }

    // --- 核心反应处理：冰元素 (Ice) ---
    public static async Task ApplyIceReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies = null, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;
        await ApplyIceCharge(source, remaining);

        // 优先级 1. 遇风扩散 (不再检查 SwirlPower，直接检查 WindPower)
        if (remaining > 0 && target.GetPower<WindPower>() is { } winp && winp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, winp.Amount);
            await ConsumeChargesWithRetention(target, source, winp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies); // 触发计数
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "ICE");
            remaining -= rc;
        }

        // 优先级 2. 融化 (与 FlamePower 反应)
        if (remaining > 0 && target.GetPower<FlamePower>() is { } fp && fp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, fp.Amount);
            await ConsumeChargesWithRetention(target, source, fp, rc);
            await TriggerMelt(target, source, rc);
            remaining -= rc;
        }

        // 优先级 3. 结冰 (与 WetPower 反应)
        if (remaining > 0 && target.GetPower<WetPower>() is { } wp && wp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, wp.Amount);
            await ConsumeChargesWithRetention(target, source, wp, rc);
            await TriggerFrozen(target, source, rc);
            remaining -= rc;
        }

        // 优先级 4. 结晶 (与 RockPower 反应)
        if (remaining > 0 && target.GetPower<RockPower>() is { } rp && rp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, rp.Amount);
            await ConsumeChargesWithRetention(target, source, rp, rc);
            await TriggerCrystallize(target, source, rc * 2);
            remaining -= rc;
        }

        // 优先级 5. 超导 (与 ElectroPower 反应)
        if (remaining > 0 && target.GetPower<ElectroPower>() is { } ep && ep.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ep.Amount);
            await ConsumeChargesWithRetention(target, source, ep, rc);
            await TriggerConduct(target, source, rc, hittableEnemies);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<IcePower>(CurrentContext, target, remaining, source, null);
        }
    }

    // --- 水元素处理 (Water) ---
    public static async Task ApplyWaterReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;
        await ApplyWaterCharge(source, remaining);
        // 遇风扩散
        if (remaining > 0 && target.GetPower<WindPower>() is { } winp && winp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, winp.Amount);
            await ConsumeChargesWithRetention(target, source, winp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "WATER");
            remaining -= rc;
        }

        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerFrozen(target, source, rc);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<WetPower>(CurrentContext, target, remaining, source, null);
        }
    }

    // --- 火元素处理 (Fire) ---
    public static async Task ApplyFireReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;
        await ApplyFireCharge(source, remaining);
        // 【新增】触发“烈焰绽放”：检查来源者是否有该能力，如果有则触发群体伤害
        if (source?.GetPower<BloomingBlazeflowerPower>() is { } bloomPower)
        {
            await bloomPower.Trigger();
        }


        // 遇风扩散
        if (remaining > 0 && target.GetPower<WindPower>() is { } winp && winp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, winp.Amount);
            await ConsumeChargesWithRetention(target, source, winp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "FIRE");
            remaining -= rc;
        }

        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerMelt(target, source, rc);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<FlamePower>(CurrentContext, target, remaining, source, null);
        }
    }

    // --- 雷元素处理 (Electro) ---
    public static async Task ApplyElectroReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;
        await ApplyElectroCharge(source, remaining);

        // 遇风扩散
        if (remaining > 0 && target.GetPower<WindPower>() is { } winp && winp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, winp.Amount);
            await ConsumeChargesWithRetention(target, source, winp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "ELECTRO");
            remaining -= rc;
        }

        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerConduct(target, source, rc, hittableEnemies);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<ElectroPower>(CurrentContext, target, remaining, source, null);
        }
    }

    // --- 风元素处理 (Wind) ---
    public static async Task ApplyWindReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;
        await ApplyWindCharge(source, remaining);

        // 风遇到任何元素直接产生该元素的扩散
        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "ICE");
            remaining -= rc;
        }
        else if (remaining > 0 && target.GetPower<WetPower>() is { } wp && wp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, wp.Amount);
            await ConsumeChargesWithRetention(target, source, wp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "WATER");
            remaining -= rc;
        }
        else if (remaining > 0 && target.GetPower<FlamePower>() is { } fp && fp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, fp.Amount);
            await ConsumeChargesWithRetention(target, source, fp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "FIRE");
            remaining -= rc;
        }
        else if (remaining > 0 && target.GetPower<ElectroPower>() is { } ep && ep.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ep.Amount);
            await ConsumeChargesWithRetention(target, source, ep, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "ELECTRO");
            remaining -= rc;
        }
        else if (remaining > 0 && target.GetPower<RockPower>() is { } rp && rp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, rp.Amount);
            await ConsumeChargesWithRetention(target, source, rp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "ROCK");
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<WindPower>(CurrentContext, target, remaining, source, null);
        }
    }

    // --- 岩元素处理 (Rock) ---
    public static async Task ApplyRockReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;
        await ApplyRockCharge(source, remaining);
        // 【新增】触发“孤云峰岩”：检查来源者是否有该能力，如果有则触发格挡
        if (source?.GetPower<PeaksOfGuyunPower>() is { } peaksPower)
        {
            await peaksPower.Trigger();
        }
        // 【新增】触发“地脉震颤”：检查来源者是否有该能力，如果有则对全体敌人造成伤害
        if (source?.GetPower<LeylineTremorPower>() is { } tremorPower)
        {
            // 将 hittableEnemies 传递过去，避免在 Power 内部硬找目标
            await tremorPower.Trigger(hittableEnemies);
        }
        // 遇风扩散
        if (remaining > 0 && target.GetPower<WindPower>() is { } winp && winp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, winp.Amount);
            await ConsumeChargesWithRetention(target, source, winp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "ROCK");
            remaining -= rc;
        }

        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerCrystallize(target, source, rc * 2);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<RockPower>(CurrentContext, target, remaining, source, null);
        }
    }

    // --- 通用扩散逻辑 (TriggerSwirlElement) ---
    public static async Task TriggerSwirlElement(Creature target, Creature source, decimal amount, IReadOnlyList<Creature>? hittableEnemies, string elementType)
    {
        if (hittableEnemies == null) return;

        foreach (Creature enemy in hittableEnemies)
        {
            if (enemy.IsAlive) // 不反馈给触发源目标
            {
                switch (elementType)
                {
                    case "ICE": await ApplyIceReaction(enemy, source, hittableEnemies, amount); break;
                    case "WATER": await ApplyWaterReaction(enemy, source, hittableEnemies, amount); break;
                    case "FIRE": await ApplyFireReaction(enemy, source, hittableEnemies, amount); break;
                    case "ELECTRO": await ApplyElectroReaction(enemy, source, hittableEnemies, amount); break;
                    case "WIND": await ApplyWindReaction(enemy, source, hittableEnemies, amount); break;
                    case "ROCK": await ApplyRockReaction(enemy, source, hittableEnemies, amount); break;
                }
            }
        }
    }

    // --- 私有反应指令执行 ---

    public static async Task TriggerFrozen(Creature target, Creature source, decimal count)
    {
        decimal multiplier = await GetReactionMultiplier(source);
        await PowerCmd.Apply<FreezingDebuffPower>(CurrentContext, target, count * multiplier, source, null);
        await NotifyReaction(source, count * multiplier);
    }

    public static async Task TriggerMelt(Creature target, Creature source, decimal count)
    {
        // 获取当前角色的反应倍率（例如是否有“元素溢出”能力）
        decimal multiplier = await GetReactionMultiplier(source);

        // 修改处：不再施加 MeltPower，改为直接造成伤害
        // 伤害计算逻辑：基础 8 点 * 反应层数 * 反应倍率
        if (CurrentContext != null)
        {
            await CreatureCmd.Damage(
                CurrentContext,
                target,
                8m * count * multiplier,
                ValueProp.Unpowered, // 使用 Unpowered 属性，表示该伤害不受力量/虚弱等 Buff 影响
                source,
                null
            );
        }

        // 依然触发反应通知，以确保“不竭岁收”或“政务精简”等能力正常触发
        await NotifyReaction(source, count * multiplier);
    }

    public static async Task TriggerCrystallize(Creature target, Creature source, decimal count)
    {
        decimal multiplier = await GetReactionMultiplier(source);

        // 检查玩家身上是否有“坚如磐石”能力
        var solidAsRockPower = source.GetPower<SolidAsRockPower>();
        if (solidAsRockPower != null)
        {
            // 通知能力：结晶已触发，本回合请保留护盾
            solidAsRockPower.TriggerCrystalize();
        }

        // 【新增】玉璋护盾逻辑：如果是结晶反应且拥有该BUFF，效果翻倍并消耗一层
        if (source.GetPower<JadeShieldPower>() is { } jade && jade.Amount > 0)
        {
            multiplier *= 2m;
            await PowerCmd.TickDownDuration(jade);
        }

        await CreatureCmd.GainBlock(source, 2m * count * multiplier, ValueProp.Unpowered, null);

        await NotifyReaction(source, count * multiplier / 2);
    }

    public static async Task TriggerSwirl(Creature target, Creature source, decimal count, IReadOnlyList<Creature>? hittableEnemies)
    {
        decimal multiplier = await GetReactionMultiplier(source);
        SwirlsThisTurn += (int)count; // 记录扩散次数
        // 【新增】检查玩家身上是否有“顺风而行”能力
        if (source?.GetPower<RideTheWindPower>() is { } rideTheWind)
        {
            // 根据扩散反应发生的层数，触发对应次数的减费
            for (int i = 0; i < (int)count; i++)
            {
                await rideTheWind.TriggerSwirl();
            }
        }
        await NotifyReaction(source, count * multiplier);
    }

    public static async Task TriggerConduct(Creature target, Creature source, decimal count, IReadOnlyList<Creature>? hittableEnemies)
    {
        decimal multiplier = await GetReactionMultiplier(source);
        if (hittableEnemies == null) return;
        foreach (Creature enemy in hittableEnemies)
        {
            if (enemy.IsAlive)
            {
                await PowerCmd.Apply<ConductPower>(CurrentContext, enemy, count * multiplier, source, null);
            }
        }
        await NotifyReaction(source, count * multiplier);
    }

    private static async Task NotifyReaction(Creature source, decimal count = 1m)
    {
        TotalReactionsThisCombat += (int)count;
        ReactionsThisTurn += (int)count; // 新增：记录本回合发生的反应
        if (source?.GetPower<RevenuePower>() is { } rp)
        {
            for (int i = 0; i < (int)count; i++)
            {
                await rp.TriggerReaction();
            }
        }
        if (source?.GetPower<EfficiencyPower>() is { } ep)
        {
            for (int i = 0; i < (int)count; i++)
            {
                await ep.TriggerReaction();
            }
        }
        if (source?.GetPower<ChainReactionPower>() is { } crp)
        {
            for (int i = 0; i < (int)count; i++)
            {
                await crp.TriggerReaction();
            }
        }
        if (source?.GetPower<AdministrativeStreamliningPower>() is { } asp)
        {
            for (int i = 0; i < (int)count; i++)
            {
                await asp.TriggerReaction();
            }
        }
    }

    private static async Task ConsumeChargesWithRetention(Creature target, Creature source, CustomPowerModel elementPower, decimal count)
    {
        for (int i = 0; i < count; i++)
        {
            await PowerCmd.TickDownDuration(elementPower);
        }
    }
    public static async Task ApplyIceCharge(Creature source, decimal amount)
    {
        if (source == null || amount <= 0) return;

        // 【新增】检查冰元素精通Buff：每次获得充能时额外增加对应的层数
        if (source.GetPower<RetainElementPower>() is { } retainPower && retainPower.Amount > 0)
        {
            amount += retainPower.Amount;
        }

        decimal oldAmount = source.GetPower<IceChargePower>()?.Amount ?? 0m;
        decimal total = oldAmount + amount;

        // 1. 计算触发“10层循环”的次数
        int num10s = (int)(Math.Floor(total / 10m) - Math.Floor(oldAmount / 10m));

        // 2. 计算触发“5层阶段”的次数
        int num5s = (int)(Math.Floor(total / 10m + (total % 10m >= 5m ? 1m : 0m))
                       - Math.Floor(oldAmount / 10m + (oldAmount % 10m >= 5m ? 1m : 0m)));

        // 3. 发放奖励
        if (num10s > 0)
            await PowerCmd.Apply<HeavenlyFallBuffPower>(CurrentContext, source, num10s * 2m, source, null);

        if (num5s > 0)
            await PowerCmd.Apply<TracesQilinPower>(CurrentContext, source, num5s * 3m, source, null);

        // 4. 更新充能层数
        decimal remainder = total % 10m;
        var existingPower = source.GetPower<IceChargePower>();
        if (existingPower != null) await PowerCmd.Remove(existingPower);

        if (remainder > 0)
            await PowerCmd.Apply<IceChargePower>(CurrentContext, source, remainder, source, null);
    }
    // 在 GanyuElementUtils 类中添加以下新方法：
    public static async Task ApplyWaterCharge(Creature source, decimal amount)
    {
        if (source == null || amount <= 0) return;
        // 【新增】检查水元素精通Buff：每次获得充能时额外增加对应的层数
        if (source.GetPower<HydroMasteryPower>() is { } hydroMastery && hydroMastery.Amount > 0)
        {
            amount += hydroMastery.Amount;
        }
        // 【新增】检查并触发元素共生
        if (source.GetPower<ElementalSymbiosisPower>() is { } symbiosis)
        {
            await symbiosis.Trigger();
        }

        decimal oldAmount = source.GetPower<WaterChargePower>()?.Amount ?? 0m;
        decimal total = oldAmount + amount;

        // 计算触发次数
        int num10s = (int)(Math.Floor(total / 10m) - Math.Floor(oldAmount / 10m));
        int num5s = (int)(Math.Floor(total / 10m + (total % 10m >= 5m ? 1m : 0m))
                       - Math.Floor(oldAmount / 10m + (oldAmount % 10m >= 5m ? 1m : 0m)));

        // 达到10层：给予 1 层海人化羽
        if (num10s > 0)
            await PowerCmd.Apply<OceanborneFeatherPower>(CurrentContext, source, num10s * 1m, source, null);

        // 达到5层：给予 2 层海月之誓
        if (num5s > 0)
            await PowerCmd.Apply<OathSilveryMoonPower>(CurrentContext, source, num5s * 2m, source, null);

        // 更新充能层数 (满10清零并保留余数)
        decimal remainder = total % 10m;
        var existingPower = source.GetPower<WaterChargePower>();
        if (existingPower != null) await PowerCmd.Remove(existingPower);

        if (remainder > 0)
            await PowerCmd.Apply<WaterChargePower>(CurrentContext, source, remainder, source, null);
    }
    public static async Task ApplyFireCharge(Creature source, decimal amount)
    {
        if (source == null || amount <= 0) return;
        // 【新增】检查火元素精通Buff：每次获得充能时额外增加对应的层数
        if (source.GetPower<PyroMasteryPower>() is { } pyroMastery && pyroMastery.Amount > 0)
        {
            amount += pyroMastery.Amount;
        }
        // 【新增】检查并触发元素共生
        if (source.GetPower<ElementalSymbiosisPower>() is { } symbiosis)
        {
            await symbiosis.Trigger();
        }

        decimal oldAmount = source.GetPower<FireChargePower>()?.Amount ?? 0m;
        decimal total = oldAmount + amount;

        // 计算触发次数
        int num10s = (int)(Math.Floor(total / 10m) - Math.Floor(oldAmount / 10m));
        int num5s = (int)(Math.Floor(total / 10m + (total % 10m >= 5m ? 1m : 0m))
                       - Math.Floor(oldAmount / 10m + (oldAmount % 10m >= 5m ? 1m : 0m)));

        // 达到10层：给予 2 层旋火轮
        if (num10s > 0)
            await PowerCmd.Apply<PyronadoPower>(CurrentContext, source, num10s * 2m, source, null);

        // 达到5层：给予 2 层锅巴出击
        if (num5s > 0)
            await PowerCmd.Apply<GuobaAttackPower>(CurrentContext, source, num5s * 2m, source, null);

        // 更新充能层数 (满10清零并保留余数)
        decimal remainder = total % 10m;
        var existingPower = source.GetPower<FireChargePower>();
        if (existingPower != null) await PowerCmd.Remove(existingPower);

        if (remainder > 0)
            await PowerCmd.Apply<FireChargePower>(CurrentContext, source, remainder, source, null);
    }
    public static async Task ApplyElectroCharge(Creature source, decimal amount)
    {
        if (source == null || amount <= 0) return;
        // 【新增】检查雷元素精通Buff：每次获得充能时额外增加对应的层数
        if (source.GetPower<ElectroMasteryPower>() is { } electroMastery && electroMastery.Amount > 0)
        {
            amount += electroMastery.Amount;
        }
        // 【新增】检查并触发元素共生
        if (source.GetPower<ElementalSymbiosisPower>() is { } symbiosis)
        {
            await symbiosis.Trigger();
        }

        decimal oldAmount = source.GetPower<ElectroChargePower>()?.Amount ?? 0m;
        decimal total = oldAmount + amount;

        // 计算触发次数
        int num10s = (int)(Math.Floor(total / 10m) - Math.Floor(oldAmount / 10m));
        int num5s = (int)(Math.Floor(total / 10m + (total % 10m >= 5m ? 1m : 0m))
                       - Math.Floor(oldAmount / 10m + (oldAmount % 10m >= 5m ? 1m : 0m)));

        // 达到10层：给予 1 层奥义·梦想真说
        if (num10s > 0)
            await PowerCmd.Apply<MusouShinsetsuPower>(CurrentContext, source, num10s * 1m, source, null);

        // 达到5层：给予 2 层神变·恶曜开眼
        if (num5s > 0)
            await PowerCmd.Apply<BalefulOmenPower>(CurrentContext, source, num5s * 2m, source, null);

        // 更新充能层数 (满10清零并保留余数)
        decimal remainder = total % 10m;
        var existingPower = source.GetPower<ElectroChargePower>();
        if (existingPower != null) await PowerCmd.Remove(existingPower);

        if (remainder > 0)
            await PowerCmd.Apply<ElectroChargePower>(CurrentContext, source, remainder, source, null);
    }
    public static async Task ApplyRockCharge(Creature source, decimal amount)
    {
        if (source == null || amount <= 0) return;
        // 【新增】检查岩元素精通Buff：每次获得充能时额外增加对应的层数
        if (source.GetPower<GeoMasteryPower>() is { } geoMastery && geoMastery.Amount > 0)
        {
            amount += geoMastery.Amount;
        }
        // 【新增】检查并触发元素共生
        if (source.GetPower<ElementalSymbiosisPower>() is { } symbiosis)
        {
            await symbiosis.Trigger();
        }

        decimal oldAmount = source.GetPower<RockChargePower>()?.Amount ?? 0m;
        decimal total = oldAmount + amount;

        int num10s = (int)(Math.Floor(total / 10m) - Math.Floor(oldAmount / 10m));
        int num5s = (int)(Math.Floor(total / 10m + (total % 10m >= 5m ? 1m : 0m))
                       - Math.Floor(oldAmount / 10m + (oldAmount % 10m >= 5m ? 1m : 0m)));

        // 达到10层：给予 1 层天星
        if (num10s > 0)
            await PowerCmd.Apply<StarfallPower>(CurrentContext, source, num10s * 1m, source, null);

        // 达到5层：给予 2 层玉璋护盾
        if (num5s > 0)
            await PowerCmd.Apply<JadeShieldPower>(CurrentContext, source, num5s * 2m, source, null);

        decimal remainder = total % 10m;
        var existingPower = source.GetPower<RockChargePower>();
        if (existingPower != null) await PowerCmd.Remove(existingPower);

        if (remainder > 0)
            await PowerCmd.Apply<RockChargePower>(CurrentContext, source, remainder, source, null);
    }
    public static async Task ApplyWindCharge(Creature source, decimal amount)
    {
        if (source == null || amount <= 0) return;
        // 【新增】检查风元素精通Buff：每次获得充能时额外增加对应的层数
        if (source.GetPower<AnemoMasteryPower>() is { } anemoMastery && anemoMastery.Amount > 0)
        {
            amount += anemoMastery.Amount;
        }
        // 【新增】检查并触发元素共生
        if (source.GetPower<ElementalSymbiosisPower>() is { } symbiosis)
        {
            await symbiosis.Trigger();
        }

        decimal oldAmount = source.GetPower<WindChargePower>()?.Amount ?? 0m;
        decimal total = oldAmount + amount;

        int num10s = (int)(Math.Floor(total / 10m) - Math.Floor(oldAmount / 10m));
        int num5s = (int)(Math.Floor(total / 10m + (total % 10m >= 5m ? 1m : 0m))
                       - Math.Floor(oldAmount / 10m + (oldAmount % 10m >= 5m ? 1m : 0m)));

        // 达到10层：给予 3 层禁·风灵作成·柒伍同构贰型
        if (num10s > 0)
            await PowerCmd.Apply<ForbiddenWindSpirit75Power>(CurrentContext, source, num10s * 3m, source, null);

        // 达到5层：给予 1 层风灵作成·陆叁零捌
        if (num5s > 0)
            await PowerCmd.Apply<WindSpirit6308Power>(CurrentContext, source, num5s * 1m, source, null);

        decimal remainder = total % 10m;
        var existingPower = source.GetPower<WindChargePower>();
        if (existingPower != null) await PowerCmd.Remove(existingPower);

        if (remainder > 0)
            await PowerCmd.Apply<WindChargePower>(CurrentContext, source, remainder, source, null);
    }
}