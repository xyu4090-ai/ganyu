using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class RideTheWind : GanyuCardModel
{
    public RideTheWind() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<RideTheWindPower>(),
        HoverTipFactory.FromPower<WindPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“顺风而行”能力
        await PowerCmd.Apply<RideTheWindPower>(choiceContext,
            base.Owner.Creature, 
            1m, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级效果：耗能减少 1 (3 -> 2)
        base.EnergyCost.UpgradeBy(-1);
    }
}