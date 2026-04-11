using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Ganyu.Scripts.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Ganyu.Scripts.Utils;

public static class GanyuElementUtils
{
    // 本场战斗的反应计数，供 ElementalStorm 等卡牌使用
    public static PlayerChoiceContext? CurrentContext;

    // 2. 新增一个“执行器”，用来在卡牌 OnPlay 中包裹反应调用
    public static async Task ExecuteReaction(PlayerChoiceContext context, Func<Task> action)
    {
        CurrentContext = context;
        try
        {
            await action(); // 执行原本的反应逻辑
        }
        finally
        {
            CurrentContext = null; // 执行完清理
        }
    }
    public static int TotalReactionsThisCombat = 0;
    // 新增：判断玩家是否有“冰元素精通”
    public static bool HasRetainElement = false;

    // 每一场战斗开始时调用，重置计数
    public static void ResetReactionCount()
    {
        TotalReactionsThisCombat = 0;
        HasRetainElement = false; // 重置标记
    }

    // --- 核心反应处理：冰元素 (Ice) ---
    // 只有冰元素可以和其他元素反应
    public static async Task ApplyIceReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies = null, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;

        // 优先级 1. 扩散标记 (SwirlPower)
        if (remaining > 0 && target.GetPower<SwirlPower>() is { } sp && sp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, sp.Amount);
            await ConsumeChargesWithRetention(target, source, sp, rc);
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

        // 优先级 4. 扩散 (与 WindPower 反应)
        if (remaining > 0 && target.GetPower<WindPower>() is { } winp && winp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, winp.Amount);
            await ConsumeChargesWithRetention(target, source, winp, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            remaining -= rc;
        }

