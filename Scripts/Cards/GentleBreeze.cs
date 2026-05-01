using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class GentleBreeze : GanyuCardModel
{
    // 初始化：1费，技能牌，普通，目标为自己（因为是随机敌人，不需要玩家手动选定敌人）
    public GentleBreeze() : base(1, CardType.Skill, CardRarity.Common, TargetType.None, true)
    {
    }

    // 悬浮提示显示风元素
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WindPower>()
    ];

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),               // 初始抽 2 张牌
        new PowerVar<WindPower>(1m)    // 风元素标记 1 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 抽牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);

        // 2. 随机挑选一名存活的敌人并给予风元素
        // 过滤出当前所有存活的敌人
        var aliveEnemies = base.CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
        
        if (aliveEnemies.Count > 0)
        {
            // 从存活敌人中随机选择一个
            // 注：此处使用 System.Random。如果有联机/SL读档完全一致的需求，可以将其替换为引擎自带的随机数生成器（例如 base.CombatState.Rng.Next(aliveEnemies.Count) 之类的API）
            var randomEnemy = base.Owner.RunState.Rng.CombatTargets.NextItem(aliveEnemies);

            await ActionWithContext(choiceContext, async () =>
            {
                // 给予该随机敌人风元素，并尝试触发反应
                await GanyuElementUtils.ApplyWindReaction(
                    randomEnemy, 
                    base.Owner.Creature, 
                    base.CombatState.HittableEnemies, 
                    base.DynamicVars.Power<WindPower>().BaseValue
                );
            });
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}