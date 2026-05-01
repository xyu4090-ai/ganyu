using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class MountainHerbGathering : GanyuCardModel
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("ExtraCards", 0m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>
    {
        HoverTipFactory.FromCard<Qingxin>(base.IsUpgraded)
    };

    public MountainHerbGathering() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 获取 X 的值并加上升级带来的额外数量
        int count = ResolveEnergyXValue() + base.DynamicVars["ExtraCards"].IntValue;

        if (count > 0)
        {
            // 直接调用 Qingxin 的静态方法生成卡牌列表
            List<Qingxin> list = Qingxin.Create(base.Owner, count, base.CombatState).ToList();

            // 如果主卡已升级，升级所有的清心
            if (base.IsUpgraded)
            {
                foreach (Qingxin item in list)
                {
                    CardCmd.Upgrade(item);
                }
            }

            // 加入抽牌堆，并伴随 UI 预览动画
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, base.Owner, CardPilePosition.Random)
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后生成的数量+1
        base.DynamicVars["ExtraCards"].UpgradeValueBy(1m);
    }
}