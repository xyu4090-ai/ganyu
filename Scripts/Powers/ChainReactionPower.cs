using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Ganyu.Scripts.Powers;

public class ChainReactionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/chain_reaction_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/chain_reaction_power.png";

    public async Task TriggerReaction()
    {
        // 使用用户指定的随机逻辑获取目标
        var target = base.Owner.Player.RunState.Rng.CombatTargets.NextItem<Creature>(base.CombatState.HittableEnemies);
        
        if (target != null && target.IsAlive)
        {
            Flash();
            // 每次反应触发时，给予 1 层冰元素 (此处数量取 Amount，即能力层数)
            await GanyuElementUtils.ApplyIceReaction(target, base.Owner, base.CombatState.HittableEnemies, base.Amount);
        }
    }
}