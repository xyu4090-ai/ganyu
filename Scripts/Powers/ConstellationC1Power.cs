using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Ganyu.Scripts.Powers;

public class ConstellationC1Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/constellation_c1.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/constellation_c1.png";
}
