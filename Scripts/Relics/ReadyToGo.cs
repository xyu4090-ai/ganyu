using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Ganyu.Scripts.Relics;

[Pool(typeof(GanyuRelicPool))]
public class ReadyToGo : CustomRelicModel
{
    public override string PackedIconPath => "res://Ganyu/images/relics/ready_to_go_small.png";
    protected override string PackedIconOutlinePath => "res://Ganyu/images/relics/ready_to_go_small.png";
    protected override string BigIconPath => "res://Ganyu/images/relics/ready_to_go.png";

    public override RelicRarity Rarity => RelicRarity.Common;
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ChargeKeyword.Charge),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    public override async Task AfterObtained()
    {
        var enchantment = ModelDb.Enchantment<ReadyToGoEnchantment>();
        var eligibleCards = PileType.Deck.GetPile(base.Owner).Cards
            .Where(c => enchantment.CanEnchant(c))
            .ToList();

        var selectedCard = (await CardSelectCmd.FromDeckForEnchantment(
            cards: eligibleCards.UnstableShuffle(base.Owner.RunState.Rng.Niche).ToList(),
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            enchantment: enchantment,
            amount: 1
        )).FirstOrDefault();

        if (selectedCard != null)
        {
            CardCmd.Enchant<ReadyToGoEnchantment>(selectedCard, 1m);
            var vfx = NCardEnchantVfx.Create(selectedCard);
            if (vfx != null)
            {
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }
        }
    }
}
