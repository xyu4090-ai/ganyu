using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class AnemoMasteryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Counter 模式，以便多次打出可以叠加层数
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/anemo_mastery_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/anemo_mastery_power.png";
}