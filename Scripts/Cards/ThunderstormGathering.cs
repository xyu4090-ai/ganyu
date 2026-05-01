using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ThunderstormGathering : GanyuCardModel
{
    public ThunderstormGathering() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),                  // 获得 1 点能量
        new PowerVar<ElectroPower>(2m)     // 给予 2 层雷元素
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        base.EnergyHoverTip,                       // 悬停显示能量图标提示
        HoverTipFactory.FromPower<FrailPower>(),   // 悬停显示原版“脆弱”词条
        HoverTipFactory.FromPower<ElectroPower>()  // 悬停显示雷元素词条
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);

        // 2. 给予自己 2 层脆弱 (FrailPower)
        await PowerCmd.Apply<FrailPower>(choiceContext,base.Owner.Creature, 2m, base.Owner.Creature, this);

        // 3. 目标若存活，给予雷元素（并检测可能触发的反应）
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            await ActionWithContext(choiceContext, async () =>
            {
                // 调用封装好的雷元素反应逻辑
                await GanyuElementUtils.ApplyElectroReaction(
                    cardPlay.Target, 
                    base.Owner.Creature, 
                    base.CombatState.HittableEnemies, 
                    base.DynamicVars.Power<ElectroPower>().BaseValue
                );
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：给予的雷元素 2 -> 3 (+1)
        base.DynamicVars.Power<ElectroPower>().UpgradeValueBy(1m);
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}