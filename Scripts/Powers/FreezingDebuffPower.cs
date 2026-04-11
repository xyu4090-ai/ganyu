using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;

namespace Ganyu.Scripts.Powers;

public class FreezingDebuffPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/freezing_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/freezing_power.png";

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只有当拥有此状态的敌人发动攻击时才减伤 
        if (dealer != base.Owner || !props.IsPoweredAttack_())
        {
            return 1m;
        }
        // 返回 0.6，代表敌人只能造成 60% 的伤害（即减伤 40%）
        return 0.6m;
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Enemy)
		{
			await PowerCmd.TickDownDuration(this);
		}
	}
}
