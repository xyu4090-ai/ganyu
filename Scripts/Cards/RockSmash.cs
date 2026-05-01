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
public sealed class RockSmash : GanyuCardModel
{
    public RockSmash() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12m, ValueProp.Move), // 基础伤害 14
        new DynamicVar("Repeat",1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 如果目标存活，检查是否拥有岩元素
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {


            await ActionWithContext(choiceContext, async () =>
            {
                // 强制触发一次结晶反应，按 1 层反应计算（获得护盾并触发相关的玉璋护盾/反应计数逻辑）
                // 注意：此处不调用 TickDownDuration，因此完美实现了“不消耗层数”
                await GanyuElementUtils.TriggerCrystallize(cardPlay.Target, base.Owner.Creature, base.DynamicVars["Repeat"].BaseValue);

                // 如果你想让收益与目标身上的岩元素层数挂钩（层数越高盾越厚），
                // 可以将上面的 1m 替换为 rockPower.Amount
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 14 -> 18 (+4)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["Repeat"].UpgradeValueBy(1m);
    }
}