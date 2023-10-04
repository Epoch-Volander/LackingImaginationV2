using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;



namespace LackingImaginationV2

{
    public class EssencePlayerData : MonoBehaviour
    {
        public static Inventory EssenceSlotInventory = new Inventory(nameof(EssenceSlotInventory), null, LackingImaginationV2Plugin.EquipSlotCount, 1);
    
        private static Player _player = Player.m_localPlayer;
        private static bool _isLoading;
        
        public const string Sentinel = "<|>";
        
        public static void OnInventoryChanged()
        {
            // LackingImaginationV2Plugin.Log($"Changed is reached ");
            if (_isLoading)
            {
                // LackingImaginationV2Plugin.Log($"loading i guess ");
                return;
            }
            Save();
        }
        
        public static void Save()
        {
            
            if (_player == null)
            {
                // LackingImaginationV2Plugin.Log($"null player ");
                return;
            }
            // LackingImaginationV2Plugin.LogWarning("save 1");
            var pkg = new ZPackage();
            EssenceSlotInventory.Save(pkg);
            SaveValue(_player, nameof(EssenceSlotInventory), pkg.GetBase64());

        }
        
        public static void Load(Player fromPlayer)
        {
            if (fromPlayer == null)
            {
                LackingImaginationV2Plugin.Log($"null player load ");
                return;
            }
            _player = fromPlayer;

            if (LoadValue(fromPlayer, nameof(EssenceSlotInventory), out var essenceSlotData))
            {
                var pkg = new ZPackage(essenceSlotData);
                _isLoading = true;
                EssenceSlotInventory.Load(pkg);
            
                if (!LackingImaginationV2Plugin.EssenceSlotsEnabled.Value)
                {
                    _player.m_inventory.MoveAll(EssenceSlotInventory);
            
                    pkg = new ZPackage(essenceSlotData);
                    EssenceSlotInventory.Save(pkg);
                    SaveValue(_player, nameof(EssenceSlotInventory), pkg.GetBase64());
                    
                    _isLoading = false;
                }
            }
        }
        
        private static void SaveValue(Player player, string key, string value)
        {
            if (player.m_customData.ContainsKey(key))
            {
                // LackingImaginationV2Plugin.LogWarning("save 2");
                player.m_customData[key] = value;
            }
            else
            {
                // LackingImaginationV2Plugin.LogWarning("save 3");
                player.m_customData.Add(key, value);
            }
               
        }
        
        private static bool LoadValue(Player player, string key, out string value)
        {
            if (player.m_customData.TryGetValue(key, out value))
                return true;
        
            var foundInKnownTexts = player.m_knownTexts.TryGetValue(key, out value);
            if (!foundInKnownTexts)
                key = Sentinel + key;
            foundInKnownTexts = player.m_knownTexts.TryGetValue(key, out value);
            if (foundInKnownTexts)
                LackingImaginationV2Plugin.LogWarning("Loaded data from knownTexts. Will be converted to customData on save.");
        
            return foundInKnownTexts;
        }
    }
    
    
     public static class PlayerExtensions
    {
        public static Inventory GetEssenceSlotInventory(this Player player)
        {
            if (player != null)
            {
                return EssencePlayerData.EssenceSlotInventory;
            }
            return null;
        }
    }
     
    
    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.Clone))]
    public static class ItemData_Clone_Patch
    {
        public static void Postfix(ItemDrop.ItemData __instance, ref ItemDrop.ItemData __result)
        {
            // Fixes bug in vanilla valheim with cloning items with custom data
            __result.m_customData = new Dictionary<string, string>(__instance.m_customData);
        }
    }

    
    [HarmonyPatch(typeof(Player), "Save")]
    public static class Player_Save_Patch
    {
        public static bool Prefix(Player __instance)
        {
            EssencePlayerData.Save();
            return true;
        }
    }

    [HarmonyPatch(typeof(Player), "Load")]
    public static class Player_Load_Patch
    {
        public static void Postfix(Player __instance)
        {
            EssencePlayerData.Load(__instance);
            EssencePlayerData.OnInventoryChanged();
            
            EssenceItemData.equipedEssence = EssenceItemData.GetEquippedEssence();
            EssenceItemData.equipedEssenceData = EssenceItemData.GetEquippedEssenceData();
            LackingImaginationUtilities.NameEssence();
            LackingImaginationUtilities.InitiateEssenceStatus(Hud.m_instance);
            EssencePlayerData.OnInventoryChanged();
        }
    }
}
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
