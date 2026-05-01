using BaseLib.Utils;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class FusionStrike : GanyuCardModel
{
    public FusionStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10m),    // 基础伤害 8 点
        new ExtraDamageVar(4m),        // 本回合每次反应额外造成的伤害
        
        // 核心计算逻辑：读取 GanyuElementUtils.ReactionsThisTurn 作为乘区倍率
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
            GanyuElementUtils.ReactionsThisTurn)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 触发攻击：直接调用结算后的 CalculatedDamage 属性
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：额外伤害 4 -> 6 (+2)
        base.DynamicVars.CalculationBase.UpgradeValueBy(4m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}