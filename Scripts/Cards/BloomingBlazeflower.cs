using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class BloomingBlazeflower : GanyuCardModel
{
    public BloomingBlazeflower() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 使用 CalculationBaseVar 来定义基础的 3 点能力伤害
        new CalculationBaseVar(6m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“烈焰绽放”能力，初始层数为 3
        await PowerCmd.Apply<BloomingBlazeflowerPower>(choiceContext,
            base.Owner.Creature, 
            base.DynamicVars.CalculationBase.BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害从 3 提升至 5
        base.DynamicVars.CalculationBase.UpgradeValueBy(2m);
    }
}