# Ganyu Mod - Slay the Spire 2

> *一个在璃月工作上千年的存在。时间对甘雨而言，不过是另一种形式的永恒。*

A full character mod for Slay the Spire 2, featuring **Ganyu** from Genshin Impact. Implements a complete **elemental reaction system** inspired by Genshin Impact's combat mechanics, with 90 cards, 62 powers, relics, and rich NPC dialogue.

---

## Core Mechanics

### Element System

Six elements can be applied to enemies as layered debuffs:

| Element | Chinese | Arrow Card | Color |
|---------|---------|------------|-------|
| Ice | 冰元素 | Ice Arrow | Cryo |
| Water | 水元素 | Water Arrow | Hydro |
| Fire | 火元素 | Fire Arrow | Pyro |
| Electro | 雷元素 | Electro Arrow | Electro |
| Rock | 岩元素 | Rock Arrow | Geo |
| Wind | 风元素 | Wind Arrow | Anemo |

### Elemental Reactions

When Ice interacts with another element (or Wind swirls), a reaction triggers:

| Reaction | Chinese | Elements | Effect |
|----------|---------|----------|--------|
| **Melt** | 融化 | Ice + Fire | 8 damage per layer consumed |
| **Freeze** | 结冰 | Ice + Water | Target deals 40% less attack damage |
| **Superconduct** | 超导 | Ice + Electro | Target takes 40% more attack damage |
| **Crystallize** | 结晶 | Ice + Rock | Gain 4 block per layer consumed |
| **Swirl** | 扩散 | Wind + any | Spread the swirled element to all enemies |
| **Ignition** | 炎化 | Fire special | Target takes 200% more damage |
| **Petrification** | 岩化 | Rock special | Target grants block to source when damaged |

### Elemental Charge System

Applying elements also charges Ganyu's own elemental energy. Each element has a **dual-milestone charge** system:

- **5 layers** -> Gain a mid-tier power (e.g., Traces of Qilin, Guoba Attack)
- **10 layers** -> Gain a powerful ultimate (e.g., Heavenly Fall, Pyronado), then reset

| Element | Tier 1 (5) | Tier 2 (10) |
|---------|------------|-------------|
| Ice | Traces of Qilin (3 block/turn) | Heavenly Fall (15 AoE + Ice + energy/turn) |
| Water | Oath of Silvery Moon (heal + Water AoE) | Oceanborne Feather (Freeze enhancement) |
| Fire | Guoba Attack (7 damage + Fire/turn) | Pyronado (10 AoE + Fire/turn) |
| Electro | Baleful Omen (energy + 7 damage + Electro/turn) | Musou Shinsetsu (consumes Electro for massive AoE) |
| Rock | Jade Shield (double Crystallize block) | Starfall (block + 25 AoE at turn end) |
| Wind | Wind Spirit 6308 (4 AoE + Wind/turn) | Forbidden Wind Spirit 75 (10 AoE + Wind/turn) |

---

## Card Library (90 cards)

### Basic Cards
- **Strike** / **Defend** - Standard starter cards

### Elemental Arrows (6)
Deal damage and apply corresponding element. The core of most builds.

### Element Mastery Cards (6)
- Ice / Hydro / Electro / Geo / Pyro / Anemo Mastery - Grant extra layers when gaining the corresponding charge

### Reaction Synergy Cards (15+)
- **Frostflake Arrow** - If target has Ice, splash damage to all enemies
- **Shatter Bolt** - Extra damage if target is Frozen
- **Crystal Burst** - Block equal to target's Rock layers x multiplier
- **Explosive Arrow** - Doubled damage if Frozen; Fire to all enemies
- **Fusion Strike** - Extra damage per elemental reaction this turn
- **Elemental Symphony** - Extra hit per different element on target
- **Absolute Zero Execution** - Double damage if Frozen
- **Rock Smash** - Force-trigger Crystallize X times
- **Glacier Smash** - Double damage + energy if Frozen

### AoE / Spread Cards (10+)
- **Cryo Cannon** - Damage all enemies + Ice
- **Falling Strike** - Damage all enemies
- **Ice Age** - Apply Ice to all enemies
- **Swirling Storm** - Wind to all enemies + draw
- **Thunderclap** - Electro to all enemies + draw
- **Abyssal Torrent** - Damage + Water to all enemies
- **Rock Bulwark** - Block + Rock to all enemies

