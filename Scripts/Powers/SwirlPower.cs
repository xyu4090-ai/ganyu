using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;

namespace Ganyu.Scripts.Powers;

public class SwirlPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/swirl_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/swirl_power.png";
	
}
