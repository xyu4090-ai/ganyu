using BaseLib.Abstracts;
using Ganyu.Scripts.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public sealed class OverdrawnPotentialPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private const string _selfDamageKey = "SelfDamage";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar("SelfDamage", 0m, ValueProp.Unblockable | ValueProp.Unpowered)];


    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/overdrawn_potential_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/overdrawn_potential_power.png";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner.Player)
        {
            DamageVar damageVar = (DamageVar)base.DynamicVars["SelfDamage"];
			await CreatureCmd.Damage(choiceContext, base.Owner, damageVar.BaseValue, damageVar.Props, base.Owner, null);

        }
    }
    public void IncrementSelfDamage()
    {
        AssertMutable();
        base.DynamicVars["SelfDamage"].BaseValue+=2;
    }
}