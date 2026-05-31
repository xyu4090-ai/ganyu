using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardModel = MegaCrit.Sts2.Core.Models.CardModel;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class MasterySelection : GanyuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public MasterySelection() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }
        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<HydroMastery>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = new List<CardModel>
        {
            base.CombatState.CreateCard<RetainElement>(base.Owner),
            base.CombatState.CreateCard<HydroMastery>(base.Owner),
            base.CombatState.CreateCard<GeoMastery>(base.Owner),
            base.CombatState.CreateCard<PyroMastery>(base.Owner),
            base.CombatState.CreateCard<ElectroMastery>(base.Owner),
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
