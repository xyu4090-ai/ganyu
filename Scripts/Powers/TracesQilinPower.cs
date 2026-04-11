using BaseLib.Abstracts;
using BaseLib.Extensions;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
namespace Ganyu.Scripts.Powers;

public class TracesQilinPower : CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;
	public override string? CustomPackedIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";
	public override string? CustomBigIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";
	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? __)
	{
		if (target == base.Owner && dealer != null && props.IsPoweredAttack_())
		{
			for (int i = 0; i < base.Amount; i++)
			{
				await GanyuElementUtils.ApplyIceReaction(dealer, base.Owner,base.CombatState.HittableEnemies);
			}

		}
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (base.Owner.Side != side)
		{
			await PowerCmd.Remove(this);
		}
	}

}