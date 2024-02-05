using BepInEx.Configuration;
using LethalLib.Modules;
using SquishCompany.Extensions;
using System;
using System.Collections.Generic;

namespace SquishCompany
{
    public class CustomItemData
    {
        public string name;
        public string itemPath;
        public string infoPath;

        // Scrap properties
        public int scrapValue;
        public int scrapRarity;
        public Levels.LevelTypes scrapLevelFlags;

        // Shop properties
        public int buyValue;


        public Type monoBehaviour;


        public bool Enabled { get; private set; }
        public bool IsBuyable { get; private set; }
        public bool IsScrap { get; private set; }


        public Item ItemAsset { get; private set; }
        public UnityEngine.GameObject Prefab => ItemAsset.spawnPrefab;


        public CustomItemData(string name_)
        {
            name = name_;

            // BUG: Sometimes path can be .prefab or .asset
            // TODO: We can find the prefab/asset based off of the path and name anyway
            //      So it does not need to be hard coded
            itemPath = System.IO.Path.Combine("Assets", "Custom", Plugin.modName, "Items", name, $"{name}.asset");
            //infoPath = $"Assets/Custom/{Plugin.modName}/Items/{name}/{name}Info.asset".ToLower();

            scrapValue = 0;
            scrapRarity = 0;

            IsScrap = false;
            IsBuyable = false;

            Enabled = true;
        }

        // - Method chains

        private void SetEnabled(bool enabled_)
        {
            Enabled = enabled_;
        }

        private void MakeScrap(int itemPrice_, int rarity_, Levels.LevelTypes scrapLevelFlags_ = Levels.LevelTypes.All)
        {
            IsScrap = true;
            scrapValue = itemPrice_;
            scrapRarity = rarity_;
            scrapLevelFlags = scrapLevelFlags_;
        }

        private void MakeBuyable(int itemPrice_)
        {
            IsBuyable = true;
            buyValue = itemPrice_;
        }

        public CustomItemData SetMonoBehaviour<T>() where T : UnityEngine.MonoBehaviour
        {
            monoBehaviour = typeof(T);
            return this;
        }



        /// <summary>
        /// Binds the config to the config file and grabs the value in the config is it has been set.
        /// If no value has been set, returns the default value (The initial set value).
        /// </summary>
        public CustomItemData Bind<T>(ConfigFile config, string key, string description, ref T configValue)
        {
            configValue = (T) config.Bind(
                new ConfigDefinition("Scrap", $"{name}{key}"),
                configValue,
                new ConfigDescription(description)
            ).BoxedValue;
            return this;
        }

        //public CustomItemData BindEnabled(ConfigFile config, bool enabled)
        //{
        //    // Bind and get value from config
        //    enabled = (bool)config.Bind("Scrap", $"{name}Enabled", enabled, $"Is {name} Enabled?").BoxedValue;
        //    SetEnabled(enabled);
        //    return this;
        //}

        public CustomItemData BindEnabled(ConfigFile config, bool enabled)
        {
            // Bind and get value from config
            Bind(config, "Enabled", $"Is {name} Enabled?", ref enabled);
            SetEnabled(enabled);
            return this;
        }

        public CustomItemData BindScrap(ConfigFile config, int price, int rarity)
        {
            // Bind and get value from config
            //price = (int)config.Bind("Scrap", $"{name}Price", price, $"How much is {name} valued?").BoxedValue;
            //rarity = (int)config.Bind("Scrap", $"{name}Rarity", rarity, $"How much does {name} spawn, higher = more common?").BoxedValue;
            Bind(config, "Price", $"How much is {name} valued?", ref price);
            Bind(config, "Rarity", $"How much does {name} spawn, higher = more common?", ref rarity);
            MakeScrap(price, rarity, Levels.LevelTypes.All);
            return this;
        }

        public CustomItemData BindBuyable(ConfigFile config, int price)
        {
            // Bind and get value from config
            Bind(config, "Price", $"How much is {name} valued?", ref price);
            MakeBuyable(price);
            return this;
        }




        public ConfigDefinition GetVolumeConfigDefinition()
        {
            return new ConfigDefinition("Volume", $"{name}");
        }

        public ConfigDescription GetVolumeConfigDescription()
        {
            return new ConfigDescription($"Audio volume for {name} (0 - 100)");
        }

        public void LoadAssetFrom(UnityEngine.AssetBundle mainAssets)
        {
            Plugin.logger.LogInfo($"Attempting to load {name} at {itemPath}");
            
            if (mainAssets is null)
                Plugin.logger.LogError($"Main Assets is null!");

            if (!mainAssets.Contains(itemPath))
                Plugin.logger.LogError($"Main Assets does not contain item path {itemPath}!");

            ItemAsset = mainAssets.LoadAsset<Item>(itemPath);

            // Check asset loaded correctly
            if (ItemAsset is null)
            {
                Plugin.logger.LogError($"Failed to load item {itemPath} from asset!!");
                Plugin.DEBUG_ASSETBUNDLE();
            }

            // Prefab => ItemAsset.spawnPrefab
            if (Prefab is null)
                Plugin.logger.LogError($"Items spawnPrefab is null!");

            Prefab.EnsureNetworkTransform();
            Prefab.OverridePhysicsPropWith(monoBehaviour);

            LethalLib.Modules.Utilities.FixMixerGroups(Prefab);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(Prefab);
        }

        public void Register()
        {
            if (ItemAsset is null)
            {
                Plugin.logger.LogError("Registering failed! Item Asset is null");
                return;
            }
            // NOTE: Can items not be both buyable and scrap?
            if (IsBuyable)
            {
                //Plugin.logger.LogInfo($"Registering shop item {customItemData_.name} with price {customItemData_.buyValue}");
                //Items.RegisterShopItem(ItemAsset, null, null, MainAssets.LoadAsset<TerminalNode>(customItemData_.infoPath), customItemData_.buyValue);
            }
            else if (IsScrap)
            {
                Plugin.logger.LogInfo($"Registering scrap item {name}");
                Items.RegisterScrap(ItemAsset, scrapRarity, scrapLevelFlags);
            }
            else
            {
                Plugin.logger.LogInfo($"Registering item {name}");
                Items.RegisterItem(ItemAsset);
            }
        }
    }
}
