using System.Collections.Generic;
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
public sealed class SilentNourishment : GanyuCardModel
{
    // 初始化：1费，技能牌，普通，目标为任意单一敌人
    public SilentNourishment() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    // 悬浮提示显示水元素
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WetPower>()
    ];

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(15m, ValueProp.Move),  // 基础格挡 7
        new PowerVar<WetPower>(3m)         // 水元素标记 1 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 玩家获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 给予目标水元素并触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyWaterReaction(
                cardPlay.Target,
                base.Owner.Creature,
                base.CombatState.HittableEnemies,
                base.DynamicVars.Power<WetPower>().BaseValue
            );
        });
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡增加 3 点 (7 -> 10)
        base.DynamicVars.Block.UpgradeValueBy(5m);
        base.DynamicVars.Power<WetPower>().UpgradeValueBy(1m);
    }
}