using BaseLib.Utils;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementalSurge : GanyuCardModel
{
    public ElementalSurge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new RepeatVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 提示玩家可能触发的各种元素效果
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.WetPower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.FlamePower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.ElectroPower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.WindPower>(),
        HoverTipFactory.FromPower<Ganyu.Scripts.Powers.RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        int count = base.DynamicVars.Repeat.IntValue;
        for (int i = 0; i < count; i++)
        {
            // 如果目标已死亡，停止后续执行
            
            if (cardPlay.Target == null || !cardPlay.Target.IsAlive) break;
            // 使用游戏的随机数生成器选择一个元素 (0-4)
            int choice =base.Owner.RunState.Rng.CombatCardSelection.NextInt(0,5);
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
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}