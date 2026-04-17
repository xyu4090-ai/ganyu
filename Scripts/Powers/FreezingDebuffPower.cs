using MegaCrit.Sts2.Core.Entities.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Ganyu.Scripts.Powers;

public class FreezingDebuffPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 开启战斗钩子接收，确保我们可以在打牌后更新 UI 数值
    public override bool ShouldReceiveCombatHooks => true;

    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/freezing_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/freezing_power.png";

    // 1. 初始化一个固定值的动态变量，默认减伤 40%
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("CurrentReduction", 40m)
    ];

    // 2. 封装一个更新减伤 UI 数值的方法
    private void UpdateReductionUI()
    {
        if (base.CombatState == null) return;
        
        decimal multiplier = 0.6m; // 基础受伤 60% (减伤 40%)
        var player = base.CombatState.Players.FirstOrDefault()?.Creature;
        
        if (player != null)
        {
            var feather = player.GetPower<OceanborneFeatherPower>();
            if (feather != null && feather.Amount > 0)
            {
                // 收益递减公式
                multiplier /= (1m + 0.5m * feather.Amount);
            }
        }

        // 将算出的实际减伤百分比更新到动态变量里 (例如减伤 60% 就存 60)
        if (base.DynamicVars.TryGetValue("CurrentReduction", out var reductionVar))
        {
            reductionVar.BaseValue = Math.Round((1m - multiplier) * 100m);
        }
    }

    // 3. 修改伤害时，直接读取我们计算好的动态变量
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 确保在计算伤害前数值是最新的
        UpdateReductionUI();

        if (dealer != base.Owner || !props.IsPoweredAttack_())
        {
            return 1m;
        }

        if (base.DynamicVars.TryGetValue("CurrentReduction", out var reductionVar))
        {
            // 如果面板显示减伤 60%，这里就返回 1 - 0.6 = 0.4 的受伤倍率
            return 1m - (reductionVar.BaseValue / 100m);
        }

        return 0.6m; // 兜底
    }

    // 4. 在各种时机触发更新，保证玩家鼠标悬停时看到的是最新数值
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 玩家每次打出牌（很可能触发元素充能产生海人化羽）后，刷新一下数值
        UpdateReductionUI();
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        UpdateReductionUI();
        
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
}