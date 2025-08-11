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
        [HarmonyPostfix]
        private static void OnStartClient_Postfix(VoiceControlListener __instance)
        {
            __instance.StartCoroutine(VoiceControlListenerPatch.CoWaitGetPlayer(__instance));
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

            addSpellsToVocabulary(sr);

            sr.ResultReady.RemoveAllListeners();
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

            sr.StartProcessing();

            while (__instance.isActiveAndEnabled)
            {
                yield return new WaitForSeconds(30f);
                var vbt = GetOrBindVbt(__instance);
                var currentSr = GetOrBindSpeechRecognizer(__instance); 

                if (currentSr != null && vbt != null && !vbt.IsTransmitting)
                {
                    try { currentSr.StopProcessing(); }
                    catch (ObjectDisposedException) { }
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

            var pages = __instance.SpellPages;
            var count = pages == null ? -1 : pages.Count;
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning($"SpellPages count = {count}");

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

        private static readonly FieldInfo ResetMicCooldownField =
            AccessTools.Field(typeof(VoiceControlListener), "resetmiccooldown");

        private static readonly FieldInfo SrField = AccessTools.Field(typeof(VoiceControlListener), "sr");

        [HarmonyPatch("restartsr")]
        [HarmonyPrefix]
        private static bool RestartSrPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = CoRestartSr(__instance);
            return false;
        }

        private static IEnumerator CoRestartSr(VoiceControlListener inst)
        {
            var sr = GetOrBindSpeechRecognizer(inst);
            if (sr == null) yield break;

            while (sr.State != 0)
                yield return null;

            sr.StartProcessing();
        }


        [HarmonyPatch("resetmic")]
        [HarmonyPrefix]
        private static bool ResetMicPrefix(VoiceControlListener __instance)
        {
            // igual que el original con cooldown
            if (ResetMicCooldownField != null)
            {
                var last = (float)(ResetMicCooldownField.GetValue(__instance) ?? 0f);
                if (Time.time - last > 10f)
                {
                    ResetMicCooldownField.SetValue(__instance, Time.time);
                    __instance.StartCoroutine(CoResetMicLong(__instance));
                }
            }
            else
            {
                __instance.StartCoroutine(CoResetMicLong(__instance));
            }
            return false;
        }

        [HarmonyPatch("resetmiclong")]
        [HarmonyPrefix]
        private static bool ResetMicLongPrefix(VoiceControlListener __instance, ref IEnumerator __result)
        {
            __result = CoResetMicLong(__instance);
            return false;
        }

        private static IEnumerator CoResetMicLong(VoiceControlListener instance)
        {
            var oldSr = GetOrBindSpeechRecognizer(instance);
            if (oldSr != null)
            {
                try { oldSr.StopProcessing(); }
                catch (ObjectDisposedException) { }
                catch (Exception e) { MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("StopProcessing old SR: " + e.Message); }
            }

            try { SrField?.SetValue(instance, null); } catch { }

            yield return new WaitForSeconds(0.5f);

            if (oldSr != null)
                UnityEngine.Object.Destroy(oldSr);

            yield return new WaitForSeconds(0.5f);

            var newSr = instance.gameObject.AddComponent<SpeechRecognizer>();

            try { SrField?.SetValue(instance, newSr); } catch { }

            var source = instance.GetComponent<DissonanceSpeechSource>()
                         ?? instance.gameObject.AddComponent<DissonanceSpeechSource>();
            newSr.SpeechSource = source;

            var provider = instance.GetComponent<StreamingAssetsLanguageModelProvider>()
                          ?? instance.gameObject.AddComponent<StreamingAssetsLanguageModelProvider>();
            provider.language = SystemLanguage.Spanish;
            provider.languageModels = new List<StreamingAssetsLanguageModel>
    {
        new StreamingAssetsLanguageModel
        {
            language = SystemLanguage.Spanish,
            path = "LanguageModels/" + modelName
        }
    };
            newSr.LanguageModelProvider = provider;

            newSr.Vocabulary = new List<string>();
            addSpellsToVocabulary(newSr);

            if (instance.SpellPages != null)
            {
                foreach (var p in instance.SpellPages) p?.ResetVoiceDetect();
            }

            newSr.ResultReady.RemoveAllListeners();
            newSr.ResultReady.AddListener((Result res) =>
            {
                try
                {
                    tryresultMethod.Invoke(instance, new object[] { res.text ?? string.Empty });
                }
                catch (Exception e)
                {
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("Error invoking tryresult (resetmiclong): " + e);
                }
            });

            newSr.ResultReady.AddListener((Result res) =>
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("res (resetmiclong): " + res.text);
            });

            yield return new WaitForSeconds(0.1f);
            newSr.StartProcessing();
        }


        // -------- Vocabulary --------
        private static void addSpellsToVocabulary(SpeechRecognizer recognizer)
        {
            

            foreach (KeyValuePair<string[], Action<VoiceControlListener>> kv in spanishCommandMap)
            {
                foreach (string item in kv.Key)
                {
                    recognizer.Vocabulary.Add(item);
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Adding " + item + " to vocabulary");
                }
            }

            foreach (KeyValuePair<string, string[]> kv2 in spanishAdditionalCommandMap)
            {
                foreach (string item2 in kv2.Value)
                {
                    recognizer.Vocabulary.Add(item2);
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Adding " + item2 + " to vocabulary");
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


        private static readonly MethodInfo restartsrMethod =
            AccessTools.Method(typeof(VoiceControlListener), "restartsr", null, null);

        private static readonly string[] SrFieldNames = { "sr" };

        private static SpeechRecognizer GetOrBindSpeechRecognizer(VoiceControlListener inst)
        {
            FieldInfo foundField = null;

            var f = AccessTools.Field(inst.GetType(), "sr");
            if (f != null && typeof(SpeechRecognizer).IsAssignableFrom(f.FieldType))
            {
                var val = f.GetValue(inst) as SpeechRecognizer;
                if (val != null)
                        return val; 
                foundField = f; 
            }

            var sr = inst.GetComponent<SpeechRecognizer>() ??
                     inst.GetComponentInChildren<SpeechRecognizer>(true);

            if (sr != null && foundField != null)
            {
                foundField.SetValue(inst, sr);
            }

            return sr;
        }
        private static VoiceBroadcastTrigger GetOrBindVbt(VoiceControlListener inst)
        {
            FieldInfo foundField = null;

            var f = AccessTools.Field(inst.GetType(), "vbt");
            if (f != null && typeof(VoiceBroadcastTrigger).IsAssignableFrom(f.FieldType))
            {
                var val = f.GetValue(inst) as VoiceBroadcastTrigger;
                if (val != null)
                    return val; 
                foundField = f; 
            }

            var vbt = inst.GetComponent<VoiceBroadcastTrigger>() ??
                      inst.GetComponentInChildren<VoiceBroadcastTrigger>(true);

            if (vbt != null && foundField != null)
            {
                foundField.SetValue(inst, vbt);
            }

            return vbt;
        }

    }
}
