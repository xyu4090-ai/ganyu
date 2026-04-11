
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class SkyborneArchery : GanyuCardModel
{
    // 基础属性定义
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Basic;
    public override TargetType TargetType
    {
        get
        {
            if (!IsUpgraded)
            {
                return TargetType.AnyEnemy;
            }
            return TargetType.AllEnemies;
        }
    }
    public SkyborneArchery() : base(energyCost, type, rarity, TargetType.AnyEnemy, true)
    {
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WetPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
        // 这里建议显示元素的提示，方便玩家在卡牌界面看到效果
        new PowerVar<WetPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AttackCommand attack = DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this);
        if (IsUpgraded)
        {
            // 1. 全体攻击逻辑
            attack = attack.TargetingAllOpponents(base.CombatState);
        }
        else
        {
            // 普通版：单体攻击
            attack = attack.Targeting(cardPlay.Target);
            // 单体反应逻辑

        }
        await attack.Execute(choiceContext);
        if (IsUpgraded)
        {
            foreach (Creature enemy in base.CombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    await ActionWithContext(choiceContext, async () =>
                    {
                        await GanyuElementUtils.ApplyWaterReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies);
                    });
                }
            }
        }
        else
        {
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyWaterReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies);
            });
        }

    }

    protected override void OnUpgrade()
    {
        // 升级后伤害 3 -> 6
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}