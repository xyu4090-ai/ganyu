using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ganyu.Scripts.Powers;

public class RevenuePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 定义自定义变量，用于在 JSON 中通过 {Remaining} 显示
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Remaining", 3m)
    ];

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/revenue_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/revenue_power.png";

    public async Task TriggerReaction()
    {
        var remainingVar = base.DynamicVars["Remaining"];
        
        // 只有剩余次数大于 0 时触发
        if (remainingVar.BaseValue > 0 && base.Owner.Player != null)
        {
            // 仅修改文本中显示的数值
            remainingVar.UpgradeValueBy(-1m);
            
            // 获得能量
            await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
            Flash();
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // 玩家回合开始时，重置文本中的计数
        if (side == base.Owner.Side)
        {
            decimal current = base.DynamicVars["Remaining"].BaseValue;
            base.DynamicVars["Remaining"].UpgradeValueBy(3m - current);
        }
    }
}