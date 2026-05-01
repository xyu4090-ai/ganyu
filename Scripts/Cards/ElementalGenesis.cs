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
public sealed class ElementalGenesis : GanyuCardModel
{
    // 声明这是一张 X 费牌
    protected override bool HasEnergyCostX => true;

    // 添加“消耗”词条
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("ExtraElements", 0m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    public ElementalGenesis() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 获取 X 的值并加上升级带来的额外数量
        int count = ResolveEnergyXValue() + base.DynamicVars["ExtraElements"].IntValue;

        if (count > 0 && cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            await ActionWithContext(choiceContext, async () =>
            {
                decimal amount = count;
                await GanyuElementUtils.TriggerConduct(cardPlay.Target, base.Owner.Creature, count, base.CombatState.HittableEnemies);
                await GanyuElementUtils.TriggerMelt(cardPlay.Target, base.Owner.Creature, count);
                await GanyuElementUtils.TriggerCrystallize(cardPlay.Target, base.Owner.Creature, count);
                await GanyuElementUtils.TriggerFrozen(cardPlay.Target, base.Owner.Creature, count);
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后所有元素赋予层数 +1
        base.DynamicVars["ExtraElements"].UpgradeValueBy(1m);
    }
}