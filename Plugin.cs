using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;
using SquishCompany.MonoBehaviours;
using SquishCompany.Extensions;

namespace SquishCompany
{
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "ByCubed7.SquishCompany";
        public const string modName = "SquishCompany";
        public const string modVersion = "0.0.4";

        private readonly Harmony harmony = new Harmony(modGUID);
        internal static ManualLogSource logger;

        private static Plugin Instance;

        public static ConfigFile GeneralConfig;
        public static ConfigFile VolumeConfig;

        private void Awake() {

            if (Instance != null)
                throw new Exception("SquishCompanyBase already has an instance! Is this mod added twice?");

            Instance = this;

            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            logger.LogInfo("Loading general configs.");
            GeneralConfig = new ConfigFile($"{Paths.ConfigPath}\\{modName}.cfg", true);
            VolumeConfig = new ConfigFile($"{Paths.ConfigPath}\\{modName}.AudioVolume.cfg", true);

            MainAssets = LoadAssetBundle();
            InitCustomItems();
            LoadAndRegisterAllItems();

            logger.LogInfo("Loading volume configs.");
            SquishConfig.LoadVolumeFrom(VolumeConfig, CustomItems);

            harmony.PatchAll(typeof(Plugin));
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Main Assets

        public static AssetBundle MainAssets { get; private set; }
        public static AssetBundle LoadAssetBundle()
        {
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sAssetLocation = Path.Combine(sAssemblyLocation, modName.ToLower());

            var assets = AssetBundle.LoadFromFile(sAssetLocation);
            if (assets == null) logger.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
            return assets;
        }
        public static void DEBUG_ASSETBUNDLE()
        {
            logger.LogInfo($"All assets:");
            foreach (string name in MainAssets.GetAllAssetNames())
                logger.LogInfo($"  - {name}");
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Custom Items

        // All of the items, enabled or not
        public static List<CustomItemData> CustomItems = new List<CustomItemData>();
        public static void InitCustomItems()
        {
            CustomItems = new List<CustomItemData>()
            {
                new CustomItemData("SquishMellow")
                    .SetMonoBehaviour<SquishMellow>()
                    .BindEnabled(GeneralConfig, true)
                    .BindScrap(GeneralConfig, 15, 40)
            };

            logger.LogInfo("Custom items initialized!");
        }

        public static Dictionary<string, CustomItemData> CustomItemDatas = new Dictionary<string, CustomItemData>();
        public static void LoadAndRegisterAllItems()
        {
            foreach (CustomItemData itemData in CustomItems)
            {
                if (!itemData.Enabled) continue;

                itemData.LoadAssetFrom(MainAssets);
                itemData.Register();

                CustomItemDatas.Add(itemData.name, itemData);
            }
        }
    }
}
