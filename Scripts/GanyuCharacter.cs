using BaseLib.Abstracts;
using Ganyu.Scripts.Cards;
using Ganyu.Scripts.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
namespace Ganyu.Scripts;

public class GanyuCharacter : PlaceholderCharacterModel
{
    // 角色名称颜色
    public override Color NameColor => new(0.78f, 0.92f, 1.0f);
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(0.78f, 0.92f, 1.0f);

    // 人物性别（男女中立）
    public override CharacterGender Gender => CharacterGender.Feminine;

    // 初始血量
    public override int StartingHp => 80;

    // 人物模型tscn路径。要自定义见下。
    public override string CustomVisualPath => "res://Ganyu/scenes/Ganyu_character.tscn";
    // 卡牌拖尾场景。
    // public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    // 人物头像路径。
    public override string CustomIconTexturePath => "res://Ganyu/images/icon.svg";
    // 人物头像2号。
    // public override string CustomIconPath => "res://scenes/ui/character_icons/ironclad_icon.tscn";
    // 能量表盘tscn路径。要自定义见下。
    public override string CustomEnergyCounterPath => "res://Ganyu/scenes/Ganyu_energy_counter.tscn";
    // 篝火休息场景。
    // public override string CustomRestSiteAnimPath => "res://scenes/rest_site/characters/ironclad_rest_site.tscn";
    // 商店人物场景。
    // public override string CustomMerchantAnimPath => "res://scenes/merchant/characters/ironclad_merchant.tscn";
    // 多人模式-手指。
    // public override string CustomArmPointingTexturePath => null;
    // 多人模式剪刀石头布-石头。
    // public override string CustomArmRockTexturePath => null;
    // 多人模式剪刀石头布-布。
    // public override string CustomArmPaperTexturePath => null;
    // 多人模式剪刀石头布-剪刀。
    // public override string CustomArmScissorsTexturePath => null;

    // 人物选择背景。
    public override string CustomCharacterSelectBg => "res://Ganyu/scenes/Ganyu_bg.tscn";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "res://Ganyu/images/char_select_Ganyu.png";
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "res://Ganyu/images/char_select_Ganyu_locked.png";
    // 人物选择过渡动画。
    // public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    // public override string CustomMapMarkerPath => null;
    // 攻击音效
    // public override string CustomAttackSfx => null;
    // 施法音效
    // public override string CustomCastSfx => null;
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    // public override string CharacterSelectSfx => null;
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override CardPoolModel CardPool => ModelDb.CardPool<GanyuCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GanyuRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GanyuPotionPool>();

    // 初始卡组
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeGanyu>(),
        ModelDb.Card<StrikeGanyu>(),
        ModelDb.Card<StrikeGanyu>(),
        ModelDb.Card<StrikeGanyu>(),
        ModelDb.Card<DefendGanyu>(),
        ModelDb.Card<DefendGanyu>(),
        ModelDb.Card<DefendGanyu>(),
        ModelDb.Card<DefendGanyu>(),
        ModelDb.Card<SkyborneArchery>(),
        ModelDb.Card<TracesQilin>(),
    ];

    // 初始遗物
    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<HeavenlyFall>(),
        ModelDb.Relic<CryaTheory>(),
    ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}