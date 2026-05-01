
using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;


namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class IceAge : GanyuCardModel
{
    public IceAge() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 对应 JSON 中的 {IcePower}，赋予 99 层冰元素
        new PowerVar<IcePower>(5m)
    ];

    // 显式声明消耗关键字，使其在卡牌界面正确显示
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            if (enemy.IsAlive)
            {
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyIceReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies,base.DynamicVars.Power<IcePower>().BaseValue);
                });
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}