using HarmonyLib;
using Recognissimo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MageArenaSpanish
{
    [HarmonyPatch(typeof(SetUpModelProvider), "Setup")]
    public static class SetUpModelProviderPatch
    {
/*        [HarmonyPrefix]
        public static bool Prefix(SetUpModelProvider __instance)
        {
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Cargando modelo...");

            StreamingAssetsLanguageModelProvider streamingAssetsLanguageModelProvider = __instance.gameObject.AddComponent<StreamingAssetsLanguageModelProvider>();
            streamingAssetsLanguageModelProvider.language = SystemLanguage.Spanish;
            streamingAssetsLanguageModelProvider.languageModels = new List<StreamingAssetsLanguageModel>
            {
                new StreamingAssetsLanguageModel
                {
                    language = SystemLanguage.Spanish,
                    path = "LanguageModels/" + SetUpModelProviderPatch.nameOfModel
                }
            };


            __instance.GetComponent<SpeechRecognizer>().LanguageModelProvider = streamingAssetsLanguageModelProvider;

            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Modelo Cargado");

            return false;
        }

        private static readonly string nameOfModel = "vosk-model-small-es-0.42";*/
    }
}
