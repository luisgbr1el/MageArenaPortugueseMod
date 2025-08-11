using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dissonance;
using HarmonyLib;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine; 

namespace MageArenaSpanishVoice.Patches
{
    [HarmonyPatch(typeof(VoiceControlListener))]
    public static class VoiceControlListenerPatch
    {

        [HarmonyPatch("OnStartClient")]
        [HarmonyPrefix]
        private static bool OnStartClient_Prefix(VoiceControlListener __instance)
        {
            __instance.StartCoroutine(VoiceControlListenerPatch.CoWaitGetPlayer(__instance));
            __instance.SpellPages = new List<ISpellCommand>();
            return false;
        }

        private static IEnumerator CoWaitGetPlayer(VoiceControlListener __instance)
        {
            while (__instance.pi == null)
            {
                if (Camera.main != null &&
                    Camera.main.transform.parent != null &&
                    Camera.main.transform.parent.TryGetComponent<PlayerInventory>(out var pi))
                {
                    __instance.pi = pi;
                }
                yield return null;
            }

            yield return null;
            yield return new WaitForSeconds(0.5f);

            var setup = __instance.GetComponent<SetUpModelProvider>() ??
                        __instance.gameObject.AddComponent<SetUpModelProvider>();
            setup.Setup();
            yield return null;

            var sr = GetOrBindSpeechRecognizer(__instance);
            if (sr == null)
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("SpeechRecognizer not found.");
                yield break;
            }

            addSpellsToVocabulary(sr); // usa tu función existente

            sr.ResultReady.RemoveAllListeners(); // evita doble binding si se reinicia
            sr.ResultReady.AddListener((Result r) =>
            {
                try
                {
                    tryresultMethod.Invoke(__instance, new object[] { r.text ?? string.Empty });
                }
                catch (Exception e)
                {
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("Error invoking tryresult: " + e);
                }
            });

            float t = 0f;
            while ((__instance.SpellPages == null || __instance.SpellPages.Count == 0) && t < 2f)
            {
                yield return null;
                t += Time.deltaTime;
            }

            sr.StartProcessing();

            while (__instance.isActiveAndEnabled)
            {
                yield return new WaitForSeconds(30f);
                var vbt = vbtRef(__instance);
                if (vbt != null && !vbt.IsTransmitting)
                {
                    sr.StopProcessing();
                    __instance.StartCoroutine((IEnumerator)restartsrMethod.Invoke(__instance, null));
                }
            }
        }

        private static string modelName = "vosk-model-small-es-0.42";

        [HarmonyPatch(typeof(LanguageModel), MethodType.Constructor, new Type[] { typeof(string) })]
        [HarmonyPrefix]
        public static void LanguageModel_Ctor_Prefix(ref string path)
        {
            string myPluginPath = MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Instance.Info.Location;
            string modDir = Path.GetDirectoryName(myPluginPath);
            string modPath = Path.Combine(modDir, "LanguageModels", modelName);
            path = modPath;
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Loading language model from: " + path);

        }


        private static readonly MethodInfo tryresultMethod =
            AccessTools.Method(typeof(VoiceControlListener), "tryresult", new[] { typeof(string) });


