using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class OvertimeWork : GanyuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 对应 JSON 中的 {HpLoss}，失去 3 点生命
        new HpLossVar(3m),
        // 对应 JSON 中的 {Energy}，初始获得 2 点能量
        new EnergyVar(2)
    ];

    // 显式声明消耗关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Exhaust
    ];

    // 在预览界面显示能量的浮窗提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    base.EnergyHoverTip
    ];

    public OvertimeWork()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 扣除生命值。参考 Offering 使用 Unblockable 等属性确保“失去生命”逻辑
        await CreatureCmd.Damage(
            choiceContext, 
            base.Owner.Creature, 
            base.DynamicVars.HpLoss.BaseValue, 
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, 
            this
        );

        // 2. 获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：获得能量从 2 变为 3
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}