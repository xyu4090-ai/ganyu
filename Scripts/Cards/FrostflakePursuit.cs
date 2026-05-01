using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Extensions;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class FrostflakePursuit : GanyuCardModel
{
    public FrostflakePursuit() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    // 当满足条件时，卡牌会发金光提示玩家
    protected override bool ShouldGlowGoldInternal => GanyuElementUtils.ReactionsThisTurn > 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12m, ValueProp.Move), // 基础伤害 12
        new PowerVar<IcePower>(1m)          // 冰元素 1 层
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 如果本回合触发过元素反应，额外给予冰元素并尝试引发反应
        if (GanyuElementUtils.ReactionsThisTurn > 0 && cardPlay.Target != null && cardPlay.Target.IsAlive)
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
        // 升级效果：伤害增加 4 点 (12 -> 16)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars.Power<IcePower>().UpgradeValueBy(1m);
    }

    // --- 以下是动态减费的核心逻辑 ---

    // 当卡牌进入手牌（抽牌/生成）时检查
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this) return Task.CompletedTask;
        if (GanyuElementUtils.ReactionsThisTurn > 0)
        {
            ReduceCost();
        }
        return Task.CompletedTask;
    }

    // 当玩家打出任意卡牌后检查（绝大多数反应是由打牌触发的）
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (GanyuElementUtils.ReactionsThisTurn > 0)
        {
            ReduceCost();
        }
        return Task.CompletedTask;
    }

    // 当回合开始时检查（用于处理遗物在回合开始阶段触发的反应）
    public override Task AfterSideTurnStart(CombatSide side, ICombatState ICombatState)
    {
        if (side == base.Owner.Creature.Side && GanyuElementUtils.ReactionsThisTurn > 0)
        {
            ReduceCost();
        }
        return Task.CompletedTask;
    }

    // 执行本回合费用设为 0 的操作
    private void ReduceCost()
    {
        base.EnergyCost.SetThisTurn(0);
    }
}