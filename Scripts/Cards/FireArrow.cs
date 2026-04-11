using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public class FireArrow : GanyuCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;

    public FireArrow() : base(energyCost, type, rarity, targetType, true) { }


    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FlamePower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move), // 基础伤害 6 [cite: 482]
        new PowerVar<FlamePower>(1m)        // 火元素标记 1 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // 1. 造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyFireReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies);
        });
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5m); // 升级后造成 9 点伤害 
    }
}