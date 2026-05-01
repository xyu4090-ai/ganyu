using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Ganyu.Scripts.Powers;
using BaseLib.Utils;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class Efficiency : GanyuCardModel
{
    public Efficiency() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 升级效果：抽 1 张牌 -> 抽 2 张牌
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家能力，层数决定每次反应抽几张
        await PowerCmd.Apply<EfficiencyPower>(choiceContext,
            base.Owner.Creature, 
            base.DynamicVars.Cards.BaseValue, 
            base.Owner.Creature, 
            this
        );
    }
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}