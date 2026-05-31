using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class ExplosiveArrow : GanyuCardModel
{
    public ExplosiveArrow() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10m),
        new ExtraDamageVar(10m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (CardModel card, Creature? target) =>
        {
            if (target != null && target.GetPower<IcePower>() is { } ip && ip.Amount > 0)
            {
                return 1m;
            }
            return 0m;
        }),
        new PowerVar<FlamePower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var target = cardPlay.Target;
        if (target != null && target.IsAlive)
        {
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyFireReaction(target, Owner.Creature, CombatState!.HittableEnemies, DynamicVars.Power<FlamePower>().BaseValue);
            });
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(4m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(4m);
    }
}