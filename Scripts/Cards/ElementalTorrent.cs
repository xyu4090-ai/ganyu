using BaseLib.Utils;
using Ganyu.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganyu.Scripts.Cards;

[Pool(typeof(GanyuCardPool))]
public sealed class ElementalTorrent : GanyuCardModel
{
    public ElementalTorrent() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    // 默认自带“消耗”词条
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        // 提示玩家所有可能被翻倍的 6 种元素
        HoverTipFactory.FromPower<IcePower>(),
        HoverTipFactory.FromPower<WetPower>(),
        HoverTipFactory.FromPower<FlamePower>(),
        HoverTipFactory.FromPower<ElectroPower>(),
        HoverTipFactory.FromPower<WindPower>(),
        HoverTipFactory.FromPower<RockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 遍历场上所有可被攻击的存活敌人
        foreach (var enemy in base.CombatState.HittableEnemies)
        {
            if (enemy.IsAlive)
            {
                // 冰元素翻倍
                var ice = enemy.GetPower<IcePower>();
                if (ice != null && ice.Amount > 0) 
                    await PowerCmd.Apply<IcePower>(choiceContext,enemy, ice.Amount, base.Owner.Creature, this);

                // 水元素翻倍
                var wet = enemy.GetPower<WetPower>();
                if (wet != null && wet.Amount > 0) 
                    await PowerCmd.Apply<WetPower>(choiceContext,enemy, wet.Amount, base.Owner.Creature, this);

                // 火元素翻倍
                var flame = enemy.GetPower<FlamePower>();
                if (flame != null && flame.Amount > 0) 
                    await PowerCmd.Apply<FlamePower>(choiceContext,enemy, flame.Amount, base.Owner.Creature, this);

                // 雷元素翻倍
                var electro = enemy.GetPower<ElectroPower>();
                if (electro != null && electro.Amount > 0) 
                    await PowerCmd.Apply<ElectroPower>(choiceContext,enemy, electro.Amount, base.Owner.Creature, this);

                // 风元素翻倍
                var wind = enemy.GetPower<WindPower>();
                if (wind != null && wind.Amount > 0) 
                    await PowerCmd.Apply<WindPower>(choiceContext,enemy, wind.Amount, base.Owner.Creature, this);

                // 岩元素翻倍
                var rock = enemy.GetPower<RockPower>();
                if (rock != null && rock.Amount > 0) 
                    await PowerCmd.Apply<RockPower>(choiceContext,enemy, rock.Amount, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：添加“保留”词条
        AddKeyword(CardKeyword.Retain);
    }
}