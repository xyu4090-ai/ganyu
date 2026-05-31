using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Ganyu.Scripts.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(TokenCardPool))]
public sealed class HydroMastery : GanyuCardModel
{
    public HydroMastery() : base(1, CardType.Power, CardRarity.Token, TargetType.Self, false)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<HydroMasteryPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HydroMasteryPower>(choiceContext,
            base.Owner.Creature,
            base.DynamicVars.Power<HydroMasteryPower>().BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}