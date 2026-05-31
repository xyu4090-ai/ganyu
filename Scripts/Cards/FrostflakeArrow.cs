using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(TokenCardPool))]
public sealed class FrostflakeArrow : GanyuCardModel
{
    public FrostflakeArrow() : base(3, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, true)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, ChargeKeyword.Charge];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move),       // 对目标伤害 4
        new CalculationExtraVar(8m),              // 对全体双倍伤害 8
        new PowerVar<IcePower>(1m)                 // 冰元素 1 层
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;

        // 提前读取活力数值（第一段攻击后活力会被消耗，需手动加到第二段）
        decimal vigorAmount = owner.GetPower<VigorPower>()?.Amount ?? 0m;

        // 1. 对目标造成伤害（正常吃活力）
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 给予目标冰元素并触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyIceReaction(
                cardPlay.Target,
                owner,
                base.CombatState.HittableEnemies,
                base.DynamicVars.Power<IcePower>().BaseValue
            );
        });

        // 3. 对全体敌人造成双倍伤害（手动加上活力数值，变相吃活力）
        await DamageCmd.Attack(base.DynamicVars["CalculationExtra"].BaseValue + vigorAmount*2)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);

        // 4. 给予全体敌人冰元素并触发反应
        foreach (var enemy in base.CombatState.HittableEnemies)
        {
            if (enemy.IsAlive)
            {
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyIceReaction(
                        enemy,
                        owner,
                        base.CombatState.HittableEnemies,
                        base.DynamicVars.Power<IcePower>().BaseValue
                    );
                });
            }
        }

        // 5. 每场战斗首次打出时获得2点能量
        if (!_playedThisCombat)
        {
            _playedThisCombat = true;
            await PlayerCmd.GainEnergy(2, base.Owner);
        }

        // 6. 每次打出后本场战斗费用减少1
        _playCount++;
        int newCost = System.Math.Max(0, 3 - _playCount);
        base.EnergyCost.SetThisCombat(newCost);

    }
    // 追踪每场战斗是否已打出过
    private static bool _playedThisCombat;
    // 追踪本场战斗打出次数
    private static int _playCount;

    public static void ResetCombatState()
    {
        _playedThisCombat = false;
        _playCount = 0;
    }
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
	{
        if (card.Id!=this.Id || base.Owner.Creature.GetPower<ArcheryMasteryPower>() == null)
        {
            return playCount;
        }
        int count=base.Owner.Creature.GetPower<ArcheryMasteryPower>().Amount;
		return playCount + count;
	}

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 15 -> 19 (+4)；双倍伤害 30 -> 38 (+8)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["CalculationExtra"].UpgradeValueBy(8m);
    }

}
