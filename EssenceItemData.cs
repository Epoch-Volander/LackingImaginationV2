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
        
        
        
        //  //////////////////////////////////////
        // public static void SetEquipedEssence(List<string> equipped)
        // {
        //     for (var i = 0; i < equipped.Count; ++i)
        //     {
        //         SetItemAt(i,0,equipped[i],Player.m_localPlayer.GetEssenceSlotInventory());
        //     }
        // }
        //
        //
        //
        // public static void SetItemAt(int x, int y, string itemName, Inventory inv)
        // {
        //     foreach (ItemDrop.ItemData itemAt in Player.m_localPlayer.GetEssenceSlotInventory().m_inventory)
        //     {
        //         if (itemAt.m_gridPos.x == x && itemAt.m_gridPos.y == y) ;
        //
        //     }
        //     
        // }
        // //////////////////////////////////
        
    }
    
    
    
    // public ItemDrop.ItemData GetItemAt(int x, int y)
    // {
    //     foreach (ItemDrop.ItemData itemAt in this.m_inventory)
    //     {
    //         if (itemAt.m_gridPos.x == x && itemAt.m_gridPos.y == y)
    //             return itemAt;
    //     }
    //     return (ItemDrop.ItemData) null;
    // }
    
    
    
    
    
    
  //    public bool EquipItem(ItemDrop.ItemData item, bool triggerEquipEffects = true)
  // {
  //   if (this.IsItemEquiped(item) || !this.m_inventory.ContainsItem(item) || this.InAttack() || this.InDodge() || this.IsPlayer() && !this.IsDead() && this.IsSwimming() && !this.IsOnGround() || item.m_shared.m_useDurability && (double) item.m_durability <= 0.0)
  //     return false;
  //   if (item.m_shared.m_dlc.Length > 0 && !DLCMan.instance.IsDLCInstalled(item.m_shared.m_dlc))
  //   {
  //     this.Message(MessageHud.MessageType.Center, "$msg_dlcrequired");
  //     return false;
  //   }
  //   if (Application.isEditor)
  //     item.m_shared = item.m_dropPrefab.GetComponent<ItemDrop>().m_itemData.m_shared;
  //   if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Tool)
  //   {
  //     this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //     this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //     this.m_rightItem = item;
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch)
  //   {
  //     if (this.m_rightItem != null && this.m_leftItem == null && this.m_rightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon)
  //     {
  //       this.m_leftItem = item;
  //     }
  //     else
  //     {
  //       this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //       if (this.m_leftItem != null && this.m_leftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Shield)
  //         this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //       this.m_rightItem = item;
  //     }
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon)
  //   {
  //     if (this.m_rightItem != null && this.m_rightItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch && this.m_leftItem == null)
  //     {
  //       ItemDrop.ItemData rightItem = this.m_rightItem;
  //       this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //       this.m_leftItem = rightItem;
  //       this.m_leftItem.m_equipped = true;
  //     }
  //     this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //     if (this.m_leftItem != null && this.m_leftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Shield && this.m_leftItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Torch)
  //       this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //     this.m_rightItem = item;
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield)
  //   {
  //     this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //     if (this.m_rightItem != null && this.m_rightItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.OneHandedWeapon && this.m_rightItem.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Torch)
  //       this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //     this.m_leftItem = item;
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
  //   {
  //     this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //     this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //     this.m_leftItem = item;
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon)
  //   {
  //     this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //     this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //     this.m_rightItem = item;
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft)
  //   {
  //     this.UnequipItem(this.m_leftItem, triggerEquipEffects);
  //     this.UnequipItem(this.m_rightItem, triggerEquipEffects);
  //     this.m_leftItem = item;
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Chest)
  //   {
  //     this.UnequipItem(this.m_chestItem, triggerEquipEffects);
  //     this.m_chestItem = item;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Legs)
  //   {
  //     this.UnequipItem(this.m_legItem, triggerEquipEffects);
  //     this.m_legItem = item;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Ammo || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.AmmoNonEquipable)
  //   {
  //     this.UnequipItem(this.m_ammoItem, triggerEquipEffects);
  //     this.m_ammoItem = item;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Helmet)
  //   {
  //     this.UnequipItem(this.m_helmetItem, triggerEquipEffects);
  //     this.m_helmetItem = item;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shoulder)
  //   {
  //     this.UnequipItem(this.m_shoulderItem, triggerEquipEffects);
  //     this.m_shoulderItem = item;
  //   }
  //   else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Utility)
  //   {
  //     this.UnequipItem(this.m_utilityItem, triggerEquipEffects);
  //     this.m_utilityItem = item;
  //   }
  //   if (this.IsItemEquiped(item))
  //     item.m_equipped = true;
  //   this.SetupEquipment();
  //   if (triggerEquipEffects)
  //     this.TriggerEquipEffect(item);
  //   return true;
  // }
  //
  // public void UnequipItem(ItemDrop.ItemData item, bool triggerEquipEffects = true)
  // {
  //   if (item == null)
  //     return;
  //   if (this.m_hiddenLeftItem == item)
  //   {
  //     this.m_hiddenLeftItem = (ItemDrop.ItemData) null;
  //     this.SetupVisEquipment(this.m_visEquipment, false);
  //   }
  //   if (this.m_hiddenRightItem == item)
  //   {
  //     this.m_hiddenRightItem = (ItemDrop.ItemData) null;
  //     this.SetupVisEquipment(this.m_visEquipment, false);
  //   }
  //   if (!this.IsItemEquiped(item))
  //     return;
  //   if (item.IsWeapon())
  //   {
  //     if (this.m_currentAttack != null && this.m_currentAttack.GetWeapon() == item)
  //     {
  //       this.m_currentAttack.Stop();
  //       this.m_previousAttack = this.m_currentAttack;
  //       this.m_currentAttack = (Attack) null;
  //     }
  //     if (!string.IsNullOrEmpty(item.m_shared.m_attack.m_drawAnimationState))
  //       this.m_zanim.SetBool(item.m_shared.m_attack.m_drawAnimationState, false);
  //     this.m_attackDrawTime = 0.0f;
  //     this.ResetLoadedWeapon();
  //   }
  //   if (this.m_rightItem == item)
  //     this.m_rightItem = (ItemDrop.ItemData) null;
  //   else if (this.m_leftItem == item)
  //     this.m_leftItem = (ItemDrop.ItemData) null;
  //   else if (this.m_chestItem == item)
  //     this.m_chestItem = (ItemDrop.ItemData) null;
  //   else if (this.m_legItem == item)
  //     this.m_legItem = (ItemDrop.ItemData) null;
  //   else if (this.m_ammoItem == item)
  //     this.m_ammoItem = (ItemDrop.ItemData) null;
  //   else if (this.m_helmetItem == item)
  //     this.m_helmetItem = (ItemDrop.ItemData) null;
  //   else if (this.m_shoulderItem == item)
  //     this.m_shoulderItem = (ItemDrop.ItemData) null;
  //   else if (this.m_utilityItem == item)
  //     this.m_utilityItem = (ItemDrop.ItemData) null;
  //   item.m_equipped = false;
  //   this.SetupEquipment();
  //   if (!triggerEquipEffects)
  //     return;
  //   this.TriggerEquipEffect(item);
  // }
  //
  // public void TriggerEquipEffect(ItemDrop.ItemData item)
  // {
  //   if (this.m_nview.GetZDO() == null || MonoUpdaters.UpdateCount == this.m_lastEquipEffectFrame)
  //     return;
  //   this.m_lastEquipEffectFrame = MonoUpdaters.UpdateCount;
  //   this.m_equipEffects.Create(this.transform.position, Quaternion.identity);
  // }
    
    
    
    
    
    
    
    
    
    
    
}
    





