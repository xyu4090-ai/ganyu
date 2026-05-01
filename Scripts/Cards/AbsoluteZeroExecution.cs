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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class AbsoluteZeroExecution : GanyuCardModel
{
    public AbsoluteZeroExecution() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(20m),    // 基础伤害 20 点
        new ExtraDamageVar(20m),        // 触发条件时的额外伤害 20 点（用于翻倍）
        
        // 动态伤害计算逻辑
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (CardModel card, Creature? target) => 
        {
            // 如果目标存在且拥有“结冰”状态，乘数为 1 (20 + 20*1 = 40)
            // 否则乘数为 0 (20 + 20*0 = 20)
            if (target != null && target.GetPower<FreezingDebuffPower>() != null)
            {
                return 1m;
            }
            return 0m;
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FreezingDebuffPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            var frozenPower = cardPlay.Target.GetPower<FreezingDebuffPower>();
            bool wasFrozen = frozenPower != null;

            // 传入 CalculatedDamage 自动完成最终伤害的结算
            await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：基础伤害 +8，额外伤害(翻倍伤害)也必须同步 +8
        // 这样升级后就是 28基础 + 28额外，结冰时造成 56 点伤害
        base.DynamicVars.CalculationBase.UpgradeValueBy(8m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(8m);
    }
}