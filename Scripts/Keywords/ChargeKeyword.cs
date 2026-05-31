using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Ganyu.Scripts;

public class ChargeKeyword
{
    [CustomEnum("CHARGE")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Charge;
}
