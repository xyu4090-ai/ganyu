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
public sealed class ElementalSymbiosis : GanyuCardModel
{
    public ElementalSymbiosis() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IceChargePower>() // 提醒玩家这不包括冰元素
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 使用 BlockVar 来定义描述中显示的 3 点或 5 点格挡
        new BlockVar(2m,ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“元素共生”能力，层数设为当前的格挡变量值
        await PowerCmd.Apply<ElementalSymbiosisPower>(choiceContext,
            base.Owner.Creature, 
            base.DynamicVars.Block.BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级效果：提供的格挡值从 3 提升至 5
        base.DynamicVars.Block.UpgradeValueBy(1m);
    }
}