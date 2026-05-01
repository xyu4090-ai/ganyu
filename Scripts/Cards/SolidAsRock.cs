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
public sealed class SolidAsRock : GanyuCardModel
{
    // 初始化卡牌：2费，能力牌，稀有，目标为自己
    public SolidAsRock() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SolidAsRockPower>(),
        HoverTipFactory.FromPower<RockPower>() // 也可以加上岩元素的提示
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“坚如磐石”能力
        await PowerCmd.Apply<SolidAsRockPower>(choiceContext,
            base.Owner.Creature, 
            1m, // 虽然该能力不计层数，但方法通常需要传入一个数量
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后减一费 (2 -> 1)
        base.EnergyCost.UpgradeBy(-1);
    }
}