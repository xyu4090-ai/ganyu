using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class OverdrawnPotential : GanyuCardModel
{
    public OverdrawnPotential() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(4),    // 初始抽 4 张
        new EnergyVar(2)    // 获得 2 点能量
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<OverdrawnPotentialPower>(),
        base.EnergyHoverTip
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 立即抽牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);

        // 2. 立即获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);

        // 3. 赋予“透支潜力”扣血能力
        (await PowerCmd.Apply<OverdrawnPotentialPower>(base.Owner.Creature, 1m, base.Owner.Creature, this))?.IncrementSelfDamage();
    }

    protected override void OnUpgrade()
    {
        // 升级效果：抽牌 4 -> 5
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        // 增加“固有”词条
        AddKeyword(CardKeyword.Innate);
    }
}