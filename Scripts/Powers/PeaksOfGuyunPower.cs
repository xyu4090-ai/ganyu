using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public sealed class PeaksOfGuyunPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    // 使用 Counter，让 Amount 代表每次触发时获得的格挡数值
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/peaks_of_guyun_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/peaks_of_guyun_power.png";


    public async Task Trigger()
    {
        // 获得等同于能力层数的格挡
        // 使用 Unpowered 确保该格挡不受力量/虚弱等非常规修饰的影响
        await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);
    }
}