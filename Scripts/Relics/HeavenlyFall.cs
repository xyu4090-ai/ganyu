using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Cards;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Relics;

/// <summary>
/// 动态变量子类，始终返回最新的 CurrentConstellation 值，
/// 确保遗物描述文本中的 {Constellation} 始终显示当前命座等级。
/// </summary>
internal class ConstellationDynamicVar : DynamicVar
{
    private readonly Func<int> _getValue;

    public ConstellationDynamicVar(Func<int> getValue) : base("Constellation", getValue())
    {
        _getValue = getValue;
    }

    protected override decimal GetBaseValueForIConvertible() => _getValue();

    // SmartFormat 通过 ToString() 获取显示文本，基类返回缓存的 _baseValue，
    // 必须用 new 隐藏以返回实时值。
    public new string ToString() => _getValue().ToString();
}

[Pool(typeof(GanyuRelicPool))]
public class HeavenlyFall : CustomRelicModel
{
    public override string PackedIconPath => "res://Ganyu/images/relics/heavenly_fall_small.png";
    protected override string PackedIconOutlinePath => "res://Ganyu/images/relics/heavenly_fall_small.png";
    protected override string BigIconPath => "res://Ganyu/images/relics/heavenly_fall.png";

    private const int DefaultBaseConstellation = 1;

    /// <summary>
    /// 当前战斗中激活的命之座遗物实例（HeavenlyFall 或 HeavenlyFallUP）
    /// </summary>
    public static CustomRelicModel? ActiveInstance { get; internal set; }

    /// <summary>
    /// 用于在遗物替换时传递当前命座等级
    /// </summary>
    public static int TransferConstellation { get; set; } = DefaultBaseConstellation;

    private int _temporaryBoost;
    private int _appliedConstellation;

    [SavedProperty]
    public int BaseConstellation
    {
        get => _baseConstellation;
        set
        {
            _baseConstellation = value;
            TransferConstellation = value; // 同步到静态变量
        }
    }
    private int _baseConstellation = DefaultBaseConstellation;

    public int CurrentConstellation => Math.Min(BaseConstellation + _temporaryBoost, 6);

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => CurrentConstellation;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ConstellationDynamicVar(() => CurrentConstellation)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ConstellationC1Power>(),
        HoverTipFactory.FromPower<ConstellationC2Power>(),
        HoverTipFactory.FromPower<ConstellationC3Power>(),
        HoverTipFactory.FromPower<ConstellationC4Power>(),
        HoverTipFactory.FromPower<ConstellationC5Power>(),
        HoverTipFactory.FromPower<ConstellationC6Power>()
    ];

    /// <summary>
    /// 同步 DynamicVars 中的 Constellation 值，确保遗物描述文本正确显示。
    /// </summary>
    private void SyncConstellationDynamicVar()
    {
        if (DynamicVars.TryGetValue("Constellation", out var var))
        {
            var.BaseValue = CurrentConstellation;
        }
    }

    /// <summary>
    /// 永久提升命之座等级（通过事件）
    /// </summary>
    public void IncreaseBaseConstellation(int amount = 1)
    {
        BaseConstellation = Math.Min(BaseConstellation + amount, 6);
        SyncConstellationDynamicVar();
        InvokeDisplayAmountChanged();
    }

    /// <summary>
    /// 临时提升命之座等级（通过卡牌，战斗内有效）
    /// </summary>
    public void IncreaseTemporaryConstellation(int amount = 1)
    {
        _temporaryBoost = Math.Min(_temporaryBoost + amount, 6 - BaseConstellation);
        SyncConstellationDynamicVar();
        InvokeDisplayAmountChanged();
    }

    /// <summary>
    /// 临时提升命之座等级并立即施加 buff
    /// </summary>
    public async Task IncreaseTemporaryConstellationImmediate(PlayerChoiceContext choiceContext, int amount, CardModel? cardSource = null)
    {
        int before = CurrentConstellation;
        IncreaseTemporaryConstellation(amount);
        int after = CurrentConstellation;
        if (after > before)
        {
            await ApplyConstellationBuffs(choiceContext, base.Owner.Creature, before, after, cardSource);
            _appliedConstellation = after;
        }
    }

    private async Task ApplyConstellationBuffs(PlayerChoiceContext choiceContext, Creature owner, int fromLevel, int toLevel, CardModel? cardSource = null)
    {
        for (int level = fromLevel + 1; level <= toLevel; level++)
        {
            switch (level)
            {
                case 1: await PowerCmd.Apply<ConstellationC1Power>(choiceContext, owner, 1m, owner, cardSource); break;
                case 2: await PowerCmd.Apply<ConstellationC2Power>(choiceContext, owner, 1m, owner, cardSource); break;
                case 3: await PowerCmd.Apply<ConstellationC3Power>(choiceContext, owner, 1m, owner, cardSource); HeavenlyFallBuffPower.UpdateDynamicVars(owner); break;
                case 4: await PowerCmd.Apply<ConstellationC4Power>(choiceContext, owner, 1m, owner, cardSource); break;
                case 5: await PowerCmd.Apply<ConstellationC5Power>(choiceContext, owner, 1m, owner, cardSource); break;
                case 6: await PowerCmd.Apply<ConstellationC6Power>(choiceContext, owner, 1m, owner, cardSource); break;
            }
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != base.Owner.Creature.Side) return;

        var player = base.Owner;
        var current = CurrentConstellation;

        // 第一回合：注册激活实例、施加初始命座buff并添加霜华矢
        if (combatState.RoundNumber <= 1)
        {
            ActiveInstance = this;
            Flash();
            _temporaryBoost = 0;
            _appliedConstellation = current;
            await ApplyConstellationBuffs(choiceContext, base.Owner.Creature, 0, current);

            var frostflakeArrow = combatState.CreateCard<FrostflakeArrow>(player);
            await CardPileCmd.Add(frostflakeArrow, PileType.Hand);
        }
        // 后续回合：检查是否有新解锁的命座
        else if (current > _appliedConstellation)
        {
            Flash();
            await ApplyConstellationBuffs(choiceContext, base.Owner.Creature, _appliedConstellation, current);
            _appliedConstellation = current;
        }
    }

    public override async Task AfterObtained()
    {
        // 新一轮获取遗物时，同步静态变量，防止上一轮的残留值影响 HeavenlyFallUP
        TransferConstellation = BaseConstellation;
    }

    public override async Task AfterCombatEnd(CombatRoom _)
    {
        _temporaryBoost = 0;
        _appliedConstellation = 0;
        InvokeDisplayAmountChanged();
    }
}
