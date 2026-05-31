using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ganyu.Scripts.Powers;

public class AdeptusTeachingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/adeptus_teaching_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/adeptus_teaching_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("PityBonus", 0m)
    ];
}
