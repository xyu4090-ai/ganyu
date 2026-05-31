using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class RockTransformationPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/rock_change.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/rock_change.png";
}
