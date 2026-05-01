using System.Threading.Tasks;
using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public sealed class BloomingBlazeflowerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    // 使用 Counter，让 Amount 代表每次触发时造成的伤害数值
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/blooming_blazeflower_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/blooming_blazeflower_power.png";


    /// <summary>
    /// 当成功给敌人施加火元素时调用
    /// </summary>
    public async Task Trigger()
    {
        var combatState = base.Owner?.CombatState;
        if (combatState == null) return;

        // 遍历所有存活的敌人并造成伤害
        foreach (var enemy in combatState.HittableEnemies)
        {
            if (enemy.IsAlive && GanyuElementUtils.CurrentContext != null)
            {
                // 使用 Unpowered 确保该伤害为固定的能力伤害，不受力量/虚弱等非常规修饰的影响
                // 这与“融化”的机制保持一致
                await CreatureCmd.Damage(
                    GanyuElementUtils.CurrentContext,
                    enemy,
                    base.Amount,
                    ValueProp.Unpowered,
                    base.Owner,
                    null
                );
            }
        }
    }
}