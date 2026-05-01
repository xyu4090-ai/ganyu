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
public sealed class ShatterBolt : GanyuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(8m),    // 基础伤害
        new ExtraDamageVar(4m),      // 额外的碎冰伤害
        // 核心逻辑：使用 CalculatedDamageVar 自动计算最终伤害
        // 最终伤害公式 = CalculationBase + (ExtraDamage * Multiplier)
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? target) => 
        {
            // 如果目标存在且拥有“结冰”状态，乘数为 1，否则为 0
            if (target != null && target.GetPower<FreezingDebuffPower>() != null)
            {
                return 1m;
            }
            return 0m;
        })
    ];

    public ShatterBolt() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FreezingDebuffPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 仿照 ElementalStorm，直接使用计算后的变量
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级：基础伤害 +3 (11)，额外伤害 +4 (14)
        base.DynamicVars.CalculationBase.UpgradeValueBy(3m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}