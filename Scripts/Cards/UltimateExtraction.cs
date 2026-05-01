using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class UltimateExtraction : GanyuCardModel
{
    // 初始化：3费，技能牌，稀有，目标为全体敌人
    public UltimateExtraction() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    // 显式声明消耗关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 定义每层元素转化的伤害值 (初始 5 点)
        new DamageVar(5m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int totalRemovedStacks = 0;

        // 1. 统计并移除全体敌人身上的所有元素层数
        foreach (var enemy in base.CombatState.HittableEnemies.Where(e => e.IsAlive))
        {
            // 定义需要清除的元素列表
            var elementPowers = new List<CustomPowerModel?>
            {
                enemy.GetPower<IcePower>(),
                enemy.GetPower<WetPower>(),
                enemy.GetPower<FlamePower>(),
                enemy.GetPower<ElectroPower>(),
                enemy.GetPower<WindPower>(),
                enemy.GetPower<RockPower>()
            };

            foreach (var power in elementPowers)
            {
                if (power != null && power.Amount > 0)
                {
                    totalRemovedStacks += (int)power.Amount;
                    await PowerCmd.Remove(power); // 移除该元素
                }
            }
        }

        // 2. 如果成功移除了元素，则结算全体伤害
        if (totalRemovedStacks > 0)
        {
            // 计算总伤害 = 移除总层数 * 每层伤害
            decimal totalDamage = totalRemovedStacks * base.DynamicVars.Damage.BaseValue;

            await DamageCmd.Attack(totalDamage)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx(null, null, "heavy_impact.mp3") // 增加爆发感音效
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后：每层伤害从 5 点提升至 8 点
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}