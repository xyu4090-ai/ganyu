using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils; 
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class BitingColdWind : GanyuCardModel
{
    public BitingColdWind() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 对应 JSON 中的 {Block}，初始 8 点，升级后增加 3 点达到 11
        new BlockVar(8m, ValueProp.Move),
        // 对应 JSON 中的 {WindPower}，固定为 1 层
        new PowerVar<WindPower>(1m)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WindPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得格挡。直接传入 BlockVar 以处理敏捷加成
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 对全体敌人施加风元素
        var enemies = base.CombatState.Enemies;
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
            {
                // 调用你的工具类，传入 HittableEnemies 以支持扩散逻辑
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyWindReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies);
                });
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：Block 数值 8 -> 11
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars.Power<WindPower>().UpgradeValueBy(1m);
    }
}