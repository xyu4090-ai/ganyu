using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class PeaksOfGuyun : GanyuCardModel
{
    public PeaksOfGuyun() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 使用 BlockVar 来定义基础的 3 点格挡
        new BlockVar(3m,ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“孤云峰岩”能力，初始层数为 3
        await PowerCmd.Apply<PeaksOfGuyunPower>(choiceContext,
            base.Owner.Creature, 
            base.DynamicVars.Block.BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后增加“固有”关键字，保证起手能抽到
        base.DynamicVars.Block.UpgradeValueBy(2);
    }
}