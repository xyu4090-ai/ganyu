using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class LeylineTremor : GanyuCardModel
{
    // 修改为：1费，能力牌，罕见，目标自身
    public LeylineTremor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(3m) // 基础伤害 3
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加“地脉震颤”能力，层数等于卡牌上的伤害数值
        await PowerCmd.Apply<LeylineTremorPower>(
            choiceContext, 
            base.Owner.Creature, 
            base.DynamicVars["CalculationBase"].BaseValue, 
            base.Owner.Creature, 
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 3 -> 5 (+2)
        base.DynamicVars["CalculationBase"].UpgradeValueBy(2m);
    }
}