using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;

namespace Ganyu.Scripts.Powers;

public sealed class OverdrawnPotentialPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/overdrawn_potential_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/overdrawn_potential_power.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, base.Owner, 5, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner, null);
        }
    }
}
