using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Ganyu.Scripts.Powers;
using BaseLib.Utils;
using Ganyu.Scripts.Utils;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class RetainElement : GanyuCardModel
{
    public RetainElement() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 赋予玩家“元素残留”能力
        await PowerCmd.Apply<RetainElementPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        GanyuElementUtils.HasRetainElement = true;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}