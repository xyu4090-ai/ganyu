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
public sealed class FrostflakeArrow : GanyuCardModel
{
    public FrostflakeArrow() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move),        // 基础伤害 10
        new CalculationExtraVar(5m),               // 溅射伤害 5
        new PowerVar<IcePower>(1m)                 // 给予 1 层冰
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 检测目标在受到攻击前是否已有冰元素
        bool hadIce = cardPlay.Target != null && cardPlay.Target.GetPower<IcePower>() != null;

        // 2. 对目标造成主伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 3. 给予 1 层冰元素并尝试触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyIceReaction(
                cardPlay.Target,
                base.Owner.Creature,
                base.CombatState.HittableEnemies,
                base.DynamicVars.Power<IcePower>().BaseValue
            );
        });

        // 4. 溅射逻辑：如果目标原本已有冰，对全体敌人（包含目标）造成溅射伤害
        if (hadIce)
        {
            await DamageCmd.Attack(base.DynamicVars["CalculationExtra"].BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 10 -> 14 (+4)；溅射 5 -> 8 (+3)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["CalculationExtra"].UpgradeValueBy(3m);
    }
}