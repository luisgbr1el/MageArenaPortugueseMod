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
        // -------- Patches --------

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
            // Espera a PlayerInventory como el original
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

            // 1) Setup del modelo (igual que el original). Tu patch del ctor de LanguageModel ya redirige la ruta.
            var setup = __instance.GetComponent<SetUpModelProvider>() ??
                        __instance.gameObject.AddComponent<SetUpModelProvider>();
            setup.Setup();
            yield return null;

            // 2) Obtener SpeechRecognizer y añadir vocabulario
            var sr = GetOrBindSpeechRecognizer(__instance);
            if (sr == null)
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("SpeechRecognizer no encontrado.");
                yield break;
            }

            addSpellsToVocabulary(sr); // usa tu función existente

            // 3) Suscribirse a ResultReady → llamar tryresult(res.text) como hace el juego
            sr.ResultReady.RemoveAllListeners(); // evita doble binding si se reinicia
            sr.ResultReady.AddListener((Result r) =>
            {
                try
                {
                    tryresultMethod.Invoke(__instance, new object[] { r.text ?? string.Empty });
                }
                catch (Exception e)
                {
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("Error al invocar tryresult: " + e);
                }
            });

            float t = 0f;
            while ((__instance.SpellPages == null || __instance.SpellPages.Count == 0) && t < 2f)
            {
                yield return null;
                t += Time.deltaTime;
            }

            // 4) Arrancar el reconocedor
            sr.StartProcessing();

            // 5) Healthcheck cada 30s (igual que el original)
            while (__instance.isActiveAndEnabled)
            {
                yield return new WaitForSeconds(30f);
                var vbt = vbtRef(__instance);
                if (vbt != null && !vbt.IsTransmitting)
                {
                    sr.StopProcessing();
                    __instance.StartCoroutine((IEnumerator)restartsrMethod.Invoke(__instance, null)); // llamas al original
                }
            }
        }




        private static string modelName = "vosk-model-small-es-0.42";

        [HarmonyPatch(typeof(LanguageModel), MethodType.Constructor, new Type[] { typeof(string) })]
        [HarmonyPrefix]
        public static void LanguageModel_Ctor_Prefix(ref string path)
        {
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("Cargando modelo español desde: " + path);
            string myPluginPath = MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Instance.Info.Location;
            string modDir = Path.GetDirectoryName(myPluginPath);
            string modPath = Path.Combine(modDir, "LanguageModels", modelName);
            path = modPath;
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("Redirigido modelo a: " + path);

        }


        private static readonly MethodInfo tryresultMethod =
            AccessTools.Method(typeof(VoiceControlListener), "tryresult", new[] { typeof(string) });


        [HarmonyPatch("tryresult")]
        [HarmonyPrefix]
        private static bool TryResultPrefix(VoiceControlListener __instance, string res)
        {

            if (string.IsNullOrWhiteSpace(res))
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Hearing: <EMPTY>");
                return false;
            }

            res = res.ToLowerInvariant().Trim();
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Hearing: " + res);


            // 1) comandos directos (no dependen de páginas)
            foreach (var kv in spanishCommandMap)
            {
                if (kv.Key.Any(keyword => res.Contains(keyword)))
                    kv.Value(__instance);
            }

            // 2) iteración directa sobre TODAS las páginas (como antes)
            foreach (ISpellCommand spellPage in __instance.SpellPages)
            {
                // nombre interno de la página
                string pageName = spellPage.GetSpellName();

                // a) usa el diccionario escalable: si el texto contiene alguna keyword española
                //    y el nombre interno de la página coincide con la key → castea
                foreach (var pair in spanishAdditionalCommandMap)
                {
                    // pair.Key = nombre interno (p.ej. "blink", "blast", "rock"...)
                    // pair.Value = keywords en español para activar ese interno
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
                        // si quieres evitar dobles casteos en la misma página, descomenta:
                        // break;
                    }
                }

                // b) fallback como el original: si el texto ya contiene el nombre del spell, castea
                //    (útil si el usuario dice el nombre interno tal cual)
                if (res.Contains(pageName.ToLowerInvariant()))
                {
                    spellPage.TryCastSpell();
                }
            }

            // IMPORTANT: no reiniciar el reconocedor aquí
            return false; // omitimos el original
        }


        private static IEnumerator RestartRecognizerCoroutine(VoiceControlListener inst)
        {
            var sr = GetOrBindSpeechRecognizer(inst);
            if (sr == null)
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogError("Sr es null al reiniciar");
                yield break;
            }

            // Parar → un frame → arrancar
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


        // -------- Vocabulario en español --------
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
            // 1) Probar campos comunes
            FieldInfo foundField = null;
            foreach (var name in SrFieldNames)
            {
                var f = AccessTools.Field(inst.GetType(), name);
                if (f != null && typeof(SpeechRecognizer).IsAssignableFrom(f.FieldType))
                {
                    var val = f.GetValue(inst) as SpeechRecognizer;
                    if (val != null)
                        return val; // ya está asignado
                    foundField = f; // recuerda un campo compatible para inyectar luego
                }
            }

            // 2) Buscar el componente en la jerarquía
            var sr = inst.GetComponent<SpeechRecognizer>() ??
                     inst.GetComponentInChildren<SpeechRecognizer>(true);

            // 3) Si lo encontramos y tenemos un campo compatible, inyectarlo
            if (sr != null && foundField != null)
            {
                foundField.SetValue(inst, sr);
            }

            return sr;
        }
    }
}
