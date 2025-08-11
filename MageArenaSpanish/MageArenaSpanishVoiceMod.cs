
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;



namespace MageArenaSpanishVoiceMod
{

    [BepInPlugin("spanish.mage.arena", "Spanish Mod", "1.1.2")]
    public class MageArenaSpanishVoiceMod : BaseUnityPlugin
    {

        internal static MageArenaSpanishVoiceMod Instance { get; private set; }

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
            MageArenaSpanishVoiceMod.Log.LogWarning("Spanish Translation Loaded");
            HarmonyLib.Harmony.DEBUG = true;

        }

        private static Harmony Harmony;

        private ManualLogSource _log;
    }


}