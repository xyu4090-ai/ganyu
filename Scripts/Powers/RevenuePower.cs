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


    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/revenue_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/revenue_power.png";

    public async Task TriggerReaction()
    {
         // 获得能量
        await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
    }
}