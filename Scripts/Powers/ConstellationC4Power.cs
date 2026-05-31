using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public class ConstellationC4Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/constellation_c4.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/constellation_c4.png";

    // 四命西狩：自身拥有降众天华时，卡牌造成的伤害提高 25%
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner)
        {
            return 1m;
        }
        if (cardSource == null)
        {
            return 1m;
        }
        if (base.Owner.GetPower<HeavenlyFallBuffPower>() == null)
        {
            return 1m;
        }
        return 1.25m;
    }
}
