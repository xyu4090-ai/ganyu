using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Ganyu.Scripts.Powers;
// 简单定义一个水元素标记能力
public class RockPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/rock_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/rock_power.png";
}