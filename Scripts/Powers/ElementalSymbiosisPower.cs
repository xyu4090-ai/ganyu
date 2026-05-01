using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public sealed class ElementalSymbiosisPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    // 该能力是数值堆叠型，Amount 代表每次触发获得的格挡量
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/elemental_symbiosis_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/elemental_symbiosis_power.png";


    /// <summary>
    /// 当获得非冰元素充能时调用
    /// </summary>
    public async Task Trigger()
    {
        // 获得等同于能力层数的格挡
        // 使用 Unpowered 确保该格挡不受力量/虚弱影响，符合“能力提供固定收益”的设计
        await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);
    }
}