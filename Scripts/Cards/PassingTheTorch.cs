using System.Collections.Generic;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class PassingTheTorch : GanyuCardModel
{
    // 初始化：1费，攻击牌，罕见，目标为单体敌人
    public PassingTheTorch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    // 悬浮提示显示火元素
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>()
    ];

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move), // 基础伤害 6
        new PowerVar<FlamePower>(1m),      // 火元素标记 1 层
        new CardsVar(1)                    // 条件触发时的抽牌数量 1
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 在造成伤害和挂元素之前，提前记录目标是否已经拥有火元素
        var flamePower = cardPlay.Target?.GetPower<FlamePower>();
        bool hasFireBeforeHit = flamePower != null && flamePower.Amount > 0;

        // 2. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 3. 给予目标火元素并尝试触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyFireReaction(
                cardPlay.Target,
                base.Owner.Creature,
                base.CombatState.HittableEnemies,
                base.DynamicVars.Power<FlamePower>().BaseValue
            );
        });

        // 4. 根据记录的状态，如果目标原本就有火元素，则抽牌
        if (hasFireBeforeHit)
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害增加 3 点 (6 -> 9)，条件触发的抽牌数量增加 1 张 (1 -> 2)
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        
        // 火元素层数保持 1 层不变，不需要写升级逻辑
    }
}