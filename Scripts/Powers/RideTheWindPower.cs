using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Ganyu.Scripts.Powers;

public sealed class RideTheWindPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    // 能力不计层数，只作为常驻状态监听
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://Ganyu/images/powers/ride_the_wind_power.png";
    public override string? CustomBigIconPath => "res://Ganyu/images/powers/ride_the_wind_power.png";

    // 当触发扩散反应时调用此方法
    public Task TriggerSwirl()
    {
        Flash();

        // 1. 获取当前手牌列表
        IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(base.Owner.Player).Cards;
        Rng combatCardSelection = base.Owner.Player.RunState.Rng.CombatCardSelection;

        // 2. 优先挑选基础耗能（不包含全局修正）大于 0 的卡牌
        CardModel cardModel = combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: false)));

        // 3. 如果没找到，退一步寻找考虑全局修正后耗能大于 0 的卡牌
        if (cardModel == null)
        {
            cardModel = combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: true)));
        }

        // 4. 将选中的卡牌本回合费用设为 0
        cardModel?.SetToFreeThisTurn();

        return Task.CompletedTask;
    }
}