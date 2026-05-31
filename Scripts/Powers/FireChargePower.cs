using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Ganyu.Scripts.Powers;

public class FireChargePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Counter 模式记录层数
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/fire_charge.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/fire_charge.png";
            protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<GuobaAttackPower>(), 
        HoverTipFactory.FromPower<PyronadoPower>()
    ];
}