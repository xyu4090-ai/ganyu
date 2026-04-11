using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Ganyu.Scripts.Relics; // 替换为你遗物所在的命名空间

namespace Ganyu.Scripts.Patches;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    [HarmonyPostfix]
    public static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        // 逻辑：如果传入的初始遗物是你的 Mod 遗物，则手动修改返回值
        // 假设你的初始遗物类名是 GanyuStarterRelic，升级版是 GanyuBossRelic
        if (starterRelic.Id == ModelDb.Relic<HeavenlyFall>().Id)
        {
            // 将结果替换为你想要的 Boss 遗物模型
            __result = ModelDb.Relic<HeavenlyFallUP>();
        }
    }
}