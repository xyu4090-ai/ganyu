using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.HoverTips;

namespace Ganyu.Scripts.Enchantments;

public class ReadyToGoEnchantment : CustomEnchantmentModel
{
    public override bool ShowAmount => false;
    public override bool HasExtraCardText => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ChargeKeyword.Charge),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
    protected override string? CustomIconPath => "res://Ganyu/images/enchantments/ready_to_go_enchantment.png";

    protected override void OnEnchant()
    {
        Card.AddKeyword(ChargeKeyword.Charge);
        Card.AddKeyword(CardKeyword.Retain);
    }
}
