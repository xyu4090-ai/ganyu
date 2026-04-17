using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;
[Pool(typeof(TokenCardPool))]
public sealed class Qingxin : GanyuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Heal", 3m)
    ];

    // 稀有度使用 CardRarity.Token
    public Qingxin() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self, false)
    {
    }

    // 官方标准的生成卡牌方法
    public static IEnumerable<Qingxin> Create(Player owner, int amount, CombatState combatState)
    {
        List<Qingxin> list = new List<Qingxin>();
        for (int i = 0; i < amount; i++)
        {
            // 通过 combatState.CreateCard 正确地从底层生成卡牌实例
            list.Add(combatState.CreateCard<Qingxin>(owner));
        }
        return list;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 恢复生命值
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars["Heal"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Heal"].UpgradeValueBy(6m);
    }
}