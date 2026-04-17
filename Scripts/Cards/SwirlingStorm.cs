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

[Pool(typeof(GanyuCardPool))] // 加入甘雨卡池
public sealed class SwirlingStorm : GanyuCardModel
{
    public SwirlingStorm() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 初始给予 1 层风元素
        new PowerVar<WindPower>(1m),
        // 抽 2 张牌
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 在卡牌界面显示风元素的提示
        HoverTipFactory.FromPower<WindPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 对所有敌人施加风元素并尝试触发扩散反应
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            if (enemy.IsAlive)
            {
                // 调用工具类，传入全场敌人列表以支持扩散逻辑
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyWindReaction(
                        enemy, 
                        base.Owner.Creature, 
                        base.CombatState.HittableEnemies,
                        base.DynamicVars.Power<WindPower>().BaseValue
                    );
                });
            }
        }

        // 2. 抽牌逻辑
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：费用 2 -> 1
        base.EnergyCost.UpgradeBy(-1);
    }
}