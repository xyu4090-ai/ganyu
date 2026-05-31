using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class ChargeMasteryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/charge_mastery_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/charge_mastery_power.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card != null && cardPlay.Card.Keywords.Contains(ChargeKeyword.Charge))
        {
            Flash();
            await PlayerCmd.GainEnergy(1, base.Owner.Player);
        }
    }
}
