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
public class AbyssalTorrent : GanyuCardModel
{
    public AbyssalTorrent() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WetPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<WetPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 对全体敌人造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);

        // 2. 给予全体敌人水元素附着并检查反应
        await ActionWithContext(choiceContext, async () =>
        {
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    // 传入 2 层水元素
                    await GanyuElementUtils.ApplyWaterReaction(
                        enemy,
                        base.Owner.Creature,
                        base.CombatState.HittableEnemies,
                        DynamicVars.Power<WetPower>().BaseValue
                    );
                }
            }
        });
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m); // 伤害提升至 16
        base.DynamicVars.Power<WetPower>().UpgradeValueBy(1m);
    }
}