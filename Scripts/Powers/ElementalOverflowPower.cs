using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class ElementalOverflowPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用通常的堆叠方式：1层双倍，2层三倍（1+Amount）
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/elemental_overflow_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/elemental_overflow_power.png";
}