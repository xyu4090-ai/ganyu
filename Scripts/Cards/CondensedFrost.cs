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

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))] // 加入甘雨卡池
public sealed class CondensedFrost : GanyuCardModel
{
    public CondensedFrost() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // 初始给予 2 层冰元素
        new PowerVar<IcePower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 在卡牌界面显示冰元素的提示
        HoverTipFactory.FromPower<IcePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 调用元素工具类施加冰元素并触发反应
        await ActionWithContext(choiceContext, async () =>
        {
            await GanyuElementUtils.ApplyIceReaction(
                cardPlay.Target, 
                base.Owner.Creature, 
                base.CombatState.HittableEnemies, 
                base.DynamicVars.Power<IcePower>().BaseValue
            );
        });
    }

    protected override void OnUpgrade()
    {
        // 升级后层数增加 1 (2 -> 3)
        base.DynamicVars.Power<IcePower>().UpgradeValueBy(1m);
    }
}