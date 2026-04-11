using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Ganyu.Scripts.Utils;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Ganyu.Scripts.Powers;

public sealed class EfficiencyPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 必须设置为 true，否则 AfterPowerAmountChanged 不会触发
    public override bool ShouldReceiveCombatHooks => true;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/efficiency_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/efficiency_power.png";

    public async Task TriggerReaction()
    {
        var context = GanyuElementUtils.CurrentContext;
        if (context != null && base.Owner.Player != null)
        {
            // 每次反应抽 Amount 张牌
            await CardPileCmd.Draw(context, (int)base.Amount, base.Owner.Player);
            Flash();
        }
    }

    // 回合结束移除自身 (实现“本回合内”效果)
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // 如果到了对方回合开始，说明我方回合已结束
        if (side != base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}