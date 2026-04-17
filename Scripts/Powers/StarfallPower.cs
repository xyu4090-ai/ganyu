using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class StarfallPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/starfall.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/starfall.png";

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            
            // 对全体敌人造成 25 点伤害
            await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, 25m, ValueProp.Unpowered, base.Owner, null);
            
            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}