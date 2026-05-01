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
public sealed class RockBulwark : GanyuCardModel
{
    // 初始化卡牌：2费，技能牌，罕见，目标为全体敌人（因为要挂元素）
    public RockBulwark() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    // 悬浮提示显示岩元素
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<RockPower>()
    ];

    // 定义卡牌数值变量
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12m, ValueProp.Move), // 基础格挡 14
        new PowerVar<RockPower>(1m)        // 岩元素标记 1 层
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 玩家获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 给予全体敌人岩元素并触发反应 (参考 AbyssalTorrent 全体判定逻辑)
        await ActionWithContext(choiceContext, async () =>
        {
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                if (enemy.IsAlive)
                {
                    await GanyuElementUtils.ApplyRockReaction(
                        enemy,
                        base.Owner.Creature,
                        base.CombatState.HittableEnemies,
                        DynamicVars.Power<RockPower>().BaseValue
                    );
                }
            }
        });
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡提升4点 (14 -> 18)，费用降低1点 (2 -> 1)
        base.DynamicVars.Block.UpgradeValueBy(4m);
        base.DynamicVars.Power<RockPower>().UpgradeValueBy(1m);
    }
}