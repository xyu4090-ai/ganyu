using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class MusouShinsetsuPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/musou_shinsetsu.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/musou_shinsetsu.png";

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            
            decimal totalElectroRemoved = 0;
            
            // 移除所有敌人的雷元素并计算总层数
            foreach (var enemy in combatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    var electro = enemy.GetPower<ElectroPower>();
                    if (electro != null && electro.Amount > 0)
                    {
                        totalElectroRemoved += electro.Amount;
                        // 直接移除该敌人的雷元素能力
                        await PowerCmd.Remove(electro);
                    }
                }
            }

            // 计算最终伤害：15 + (每移除1层额外5点)
            decimal finalDamage = 15m + (5m * totalElectroRemoved);
            
            // 对全体敌人造成伤害
            await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, finalDamage, ValueProp.Unpowered, base.Owner, null);
            
            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}