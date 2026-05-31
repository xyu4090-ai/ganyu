using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class KaleidoscopeElements : GanyuCardModel
{
    public KaleidoscopeElements() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move), // 基础伤害 4
        new RepeatVar(5)                   // 攻击次数 5
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 提示玩家所有可能触发的 6 种元素
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = base.DynamicVars.Repeat.IntValue;

        // 使用 WithHitCount 确保每段伤害都正确应用活力等加成（参考元素交响曲）
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(count)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 每段命中后给予 1 层随机元素
        for (int i = 0; i < count; i++)
        {
            if (cardPlay.Target == null || !cardPlay.Target.IsAlive) break;

            int choice = base.Owner.RunState.Rng.CombatCardSelection.NextInt(0, 6);

            await ActionWithContext(choiceContext, async () =>
            {
                switch (choice)
                {
                    case 0: await GanyuElementUtils.ApplyIceReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                    case 1: await GanyuElementUtils.ApplyWaterReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                    case 2: await GanyuElementUtils.ApplyFireReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                    case 3: await GanyuElementUtils.ApplyElectroReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                    case 4: await GanyuElementUtils.ApplyWindReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                    case 5: await GanyuElementUtils.ApplyRockReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                }
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害从 4 -> 6 (+2)
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}