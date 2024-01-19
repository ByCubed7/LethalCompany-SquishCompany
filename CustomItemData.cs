using BepInEx.Configuration;
using LethalLib.Modules;

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

        public bool enabled;

        public System.Type monoBehaviour;


        public bool IsBuyable { get; private set; }
        public bool IsScrap { get; private set; }

        public CustomItemData(string name_)
        {
            name = name_;

            itemPath = $"Assets/Custom/{Plugin.modName}/Items/{name}/{name}.asset".ToLower();
            infoPath = $"Assets/Custom/{Plugin.modName}/Items/{name}/{name}Info.asset".ToLower();

            scrapValue = 0;
            scrapRarity = 0;

            IsScrap = false;
            IsBuyable = false;

            enabled = true;
        }

        // - Method chains

        public CustomItemData MakeScrap(int itemPrice_, int rarity_, Levels.LevelTypes scrapLevelFlags_ = Levels.LevelTypes.All)
        {
            IsScrap = true;
            scrapValue = itemPrice_;
            scrapRarity = rarity_;
            scrapLevelFlags = scrapLevelFlags_;
            return this;
        }

        public CustomItemData MakeBuyable(int itemPrice_)
        {
            IsBuyable = true;
            buyValue = itemPrice_;
            return this;
        }

        public CustomItemData SetEnabled(bool enabled_)
        {
            enabled = enabled_;
            return this;
        }

        public CustomItemData SetMonoBehaviour<T>() where T : UnityEngine.MonoBehaviour
        {
            monoBehaviour = typeof(T);
            return this;
        }

        // 

        public float GetVolume(ConfigFile config)
        {
            return (float) config[ConfigGetVolumeConfigDefinition()].BoxedValue;
        }

        public ConfigDefinition ConfigGetVolumeConfigDefinition()
        {
            return new ConfigDefinition("Volume", $"{name}");
        }

        const float DEFAULT_VOLUME = 100f;
        public ConfigEntry<float> ConfigBindVolume(ConfigFile config)
        {
            ConfigEntry<float> configEntry = config.Bind(
                ConfigGetVolumeConfigDefinition(), DEFAULT_VOLUME,
                new ConfigDescription($"Audio volume for {name} (0 - 100)")
            );
            return configEntry;
        }
    }
}
