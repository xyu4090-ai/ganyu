using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class Resonance : GanyuCardModel
{
    public Resonance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WindPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target;
        // 获取除目标以外的所有存活敌人
        var otherEnemies = base.CombatState.HittableEnemies.Where(e => e != target && e.IsAlive).ToList();
        
        if (otherEnemies.Count == 0) return;

        // 1. 记录目标身上存在的元素类型及其层数
        var elementData = new List<(Type PowerType, decimal Amount)>();
        
        if (target.GetPower<IcePower>() is { } ip) elementData.Add((typeof(IcePower), ip.Amount));
        if (target.GetPower<WetPower>() is { } wp) elementData.Add((typeof(WetPower), wp.Amount));
        if (target.GetPower<FlamePower>() is { } fp) elementData.Add((typeof(FlamePower), fp.Amount));
        if (target.GetPower<ElectroPower>() is { } ep) elementData.Add((typeof(ElectroPower), ep.Amount));
        if (target.GetPower<WindPower>() is { } winp) elementData.Add((typeof(WindPower), winp.Amount));
        if (target.GetPower<RockPower>() is { } rp) elementData.Add((typeof(RockPower), rp.Amount));

        // 2. 将这些元素分摊给其他敌人
        foreach (var data in elementData)
        {
            foreach (var enemy in otherEnemies)
            {
                await ActionWithContext(choiceContext, async () =>
                {
                    // 根据记录的类型调用对应的 Apply 方法，以触发可能的元素反应
                    if (data.PowerType == typeof(IcePower)) 
                        await GanyuElementUtils.ApplyIceReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies, data.Amount);
                    else if (data.PowerType == typeof(WetPower))
                        await GanyuElementUtils.ApplyWaterReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies, data.Amount);
                    else if (data.PowerType == typeof(FlamePower))
                        await GanyuElementUtils.ApplyFireReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies, data.Amount);
                    else if (data.PowerType == typeof(ElectroPower))
                        await GanyuElementUtils.ApplyElectroReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies, data.Amount);
                    else if (data.PowerType == typeof(WindPower))
                        await GanyuElementUtils.ApplyWindReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies, data.Amount);
                    else if (data.PowerType == typeof(RockPower))
                        await GanyuElementUtils.ApplyRockReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies, data.Amount);
                });
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：费用 1 -> 0
        base.EnergyCost.UpgradeBy(-1);
    }
}