        // 优先级 5. 结晶 (与 RockPower 反应)
        if (remaining > 0 && target.GetPower<RockPower>() is { } rp && rp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, rp.Amount);
            await ConsumeChargesWithRetention(target, source, rp, rc);
            await TriggerCrystallize(target, source, rc * 2);
            remaining -= rc;
        }

        // 优先级 6. 超导 (与 ElectroPower 反应)
        if (remaining > 0 && target.GetPower<ElectroPower>() is { } ep && ep.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ep.Amount);
            await ConsumeChargesWithRetention(target, source, ep, rc);
            await TriggerConduct(target, source, rc, hittableEnemies);
            remaining -= rc;
        }

        // 最终：如果上述反应触发完后仍有剩余，则附着为残留冰元素
        if (remaining > 0)
        {
            await PowerCmd.Apply<IcePower>(target, remaining, source, null);
        }
    }

    // --- 水元素处理 (Water) ---
    public static async Task ApplyWaterReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;

        // 扩散标记判定
        if (remaining > 0 && target.GetPower<SwirlPower>() is { } sp && sp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, sp.Amount);
            await ConsumeChargesWithRetention(target, source, sp, rc);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "WATER");
            remaining -= rc;
        }

        // 仅与冰元素反应
        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerFrozen(target, source, rc);
            remaining -= rc;
        }

        // 其它元素共存不反应
        if (remaining > 0)
        {
            await PowerCmd.Apply<WetPower>(target, remaining, source, null);
        }
    }

    // --- 火元素处理 (Fire) ---
    public static async Task ApplyFireReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;

        if (remaining > 0 && target.GetPower<SwirlPower>() is { } sp && sp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, sp.Amount);
            await ConsumeChargesWithRetention(target, source, sp, rc);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "FIRE");
            remaining -= rc;
        }

        // 仅与冰元素反应
        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerMelt(target, source, rc);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<FlamePower>(target, remaining, source, null);
        }
    }

    // --- 雷元素处理 (Electro) ---
    public static async Task ApplyElectroReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;

        if (remaining > 0 && target.GetPower<SwirlPower>() is { } sp && sp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, sp.Amount);
            await ConsumeChargesWithRetention(target, source, sp, rc);
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
            await PowerCmd.Apply<ElectroPower>(target, remaining, source, null);
        }
    }

    // --- 风元素处理 (Wind) ---
    public static async Task ApplyWindReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;

        if (remaining > 0 && target.GetPower<SwirlPower>() is { } sp && sp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, sp.Amount);
            await ConsumeChargesWithRetention(target, source, sp, rc);
            await TriggerSwirlElement(target, source, rc, hittableEnemies, "WIND");
            remaining -= rc;
        }

        if (remaining > 0 && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
        {
            decimal rc = Math.Min(remaining, ip.Amount);
            await ConsumeChargesWithRetention(target, source, ip, rc);
            await TriggerSwirl(target, source, rc, hittableEnemies);
            remaining -= rc;
        }

        if (remaining > 0)
        {
            await PowerCmd.Apply<WindPower>(target, remaining, source, null);
        }
    }

    // --- 岩元素处理 (Rock) ---
    public static async Task ApplyRockReaction(Creature target, Creature source, IReadOnlyList<Creature>? hittableEnemies, decimal amount = 1m)
    {
        if (amount <= 0) return;
        decimal remaining = amount;

        if (remaining > 0 && target.GetPower<SwirlPower>() is { } sp && sp.Amount > 0)
        {
            decimal rc = Math.Min(remaining, sp.Amount);
            await ConsumeChargesWithRetention(target, source, sp, rc);
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
            await PowerCmd.Apply<RockPower>(target, remaining, source, null);
        }
    }

    // --- 通用扩散逻辑 (TriggerSwirlElement) ---
    private static async Task TriggerSwirlElement(Creature target, Creature source, decimal amount, IReadOnlyList<Creature>? hittableEnemies, string elementType)
    {
        if (hittableEnemies == null) return;

        foreach (Creature enemy in hittableEnemies)
        {
            // 为防止自身死循环，扩散不反馈给原目标
            if (enemy.IsAlive)
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

    private static async Task TriggerFrozen(Creature target, Creature source, decimal count)
    {
        await PowerCmd.Apply<FreezingDebuffPower>(target, count, source, null);
        await NotifyReaction(source, count);
    }

    private static async Task TriggerMelt(Creature target, Creature source, decimal count)
    {
        await PowerCmd.Apply<MeltPower>(target, count, source, null);
        await NotifyReaction(source, count);
    }

    private static async Task TriggerCrystallize(Creature target, Creature source, decimal count)
    {   
        await PowerCmd.Apply<CrystalizePower>(target, count, source, null);
        await NotifyReaction(source, count/2);
    }

    private static async Task TriggerSwirl(Creature target, Creature source, decimal count, IReadOnlyList<Creature>? hittableEnemies)
    {
        await PowerCmd.Apply<SwirlPower>(target, count, source, null);
        await NotifyReaction(source, count);
    }

    private static async Task TriggerConduct(Creature target, Creature source, decimal count, IReadOnlyList<Creature>? hittableEnemies)
    {
        if (hittableEnemies == null) return;
        foreach (Creature enemy in hittableEnemies)
        {
            if (enemy.IsAlive)
            {
                await PowerCmd.Apply<ConductPower>(enemy, count, source, null);
            }
        }
        await NotifyReaction(source, count);
    }

    private static async Task NotifyReaction(Creature source, decimal count = 1m)
    {
        TotalReactionsThisCombat += (int)count;
        if (source?.GetPower<RevenuePower>() is { } rp)
        {
            for (int i = 0; i < (int)count; i++)
            {
                await rp.TriggerReaction();
            }
        }
        // 新增：处理“高效办公” (EfficiencyPower)
        if (source?.GetPower<EfficiencyPower>() is { } ep)
        {
            for (int i = 0; i < (int)count; i++)
            {
                await ep.TriggerReaction();
            }
        }

    }
    private static async Task ConsumeChargesWithRetention(Creature target, Creature source, CustomPowerModel elementPower, decimal count)
    {

        // 执行实际需要的层数扣除，并使用 await 确保顺序
        for (int i = 0; i < count; i++)
        {
            await PowerCmd.TickDownDuration(elementPower);
        }
    }
}