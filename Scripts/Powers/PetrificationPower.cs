using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;

namespace Ganyu.Scripts.Powers;

public class PetrificationPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/crystalize_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/crystalize_power.png";

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target == base.Owner && dealer != null && dealer != base.Owner&&props.IsPoweredAttack() )
		{
			Player player = dealer.Player ?? base.Applier?.Player;
			if (player != null)
			{
				await CreatureCmd.GainBlock(player.Creature, base.Amount, ValueProp.Unpowered, null);
			}
		}
	}
}
