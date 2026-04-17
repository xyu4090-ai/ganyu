using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class CrystalBurst : GanyuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 基础伤害变量
        new DamageVar(8m, ValueProp.Move),
        // 基础格挡设为 0
        new CalculationBaseVar(0m),
        // 每层岩元素提供的格挡系数
        new CalculationExtraVar(5m),
        // 核心逻辑：使用 CalculatedBlockVar 自动计算最终格挡
        // 最终格挡 = CalculationBase + (CalculationExtra * Multiplier)
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? target) => 
        {
            // 修改为：乘数为目标的“岩元素”层数
            return target?.GetPower<RockPower>()?.Amount ?? 0m;
        })
    ];

    public CrystalBurst() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 修改为：在卡牌预览时显示“岩元素”状态的说明
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 获得格挡
        if (base.DynamicVars.TryGetValue("CalculatedBlock", out var variable) && variable is CalculatedBlockVar cbv)
        {
            // 调用 Calculate 时传入目标，以正确获取其岩元素层数
            decimal finalBlock = cbv.Calculate(cardPlay.Target);
            if (finalBlock > 0)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, finalBlock, cbv.Props, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：基础伤害 8 -> 12 (+4)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}