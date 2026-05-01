
using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class Mirage : GanyuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
       public Mirage() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<OceanborneFeatherPower>(),
        HoverTipFactory.FromPower<WetPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<OceanborneFeatherPower>(1m), // 基础：1层海人化羽
        new PowerVar<WetPower>(1m)                // 基础：2层水元素
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 赋予玩家“海人化羽”能力
        await PowerCmd.Apply<OceanborneFeatherPower>(choiceContext,
            base.Owner.Creature,
            DynamicVars.Power<OceanborneFeatherPower>().BaseValue,
            base.Owner.Creature,
            this
        );

        // 2. 给予全体敌人水元素附着并触发反应检查
        await ActionWithContext(choiceContext, async () =>
        {
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    // 传入水元素层数变量
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
        // 升级后海人化羽层数提升 1 层（即变为 2 层）
        base.DynamicVars.Power<OceanborneFeatherPower>().UpgradeValueBy(1m);
    }
}