using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ganyu.Scripts.Powers;

public class ConductPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
		protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamageIncrease", 1.4m)];
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/conduct_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/conduct_power.png";

	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		decimal num = base.DynamicVars["DamageIncrease"].BaseValue;
		return num;
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Enemy)
		{
			await PowerCmd.TickDownDuration(this);
		}
	}
}
