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
public sealed class AnemoMastery : GanyuCardModel
{
    // 费用为1，类型为能力牌，稀有度设为罕见（Uncommon）
    public AnemoMastery() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    // 声明能力变量，初始给予 1 层
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AnemoMasteryPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“风元素精通”能力
        await PowerCmd.Apply<AnemoMasteryPower>(choiceContext,
            base.Owner.Creature, 
            base.DynamicVars.Power<AnemoMasteryPower>().BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级效果：赋予“固有”属性
        base.EnergyCost.UpgradeBy(-1);
    }
}