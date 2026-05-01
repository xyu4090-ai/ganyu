using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class FrostflakeBloom : GanyuCardModel
{
    // 初始化：1费，技能牌，普通，目标为自己（提供格挡且随机挑选敌人）
    public FrostflakeBloom() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    // 悬浮提示显示冰元素
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>()
    ];

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4m, ValueProp.Move),       // 基础格挡 7
        new DynamicVar("MaxTriggers", 3m),      // 最多触发次数 3
        new PowerVar<IcePower>(1m)              // 每次随机给予的冰元素 1 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 玩家获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 统计场上所有的非冰元素层数
        decimal nonIceStacks = 0;
        var aliveEnemies = base.CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();

        foreach (var enemy in aliveEnemies)
        {
            if (enemy.GetPower<WetPower>() is { Amount: > 0 } wp) nonIceStacks += wp.Amount;
            if (enemy.GetPower<FlamePower>() is { Amount: > 0 } fp) nonIceStacks += fp.Amount;
            if (enemy.GetPower<ElectroPower>() is { Amount: > 0 } ep) nonIceStacks += ep.Amount;
            if (enemy.GetPower<WindPower>() is { Amount: > 0 } wip) nonIceStacks += wip.Amount;
            if (enemy.GetPower<RockPower>() is { Amount: > 0 } rp) nonIceStacks += rp.Amount;
        }

        // 3. 计算实际触发次数（取非冰元素总层数和最大触发次数中的较小值）
        int maxTriggers = base.DynamicVars["MaxTriggers"].IntValue;
        int actualTriggers = Math.Min((int)nonIceStacks, maxTriggers);

        // 4. 随机给予冰元素并触发反应
        if (actualTriggers > 0 && aliveEnemies.Count > 0)
        {
            var rng = new Random();

            await ActionWithContext(choiceContext, async () =>
            {
                for (int i = 0; i < actualTriggers; i++)
                {
                    // 每次施加前重新获取存活的敌人（防止前面的反应把敌人打死导致报错）
                    var currentAliveEnemies = base.CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
                    if (currentAliveEnemies.Count == 0) break;

                    // 随机选择一名存活的敌人
                    var randomEnemy  = base.Owner.RunState.Rng.CombatTargets.NextItem(aliveEnemies);;

                    // 给予冰元素
                    await GanyuElementUtils.ApplyIceReaction(
                        randomEnemy,
                        base.Owner.Creature,
                        base.CombatState.HittableEnemies,
                        base.DynamicVars.Power<IcePower>().BaseValue
                    );
                }
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡 +3 (7 -> 10)，最大触发次数 +2 (3 -> 5)
        base.DynamicVars.Block.UpgradeValueBy(2m);
        base.DynamicVars["MaxTriggers"].UpgradeValueBy(2m);
    }
}