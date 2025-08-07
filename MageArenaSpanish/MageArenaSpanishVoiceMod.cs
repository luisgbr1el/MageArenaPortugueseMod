
using System;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using Recognissimo.Components;
using BepInEx.Logging;



namespace MageArenaSpanishVoiceMod
{

    [BepInPlugin("spanish.mage.arena", "Spanish Mod", "1.0.0")]
    public class MageArenaSpanishVoiceMod : BaseUnityPlugin
    {

        internal static MageArenaSpanishVoiceMod Instance { get; private set; }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000008 RID: 8 RVA: 0x000020B1 File Offset: 0x000002B1
        internal static ManualLogSource Log
        {
            get
            {
                return MageArenaSpanishVoiceMod.Instance._log;
            }
        }
        private void Awake()
        {
            this._log = base.Logger;
            MageArenaSpanishVoiceMod.Instance = this;
            MageArenaSpanishVoiceMod.Harmony = new Harmony("spanish.mage.arena");
            MageArenaSpanishVoiceMod.Harmony.PatchAll();
            MageArenaSpanishVoiceMod.Log.LogInfo("Spanish Translation Loaded");
        }


        static void PatchVoiceControlListener(VoiceControlListener __instance)
        {
            // Busca el SpeechRecognizer
            var sr = __instance.GetComponent<SpeechRecognizer>();
            if (sr == null) return;

            // Agrega comandos de voz en español al vocabulario
            sr.Vocabulary.Add("espejo");     // mirror
            sr.Vocabulary.Add("bola de fuego"); // fireball
            sr.Vocabulary.Add("congelar");   // freeze
            sr.Vocabulary.Add("gusano");     // worm
            sr.Vocabulary.Add("agujero");    // hole
            sr.Vocabulary.Add("misil magico"); // magic missile

            Debug.Log("[VozEspanolMod] Palabras en español agregadas al vocabulario de voz.");
        }


        static bool PatchTryResult(VoiceControlListener __instance, string res)
        {
            if (res == null) return true; // Llama al original si no hay texto

            // Lógica: Si se reconoce en español, llama al hechizo correspondiente
            if (res.Contains("fuego") || res.Contains("bola"))
            {
                __instance.CastFireball();
                return false;
            }
            else if (res.Contains("congelar"))
            {
                __instance.CastFrostBolt();
                return false;
            }
            else if (res.Contains("gusano"))
            {
                __instance.CastWorm();
                return false;
            }
            else if (res.Contains("agujero"))
            {
                __instance.CastHole();
                return false;
            }
            else if (res.Contains("misil") || res.Contains("magico"))
            {
                __instance.CastMagicMissle();
                return false;
            }
            else if (res.Contains("espejo"))
            {
                __instance.ActivateMirror();
                return false;
            }

            // Si no es un comando en español, permite que siga la lógica original
            return true;
        }

        private static Harmony Harmony;

        private ManualLogSource _log;
    }


}