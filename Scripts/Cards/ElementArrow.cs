using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementArrow : GanyuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public ElementArrow() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AllEnemies, true)
    {
    }
        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<WaterArrow>(),
        HoverTipFactory.FromCard<FireArrow>(),
        HoverTipFactory.FromCard<WindArrow>(),
        HoverTipFactory.FromCard<ElectroArrow>(),
        HoverTipFactory.FromCard<RockArrow>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = new List<CardModel>
        {
            base.CombatState.CreateCard<WaterArrow>(base.Owner),
            base.CombatState.CreateCard<FireArrow>(base.Owner),
            base.CombatState.CreateCard<WindArrow>(base.Owner),
            base.CombatState.CreateCard<ElectroArrow>(base.Owner),
            base.CombatState.CreateCard<RockArrow>(base.Owner),
        };

        if (base.IsUpgraded)
        {
            foreach (CardModel card in cards)
            {
                CardCmd.Upgrade(card);
            }
        }

        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
        CardModel selectedCard = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, prefs)).FirstOrDefault();

        if (selectedCard != null)
        {
            await CardPileCmd.Add(selectedCard, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
