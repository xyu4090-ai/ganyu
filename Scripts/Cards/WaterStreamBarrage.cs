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
public class WaterStreamBarrage : GanyuCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;

    public WaterStreamBarrage() : base(energyCost, type, rarity, targetType, true) { }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WetPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move) // 基础伤害设定为 3
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 根据是否升级来决定攻击的段数（未升级为 2 次，升级后为 3 次）
        int repeatCount = base.IsUpgraded ? 3 : 2;

        for (int i = 0; i < repeatCount; i++)
        {
            // 1. 造成单次伤害
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);

            // 2. 每次命中后给予 1 层水元素，并检查反应逻辑
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyWaterReaction(
                    cardPlay.Target, 
                    base.Owner.Creature, 
                    base.CombatState.HittableEnemies, 
                    1m
                );
            });
        }
    }

    protected override void OnUpgrade()
    {
        
    }
}