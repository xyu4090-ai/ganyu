using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class ExplosiveArrow : GanyuCardModel
{
    public ExplosiveArrow() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<FreezingDebuffPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10m),    // 基础伤害 20 点
        new ExtraDamageVar(10m),        // 触发条件时的额外伤害 20 点（用于翻倍）
        new PowerVar<FlamePower>(2m),
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

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(4m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(4m);
        base.DynamicVars.Power<FlamePower>().UpgradeValueBy(1m);
    }
}