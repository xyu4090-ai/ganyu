using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Ganyu.Scripts.Powers;
// 简单定义一个水元素标记能力
public class WetPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/wet_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/wet_power.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FreezingDebuffPower>()
    ];

}