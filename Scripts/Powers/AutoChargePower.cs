using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class AutoChargePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/auto_charge_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/auto_charge_power.png";
}
