using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Powers;

public class StarfallPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/starfall.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/starfall.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>[new DamageVar(25m, ValueProp.Unpowered)];

	public void SetDamage(decimal damage)
	{
		AssertMutable();
		base.DynamicVars.Damage.BaseValue += damage;
	}

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            SetDamage(base.Owner.Block);
            // 对全体敌人造成 25 点伤害
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage.BaseValue, ValueProp.Unpowered, base.Owner, null);

            // 触发后减少一层
            await PowerCmd.TickDownDuration(this);
        }
    }
}