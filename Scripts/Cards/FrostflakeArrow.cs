using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
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
public sealed class FrostflakeArrow : GanyuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(12m),    // 基础伤害 12
        new ExtraDamageVar(8m),       // 额外伤害 8
        // 核心逻辑：自动根据目标是否有冰元素计算最终伤害
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? target) => 
        {
            // 如果目标存在且已有“冰元素”标记，乘数为 1，触发 ExtraDamage
            if (target != null && target.GetPower<IcePower>() != null)
            {
                return 1m;
            }
            return 0m;
        }),
        new PowerVar<IcePower>(1m)    // 再次给予的冰元素层数
    ];

    public FrostflakeArrow() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害。由于使用了 CalculatedDamage，当目标有冰元素时会自动显示 20 点伤害
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 额外效果：如果目标已有冰元素，再次给予 1 层冰元素并尝试反应
        if (cardPlay.Target.IsAlive && cardPlay.Target.GetPower<IcePower>() != null)
        {
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyIceReaction(
                    cardPlay.Target, 
                    base.Owner.Creature, 
                    base.CombatState.HittableEnemies, 
                    base.DynamicVars.Power<IcePower>().BaseValue
                );
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：基础伤害从 12 增加至 16 (+4)
        base.DynamicVars.CalculationBase.UpgradeValueBy(4m);
    }
}