using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class TempChargePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldReceiveCombatHooks => true;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/temp_charge_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/temp_charge_power.png";

    private readonly Dictionary<CardModel, Dictionary<string, decimal>> _modifiedVars = new();

    private static readonly string[] ChargeVarNames = ["Damage", "Block", "CalculationBase", "CalculationExtra", "ExtraDamage"];

    public void ApplyBonusToHand()
    {
        int bonus = (int)base.Amount;
        _modifiedVars.Clear();

        var handCards = PileType.Hand.GetPile(base.Owner.Player).Cards;
        foreach (var card in handCards)
        {
            if (!card.Keywords.Contains(ChargeKeyword.Charge)) continue;

            var modifications = new Dictionary<string, decimal>();
            foreach (var varName in ChargeVarNames)
            {
                try
                {
                    var dynamicVar = card.GetDynamicVar(varName);
                    if (dynamicVar != null)
                    {
                        decimal actualBonus = varName == "CalculationExtra" ? bonus * 2 : bonus;
                        dynamicVar.BaseValue += actualBonus;
                        modifications[varName] = actualBonus;
                    }
                }
                catch { }
            }
            if (modifications.Count > 0)
            {
                _modifiedVars[card] = modifications;
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return;

        foreach (var (card, mods) in _modifiedVars)
        {
            foreach (var (varName, amount) in mods)
            {
                try
                {
                    var dynamicVar = card.GetDynamicVar(varName);
                    if (dynamicVar != null)
                    {
                        dynamicVar.BaseValue -= amount;
                    }
                }
                catch { }
            }
        }
        _modifiedVars.Clear();

        await PowerCmd.Remove(this);
    }
}
