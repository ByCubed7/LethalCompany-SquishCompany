using BepInEx;
using BepInEx.Logging;
using LethalLib.Extras;
using LethalLib.Modules;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using BepInEx.Configuration;
using Unity.Netcode.Components;
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
        public const string modVersion = "0.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);
        internal static ManualLogSource logger;

        private static Plugin Instance;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Config 

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
            SquishConfig.LoadGeneralFrom(ref GeneralConfig);

            //logger.LogInfo("Removing default items.");
            //foreach (var item in Items.scrapItems)
           //     Items.RemoveScrapFromLevels(item.origItem, Levels.LevelTypes.All);

            LoadAllCustomItems();

            logger.LogInfo("Loading volume configs.");
            SquishConfig.LoadVolumeFrom(ref VolumeConfig, Prefabs, CustomItemDatas);


            harmony.PatchAll(typeof(Plugin));
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public static AssetBundle MainAssets
        {
            get {
                if (_mainAssets == null)
                {
                    _mainAssets = LoadAssetBundle();
                    logger.LogInfo("Loaded asset bundle");
                } 
                return _mainAssets;
            }
        }
        private static AssetBundle _mainAssets;

        public static void DEBUG_ASSETBUNDLE()
        {
            logger.LogInfo($"All assets:");
            foreach (string name in MainAssets.GetAllAssetNames())
                logger.LogInfo($"  - {name}");
            
        }

        public static AssetBundle LoadAssetBundle()
        {
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sAssetLocation = Path.Combine(sAssemblyLocation, modName.ToLower());

            var assets = AssetBundle.LoadFromFile(sAssetLocation);
            if (assets == null) logger.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
            return assets;
        }

        public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
        public static Dictionary<string, CustomItemData> CustomItemDatas = new Dictionary<string, CustomItemData>();
        public static void LoadAllCustomItems()
        {
            List<CustomItemData> customItems = new List<CustomItemData>()
            {
                new CustomItemData("SquishMellow")
                    .SetEnabled(SquishConfig.squishMellowEnabled.Value)
                    .MakeScrap(SquishConfig.squishMellowPrice.Value, SquishConfig.squishMellowRarity.Value)
                    .SetMonoBehaviour<SquishMellow>()
            };

            foreach (CustomItemData itemData in customItems)
                RegisterItem(itemData);

            foreach (CustomItemData itemData in customItems)
                CustomItemDatas.Add(itemData.name, itemData);

            logger.LogInfo("Custom items loaded!");
        }

        public static void RegisterItem(CustomItemData customItemData_)
        {
            if (!customItemData_.enabled) return;
            customItemData_.scrapRarity = 10;

            logger.LogInfo($"Attempting to load {customItemData_.name} at {customItemData_.itemPath}");

            Item itemAsset = MainAssets.LoadAsset<Item>(customItemData_.itemPath);

            if (itemAsset != null) logger.LogInfo($"Loaded!");
            else logger.LogError($"Failed to load item {customItemData_.itemPath} from asset!!");
            if (itemAsset.spawnPrefab == null) logger.LogError($"Items spawnPrefab is null!");

            itemAsset.spawnPrefab.EnsureNetworkTransform();
            itemAsset.spawnPrefab.OverridePhysicsPropWith(customItemData_.monoBehaviour);

            LethalLib.Modules.Utilities.FixMixerGroups(itemAsset.spawnPrefab);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(itemAsset.spawnPrefab);

            Prefabs.Add(customItemData_.name, itemAsset.spawnPrefab);

            // NOTE: Can items not be both buyable and scrap?
            if (customItemData_.IsBuyable)
            {
                logger.LogInfo($"Registering shop item {customItemData_.name} with price {customItemData_.buyValue}");
                Items.RegisterShopItem(itemAsset, null, null, MainAssets.LoadAsset<TerminalNode>(customItemData_.infoPath), customItemData_.buyValue);
            }
            else if (customItemData_.IsScrap)
            {
                logger.LogInfo($"Registering scrap item {customItemData_.name}");
                Items.RegisterScrap(itemAsset, customItemData_.scrapRarity, customItemData_.scrapLevelFlags);
            }
            else
            {
                logger.LogInfo($"Registering item {customItemData_.name}");
                Items.RegisterItem(itemAsset);
            }
        }


    }
}
