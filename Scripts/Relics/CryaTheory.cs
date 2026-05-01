// 加入哪个遗物池，此处为通用
using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
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
public class CryaTheory : CustomRelicModel
{
    // 小图标
    public override string PackedIconPath => $"res://Ganyu/images/relics/crya_theory_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Ganyu/images/relics/crya_theory_small.png";
    // 大图标
    protected override string BigIconPath => $"res://Ganyu/images/relics/crya_theory.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<MeltPower>(),
        HoverTipFactory.FromPower<FreezingDebuffPower>(),
        HoverTipFactory.FromPower<SwirlPower>(),
        HoverTipFactory.FromPower<CrystalizePower>(),
        HoverTipFactory.FromPower<ConductPower>(),
        HoverTipFactory.FromPower<TracesQilinPower>(),
        HoverTipFactory.FromPower<HeavenlyFallBuffPower>(),
        HoverTipFactory.FromPower<OathSilveryMoonPower>(),
        HoverTipFactory.FromPower<OceanborneFeatherPower>(),
        HoverTipFactory.FromPower<GuobaAttackPower>(),
        HoverTipFactory.FromPower<PyronadoPower>(),
        HoverTipFactory.FromPower<BalefulOmenPower>(),
        HoverTipFactory.FromPower<MusouShinsetsuPower>(),
        HoverTipFactory.FromPower<JadeShieldPower>(),
        HoverTipFactory.FromPower<StarfallPower>(),
        HoverTipFactory.FromPower<WindSpirit6308Power>(),
        HoverTipFactory.FromPower<ForbiddenWindSpirit75Power>()
    ];
    public override RelicRarity Rarity => RelicRarity.Starter;


    public override async Task BeforeCombatStart()
    {

        GanyuElementUtils.ResetReactionCount();

    }
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState ICombatState)
    {
        // 如果是玩家回合开始，重置本回合的反应计数器
        if (side == base.Owner.Creature.Side)
        {
            GanyuElementUtils.ResetTurnCount();
        }
    }

}