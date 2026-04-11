using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using Ganyu.Scripts.Cards;
using MegaCrit.Sts2.Core.Models.Relics; // 引用你卡牌所在的命名空间

namespace Ganyu.Scripts.Patches;

// 加上这个特性，Harmony 才能在初始化时找到它
[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        // 获取你的基础卡牌 ID
        ModelId baseId = ModelDb.Card<TracesQilin>().Id;

        // 建立映射关系
        if (!__result.ContainsKey(baseId))
        {
            // 确保你已经写好了 TracesOfQilinArchaic 这个类
            __result.Add(baseId, ModelDb.Card<TracesQilinUp>());
        }
    }
}