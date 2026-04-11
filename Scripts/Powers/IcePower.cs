using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
namespace Ganyu.Scripts.Powers;

public class IcePower : CustomPowerModel
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Debuff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/ice_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/ice_power.png";
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            if (GanyuElementUtils.HasRetainElement)
            {
                return;
            }

            // 如果没有精通，则正常减少 1 层
            await PowerCmd.TickDownDuration(this);
        }
    }
}