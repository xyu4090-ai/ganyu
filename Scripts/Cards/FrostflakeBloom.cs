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
        new BlockVar(4m, ValueProp.Move),       // 基础格挡 4
        new PowerVar<IcePower>(1m),              // 每次随机给予的冰元素 1 层
        new DynamicVar("ExtraBlock", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 玩家获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 统计场上所有的冰元素层数
        decimal IceStacks = 0;
        var aliveEnemies = base.CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();

        foreach (var enemy in aliveEnemies)
        {
            if (enemy.GetPower<IcePower>() is { Amount: > 0 } ip) IceStacks += ip.Amount;
        }
        decimal finalBlock = IceStacks * base.DynamicVars["ExtraBlock"].BaseValue; 
        await CreatureCmd.GainBlock(base.Owner.Creature, finalBlock,ValueProp.Unpowered, cardPlay);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡 +3 (7 -> 10)，最大触发次数 +2 (3 -> 5)
        base.DynamicVars.Block.UpgradeValueBy(2m);
        base.DynamicVars["ExtraBlock"].UpgradeValueBy(1m);
    }
}