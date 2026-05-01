using System.Collections.Generic;
using System.Threading.Tasks;
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
public sealed class GlacierSmash : GanyuCardModel
{
    // 初始化：3费，攻击牌，稀有，目标为单体敌人
    public GlacierSmash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    // 悬浮提示显示结冰状态和能量图标说明
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FreezingDebuffPower>(),
        HoverTipFactory.ForEnergy(this)
    ];

    // 重点修改：使用动态计算变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(9m),    // 基础伤害 20
        new ExtraDamageVar(9m),        // 触发结冰时的额外伤害 20（用于实现翻倍）
        new EnergyVar(1),               // 返还能量 2
        
        // 动态伤害计算逻辑：最终伤害 = 基础伤害 + (额外伤害 * 乘数)
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (CardModel card, Creature? target) =>
        {
            // 如果目标存在且拥有“结冰”状态，乘数为 1 (20 + 20*1 = 40)
            // 否则乘数为 0 (20 + 20*0 = 20)
            if (target != null && target.GetPower<FreezingDebuffPower>() is { Amount: > 0 })
            {
                return 1m;
            }
            return 0m;
        })
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 检查目标是否有结冰状态（用于判定是否返还能量）
        var freezePower = cardPlay.Target?.GetPower<FreezingDebuffPower>();
        bool isFrozen = freezePower != null && freezePower.Amount > 0;

        // 1. 造成伤害 (直接使用 CalculatedDamage，它已经包含了翻倍判定)
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx(null, null, isFrozen ? "heavy_impact.mp3" : "blunt_attack.mp3") // 根据是否结冰改变受击音效
            .Execute(choiceContext);

        // 2. 如果触发了结冰条件，返还能量
        if (isFrozen)
        {
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：基础伤害增加 6 点 (20 -> 26)
        // 为了确保翻倍后的数值正确，额外伤害也需要同步增加 6 点 (20 -> 26)
        base.DynamicVars.CalculationBase.UpgradeValueBy(3m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(3m);
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}