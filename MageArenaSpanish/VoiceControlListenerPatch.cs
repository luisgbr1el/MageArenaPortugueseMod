using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BepInEx;
using BlackMagicAPI;
using HarmonyLib;
using Recognissimo;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.Profiling;

namespace MageArenaSpanish.Patches.Voice
{
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(VoiceControlListener))]
    internal class VoiceControlListenerPatch
    {
        [HarmonyPatch("OnStartClient")]
        [HarmonyPrefix]
        private static void OnStartClient_Prefix(VoiceControlListener __instance)
        {
            __instance.StartCoroutine(VoiceControlListenerPatch.CoWaitGetPlayer(__instance));
        }

        private static IEnumerator CoWaitGetPlayer(VoiceControlListener __instance)
        {
            while (__instance.pi == null)
            {
                PlayerInventory playerInventory = null;
                bool flag = Camera.main.transform.parent != null && Camera.main.transform.parent.TryGetComponent<PlayerInventory>(out playerInventory);



                if (flag)
                {
                    __instance.pi = playerInventory;
                }
                yield return null;
                playerInventory = null;
            }
            yield return null;
            yield return new WaitForSeconds(0.5f);
            VoiceControlListenerPatch.AddToVocabulary(__instance);
            yield break;
        }

        private static void AddToVocabulary(VoiceControlListener __instance)
        {
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Cargando vocabulario nuevo...");

            // Accede a 'sr' aunque sea privado
            var srField = AccessTools.Field(typeof(VoiceControlListener), "sr");
            var sr = srField.GetValue(__instance) as SpeechRecognizer;
            if (sr == null)
            {
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("No se pudo obtener SpeechRecognizer.");
                return;
            }

            sr.Vocabulary.Add("espejo");     // mirror
            sr.Vocabulary.Add("bola de fuego"); // fireball
            sr.Vocabulary.Add("congelar");   // freeze
            sr.Vocabulary.Add("entrada");     // worm
            sr.Vocabulary.Add("salida");    // hole
            sr.Vocabulary.Add("misil magico"); // magic missile

            foreach (string text in VoiceControlListenerPatch._phoneticMap.Keys)
            {
                if (!sr.Vocabulary.Contains(text))
                {
                    sr.Vocabulary.Add(text);
                    MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("Añadido " + text + " al vocabulario.");
                }
            }
        }

        [HarmonyPatch("tryresult")]
        [HarmonyPrefix]
        private static void TryResult_LogPrefix(string res)
        {
            // Puedes usar tu logger, aquí uso Debug.Log para que se vea en el log de Unity
            //Debug.Log("[Reconocido por voz] " + res);
            // Si tienes tu logger personalizado:
            if (!string.IsNullOrEmpty(res))
                MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogInfo("[Reconocido por voz] " + res);
        }


        [HarmonyPatch("tryresult")]
        [HarmonyPrefix]
        private static bool TryResult_Prefix(VoiceControlListener __instance, string res)
        {
            // ¡AQUÍ TU NUEVA LÓGICA!
            if (!string.IsNullOrEmpty(res))
            {
                // Logging para debug
                Debug.Log("[Reconocido por voz] " + res);

                // Comandos en inglés (original) y español (tú modificas)
                if (res.Contains("fire") || res.Contains("ball") || res.Contains("fuego") || res.Contains("bola"))
                {
                    __instance.CastFireball();
                }
                else if (res.Contains("freeze") || res.Contains("ease") || res.Contains("congelar"))
                {
                    __instance.CastFrostBolt();
                }
                else if (res.Contains("worm") || res.Contains("entrada"))
                {
                    __instance.CastWorm();
                }
                else if (res.Contains("hole") || res.Contains("salida"))
                {
                    __instance.CastHole();
                }
                else if (res.Contains("missle") || res.Contains("magic") || res.Contains("misil") || res.Contains("mágico") || res.Contains("magico") )
                {
                    __instance.CastMagicMissle();
                }
                else if (res.Contains("mirror") || res.Contains("espejo"))
                {
                    __instance.ActivateMirror();
                }

                if (res.Contains("blink") || res.Contains("parpadeo"))
                {
                    foreach (ISpellCommand spellPage in __instance.SpellPages)
                    {
                        if (spellPage.GetSpellName() == "blink" || spellPage.GetSpellName() == "parpadeo")
                        {
                            spellPage.TryCastSpell();
                        }
                    }
                    return false; // No ejecuta el original
                }

                if (res.Contains("dark") || res.Contains("oscuro"))
                {
                    foreach (ISpellCommand spellPage2 in __instance.SpellPages)
                    {
                        if (spellPage2.GetSpellName() == "blast" || spellPage2.GetSpellName() == "explosión")
                        {
                            spellPage2.TryCastSpell();
                        }
                    }
                    return false;
                }

                foreach (ISpellCommand spellPage3 in __instance.SpellPages)
                {
                    if (res.Contains(spellPage3.GetSpellName()))
                    {
                        spellPage3.TryCastSpell();
                    }
                }
                return false; // Muy importante: esto cancela el método original
            }

            // Si no había comando, sigue el mismo comportamiento original:
            var srField = AccessTools.Field(typeof(VoiceControlListener), "sr");
            var sr = srField.GetValue(__instance) as Recognissimo.Components.SpeechRecognizer;
            if (sr != null)
            {
                sr.StopProcessing();
                __instance.StartCoroutine((System.Collections.IEnumerator)AccessTools.Method(typeof(VoiceControlListener), "restartsr").Invoke(__instance, null));
            }

            // Y retorna false para NO ejecutar el original
            return false;
        }

        [HarmonyPatch(typeof(LanguageModel), MethodType.Constructor, new Type[] { typeof(string) })]
        [HarmonyPrefix]
        public static void LanguageModel_Ctor_Prefix(ref string path)
        {
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("Cargando modelo español: " + path);
            string myPluginPath = MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Instance.Info.Location;
            string modDir = Path.GetDirectoryName(myPluginPath);
            string modPath = Path.Combine(modDir, "LanguageModels", "vosk-model-small-en-us-0.15");
            path = modPath;
            MageArenaSpanishVoiceMod.MageArenaSpanishVoiceMod.Log.LogWarning("Redirigido modelo a: " + path);

        }

        private static readonly Dictionary<string, string> _phoneticMap = new Dictionary<string, string>
        {
            {
                "fuegos",
                "fuego"
            },
            {
                "juego",
                "fuego"
            },
            {
                "fuego",
                "fuego"
            },
            {
                "fiar",
                "fire"
            },
            {
                "fayer",
                "fire"
            },
            {
                "bola",
                "ball"
            },
            {
                "bolas",
                "ball"
            },
            {
                "frees",
                "freeze"
            },
            {
                "ease",
                "freeze"
            },
            {
                "frieze",
                "freeze"
            },
            {
                "friz",
                "freeze"
            },
            {
                "freese",
                "freeze"
            },
            {
                "entrada",
                "worm"
            },
            {
                "salida",
                "hole"
            }
/*            {
                "froze",
                "freeze"
            },
            {
                "fleez",
                "freeze"
            },
            {
                "furizu",
                "freeze"
            },
            {
                "werm",
                "worm"
            },
            {
                "vorm",
                "worm"
            },
            {
                "warm",
                "worm"
            },
            {
                "wurm",
                "worm"
            },
            {
                "walrm",
                "worm"
            },
            {
                "hol",
                "hole"
            },
            {
                "howl",
                "hole"
            },
            {
                "hohl",
                "hole"
            },
            {
                "haul",
                "hole"
            },
            {
                "hool",
                "hole"
            },
            {
                "missel",
                "missle"
            },
            {
                "misle",
                "missle"
            },
            {
                "majik",
                "magico"
            },
            {
                "madgic",
                "magico"
            },
            {
                "mejik",
                "magic"
            },
            {
                "missail",
                "missle"
            },
            {
                "misail",
                "missle"
            },
            {
                "mizile",
                "missle"
            },
            {
                "blenk",
                "blink"
            },
            {
                "blank",
                "blink"
            },
            {
                "bleenk",
                "blink"
            },
            {
                "brink",
                "blink"
            },
            {
                "burinku",
                "blink"
            },
            {
                "dork",
                "dark"
            },
            {
                "derk",
                "dark"
            },
            {
                "dak",
                "dark"
            },
            {
                "dahk",
                "dark"
            },
            {
                "blost",
                "blast"
            },
            {
                "blust",
                "blast"
            },
            {
                "brast",
                "blast"
            },
            {
                "burasto",
                "blast"
            },
            {
                "merror",
                "mirror"
            },
            {
                "mira",
                "mirror"
            },
            {
                "meeror",
                "mirror"
            },
            {
                "mirrar",
                "mirror"
            },
            {
                "mirorr",
                "mirror"
            },
            {
                "miller",
                "mirror"
            },
            {
                "miroru",
                "mirror"
            },
            {
                "devine",
                "divine"
            },
            {
                "divayn",
                "divine"
            },
            {
                "diveen",
                "divine"
            },
            {
                "divyne",
                "divine"
            },
            {
                "dibin",
                "divine"
            },
            {
                "divain",
                "divine"
            },
            {
                "divinu",
                "divine"
            },
            {
                "wips",
                "wisp"
            },
            {
                "vesp",
                "wisp"
            },
            {
                "whisp",
                "wisp"
            },
            {
                "wesp",
                "wisp"
            },
            {
                "wipsu",
                "wisp"
            },
            {
                "vispu",
                "wisp"
            },
            {
                "wispu",
                "wisp"
            },
            {
                "thunder",
                "thunder"
            },
            {
                "thuner",
                "thunder"
            },
            {
                "tunder",
                "thunder"
            },
            {
                "sander",
                "thunder"
            },
            {
                "thunda",
                "thunder"
            },
            {
                "sandabolt",
                "thunder"
            },
            {
                "bolt",
                "bolt"
            },
            {
                "bolto",
                "bolt"
            },
            {
                "volto",
                "bolt"
            },
            {
                "bort",
                "bolt"
            },
            {
                "bold",
                "bolt"
            },
            {
                "thunderbolto",
                "thunder"
            },
            {
                "thunderbold",
                "thunder"
            },
            {
                "rok",
                "rock"
            },
            {
                "rokk",
                "rock"
            },
            {
                "raku",
                "rock"
            },
            {
                "wock",
                "rock"
            }*/
        };


        private static readonly HashSet<string> _highConfidenceSpells = new HashSet<string>
        {
            "fire",
            "ball",
            "freeze",
            "entrada",
            "salida",
            "misil",
            "magico",
            "flash",
            "dark",
            "blast"
        };
    }
}