using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Ganyu.Scripts.Powers;

public class ArcheryMasteryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/archery_mastery_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/archery_mastery_power.png";

}