        [HarmonyPatch("tryresult")]
        [HarmonyPrefix]
        private static bool TryResultPrefix(VoiceControlListener __instance, string res)
        {

            if (string.IsNullOrWhiteSpace(res))
            {
                return false;
            }

            res = res.ToLowerInvariant().Trim();
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Hearing: " + res);


            foreach (var kv in spanishCommandMap)
            {
                if (kv.Key.Any(keyword => res.Contains(keyword)))
                    kv.Value(__instance);
            }

            foreach (ISpellCommand spellPage in __instance.SpellPages)
            {
                string pageName = spellPage.GetSpellName();

                foreach (var pair in spanishAdditionalCommandMap)
                {
                    bool hit = false;
                    foreach (var kw in pair.Value)
                    {
                        if (res.Contains(kw))
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (!hit) continue;

                    if (string.Equals(pageName, pair.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        spellPage.TryCastSpell();
                    }
                }

                if (res.Contains(pageName.ToLowerInvariant()))
                {
                    spellPage.TryCastSpell();
                }
            }

            return false;
        }


        private static IEnumerator RestartRecognizerCoroutine(VoiceControlListener inst)
        {
            var sr = GetOrBindSpeechRecognizer(inst);
            if (sr == null)
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("Sr is null");
                yield break;
            }

            sr.StopProcessing();
            yield return null;

            sr.StartProcessing();
        }



/*        [HarmonyPatch("resetmiclong")]
        [HarmonyPrefix]
        private static bool ResetMicLongPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = ModifiedResetMicLong(__instance);
            return false;
        }

        private static IEnumerator ModifiedResetMicLong(VoiceControlListener instance)
        {
            var sr = GetOrBindSpeechRecognizer(instance);
            if (sr != null) sr.StopProcessing();

            yield return new WaitForSeconds(1f);

            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("MIC RESET");
            instance.StartCoroutine((IEnumerator)restartsrMethod.Invoke(instance, null));
            yield break;
        }

        [HarmonyPatch("restartsr")]
        [HarmonyPrefix]
        private static bool RestartSrPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = SafeRestartSr(__instance);
            return false;
        }

        private static IEnumerator SafeRestartSr(VoiceControlListener instance)
        {
            var sr = GetOrBindSpeechRecognizer(instance);
            if (sr != null) sr.StopProcessing();

            // Un frame de respiro y reiniciar
            yield return null;
            instance.StartCoroutine((IEnumerator)restartsrMethod.Invoke(instance, null));
            yield break;
        }*/


        // -------- Vocabulary --------
        private static void addSpellsToVocabulary(SpeechRecognizer recognizer)
        {
            

            foreach (KeyValuePair<string[], Action<VoiceControlListener>> kv in spanishCommandMap)
            {
                foreach (string item in kv.Key)
                {
                    recognizer.Vocabulary.Add(item);
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("Adding " + item + " to vocabulary");
                }
            }

            foreach (KeyValuePair<string, string[]> kv2 in spanishAdditionalCommandMap)
            {
                foreach (string item2 in kv2.Value)
                {
                    recognizer.Vocabulary.Add(item2);
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("Adding " + item2 + " to vocabulary");
                }
            }
        }

        private static readonly Dictionary<string[], Action<VoiceControlListener>> spanishCommandMap =
            new Dictionary<string[], Action<VoiceControlListener>>
            {
                {
                    new string[] { "bola", "fuego" }, // "fireball"
                    v => v.CastFireball()
                },
                {
                    new string[] { "congelar" }, // freeze
                    v => v.CastFrostBolt()
                },
                {
                    new string[] { "entrada" }, // worm
                    v => v.CastWorm()
                },
                {
                    new string[] { "salida" }, // hole
                    v => v.CastHole()
                },
                {
                    new string[] { "misil", "mágico" }, // "magic missile"
                    v => v.CastMagicMissle()
                },
                {
                    new string[] { "espejo" }, // mirror
                    v => v.ActivateMirror()
                }
            };

        private static readonly Dictionary<string, string[]> spanishAdditionalCommandMap =
            new Dictionary<string, string[]>
            {
                { "rock",        new[] { "roca", "piedra" } }, // rock
                { "wisp",        new[] { "ánima", "luz" } }, // wisp
                { "blast",       new[] { "ráfaga", "oscura", "rafaga" } },        // dark blast
                { "divine",      new[] { "divina", "luz divina" } },   // divine light
                { "blink",       new[] { "destello", "teletransporte", "parpadeo" } }, //blink
                { "thunderbolt", new[] { "rayo", "trueno" } } //thunderbolt
            };


        private static readonly AccessTools.FieldRef<VoiceControlListener, VoiceBroadcastTrigger> vbtRef =
            AccessTools.FieldRefAccess<VoiceControlListener, VoiceBroadcastTrigger>("vbt");

        private static readonly MethodInfo restartsrMethod =
            AccessTools.Method(typeof(VoiceControlListener), "restartsr", null, null);

        private static readonly string[] SrFieldNames = { "sr", "_sr", "recognizer", "speechRecognizer", "speechRec" };

        private static SpeechRecognizer GetOrBindSpeechRecognizer(VoiceControlListener inst)
        {
            FieldInfo foundField = null;
            foreach (var name in SrFieldNames)
            {
                var f = AccessTools.Field(inst.GetType(), name);
                if (f != null && typeof(SpeechRecognizer).IsAssignableFrom(f.FieldType))
                {
                    var val = f.GetValue(inst) as SpeechRecognizer;
                    if (val != null)
                        return val; 
                    foundField = f; 
                }
            }

            var sr = inst.GetComponent<SpeechRecognizer>() ??
                     inst.GetComponentInChildren<SpeechRecognizer>(true);

            if (sr != null && foundField != null)
            {
                foundField.SetValue(inst, sr);
            }

            return sr;
        }
    }
}
