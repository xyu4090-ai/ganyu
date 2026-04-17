using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class JadeShieldPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Counter 模式，每触发一次结晶反应消耗一层
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/jade_shield.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/jade_shield.png";
}