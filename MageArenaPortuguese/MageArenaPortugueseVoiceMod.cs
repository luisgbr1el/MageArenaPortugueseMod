using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace MageArenaPortugueseVoiceMod
{

    [BepInPlugin("com.luisgbr1el.MageArenaPortugueseVoiceMod", "MageArenaPortugueseVoiceMod", "1.0.0")]
    public class MageArenaPortugueseVoiceMod : BaseUnityPlugin
    {
        internal static MageArenaPortugueseVoiceMod Instance { get; private set; }

        internal static ManualLogSource Log
        {
            get
            {
                return MageArenaPortugueseVoiceMod.Instance._log;
            }
        }

        private void Awake()
        {
            this._log = base.Logger;
            MageArenaPortugueseVoiceMod.Instance = this;
            MageArenaPortugueseVoiceMod.Harmony = new Harmony("com.luisgbr1el.MageArenaPortugueseVoiceMod");
            MageArenaPortugueseVoiceMod.Harmony.PatchAll();
            MageArenaPortugueseVoiceMod.Log.LogWarning("Portuguese Translation Loaded");
            HarmonyLib.Harmony.DEBUG = true;

        }

        private static Harmony Harmony;

        private ManualLogSource _log;
    }
}