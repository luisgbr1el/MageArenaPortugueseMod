using HarmonyLib;
using MageArenaPortuguese.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageArenaPortuguese.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), "Start")]
    class MainMenuManagerPatch
    {
        static void Postfix()
        {
            TranslateUI();
        }

        static void TranslateUI()
        {
            Text[] textosUI = GameObject.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Text txt in textosUI)
            {
                string original = txt.text;
                string traduzido = TranslationManager.Get("menu", original);

                if (traduzido != original)
                    txt.text = traduzido;
                else
                    MageArenaPortugueseVoiceMod.MageArenaPortugueseVoiceMod.Log.LogWarning($"NÃO TRADUZIDO (Text): \"{original.Trim()}\"");
            }

            TMP_Text[] textosTMP = GameObject.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TMP_Text tmp in textosTMP)
            {
                string original = tmp.text;
                string traduzido = TranslationManager.Get("menu", original);

                if (traduzido != original)
                    tmp.text = traduzido;
                else
                    MageArenaPortugueseVoiceMod.MageArenaPortugueseVoiceMod.Log.LogWarning($"NÃO TRADUZIDO (TMP_Text): \"{original.Trim()}\"");
            }
        }
    }
}
