using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Ganyu.Scripts.Powers;

public class RockChargePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/rock_charge.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/rock_charge.png";
       protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<JadeShieldPower>(),
        HoverTipFactory.FromPower<StarfallPower>()
    ];
}