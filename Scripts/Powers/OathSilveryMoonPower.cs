using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class OathSilveryMoonPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/oath_silvery_moon.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/oath_silvery_moon.png";

    // 回合开始时触发
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            
            // 恢复2点生命值
            await CreatureCmd.Heal(base.Owner, 2m);

            // 给予所有敌人一层水元素
            foreach (var enemy in combatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    await GanyuElementUtils.ApplyWaterReaction(enemy, base.Owner, combatState.HittableEnemies, 1m);
                }
            }

            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}