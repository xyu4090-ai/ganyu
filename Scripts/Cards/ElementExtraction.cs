using BaseLib.Extensions;
using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementExtraction : GanyuCardModel
{
    public ElementExtraction() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8m, ValueProp.Move), // 基础格挡 8
        new CardsVar(1)                   // 抽 1 张牌
    ];

    // 添加悬浮提示，方便玩家了解可能被移除的元素类型
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 移除目标随机 1 层元素
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            var elementPowers = new List<PowerModel>();
            
            // 收集目标身上当前所有存在的元素 Power
            if (cardPlay.Target.GetPower<IcePower>() is { Amount: > 0 } ip) elementPowers.Add(ip);
            if (cardPlay.Target.GetPower<WetPower>() is { Amount: > 0 } wp) elementPowers.Add(wp);
            if (cardPlay.Target.GetPower<FlamePower>() is { Amount: > 0 } fp) elementPowers.Add(fp);
            if (cardPlay.Target.GetPower<ElectroPower>() is { Amount: > 0 } ep) elementPowers.Add(ep);
            if (cardPlay.Target.GetPower<WindPower>() is { Amount: > 0 } winp) elementPowers.Add(winp);
            if (cardPlay.Target.GetPower<RockPower>() is { Amount: > 0 } rp) elementPowers.Add(rp);

            if (elementPowers.Count > 0)
            {
                // 使用游戏原生的 RNG 方法随机挑选一种元素
                var chosenPower = base.Owner.RunState.Rng.CombatTargets.NextItem(elementPowers);
                
                // TickDownDuration 会使该 Power 的层数减 1
                await PowerCmd.TickDownDuration(chosenPower);
            }
        }

        // 2. 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 3. 抽牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：格挡 8 -> 12 (+4)
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}