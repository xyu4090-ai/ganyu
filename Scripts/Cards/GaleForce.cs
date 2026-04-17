using BaseLib.Utils;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class GaleForce : GanyuCardModel
{
    public GaleForce() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move), // 基础伤害 10
        new CardsVar(1)                    // 抽牌数量 1
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 执行攻击
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 检查本回合是否触发过扩散反应
        // 即使是本次攻击触发的扩散也会被计入，因为 DamageCmd 已执行完毕
        if (GanyuElementUtils.SwirlsThisTurn > 0)
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
        }
    }
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            return GanyuElementUtils.SwirlsThisTurn > 0 ;
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害增加 4 点 (10 -> 14)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}