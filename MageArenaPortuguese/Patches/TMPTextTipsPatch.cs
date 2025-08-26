using HarmonyLib;
using MageArenaPortuguese.Config;
using System.Reflection;
using TMPro;

namespace MageArenaPortuguese.Patches
{
    [HarmonyPatch(typeof(TMP_Text))]
    class TMPTextTipsPatch
    {
        static MethodBase TargetMethod()
        {
            return typeof(TMP_Text).GetProperty("text").GetSetMethod(true);
        }

        static void Prefix(TMP_Text __instance, ref string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string translated = TranslationManager.Get("tips", value);

            if (translated != value)
                value = translated;
            else
                MageArenaPortugueseVoiceMod.MageArenaPortugueseMod.Log.LogWarning($"NÃO TRADUZIDO: \"{value.Trim()}\"");
        }
    }
}