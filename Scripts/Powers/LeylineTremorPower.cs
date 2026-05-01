using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public sealed class LeylineTremorPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    // 采用 Stackable 允许玩家打出多张叠加伤害
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/leyline_tremor_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/leyline_tremor_power.png";

    // 接收从 Utils 传过来的所有可攻击敌人列表
    public async Task Trigger(IReadOnlyList<Creature>? hittableEnemies)
    {
        if (hittableEnemies == null) return;

        Flash();

        // 对全体敌人造成等同于层数(Amount)的伤害，复用 CurrentContext
        await CreatureCmd.Damage(GanyuElementUtils.CurrentContext, hittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner, null);
    }
}