using BaseLib.Utils;
using Ganyu.Scripts.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class PrismaticArray : GanyuCardModel
{
    public PrismaticArray() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 基础伤害 3 点
        new DamageVar(3m, ValueProp.Move),
        // 命中次数 3 次 (参考 Ricochet 使用 RepeatVar)
        new RepeatVar(5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 在卡牌界面显示冰元素的提示
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取循环次数
        int count = base.DynamicVars.Repeat.IntValue;

        // 循环执行攻击，以确保每一击都能正确处理元素反应的先后顺序
        for (int i = 0; i < count; i++)
        {
            // 如果目标已经死亡，停止后续攻击
            if (cardPlay.Target == null || !cardPlay.Target.IsAlive)
            {
                break;
            }

            // 1. 执行单次攻击 (参考 Ricochet 的 Damage 变量与效果)
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);

            // 2. 命中后给予 1 层冰元素并尝试反应
            await ActionWithContext(choiceContext, async () =>
            {
                await GanyuElementUtils.ApplyIceReaction(
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
        // 升级效果：单次伤害增加 1 点 (3 -> 4)
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}