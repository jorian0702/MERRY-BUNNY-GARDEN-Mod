using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace MerryBunnyCheat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"[MerryBunnyCheat] Initializing...");

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var go = new GameObject("MerryBunnyCheatController");
            GameObject.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<CheatController>();

            Log.LogInfo("[MerryBunnyCheat] Ready!");
            Log.LogInfo("[MerryBunnyCheat] F1=FullUnlock F2=MaxPanties F3=AllSRank F4=Gallery F5=TextRead F10=Help");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.jorian.merrybunnycheat";
        public const string PLUGIN_NAME = "Merry Bunny Garden Cheat";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
