using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class ScorchedEarthTactics : GanyuCardModel
{
    public override bool GainsBlock => true; // 标识该卡牌会提供格挡

    public ScorchedEarthTactics() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromPower<FlamePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5m, ValueProp.Move),    // 基础格挡：8点
        new PowerVar<FlamePower>(2m)         // 赋予目标 2 层火元素
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // 2. 参考 TrueGrit，呼出选择界面，让玩家手动选择手牌中的 1 张牌
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), 
            context: choiceContext, 
            player: base.Owner, 
            filter: null, 
            source: this
        )).FirstOrDefault();

        if (cardModel != null)
        {
            // 记录所选卡牌是否为攻击牌
            bool isAttackCard = cardModel.Type == CardType.Attack;

            // 消耗该牌
            await CardCmd.Exhaust(choiceContext, cardModel);

            // 3. 如果消耗的是攻击牌，随机给予一名存活敌人火元素
            if (isAttackCard)
            {
                var aliveEnemies = base.CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
                if (aliveEnemies.Count > 0)
                {
                    // 使用游戏的随机数生成器从存活敌人中随机抽取一名
                    var targetEnemy = base.Owner.RunState.Rng.CombatTargets.NextItem(aliveEnemies);
                    
                    if (targetEnemy != null)
                    {
                        await ActionWithContext(choiceContext, async () =>
                        {
                            await GanyuElementUtils.ApplyFireReaction(
                                targetEnemy, 
                                base.Owner.Creature, 
                                base.CombatState.HittableEnemies, 
                                DynamicVars.Power<FlamePower>().BaseValue
                            );
                        });
                    }
                }
            }
            else
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后格挡提升 3 点 (8 -> 11)
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars.Power<FlamePower>().UpgradeValueBy(1m);
    }
}