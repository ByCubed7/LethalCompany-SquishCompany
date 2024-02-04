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

        /// <summary>
        /// Registers all of the audio components volumes in the audio config.
        /// This MUST be called after all of the item assets have been loaded.
        /// </summary>
        public static void LoadVolumeFrom(ConfigFile config, List<CustomItemData> itemDatas)
        {
            foreach (CustomItemData itemData in itemDatas)
            {
                ConfigEntry<float> configEntry = ConfigBindVolume(config, itemData);
                SetAllAudioSourcesToValue(itemData.Prefab, configEntry.Value);
            }
        }

        public static void SetAllAudioSourcesToValue(GameObject prefab, float value)
        {
            //  Get all audio sources attached
            var audioSources = prefab.GetComponentsInChildren<AudioSource>();
            if (audioSources.Length == 0) return;

            // Adjust volumes, assume the current audioSource is the max we want it
            foreach (AudioSource audioSource in audioSources)
                audioSource.volume *= value / 100;

            //float volume = GetVolume(config, itemData);
        }


        const float DEFAULT_VOLUME = 50f;

        // Generates and binds a volume config entry for the item.
        static public ConfigEntry<float> ConfigBindVolume(ConfigFile config, CustomItemData itemData)
        {
            ConfigEntry<float> configEntry = config.Bind(
                itemData.GetVolumeConfigDefinition(), DEFAULT_VOLUME,
                itemData.GetVolumeConfigDescription()
            );
            return configEntry;
        }

        // 

        //static public float GetVolume(ConfigFile config, CustomItemData itemData)
        //{
        //    return (float)config[itemData.GetVolumeConfigDefinition()].BoxedValue;
        //}
    }
}
