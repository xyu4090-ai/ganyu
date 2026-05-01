using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Utils; // 引用你的元素工具类
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class SnowVeil : GanyuCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;

    public SnowVeil() : base(energyCost, type, rarity, TargetType.AllEnemies, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(6m, ValueProp.Move),
    new PowerVar<IcePower>(1m)
    ];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得格挡
        // 使用 DynamicVars.Block.BaseValue 确保受到敏捷加成
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 对所有敌人施加冰元素并尝试触发反应
        var enemies = base.CombatState.Enemies;
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
            {
                // 调用你封装的工具类，传入全场敌人列表以支持“扩散”等反应
                await ActionWithContext(choiceContext, async () =>
                {
                    await GanyuElementUtils.ApplyIceReaction(enemy, base.Owner.Creature, base.CombatState.HittableEnemies);
                });
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}