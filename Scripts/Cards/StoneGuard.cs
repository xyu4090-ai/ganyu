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
public sealed class StoneGuard : GanyuCardModel
{
    public StoneGuard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 初始格挡 8 点
        new BlockVar(8m, ValueProp.Move),   
        // 初始给予 2 层岩元素
        new PowerVar<RockPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 自动关联岩元素及其“结晶”反应的说明
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 自身获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 给予目标敌人岩元素并尝试触发结晶反应
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyRockReaction(
                    cardPlay.Target,
                    base.Owner.Creature,
                    base.CombatState.HittableEnemies,
                    base.DynamicVars.Power<RockPower>().BaseValue
                );
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡 8 -> 11 (+3)，岩元素层数 2 -> 3 (+1)
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars.Power<RockPower>().UpgradeValueBy(1m);
    }
}