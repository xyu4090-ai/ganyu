using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class ThunderstormMatrixPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 设置为 Stack，允许多次打出该能力卡时倍率互相叠加（例如3+3=6倍）
    public override PowerStackType StackType => PowerStackType.Counter; 
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/thunderstorm_matrix.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/thunderstorm_matrix.png";

    // 回合结束时触发
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState ICombatState)
    {
        if (side != base.Owner.Side)
        {
            var aliveEnemies = ICombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            bool hasFlashed = false;

            foreach (var enemy in aliveEnemies)
            {
                // 获取敌人身上的雷元素层数
                var electroPower = enemy.Powers.FirstOrDefault(p => p is ElectroPower);
                
                if (electroPower != null && electroPower.Amount > 0)
                {
                    if (!hasFlashed)
                    {
                        Flash();
                        hasFlashed = true; // 确保一回合无论多少个敌人只闪烁一次特效
                    }

                    // 最终伤害 = 敌人的雷元素层数 * 能力自身的层数(倍率)
                    decimal damageAmount = electroPower.Amount * base.Amount;

                    // 对该敌人造成伤害
                    await CreatureCmd.Damage(
                        new ThrowingPlayerChoiceContext(), 
                        enemy, 
                        damageAmount, 
                        ValueProp.Unpowered, // 能力造成的绝对伤害，通常不受力量等增益影响
                        base.Owner, 
                        null
                    );
                }
            }
        }
    }
}