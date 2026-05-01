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
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class Thunderclap : GanyuCardModel
{
    public Thunderclap() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 给予 1 层雷元素
        new PowerVar<ElectroPower>(1m),
        // 初始抽 1 张牌
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 自动关联雷元素的浮窗说明
        HoverTipFactory.FromPower<ElectroPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 对全体敌人施加雷元素
        // 遍历所有可攻击的敌人
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            if (enemy.IsAlive)
            {
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyElectroReaction(
                        enemy, 
                        base.Owner.Creature, 
                        base.CombatState.HittableEnemies, 
                        base.DynamicVars.Power<ElectroPower>().BaseValue
                    );
                });
            }
        }

        // 2. 抽牌逻辑
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Power<ElectroPower>().UpgradeValueBy(1m);
    }
}