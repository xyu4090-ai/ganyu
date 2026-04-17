using Ganyu.Scripts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
internal static class GanyuCharSelectBgPatch
{
    private static void Postfix(NCharacterSelectScreen __instance, NCharacterSelectButton charSelectButton, CharacterModel characterModel)
    {
        // 检查当前选择的角色是否是 Ganyu
        if (characterModel is GanyuCharacter)
        {
            NovaAudioHelper.PlayOneShot("res://Ganyu/audios/select_bgm.wav", 1.0f);
        }
    }
}