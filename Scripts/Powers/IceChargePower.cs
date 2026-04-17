using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class IceChargePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Counter 模式以记录层数
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/ice_charge.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/ice_charge.png";
}