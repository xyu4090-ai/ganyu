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
public sealed class Snowmelt : GanyuCardModel
{
    public Snowmelt() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),    // 初始抽 1 张牌
        new EnergyVar(1)    // 获得 1 点能量
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        base.EnergyHoverTip
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 确保目标存在且存活
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            // 获取目标身上的冰元素
            var icePower = cardPlay.Target.GetPower<IcePower>();
            
            // 判定是否拥有至少 1 层冰元素
            if (icePower != null && icePower.Amount >= 1m)
            {
                // 1. 消耗 1 层冰元素
                await PowerCmd.TickDownDuration(icePower);
                
                // 2. 获得 1 点能量
                await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
                
                // 3. 抽牌 (基础 1 张，升级后 2 张)
                await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：抽牌 1 -> 2
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}