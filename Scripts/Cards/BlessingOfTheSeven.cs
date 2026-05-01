using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class BlessingOfTheSeven : GanyuCardModel
{
    // 初始化：0费，技能牌，罕见，目标为自己
    public BlessingOfTheSeven() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("charge",2m)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获取手牌中的所有 状态牌(Status) 和 诅咒牌(Curse)
        List<CardModel> list = GetStatuses(base.Owner).ToList();

        int exhaustedCount = 0;

        // 2. 消耗这些牌
        foreach (CardModel item in list)
        {
            // 执行消耗指令
            // (注：如果在你的代码版本中单卡消耗 API 略有不同，可替换为对应的 CardPileCmd.Exhaust(choiceContext, item) 等形式)
            await CardCmd.Exhaust(choiceContext, item);
            exhaustedCount++;
        }

        // 3. 根据消耗的数量给予随机元素充能与抽牌
        if (exhaustedCount > 0)
        {
            // 使用 System.Random 进行随机分配
            int element = base.Owner.RunState.Rng.CombatCardSelection.NextInt(0, 6);

            for (int i = 0; i < exhaustedCount; i++)
            {
                // 随机 0 到 5 对应 6 种元素
                switch (element)
                {
                    case 0: await GanyuElementUtils.ApplyIceCharge(base.Owner.Creature, base.DynamicVars["charge"].BaseValue); break;
                    case 1: await GanyuElementUtils.ApplyWaterCharge(base.Owner.Creature, base.DynamicVars["charge"].BaseValue); break;
                    case 2: await GanyuElementUtils.ApplyFireCharge(base.Owner.Creature, base.DynamicVars["charge"].BaseValue); break;
                    case 3: await GanyuElementUtils.ApplyElectroCharge(base.Owner.Creature, base.DynamicVars["charge"].BaseValue); break;
                    case 4: await GanyuElementUtils.ApplyWindCharge(base.Owner.Creature, base.DynamicVars["charge"].BaseValue); break;
                    case 5: await GanyuElementUtils.ApplyRockCharge(base.Owner.Creature, base.DynamicVars["charge"].BaseValue); break;
                }


                await CardPileCmd.Draw(choiceContext, 1, base.Owner);

            }
        }
    }
    private static IEnumerable<CardModel> GetStatuses(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where((CardModel c) => (c.Type == CardType.Status || c.Type == CardType.Curse) && c.Pile.Type != PileType.Exhaust);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["charge"].UpgradeValueBy(1);
    }
}