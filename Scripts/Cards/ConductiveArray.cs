using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ConductiveArray : GanyuCardModel
{
    public ConductiveArray() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),                  // 初始抽 3 张牌
        new PowerVar<ElectroPower>(1m)    // 赋予 1 层雷元素
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectroPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 抽牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);

        // 2. 打开手牌弃牌界面，选择 1 张牌
        CardModel cardModel = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            base.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            this
        )).FirstOrDefault();

        // 如果玩家确实选择了一张牌
        if (cardModel != null)
        {
            // 执行弃牌动作
            await CardCmd.Discard(choiceContext, cardModel);

            // 3. 判断丢弃的牌是否为攻击牌
            if (cardModel.Type == CardType.Attack)
            {
                // 给全体存活的敌人施加 1 层雷元素
                foreach (var enemy in base.CombatState.HittableEnemies)
                {
                    if (enemy.IsAlive)
                    {
                        await ActionWithContext(choiceContext, async () =>
                        {
                            await GanyuElementUtils.ApplyElectroReaction(
                                               enemy,
                                               base.Owner.Creature,
                                               base.CombatState.HittableEnemies,
                                               base.DynamicVars.Power<ElectroPower>().BaseValue
                                    );
                        });
                    }
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}