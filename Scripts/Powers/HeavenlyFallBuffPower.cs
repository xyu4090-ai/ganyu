using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Powers;

public class HeavenlyFallBuffPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/heavenly_fall_buff.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/heavenly_fall_buff.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new DamageVar(15m, ValueProp.Unpowered)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    /// <summary>
    /// 根据命之座状态更新动态变量（能量和伤害）
    /// </summary>
    public static void UpdateDynamicVars(Creature owner)
    {
        var power = owner.GetPower<HeavenlyFallBuffPower>();
        if (power == null) return;

        bool hasC3 = owner.GetPower<ConstellationC3Power>() != null;
        power.DynamicVars.Energy.BaseValue = hasC3 ? 2m : 1m;
        power.DynamicVars.Damage.BaseValue = hasC3 ? 25m : 15m;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState ICombatState)
    {
        if (side == base.Owner.Side && base.Owner.Player != null)
        {
            Flash();
            UpdateDynamicVars(base.Owner);

            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner.Player);
            GanyuAudioHelper.PlayOneShot("res://Ganyu/audios/ganyu_10.mp3");

            foreach (var enemy in ICombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    await GanyuElementUtils.ApplyIceReaction(enemy, base.Owner, ICombatState.HittableEnemies, 1m);
                }
            }

            await PowerCmd.TickDownDuration(this);
        }
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState ICombatState)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            UpdateDynamicVars(base.Owner);

            int baseDamage = (int)base.DynamicVars.Damage.BaseValue;
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, baseDamage, ValueProp.Unpowered, base.Owner, null);
        }
    }
}
