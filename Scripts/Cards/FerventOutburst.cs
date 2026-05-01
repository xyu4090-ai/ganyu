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
public sealed class FerventOutburst : GanyuCardModel
{
    public FerventOutburst() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<FlamePower>(2m),
        new PowerVar<IgnitionPower>(1m) // 新增：炎化层数变量，初始1层
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<IgnitionPower>() // 替换：移除冰元素的提示，改为炎化状态的悬浮提示
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 移除了根据目标身上冰元素回能的逻辑

        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            // 1. 施加火元素反应
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyFireReaction(
                    cardPlay.Target,
                    base.Owner.Creature,
                    base.CombatState.HittableEnemies,
                    base.DynamicVars.Power<FlamePower>().BaseValue
                );
            });

            // 2. 施加炎化状态
            await PowerCmd.Apply<IgnitionPower>(choiceContext,
                cardPlay.Target, 
                base.DynamicVars.Power<IgnitionPower>().BaseValue, 
                base.Owner.Creature, 
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Power<FlamePower>().UpgradeValueBy(2m);
    }
}