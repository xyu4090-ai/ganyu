using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
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
public sealed class StaticSpark : GanyuCardModel
{
    public StaticSpark() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move),     // 基础伤害 4
        new PowerVar<ElectroPower>(1m)         // 基础雷元素 1 层
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectroPower>() // 悬停提示雷元素词条
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 伤害结算后目标若存活，给予雷元素（并检测反应）
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            await ActionWithContext(choiceContext, async () =>
            {
                // 调用封装好的雷元素反应逻辑，传入从动态变量中读取的层数
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
        // 升级效果：伤害 4 -> 6 (+2)
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级效果：雷元素层数 1 -> 2 (+1)
        base.DynamicVars.Power<ElectroPower>().UpgradeValueBy(1m);
    }
}