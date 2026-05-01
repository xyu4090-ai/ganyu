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
public sealed class ElectroWebWeaving : GanyuCardModel
{
    public ElectroWebWeaving() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ElectroPower>(2m), // 基础给予 2 层雷元素
        new PowerVar<WeakPower>(1m)     // 基础给予 1 层虚弱
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WeakPower>() // 悬停时显示原版“虚弱”词条说明
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 遍历所有场上的可被攻击的敌人
        foreach (var enemy in base.CombatState.HittableEnemies)
        {
            if (enemy.IsAlive)
            {
                await ActionWithContext(choiceContext, async () =>
                {
                    // 1. 给予雷元素（自动检测目标身上是否有冰或风，从而触发超导/扩散）
                    await GanyuElementUtils.ApplyElectroReaction(
                        enemy, 
                        base.Owner.Creature, 
                        base.CombatState.HittableEnemies, 
                        base.DynamicVars.Power<ElectroPower>().BaseValue
                    );

                    // 2. 给予虚弱
                    await PowerCmd.Apply<WeakPower>(choiceContext,
                        enemy, 
                        base.DynamicVars.Power<WeakPower>().BaseValue, 
                        base.Owner.Creature, 
                        this
                    );
                });
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：雷元素 2 -> 3 (+1)
        base.DynamicVars.Power<ElectroPower>().UpgradeValueBy(1m);
        // 升级效果：虚弱 1 -> 2 (+1)
        base.DynamicVars.Power<WeakPower>().UpgradeValueBy(1m);
    }
}