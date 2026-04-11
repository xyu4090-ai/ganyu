using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class QilinWalk : GanyuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6m),
        new CalculationExtraVar(2m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
        {
            if (CombatManager.Instance?.History == null || card.CombatState == null)
            {
                return 0;
            }
            return CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => 
                e.HappenedThisTurn(card.CombatState) && 
                e.CardPlay.Card.Type == CardType.Skill && 
                e.CardPlay.Card.Owner == card.Owner);
        })
    ];

    public QilinWalk() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 【核心修复】：必须通过字符串索引器拿到变量并手动 Calculate()
        // 报错日志确认了键名为 "CalculatedBlock"
        if (base.DynamicVars.TryGetValue("CalculatedBlock", out var variable) && variable is CalculatedBlockVar cbv)
        {
            // 显式计算最终格挡值：6 + (技能数 * 2)
            decimal finalBlock = cbv.Calculate(base.Owner.Creature);

            // 调用接收 decimal 的 GainBlock 重载
            // 传入 cbv.Props (ValueProp.Move) 以确保触发敏捷加成
            await CreatureCmd.GainBlock(
                base.Owner.Creature, 
                finalBlock, 
                cbv.Props, 
                cardPlay
            );
        }
    }

    protected override void OnUpgrade()
    {
        if (base.DynamicVars.TryGetValue("CalculationExtra", out var extraVar) && extraVar is CalculationExtraVar cev)
        {
            cev.UpgradeValueBy(1m);
        }
    }
}