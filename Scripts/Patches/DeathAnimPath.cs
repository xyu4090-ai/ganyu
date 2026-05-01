using Ganyu.Scripts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
namespace Ganyu.Scripts.Patches;

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class NCreature_StartDeathAnim
{
    static void Postfix(NCreature __instance, bool shouldRemove)
    {
        if (__instance.Entity.IsPlayer && __instance.Entity?.Player?.Character is GanyuCharacter)
        {
            __instance.SetAnimationTrigger("Dead");
        }
    }
}