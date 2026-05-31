using System.Threading.Tasks;
using BaseLib.Abstracts;
using Ganyu.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Ganyu.Scripts;

public class IceEnchantSingleton : CustomSingletonModel
{
    public IceEnchantSingleton() : base(true, true)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card == null || !cardPlay.Card.Keywords.Contains(IceEnchantKeyword.IceEnchant)) return;

        var owner = cardPlay.Card.Owner?.Creature;
        if (owner == null) return;

        var enemies =cardPlay.Card.Owner?.Creature.CombatState.HittableEnemies;
        if (enemies == null) return;

        foreach (Creature enemy in enemies)
        {
            if (enemy.IsAlive)
            {
                await GanyuElementUtils.ApplyIceReaction(enemy, owner, enemies, 1m);
            }
        }

        cardPlay.Card.RemoveKeyword(IceEnchantKeyword.IceEnchant);
    }
}
