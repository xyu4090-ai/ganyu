using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementalSymphony : GanyuCardModel
{
    private const string _calculatedHitsKey = "CalculatedHits";

    // 初始化：2费，攻击牌，稀有，目标为单个敌人
    public ElementalSymphony() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    // 显示所有相关元素的悬浮提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move), // 基础伤害 10
        new CalculationBaseVar(1m),         // 基础攻击次数：1次
        new CalculationExtraVar(1m),        // 额外攻击乘区：每种元素加 1 次
        
        // 动态计算额外攻击次数的乘数（即目标身上的元素种类数量）
        new CalculatedVar(_calculatedHitsKey).WithMultiplier((CardModel card, Creature? target) => 
        {
            if (target == null) return 0m;
            int count = 0;
            
            // 依次检查六种元素附着
            if (target.GetPower<IcePower>() is { Amount: > 0 }) count++;
            if (target.GetPower<WetPower>() is { Amount: > 0 }) count++;
            if (target.GetPower<FlamePower>() is { Amount: > 0 }) count++;
            if (target.GetPower<ElectroPower>() is { Amount: > 0 }) count++;
            if (target.GetPower<WindPower>() is { Amount: > 0 }) count++;
            if (target.GetPower<RockPower>() is { Amount: > 0 }) count++;
            
            return (decimal)count;
        })
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 获取计算后的总攻击次数（基础 1 次 + 额外元素种类次）
        int hitCount = (int)((CalculatedVar)base.DynamicVars[_calculatedHitsKey]).Calculate(cardPlay.Target);

        // 造成伤害并设置多段攻击
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：基础伤害 +4 (10 -> 14)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}