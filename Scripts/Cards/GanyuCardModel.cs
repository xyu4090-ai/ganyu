using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Ganyu.Scripts.Cards;

public abstract class GanyuCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://Ganyu/images/cards/{GetType().Name}.png";

    public GanyuCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    protected async Task ActionWithContext(PlayerChoiceContext context, Func<Task> action)
    {
        GanyuElementUtils.CurrentContext = context;
        try {
            await action();
        } finally {
            GanyuElementUtils.CurrentContext = null;
        }
    }
}
