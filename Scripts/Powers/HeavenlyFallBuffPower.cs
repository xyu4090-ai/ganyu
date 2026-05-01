using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public class HeavenlyFallBuffPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/heavenly_fall_buff.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/heavenly_fall_buff.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
    ];

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState ICombatState)
    {
        if (side == base.Owner.Side && base.Owner.Player != null)
        {
            Flash();
            // 1. 获得 1 点能量
            await PlayerCmd.GainEnergy(1, base.Owner.Player);

            // 3. 全体 1 层冰元素
            // 这里调用工具类触发反应检查
            foreach (var enemy in ICombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    await GanyuElementUtils.ApplyIceReaction(enemy, base.Owner, ICombatState.HittableEnemies, 1m);
                }
            }

            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState ICombatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, 15, ValueProp.Unpowered, base.Owner, null);
        }
    }
}
