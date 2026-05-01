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
using MegaCrit.Sts2.Core.Models.Powers;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class LightningPierce : GanyuCardModel
{
    public LightningPierce() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move), // 基础伤害 7 点
        new PowerVar<ElectroPower>(1m),     // 给予 2 层雷元素
        new DynamicVar("Repeat",1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<VulnerablePower>() // 悬停时显示原版“易伤”的词条说明
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 如果目标存活，检查是否会触发反应并挂载元素
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            // 核心逻辑：在赋予雷元素前，检查目标身上是否有“冰元素”
            // 如果有，说明接下来的 ApplyElectroReaction 必定会引发超导
            bool willSuperconduct = cardPlay.Target.GetPower<IcePower>() != null;

            await ActionWithContext(choiceContext, async () =>
            {
                // 赋予 2 层雷元素（若满足条件，底层会自动触发超导并通知相关的能力结算）
                await GanyuElementUtils.ApplyElectroReaction(
                    cardPlay.Target, 
                    base.Owner.Creature, 
                    base.CombatState.HittableEnemies, 
                    base.DynamicVars.Power<ElectroPower>().BaseValue
                );

                // 3. 如果成功引发了超导，且目标存活，追加 1 层易伤
                if (willSuperconduct && cardPlay.Target.IsAlive)
                {
                    // 应用原版的易伤能力 (VulnerablePower)
                    await PowerCmd.Apply<VulnerablePower>(choiceContext,cardPlay.Target, base.DynamicVars["Repeat"].BaseValue, base.Owner.Creature, this);
                }
            });
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：伤害 7 -> 10 (+3)
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["Repeat"].UpgradeValueBy(1m);

    }
}