using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using LocalizationManager;
using LocationManager;
using PieceManager;
using ServerSync;
using SkillManager;
using StatusEffectManager;
using UnityEngine;
using PrefabManager = ItemManager.PrefabManager;
using Range = LocationManager.Range;
using System.Collections.Generic;
using System.CodeDom;
using System.Linq;
using UnityEngine.Rendering;
using Skill = SkillManager.Skill;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using ItemDataManager;



namespace LackingImaginationV2
{ 
    public class EssenceItemData : ItemData
    {

        public static List<string> equipedEssence = new();
        public static List<ItemDrop.ItemData> equipedEssenceData = new();
       
        
        public static List<ItemDrop.ItemData> GetEquippedEssenceData()
        {
            List<ItemDrop.ItemData> essencehand = new();
            for (var i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; ++i)
            {
                essencehand.Add(Player.m_localPlayer.GetEssenceSlotInventory().GetItemAt(i,0));
            }
            return essencehand;
        }
        
        public static List<string> GetEquippedEssence()
        {
            List<string> essencehand = new();
            for (var i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; ++i)
            {
                // LackingImaginationV2Plugin.Log($"{i}item={Player.m_localPlayer.GetEssenceSlotInventory().GetItemAt(i,0)?.m_shared.m_name}");
                essencehand.Add(Player.m_localPlayer.GetEssenceSlotInventory().GetItemAt(i,0)?.m_shared.m_name);
            }
            return essencehand;
        }
        
        
    }
    
}
    





