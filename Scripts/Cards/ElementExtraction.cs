using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementExtraction : GanyuCardModel
{
    public ElementExtraction() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            int totalRemoved = 0;
            var target = cardPlay.Target;

            var elementPowers = new List<PowerModel>();
            if (target.GetPower<IcePower>() is { Amount: > 0 } ip) elementPowers.Add(ip);
            if (target.GetPower<WetPower>() is { Amount: > 0 } wp) elementPowers.Add(wp);
            if (target.GetPower<FlamePower>() is { Amount: > 0 } fp) elementPowers.Add(fp);
            if (target.GetPower<ElectroPower>() is { Amount: > 0 } ep) elementPowers.Add(ep);
            if (target.GetPower<WindPower>() is { Amount: > 0 } winp) elementPowers.Add(winp);
            if (target.GetPower<RockPower>() is { Amount: > 0 } rp) elementPowers.Add(rp);

            foreach (var power in elementPowers)
            {
                totalRemoved += (int)power.Amount;
                await PowerCmd.Remove(power);
            }

            if (totalRemoved > 0)
            {
                decimal blockPerStack = DynamicVars.CalculationBase.BaseValue;
                await CreatureCmd.GainBlock(Owner.Creature, totalRemoved * blockPerStack, ValueProp.Unpowered, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
    }
}