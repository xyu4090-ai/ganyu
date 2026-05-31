using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class OverdrawnPotential : GanyuCardModel
{
    public OverdrawnPotential() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<OverdrawnPotentialPower>(),
        HoverTipFactory.FromPower<HeavenlyFallBuffPower>(),
        HoverTipFactory.FromPower<TracesQilinPower>(),
        base.EnergyHoverTip
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得 1 层降众天华
        await PowerCmd.Apply<HeavenlyFallBuffPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);

        // 获得 1 层山泽麟迹
        await PowerCmd.Apply<TracesQilinPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);

        // 通过遗物立即解锁所有命之座
        var activeRelic = HeavenlyFall.ActiveInstance;
        if (activeRelic is HeavenlyFall hf)
        {
            await hf.IncreaseTemporaryConstellationImmediate(choiceContext, 6, this);
        }
        else if (activeRelic is HeavenlyFallUP hfUp)
        {
            await hfUp.IncreaseTemporaryConstellationImmediate(choiceContext, 6, this);
        }

        // 赋予"透支潜力"扣血能力
        await PowerCmd.Apply<OverdrawnPotentialPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：费用 1 -> 0
        base.EnergyCost.UpgradeBy(-1);
    }
}
