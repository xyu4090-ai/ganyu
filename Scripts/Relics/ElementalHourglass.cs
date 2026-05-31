using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace Ganyu.Scripts.Relics;

[Pool(typeof(GanyuRelicPool))]
public class ElementalHourglass : CustomRelicModel
{
    // 小图标
    public override string PackedIconPath => $"res://Ganyu/images/relics/elemental_hourglass_small.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://Ganyu/images/relics/elemental_hourglass_small.png";
    // 大图标
    protected override string BigIconPath => $"res://Ganyu/images/relics/elemental_hourglass.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IceChargePower>(),
        HoverTipFactory.FromPower<WaterChargePower>(),
        HoverTipFactory.FromPower<FireChargePower>(),
        HoverTipFactory.FromPower<ElectroChargePower>(),
        HoverTipFactory.FromPower<RockChargePower>(),
        HoverTipFactory.FromPower<WindChargePower>()
    ];

    public override RelicRarity Rarity => RelicRarity.Starter;
}
