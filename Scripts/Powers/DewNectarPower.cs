using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
namespace Ganyu.Scripts.Powers;

public class DewNectarPower : CustomPowerModel
{
    // 类型，Buff或Debuff
    public override PowerType Type => PowerType.Buff;
    // 叠加类型，Counter表示可叠加，Single表示不可叠加
    public override PowerStackType StackType => PowerStackType.Single;

    // 自定义图标路径，自己指定，或者创建一个基类来统一指定图标路径
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/dew_nectar_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/dew_nectar_power.png";
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack_())
        {
            return 1m;
        }
        if (cardSource == null)
        {
            return 1m;
        }
        if (dealer != base.Owner && !base.Owner.Pets.Contains<Creature>(dealer))
        {
            return 1m;
        }
        if (target == null )
        {
            return 1m;
        }
        return 1.15m;
    }
}