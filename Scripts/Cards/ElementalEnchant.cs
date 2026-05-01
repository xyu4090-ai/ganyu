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
public sealed class ElementalEnchant : GanyuCardModel
{
    public ElementalEnchant() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 初始格挡 5 点
        new BlockVar(5m, ValueProp.Move),
        // 给予的元素层数 2 层
        new DynamicVar("ElementAmount", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 在卡牌悬浮界面显示可能抽到的非冰元素提示
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 随机给予目标 2 层非冰元素的元素
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            // 在 0-4 之间随机选择一个数字 (对应 5 种非冰元素)
            int choice = base.Owner.RunState.Rng.CombatCardSelection.NextInt(0, 5);
            decimal amount = base.DynamicVars["ElementAmount"].BaseValue;

            await ActionWithContext(choiceContext, async () =>
            {
                switch (choice)
                {
                    case 0: await GanyuElementUtils.ApplyWaterReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies, amount); break;
                    case 1: await GanyuElementUtils.ApplyFireReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies, amount); break;
                    case 2: await GanyuElementUtils.ApplyElectroReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies, amount); break;
                    case 3: await GanyuElementUtils.ApplyWindReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies, amount); break;
                    case 4: await GanyuElementUtils.ApplyRockReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies, amount); break;
                }
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡 5 -> 8 (+3)
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars["ElementAmount"].UpgradeValueBy(1m);
    }
}