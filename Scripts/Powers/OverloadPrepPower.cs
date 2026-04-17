using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class OverloadPrepPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Counter 模式，如果多次叠加则可以触发多次双倍
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/overload_prep_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/overload_prep_power.png";
}