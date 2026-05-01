using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
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

public class BalefulOmenPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/baleful_omen.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/baleful_omen.png";

    // 在抽牌前（即回合开始时）触发
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            
            // 获得 1 点能量
            await PlayerCmd.GainEnergy(1, player);

            var aliveEnemies = combatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (aliveEnemies.Count > 0)
            {
                // 随机选择一名存活的敌人
                var target = player.RunState.Rng.CombatTargets.NextItem<Creature>(aliveEnemies);
                if (target != null)
                {
                    // 造成 7 点伤害
                    await CreatureCmd.Damage(choiceContext, target, 7m, ValueProp.Unpowered, base.Owner, null);
                    
                    // 如果敌人没有被伤害打死，则给予 1 层雷元素并尝试触发反应
                    if (target.IsAlive)
                    {
                        await GanyuElementUtils.ExecuteReaction(choiceContext, async () => {
                            await GanyuElementUtils.ApplyElectroReaction(target, base.Owner, combatState.HittableEnemies, 1m);
                        });
                    }
                }
            }
            
            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}