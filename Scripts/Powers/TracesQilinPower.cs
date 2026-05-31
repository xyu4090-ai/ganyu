using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;

namespace Ganyu.Scripts.Powers;

public class TracesQilinPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";

    // 回合结束前触发：获得等同于层数的格挡，然后计数减1
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);

            // 五命折草：山泽麟迹的计数不再减少
            if (base.Owner.GetPower<ConstellationC5Power>() == null)
            {
                await PowerCmd.TickDownDuration(this);
            }
        }
    }
}