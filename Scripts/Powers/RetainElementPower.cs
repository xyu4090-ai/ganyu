using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class RetainElementPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Single 模式，因为概率是固定的
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/retain_element_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/retain_element_power.png";
}