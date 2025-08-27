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
                    MageArenaPortugueseVoiceMod.MageArenaPortugueseMod.Log.LogWarning($"NÃO TRADUZIDO (Text): \"{original.Trim()}\"");
            }

            TMP_Text[] textosTMP = GameObject.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TMP_Text tmp in textosTMP)
            {
                string original = tmp.text;
                string traduzido = TranslationManager.Get("menu", original);

                if (traduzido != original)
                    tmp.text = traduzido;
                else
                    MageArenaPortugueseVoiceMod.MageArenaPortugueseMod.Log.LogWarning($"NÃO TRADUZIDO (TMP_Text): \"{original.Trim()}\"");
            }
        }

        [HarmonyPatch("ChangeTeamText")]
        [HarmonyPrefix]
        static void ChangeTeamText_Prefix(ref string lvlandrank)
        {
            if (!string.IsNullOrEmpty(lvlandrank))
                lvlandrank = lvlandrank.Replace("_", " ");
        }

        [HarmonyPatch("ChangeTeamText")]
        [HarmonyPostfix]
        static void ChangeTeamText_Postfix(int team, int index, string lvlandrank, MainMenuManager __instance)
        {
            if (string.IsNullOrEmpty(lvlandrank))
                return;

            string[] levelAndRank = lvlandrank.Split(' ');

            if (levelAndRank.Length < 2)
                return;

            string translatedRanking = TranslationManager.Get("rankings", levelAndRank[2]);

            string translated = $"Nível {levelAndRank[1]} ({translatedRanking})";

            if (team == 0)
                __instance.team1rankandleveltext[index].text = translated;
            else
                __instance.team2rankandleveltext[index].text = translated;
        }

        [HarmonyPatch("SyncUpdateNames")]
        [HarmonyPrefix]
        static void SyncUpdateNames_Prefix(ref string[] PlayerNames, ref string[] PlayerRanks)
        {
            for (int i = 0; i < PlayerNames.Length; i++)
            {
                if (PlayerNames[i] != null)
                {
                    if (PlayerRanks[i] != null)
                        PlayerRanks[i] = PlayerRanks[i].Replace("_", " ");
                }
            }
        }

        [HarmonyPatch("SyncUpdateNames")]
        [HarmonyPostfix]
        static void SyncUpdateNames_Postfix(ref string[] PlayerNames, ref string[] PlayerRanks, MainMenuManager __instance)
        {
            for (int i = 0; i < PlayerNames.Length; i++)
            {
                if (PlayerNames[i] != null)
                {
                    if (string.IsNullOrEmpty(PlayerRanks[i]))
                        continue;

                    string[] levelAndRank = PlayerRanks[i].Split(' ');

                    if (levelAndRank.Length < 2)
                        continue;

                    string translatedRanking = TranslationManager.Get("rankings", levelAndRank[2]);

                    string translated = $"Nível {levelAndRank[1]} ({translatedRanking})";

                    __instance.rankandleveltext[i].text = translated;
                }
            }
        }
    }
}
