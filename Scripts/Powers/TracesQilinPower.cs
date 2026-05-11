using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;

namespace Ganyu.Scripts.Powers;

public class TracesQilinPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/traces_qilin_power.png";

    // 回合开始时触发
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState ICombatState)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            // 给予 3 点基础格挡
            await CreatureCmd.GainBlock(base.Owner, 3m, ValueProp.Unpowered, null);

            // 随机给予一名敌人 1 层冰元素
            var aliveEnemies = ICombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (aliveEnemies.Count > 0)
            {
                var target = base.Owner.Player.RunState.Rng.CombatTargets.NextItem<Creature>(aliveEnemies);
                if (target != null)
                {
                    await GanyuElementUtils.ApplyIceReaction(target, base.Owner, ICombatState.HittableEnemies, 1m);
                }
            }

            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}