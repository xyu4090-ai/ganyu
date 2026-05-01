using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class IgniteFlame : GanyuCardModel
{
    // 0费，技能卡，普通稀有度，作用于全体敌人
    public IgniteFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),    // 初始抽 1 张牌   
    new PowerVar<FlamePower>(1m) 
];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 给予全体敌人 1 层火元素并触发反应逻辑
        await ActionWithContext(choiceContext, async () =>
        {
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    // 传入 1 层火元素
                    await GanyuElementUtils.ApplyFireReaction(
                        enemy,
                        base.Owner.Creature,
                        base.CombatState.HittableEnemies,
                        1m
                    );
                }
            }
        });

        // 2. 抽 1 张牌 (假设你的项目中通过 PlayerCmd 或类似机制处理抽牌)
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}