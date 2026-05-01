using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class CleansingRipple : GanyuCardModel
{
    public CleansingRipple() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6m, ValueProp.Move),
        new PowerVar<WetPower>(1m),         // 水元素标记 1 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // 2. 造成伤害
        await CreatureCmd.GainBlock(
                       base.Owner.Creature,
                       base.DynamicVars.Block,
                       cardPlay
                   );

        // 3. 给予目标水元素并尝试触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyWaterReaction(
                cardPlay.Target,
                base.Owner.Creature,
                base.CombatState.HittableEnemies,
                base.DynamicVars.Power<WetPower>().BaseValue
            );
        });
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害增加 3 点 (8 -> 11)，虚弱增加 1 层 (1 -> 2)
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars.Power<WetPower>().UpgradeValueBy(1m);
    }
}