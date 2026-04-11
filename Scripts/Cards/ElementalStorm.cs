using BaseLib.Utils;
using Ganyu.Scripts.Cards;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps; // 引用你的工具类命名空间
namespace Ganyu.Scripts.Cards;
[Pool(typeof(GanyuCardPool))]
public sealed class ElementalStorm : GanyuCardModel // 建议继承你的基类
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(9m),    // 基础伤害
        new ExtraDamageVar(3m),       // 每层增加的伤害
        // 核心修改处：将逻辑改为读取 TotalReactionsThisCombat
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
            GanyuElementUtils.TotalReactionsThisCombat)
    ];

    public ElementalStorm() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 直接使用计算后的伤害（CalculatedDamage）
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级后每层反应增加的伤害从 2 变为 3 (可选)
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}