using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public sealed class AdministrativeStreamliningPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 使用 Counter 模式，如果玩家通过其他手段获得多层，每层都能额外抽 1 张
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/administrative_streamlining_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/administrative_streamlining_power.png";

    public async Task TriggerReaction()
    {
        var context = GanyuElementUtils.CurrentContext;
        if (context != null && base.Owner.Player != null)
        {
            // 触发反应时，根据能力层数抽牌
            await CardPileCmd.Draw(context, (int)base.Amount, base.Owner.Player);
            Flash();
        }
    }
}