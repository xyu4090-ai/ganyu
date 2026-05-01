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
public sealed class CrossStrike : GanyuCardModel
{
    public CrossStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    // 声明打击标签，使其能吃到“完美打击”等遗物/卡牌的加成
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move), // 基础伤害 7
        new PowerVar<IcePower>(2m)        // 1 层风元素
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WindPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 如果目标存活，检查其身上是否有任何元素附着
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            // 判定目标是否持有冰、水、火、雷、岩或风元素中的任意一种
            bool hasAnyElement = 
                cardPlay.Target.GetPower<WetPower>() != null ||
                cardPlay.Target.GetPower<FlamePower>() != null ||
                cardPlay.Target.GetPower<ElectroPower>() != null ||
                cardPlay.Target.GetPower<RockPower>() != null ||
                cardPlay.Target.GetPower<WindPower>() != null;

            // 如果有附着，则给予 1 层风元素引发扩散
            if (hasAnyElement)
            {
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyWindReaction(
                        cardPlay.Target, 
                        base.Owner.Creature, 
                        base.CombatState.HittableEnemies,
                        base.DynamicVars.Power<WindPower>().BaseValue
                    );
                });
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 7 -> 10 (+3)
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars.Power<IcePower>().UpgradeValueBy(1m);
    }
}