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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class MeteorStrike : GanyuCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;

    public MeteorStrike() : base(energyCost, type, rarity, targetType, true) { }

    // 悬浮提示显示岩元素
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<RockPower>()
    ];

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move), // 基础伤害 26
        new PowerVar<RockPower>(2m)         // 岩元素标记 3 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成高额单体伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 给予目标岩元素并触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            // 显式传入卡牌中定义的岩元素层数（DynamicVars.Power<RockPower>().BaseValue）
            await GanyuElementUtils.ApplyRockReaction(
                cardPlay.Target, 
                base.Owner.Creature, 
                base.CombatState.HittableEnemies,
                base.DynamicVars.Power<RockPower>().BaseValue
            );
        });
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 +8 (26 -> 34)，岩元素 +1 (3 -> 4)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars.Power<RockPower>().UpgradeValueBy(1m);
    }
}