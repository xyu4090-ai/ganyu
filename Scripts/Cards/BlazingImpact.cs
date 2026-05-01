using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class BlazingImpact : GanyuCardModel
{
    // 费用 1，攻击牌，普通稀有度，单体目标
    public BlazingImpact() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(14m, ValueProp.Move),   // 基础伤害 9
        new PowerVar<FlamePower>(2m)         // 赋予目标 2 层火元素
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 附加火元素并触发反应逻辑
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyFireReaction(
                cardPlay.Target, 
                base.Owner.Creature, 
                base.CombatState.HittableEnemies, 
                DynamicVars.Power<FlamePower>().BaseValue // 传入 2 层
            );
        });
    }

    protected override void OnUpgrade()
    {
        // 升级后伤害提升 4 点 (达到 13 点)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars.Power<FlamePower>().UpgradeValueBy(1m); 
    }
}
