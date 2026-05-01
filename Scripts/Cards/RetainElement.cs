using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Ganyu.Scripts.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Extensions;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class RetainElement : GanyuCardModel
{
    public RetainElement() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    // 声明能力变量，初始给予 1 层
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RetainElementPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家修改后的“冰元素精通”能力
        await PowerCmd.Apply<RetainElementPower>(choiceContext,
            base.Owner.Creature, 
            base.DynamicVars.Power<RetainElementPower>().BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}