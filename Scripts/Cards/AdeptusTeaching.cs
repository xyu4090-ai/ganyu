using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class AdeptusTeaching : GanyuCardModel
{
    public AdeptusTeaching() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AdeptusTeachingPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AdeptusTeachingPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
        AdeptusTeachingSingleton.Instance?.RegisterPlayer(base.Owner);
        AdeptusTeachingSingleton.Instance?.InitPowerDynamicVars(base.Owner.Creature, base.Owner.GetHashCode());
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
