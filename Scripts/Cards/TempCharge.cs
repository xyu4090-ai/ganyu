using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class TempCharge : GanyuCardModel
{
    public TempCharge() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TempChargePower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<TempChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TempChargePower>(choiceContext,
            base.Owner.Creature,
            base.DynamicVars.Power<TempChargePower>().BaseValue,
            base.Owner.Creature,
            this
        );
        var power = base.Owner.Creature.GetPower<TempChargePower>();
        power?.ApplyBonusToHand();
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Power<TempChargePower>().UpgradeValueBy(1m);
    }
}
