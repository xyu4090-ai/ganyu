using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public class TracesQilinPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";

    // 回合开始时触发
    public override async Task AfterSideTurnStart( CombatSide side, ICombatState ICombatState)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            // 给予 3 点基础格挡，使用 Unpowered 确保不受敏捷加成（根据需求描述）
            await CreatureCmd.GainBlock(base.Owner, 3m, ValueProp.Unpowered, null);
            
            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}