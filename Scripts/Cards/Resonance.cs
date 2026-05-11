using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class Resonance : GanyuCardModel
{
    public Resonance() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(5m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target;
        if (target == null || !target.IsAlive) return;

        // 1. 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 检查目标身上非冰/风的元素，获得对应充能
        await ActionWithContext(choiceContext, async () =>
        {
            if (target.GetPower<WetPower>() is { } wp && wp.Amount > 0)
                await GanyuElementUtils.ApplyWaterCharge(base.Owner.Creature, wp.Amount);

            if (target.GetPower<FlamePower>() is { } fp && fp.Amount > 0)
                await GanyuElementUtils.ApplyFireCharge(base.Owner.Creature, fp.Amount);

            if (target.GetPower<ElectroPower>() is { } ep && ep.Amount > 0)
                await GanyuElementUtils.ApplyElectroCharge(base.Owner.Creature, ep.Amount);

            if (target.GetPower<RockPower>() is { } rp && rp.Amount > 0)
                await GanyuElementUtils.ApplyRockCharge(base.Owner.Creature, rp.Amount);
        });

        // 3. 对目标施加 1 层冰元素
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyIceReaction(target, base.Owner.Creature, base.CombatState.HittableEnemies, 1m);
        });
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡 5 -> 8，耗能 1 -> 0
        base.EnergyCost.UpgradeBy(-1);
        base.DynamicVars.Block.UpgradeValueBy(3);
    }
}
