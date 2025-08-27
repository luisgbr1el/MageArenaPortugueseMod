using HarmonyLib;
using MageArenaPortuguese.Config;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MageArenaPortuguese.Patches
{
    [HarmonyPatch]
    class InteractablePatch
    {
        private const string _graspPrefix = "Grasp ";
        private const string _makePrefix = "Make ";
        private const string _placePrefix = "Place ";
        private const string _openPrefix = "Open ";
        private const string _openePrefix = "Opene ";
        private const string _shutPrefix = "Shut ";

        private static readonly HashSet<string> loggedTexts = new HashSet<string>();

        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var type in AccessTools.AllTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (typeof(IInteractable).IsAssignableFrom(type) ||
                    type.GetInterface("ITimedInteractable") != null)
                {
                    var m = AccessTools.Method(type, "DisplayInteractUI");
                    if (m != null)
                        yield return m;
                }
            }
        }

        static void Postfix(ref string __result)
        {
            if (string.IsNullOrWhiteSpace(__result))
                return;

            TranslationManager.InteractionType? interactionType = null;

            if (__result.StartsWith(_graspPrefix, StringComparison.OrdinalIgnoreCase))
            {
                interactionType = TranslationManager.InteractionType.Pegar;
                __result = __result.Substring(_graspPrefix.Length).Trim();
            }
            else if (__result.StartsWith(_makePrefix, StringComparison.OrdinalIgnoreCase))
            {
                interactionType = TranslationManager.InteractionType.Fazer;
                __result = __result.Substring(_makePrefix.Length).Trim();
            }
            else if (__result.StartsWith(_placePrefix, StringComparison.OrdinalIgnoreCase))
            {
                interactionType = TranslationManager.InteractionType.Colocar;
                __result = __result.Substring(_placePrefix.Length).Trim();
            }
            else if (__result.StartsWith(_openPrefix, StringComparison.OrdinalIgnoreCase) || __result.StartsWith(_openePrefix, StringComparison.OrdinalIgnoreCase))
            {
                interactionType = TranslationManager.InteractionType.Abrir;
                __result = __result.Substring(_openPrefix.Length).Trim();
            }
            else if (__result.StartsWith(_shutPrefix, StringComparison.OrdinalIgnoreCase))
            {
                interactionType = TranslationManager.InteractionType.Fechar;
                __result = __result.Substring(_shutPrefix.Length).Trim();
            }

            string translated = TranslationManager.Get("interactable", __result, interactionType);

            if (translated != __result)
                __result = translated;
            else
            {
                string key = __result.Trim();

                // Logs para debugging
                //if (loggedTexts.Add(key))
                //{
                //    MageArenaPortugueseVoiceMod.MageArenaPortugueseMod.Log.LogWarning($"NÃO TRADUZIDO (Item): \"{key}\"");
                //}
            }
        }
    }
}