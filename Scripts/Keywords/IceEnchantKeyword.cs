using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Ganyu.Scripts;

public class IceEnchantKeyword
{
    [CustomEnum("ICE_ENCHANT")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword IceEnchant;
}
