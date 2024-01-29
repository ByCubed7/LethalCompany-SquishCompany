using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace SquishCompany
{
    public class SquishConfig
    {
        // SquishMellow
        public static ConfigEntry<int> squishMellowPrice;
        public static ConfigEntry<int> squishMellowRarity;
        public static ConfigEntry<bool> squishMellowEnabled;

        public static void LoadGeneralFrom(ref ConfigFile config)
        {
            squishMellowPrice   = config.Bind<int>("Scrap", "SquishMellowPrice", 15, "How much does SquishMellow cost?");
            squishMellowRarity  = config.Bind<int>("Scrap", "SquishMellowRarity", 40, "How much does SquishMellow spawn, higher = more common");
            squishMellowEnabled = config.Bind<bool>("Scrap", "SquishMellowEnabled", true, "Is SquishMellow enabled?");
        }

        // Registers all of the audio components volumes in the audio config
        public static void LoadVolumeFrom(ref ConfigFile config, Dictionary<string, GameObject> prefabs, Dictionary<string, CustomItemData> itemDatas)
        {
            // Loop through prefabs
            foreach (var prefabSet in prefabs)
            {
                var prefabName = prefabSet.Key;
                var prefab = prefabSet.Value;

                //  Get all audio sources attached
                var audioSources = prefab.GetComponentsInChildren<AudioSource>();
                if (audioSources.Length == 0) continue;

                // Bind config value
                //ConfigEntry<float> configValue = config.Bind("Volume", $"{prefabName}", 100f, $"Audio volume for {prefabName} (0 - 100)");

                CustomItemData itemData = itemDatas[prefabName];
                ConfigEntry<float> configValue = itemData.ConfigBindVolume(config);
                //float volume = itemData.GetVolume(config);

                // Adjust volumes for each
                foreach (AudioSource audioSource in audioSources)
                    audioSource.volume *= configValue.Value / 100;
            }
        }
    }
}
