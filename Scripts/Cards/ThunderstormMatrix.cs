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
public sealed class ThunderstormMatrix : GanyuCardModel
{
    public ThunderstormMatrix() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ThunderstormMatrixPower>(),
        HoverTipFactory.FromPower<ElectroPower>() // 提供雷元素的悬停提示
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 根据是否升级决定赋予的倍率层数（未升级为3，升级后为5）
        decimal amount = base.IsUpgraded ? 5m : 3m;

        await PowerCmd.Apply<ThunderstormMatrixPower>(choiceContext,
            base.Owner.Creature, 
            amount, 
            base.Owner.Creature, 
            this
        );
    }
}