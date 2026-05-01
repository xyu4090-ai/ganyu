
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
    public SkyborneArchery() : base(energyCost, type, rarity, TargetType.AnyEnemy, true)
    {
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 提示玩家可能触发的各种元素效果
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.WetPower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.FlamePower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.ElectroPower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.WindPower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.RockPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
        // 这里建议显示元素的提示，方便玩家在卡牌界面看到效果
        new PowerVar<WetPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AttackCommand attack = DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this);
        // 普通版：单体攻击
        attack = attack.Targeting(cardPlay.Target);
        // 单体反应逻辑
        await attack.Execute(choiceContext);

        if (cardPlay.Target == null || !cardPlay.Target.IsAlive) return;
        // 使用游戏的随机数生成器选择一个元素 (0-4)
        int choice = base.Owner.RunState.Rng.CombatCardSelection.NextInt(0, 5);
        await ActionWithContext(choiceContext, async () =>
        {
            switch (choice)
            {
                case 0: await GanyuElementUtils.ApplyWaterReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                case 1: await GanyuElementUtils.ApplyFireReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                case 2: await GanyuElementUtils.ApplyElectroReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                case 3: await GanyuElementUtils.ApplyWindReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
                case 4: await GanyuElementUtils.ApplyRockReaction(cardPlay.Target, base.Owner.Creature, base.CombatState.HittableEnemies); break;
            }
        });

    }

    protected override void OnUpgrade()
    {
        // 升级后伤害 3 -> 6
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}