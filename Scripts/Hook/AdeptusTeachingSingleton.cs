using BaseLib.Abstracts;
using BaseLib.Utils;
using Ganyu.Scripts.Enchantments;
using Ganyu.Scripts.Powers;
using Ganyu.Scripts.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Helpers;

namespace Ganyu.Scripts;

public class AdeptusTeachingSingleton : CustomSingletonModel
{
    private const decimal BaseConstellationChance = 0.5m;
    private const decimal PityIncrement = 0.1m;

    public static AdeptusTeachingSingleton? Instance { get; private set; }

    [SavedProperty]
    private Dictionary<int, decimal> _pityBonus = new();
    private readonly HashSet<Player> _currentPlayers = new();

    public AdeptusTeachingSingleton() : base(true, true)
    {
        Instance = this;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        foreach (var creature in participants)
        {
            if (creature.Player != null && creature.GetPower<AdeptusTeachingPower>() != null)
            {
                UpdatePowerDynamicVars(creature, _pityBonus.GetValueOrDefault(creature.Player.GetHashCode(), 0m));
            }
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        var players = _currentPlayers.ToList();
        _currentPlayers.Clear();

        foreach (var player in players)
        {
            if (player.Creature == null) continue;
            await ProcessPlayer(player);
        }
    }

    private async Task ProcessPlayer(Player player)
    {
        int playerId = player.GetHashCode();
        decimal pityBonus = _pityBonus.GetValueOrDefault(playerId, 0m);
        decimal constellationChance = BaseConstellationChance + pityBonus;

        bool constellationMaxed = player.Creature.GetPower<ConstellationC6Power>() != null;

        bool gotConstellation;
        if (constellationMaxed)
        {
            gotConstellation = false;
        }
        else
        {
            decimal roll = (decimal)player.RunState.Rng.Niche.NextDouble();
            gotConstellation = roll < constellationChance;
        }

        if (gotConstellation)
        {
            var activeRelic = HeavenlyFall.ActiveInstance;
            if (activeRelic is HeavenlyFall hf)
            {
                hf.IncreaseBaseConstellation();
            }
            else if (activeRelic is HeavenlyFallUP hfUp)
            {
                hfUp.IncreaseBaseConstellation();
            }
            _pityBonus[playerId] = 0m;
        }
        else
        {
            if (!constellationMaxed)
            {
                _pityBonus[playerId] = pityBonus + PityIncrement;
            }
            await ApplyReadyToGoEnchantment(player);
        }

        var finalPity = _pityBonus.GetValueOrDefault(playerId, 0m);
        UpdatePowerDynamicVars(player.Creature, finalPity);
    }

    public void RegisterPlayer(Player player)
    {
        _currentPlayers.Add(player);
    }

    public void InitPowerDynamicVars(Creature creature, int playerId)
    {
        decimal pityBonus = _pityBonus.GetValueOrDefault(playerId, 0m);
        UpdatePowerDynamicVars(creature, pityBonus);
    }

    private static void UpdatePowerDynamicVars(Creature creature, decimal pityBonus)
    {
        var power = creature.GetPower<AdeptusTeachingPower>();
        if (power != null)
        {
            power.DynamicVars["PityBonus"].BaseValue = pityBonus * 100m;
        }
    }

    private static async Task ApplyReadyToGoEnchantment(Player player)
    {
        var enchantment = ModelDb.Enchantment<ReadyToGoEnchantment>();
        var eligibleCards = PileType.Deck.GetPile(player).Cards
            .Where(c => enchantment.CanEnchant(c))
            .ToList();

        if (eligibleCards.Count == 0) return;

        var selectedCard = (await CardSelectCmd.FromDeckForEnchantment(
            cards: eligibleCards.UnstableShuffle(player.RunState.Rng.Niche).ToList(),
            prefs: new CardSelectorPrefs(new LocString("cards", "GANYU-ADEPTUS_TEACHING.selectionScreenPrompt"), 1),
            enchantment: enchantment,
            amount: 1
        )).FirstOrDefault();

        if (selectedCard != null)
        {
            CardCmd.Enchant<ReadyToGoEnchantment>(selectedCard, 1m);
            var vfx = NCardEnchantVfx.Create(selectedCard);
            if (vfx != null)
            {
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }
        }
    }
}
