using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Ganyu.Scripts.Powers;

public sealed class SolidAsRockPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    
    // 作为一个常驻能力，不需要像 Blur 那样每回合递减层数，因此用 None 即可
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/solid_as_rock_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/solid_as_rock_power.png";

    // 记录本回合是否触发了结晶反应
    private bool _retainBlockThisTurn = false;

    // 悬浮提示显示格挡说明
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    // **重要**：当结晶反应触发时，在你的反应处理代码中调用此方法！
    public void TriggerCrystalize()
    {
        _retainBlockThisTurn = true;
        Flash();
    }

    // 拦截回合结束时的格挡清除逻辑
    public override bool ShouldClearBlock(Creature creature)
    {
        if (base.Owner != creature)
        {
            return true;
        }
        // 如果本回合触发了结晶反应，则返回 false 阻止清除
        return !_retainBlockThisTurn;
    }

    // 成功阻止清除格挡后的回调
    public override Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer)
        {
            return Task.CompletedTask;
        }
        Flash();
        // 保留成功后重置状态，等待下个回合再次触发
        _retainBlockThisTurn = false;
        return Task.CompletedTask;
    }

    // 在玩家回合开始时重置状态，确保状态干净
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState ICombatState)
    {
        if (side == CombatSide.Player)
        {
            _retainBlockThisTurn = false;
        }
    }
}