using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class RetainElementPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 改为 Counter 模式，以便叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/retain_element_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/retain_element_power.png";
}