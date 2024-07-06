using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using MGSC;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Windows.Speech;

namespace localmodloader
{
    [BepInPlugin("kahdeg.localmodloader", "kahdeg_localmodloader", "1.0.0")]
    public class LocalModloader : BaseUnityPlugin
    {
        public static LocalModloader Instance { get; private set; }
        public static ManualLogSource Logger { get; private set; }
        public static string LocalModsDirectory { get; private set; }

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            Logger.LogInfo("LocalModloader loaded!");
            LocalModsDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(MGSC.LogCollector.GetLogPath()), "mods"));
            Directory.CreateDirectory(LocalModsDirectory);
            Logger.LogInfo($"Local mods directory: {LocalModsDirectory}");

            var harmony = new Harmony("kahdeg.localmodloader");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SteamWrapper), "HandleQueryCompleted")]
    public static class Patch_SteamWrapper_HandleQueryCompleted
    {
        public static void Postfix(Steamworks.SteamUGCQueryCompleted_t response, bool bIOFailure, SteamWrapper __instance)
        {
            LocalModloader.Logger.LogInfo("Loading local mods...");
            string[] localMods = Directory.GetDirectories(LocalModloader.LocalModsDirectory);
            for (int i = 0; i < localMods.Length; i++)
            {
                __instance.WorkshopItems.Add((ulong)40000000000 + (ulong)i, localMods[i]);
            }
        }
    }
}
