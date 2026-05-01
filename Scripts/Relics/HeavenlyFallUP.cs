// 加入哪个遗物池，此处为通用
using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Relics;

[Pool(typeof(GanyuRelicPool))]
public class HeavenlyFallUP : CustomRelicModel
{
    // 小图标
    public override string PackedIconPath => $"res://Ganyu/images/relics/heavenly_fall_up_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Ganyu/images/relics/heavenly_fall_up_small.png";
    // 大图标
    protected override string BigIconPath => $"res://Ganyu/images/relics/heavenly_fall_up.png";

    public const int turnsThreshold = 5;

    private const string _turnsKey = "Turns";

    private bool _isActivating;

    private int _turnsSeen;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            if (!IsActivating)
            {
                return TurnsSeen;
            }
            return base.DynamicVars["Turns"].IntValue;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new DynamicVar("Turns", 3m),
        new PowerVar<IcePower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromPower<DewNectarPower>(),
        HoverTipFactory.FromPower<IcePower>()
    ];

    private bool IsActivating
    {
        get
        {
            return _isActivating;
        }
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int TurnsSeen
    {
        get
        {
            return _turnsSeen;
        }
        set
        {
            AssertMutable();
            _turnsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState ICombatState)
    {
        if (side == base.Owner.Creature.Side && ICombatState.RoundNumber <= 1)
        {
            Flash();
            await PowerCmd.Apply<IcePower>(choiceContext, ICombatState.HittableEnemies, base.DynamicVars["IcePower"].BaseValue, base.Owner.Creature, null);
        }
        if (side == base.Owner.Creature.Side)
        {
            TurnsSeen = (TurnsSeen + 1) % base.DynamicVars["Turns"].IntValue;
            base.Status = ((TurnsSeen == base.DynamicVars["Turns"].IntValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
            if (TurnsSeen == 0)
            {
                await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
                await PowerCmd.Apply<DewNectarPower>(choiceContext,base.Owner.Creature, 1m, base.Owner.Creature, null);
                Flash();
            }
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}