### Resource Engine Cards (10+)
- **Unending Revenue** - Each reaction grants energy
- **Efficiency** - Each reaction draws cards
- **Administrative Streamlining** - Each reaction draws 1 card
- **Overtime Work** - Lose 2 HP, gain 1 energy + 1 draw/turn
- **Overdrawn Potential** - Draw cards + energy, lose 2 HP/turn
- **Chain Reaction** - Each reaction applies Ice to random enemy
- **Elemental Overflow** - All reactions double their output

### Utility / Special Cards
- **Elemental Resonance** - Copy elements from one enemy to all others
- **Elemental Torrent** - Exhaust; double all element layers on all enemies
- **Ultimate Extraction** - Exhaust; remove ALL elements, deal damage per layer
- **Blessing of the Seven** - Consume Status/Curse cards for random charges
- **Mountain Herb Gathering** - Add healing Qingxin cards to draw pile
- **Elemental Pull** - Recover a discard card at 0 cost

---

## Power System (62 powers)

### Passive Build-Around Powers
| Power | Effect |
|-------|--------|
| Unending Revenue | +1 energy per reaction |
| Administrative Streamlining | +1 draw per reaction |
| Chain Reaction | Each reaction applies Ice to random enemy |
| Elemental Overflow | Double reaction layer output |
| Overload Prep | Next N reactions double their effect |
| Ride the Wind | Each Swirl makes a random card cost 0 |
| Solid as Rock | Crystallize retains block this turn |
| Elemental Symbiosis | Gain block when gaining non-Ice charge |
| Peaks of Guyun | Gain block when enemies receive Rock |
| Blooming Blazeflower | Applying Fire deals damage to all enemies |
| Leyline Tremor | Applying Rock deals damage to all enemies |
| Thunderstorm Matrix | Turn end: Electro layers x 3 damage to all |

### Offense/Defense Powers
- **Ignition** - Target takes 200% more damage
- **Freeze** - Target deals 40% less damage
- **Superconduct** - Target takes 40% more attack damage
- **Oceanborne Feather** - Enhances Freeze damage reduction by +50%
- **Dew Nectar** - Attacks deal 15% more damage

---

## Relics

| Relic | Effect |
|-------|--------|
| **Heavenly Fall** (降众天华) | Battle start: apply Ice to all enemies. Periodically: gain energy + Dew Nectar |
| **Heavenly Fall UP** | Enhanced version of the above |

---

## Patches & Integrations

The mod patches the following base game systems:
- **Audio Helper** - Custom Ganyu sound effects
- **Character Select Background** - Custom selection screen
- **Card Trail VFX** - Custom particle effects for card plays
- **Archaic Tooth / Touch of Orobas** - Relic interaction patches

---

## NPC Dialogue

9 unique NPC conversation trees with thematic writing centered on Ganyu's character concept of **"waiting"** (等待). After a thousand years of service in Liyue, Ganyu's dialogue reflects patience, eternity, and existential longing.

Notable NPCs: Neow, Darv, Tanx, Vakuu, The Architect (final boss).

---

## Project Structure

```
GanyuMod/
├── Scripts/
│   ├── Entry.cs              # Mod entry point
│   ├── GanyuCharacter.cs     # Character definition
│   ├── GanyuCardPool.cs      # Card pool registration
│   ├── GanyuRelicPool.cs     # Relic pool registration
│   ├── GanyuPotionPool.cs    # Potion pool registration
│   ├── Cards/                # 90 card implementations
│   ├── Powers/               # 62 power implementations
│   ├── Relics/               # 3 relic implementations
│   ├── Patches/              # Game system patches
│   ├── Nodes/                # VFX & custom nodes
│   └── Utils/
│       └── GanyuElementUtils.cs  # Core element reaction engine (691 lines)
└── Ganyu/
    ├── localization/zhs/     # Chinese localization (cards, powers, relics, NPCs)
    ├── scenes/               # Character scenes & VFX
    ├── images/               # Card art, power icons, UI assets
    └── themes/               # Visual themes
```

---

## Tech Stack

- **Engine**: Godot (via Slay the Spire 2 modding framework)
- **Language**: C#
- **Asset Pipeline**: Godot `.tscn` scenes, `.png` sprites, `.wav` audio

---

## Localization

All content is fully localized in Chinese (Simplified). Localization files:

- `cards.json` - 90 card names and descriptions
- `powers.json` - 62 power names and descriptions
- `relics.json` - Relic names and descriptions
- `characters.json` - Character profile and dialogue
- `ancients.json` - NPC dialogue trees (98 entries across 9 NPCs)
