using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace MageArenaPortugueseVoiceMod
{

    [BepInPlugin("com.luisgbr1el.MageArenaPortugueseVoiceMod", "MageArenaPortugueseVoiceMod", "1.0.0")]
    public class MageArenaPortugueseMod : BaseUnityPlugin
    {
        internal static MageArenaPortugueseMod Instance { get; private set; }

        internal static ManualLogSource Log
        {
            get
            {
                return MageArenaPortugueseMod.Instance._log;
            }
        }

        private void Awake()
        {
            this._log = base.Logger;
            MageArenaPortugueseMod.Instance = this;
            MageArenaPortugueseMod.Harmony = new Harmony("com.luisgbr1el.MageArenaPortugueseVoiceMod");
            MageArenaPortugueseMod.Harmony.PatchAll();
            MageArenaPortugueseMod.Log.LogWarning("Portuguese Translation Loaded");
            HarmonyLib.Harmony.DEBUG = true;

        }

        private static Harmony Harmony;

        private ManualLogSource _log;
    }
}