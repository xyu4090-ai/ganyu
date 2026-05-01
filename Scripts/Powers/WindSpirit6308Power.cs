using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class WindSpirit6308Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/wind_spirit_6308.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/wind_spirit_6308.png";

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState ICombatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            
            // 对全体敌人造成 4 点伤害
            await CreatureCmd.Damage(choiceContext, ICombatState.HittableEnemies, 4m, ValueProp.Unpowered, base.Owner, null);
            
            // 给予所有存活敌人 1 层风元素
            foreach (var enemy in ICombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    await GanyuElementUtils.ExecuteReaction(choiceContext, async () => {
                        await GanyuElementUtils.ApplyWindReaction(enemy, base.Owner, ICombatState.HittableEnemies, 1m);
                    });
                }
            }
            
            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}