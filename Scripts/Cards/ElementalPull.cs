using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementalPull : GanyuCardModel
{
    // 修改为 1费，技能牌，目标为自己
    public ElementalPull() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    // 默认带有“消耗”关键字
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获取弃牌堆
        CardPile pile = PileType.Discard.GetPile(base.Owner);
        
        // 如果弃牌堆没有牌，则不执行后续操作
        if (pile.Cards.Count == 0) return;

        // 2. 打开选择界面，从弃牌堆选择 1 张牌
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
        CardModel selectedCard = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, base.Owner, prefs)).FirstOrDefault();
        
        if (selectedCard != null)
        {
            // 3. 将选中的牌放入手牌
            await CardPileCmd.Add(selectedCard, PileType.Hand);
            
            // 4. 将其本回合的耗能变为 0 (复用 RideTheWindPower 中的原版机制)
            selectedCard.SetToFreeThisTurn();
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：移除“消耗”关键字
        RemoveKeyword(CardKeyword.Exhaust);
    }
}