using BaseLib.Abstracts;
using BaseLib.Extensions;
using Ganyu.Scripts.Cards;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Ganyu.Scripts;

public class ChargeSingleton : CustomSingletonModel
{
    private readonly Dictionary<CardModel, int> _chargeStacks = new();

    public ChargeSingleton() : base(true, true)
    {
    }


    public override async Task BeforeSideTurnStart(PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;

        var playerCreature = participants.FirstOrDefault(c => c.Player != null);
        if (playerCreature?.Player == null) return;

        var handCards = PileType.Hand.GetPile(playerCreature.Player).Cards;
        bool hasAutoCharge = playerCreature.GetPower<AutoChargePower>() != null;

        foreach (var card in handCards)
        {
            if (card.Keywords.Contains(ChargeKeyword.Charge)&&combatState.RoundNumber>1)
            {
                ApplyChargeBonus(card);
            }
        }

        if (hasAutoCharge && combatState.RoundNumber > 1)
        {
            foreach (var card in PileType.Deck.GetPile(playerCreature.Player).Cards)
            {
                if (card.Keywords.Contains(ChargeKeyword.Charge))
                    ApplyChargeBonus(card);
            }
            foreach (var card in PileType.Discard.GetPile(playerCreature.Player).Cards)
            {
                if (card.Keywords.Contains(ChargeKeyword.Charge))
                    ApplyChargeBonus(card);
            }
        }
    }

    private static void ApplyChargeBonus(CardModel card)
    {
        int bonus = 2;
        var focusPower = card.Owner.Creature?.GetPower<FocusPower>();
        if (focusPower != null && focusPower.Amount > 0)
        {
            bonus += (int)focusPower.Amount;
        }
        var tempChargePower = card.Owner.Creature?.GetPower<TempChargePower>();
        if (tempChargePower != null && tempChargePower.Amount > 0)
        {
            bonus += (int)tempChargePower.Amount;
        }
        TryIncreaseDynamicVar(card, "Damage", bonus);
        TryIncreaseDynamicVar(card, "Block", bonus);
        TryIncreaseDynamicVar(card, "CalculationBase", bonus);
        TryIncreaseDynamicVar(card, "CalculationExtra", bonus * 2);
        TryIncreaseDynamicVar(card, "ExtraDamage", bonus);
    }

    private static void TryIncreaseDynamicVar(CardModel card, string varName, int bonus)
    {
        try
        {
            var dynamicVar = card.GetDynamicVar(varName);
            if (dynamicVar != null)
            {
                dynamicVar.BaseValue += bonus;
            }
        }
        catch
        {
            // 该卡牌没有此变量，忽略
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {

        if (cardPlay.Card != null && cardPlay.Card.Keywords.Contains(ChargeKeyword.Charge))
        {
            _chargeStacks.Remove(cardPlay.Card);
        }
    }
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
	{
        if (card.Owner.Creature.GetPower<TracesQilinPower>() == null || card.Owner.Creature.GetPower<ConstellationC6Power>() == null)
        {
            return playCount;
        }
        if(!card.Keywords.Contains(ChargeKeyword.Charge))
        {
            return playCount;
        }
		return playCount + 1;
	}

    public override async Task AfterCombatEnd(CombatRoom _)
    {
        _chargeStacks.Clear();
        FrostflakeArrow.ResetCombatState();
    }
    public override async Task BeforeCombatStart()
    {
        _chargeStacks.Clear();
        FrostflakeArrow.ResetCombatState();
    }
}
