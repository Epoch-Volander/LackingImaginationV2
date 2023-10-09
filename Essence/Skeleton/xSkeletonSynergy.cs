using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Threading;
using TMPro;
using UnityEngine.UI;


namespace LackingImaginationV2
{
    
    [HarmonyPatch]
    public class xSkeletonSynergy
    {
        
        private static List<string> boundVulkanList = new List<string>()
        {
            LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name,
            LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name
        };
        private static List<string> boundList = new List<string>()
        {
            
        };
        
        [HarmonyPatch(typeof(Menu), "OnLogout", null)]
        public class Skele_Remove_OnLogout_Patch
        {
            public static bool Prefix()
            {
                foreach (string bound in boundVulkanList)
                {
                    if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                    {
                        Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                    }
                }
                foreach (string bound in boundList)
                {
                    if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                    {
                        Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                    }
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Menu), "OnQuit", null)]
        public class Skele_Remove_QuitYes_Patch
        {
            public static bool Prefix()
            {
                foreach (string bound in boundVulkanList)
                {
                    if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                    {
                        Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                    }
                }
                foreach (string bound in boundList)
                {
                    if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                    {
                        Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                    }
                }
                return true;
            }
        }
        
         [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.DropItem))]
        public static class Skele_DropItem_Patch
        {
            static bool Prefix(Player __instance, Inventory inventory, ItemDrop.ItemData item, int amount)
            {
                if (amount == 0 || item.m_shared.m_questItem)
                    return true; // Continue with the original method if amount is 0 or it's a quest item.
                
                if (boundVulkanList.Contains(item.m_shared.m_name))
                {
                    if(__instance.IsItemEquiped(item)) __instance.UnequipItem(item);
                    inventory.RemoveItem(item);
                    if(xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xBrennaEssence.Throwable = false;
                    InventoryGui.instance.SetupDragItem((ItemDrop.ItemData) null, (Inventory) null, 1);
                    inventory.Changed();
                    // Item name matches the condition, so we won't drop it.
                    return false;
                }

                if (boundList.Contains(item.m_shared.m_name))
                {

                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveItemToThis), new Type[] { typeof(Inventory), typeof(ItemDrop.ItemData) })]
        public static class Skele_MoveItemToThis1_Patch
        {
            static bool Prefix(Inventory __instance, ref Inventory fromInventory, ref ItemDrop.ItemData item)
            {
                if (__instance != fromInventory && boundVulkanList.Contains(item.m_shared.m_name))
                {
                    if(Player.m_localPlayer.IsItemEquiped(item)) Player.m_localPlayer.UnequipItem(item);
                    fromInventory.RemoveItem(item);
                    InventoryGui.instance.SetupDragItem((ItemDrop.ItemData) null, (Inventory) null, 1);
                    // __instacne.RemoveItem(item);
                    if(xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xBrennaEssence.Throwable = false;
                    
                    __instance.Changed();
                    fromInventory.Changed();
                    return false;
                }

                if (__instance != fromInventory && boundList.Contains(item.m_shared.m_name))
                {
                    
                    
                    
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveItemToThis), new Type[] { typeof(Inventory), typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int) })]
        public static class Skele_MoveItemToThis2_Patch
        {
            static void Postfix(Inventory __instance, ref Inventory fromInventory, ref ItemDrop.ItemData item, ref int amount,  ref int x, ref int y)
            {
                if (__instance != fromInventory && boundVulkanList.Contains(item.m_shared.m_name))
                {
                    __instance.RemoveItem(__instance.GetItemAt(x, y));

                    if(xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xBrennaEssence.Throwable = false;
                    
                    __instance.Changed();
                }

                if (__instance != fromInventory && boundList.Contains(item.m_shared.m_name))
                {

                }

            }
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
        public static class Skele_Throwable_StartAttack_Patch
        {
            public static bool Prefix(Humanoid __instance, ref bool __result, bool secondaryAttack)
            {
                if (!secondaryAttack) return true;
            
                __instance.ClearActionQueue();
                if (__instance.InAttack() && !__instance.HaveQueuedChain() || __instance.InDodge() || !__instance.CanMove() || __instance.IsKnockedBack() || __instance.IsStaggering() || __instance.InMinorAction())
                {
                    return true;
                }
            
                var currentWeapon = __instance.GetCurrentWeapon();
                if (currentWeapon == null || currentWeapon.m_dropPrefab == null)
                {
                    return true;
                }

                if (!boundVulkanList.Contains(currentWeapon.m_shared.m_name) && !boundList.Contains(currentWeapon.m_shared.m_name))
                {
                    return true;
                }
                
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (currentWeapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name )
                {
                    if(!xBrennaEssence.Throwable) return true;
                }

                GameObject PoisonMace;

                string weapon = "";

                if (boundVulkanList.Contains(currentWeapon.m_shared.m_name)) weapon = "fire";
                if (boundList.Contains(currentWeapon.m_shared.m_name)) weapon = "poison";
                
                var spearPrefab = ObjectDB.instance?.GetItemPrefab("SpearWolfFang");
                if (spearPrefab == null)
                {
                    return true;
                }
               
                if (__instance.m_currentAttack != null)
                {
                    __instance.m_currentAttack.Stop();
                    __instance.m_previousAttack = __instance.m_currentAttack;
                    __instance.m_currentAttack = null;
                }

                var attack = spearPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_secondaryAttack.Clone();
                if (!attack.Start(__instance, __instance.m_body, __instance.m_zanim, __instance.m_animEvent, __instance.m_visEquipment, currentWeapon, __instance.m_previousAttack, __instance.m_timeSinceLastAttack, __instance.GetAttackDrawPercentage()))
                {
                    return false;
                }
            
                __instance.m_currentAttack = attack;
                __instance.m_lastCombatTimer = 0.0f;
                __result = true;

                if (weapon == "fire")
                {
                    xBrennaEssence.Throwable = false;
                    ScheduleDelete(xBrennaEssence.Aura);
                }
                else if (weapon == "poison")
                {
                    
                }
                
                
                return false;
            }
            private static void ScheduleDelete(GameObject aura)
            {
                CoroutineRunner.Instance.StartCoroutine(ScheduleDeleteCoroutine(aura));
            }
            // ReSharper disable Unity.PerformanceAnalysis
            private static IEnumerator ScheduleDeleteCoroutine(GameObject aura)
            {
                yield return new WaitForSeconds(2f);
            
                UnityEngine.GameObject.Destroy(aura);
            }    
        }
        
        [HarmonyPatch(typeof(Attack), nameof(Attack.ProjectileAttackTriggered))]
        public static class Skele_Throwable_ProjectileAttackTriggered_Patch
        {
            public static void Prefix(Attack __instance, ref EffectList __state)
            {
                if(boundVulkanList.Contains(__instance.m_weapon.m_shared.m_name) || boundList.Contains(__instance.m_weapon.m_shared.m_name))
                {
                    __state = __instance.m_weapon.m_shared.m_triggerEffect;
                    __instance.m_weapon.m_shared.m_triggerEffect = new EffectList();
                }
            }
		      
            public static void Postfix(Attack __instance, EffectList __state)
            {
                if(boundVulkanList.Contains(__instance.m_weapon.m_shared.m_name) || boundList.Contains(__instance.m_weapon.m_shared.m_name))
                {
                    if (__instance.m_weapon.m_lastProjectile.GetComponent<Projectile>() is Projectile projectile)
                    {
                        projectile.m_spawnOnHitEffects = new EffectList { m_effectPrefabs = projectile.m_spawnOnHitEffects.m_effectPrefabs.Concat(__state.m_effectPrefabs).ToArray() };
                        projectile.m_aoe = __instance.m_weapon.m_shared.m_attack.m_attackRayWidth;
                    }
                    __instance.m_weapon.m_shared.m_triggerEffect = __state;
                }
            }
        }        
        
        [HarmonyPatch(typeof(Attack), nameof(Attack.FireProjectileBurst))]
        public static class Skele_Throwable_FireProjectileBurst_Patch
        {
            public static void Postfix(Attack __instance)
            {
                if (__instance.m_weapon.m_lastProjectile != null && (boundVulkanList.Contains(__instance.m_weapon.m_shared.m_name) || boundList.Contains(__instance.m_weapon.m_shared.m_name)))
                {
                    var existingMesh = __instance.m_weapon.m_lastProjectile.transform.Find("fangspear");
                    if (existingMesh != null)
                    {
                        ZNetScene.instance.Destroy(existingMesh.gameObject);
                    }
                    var weaponMesh = __instance.m_weapon.m_dropPrefab.transform.Find("attach");
                    if (weaponMesh == null)
                    {
                        return;
                    }
                    var newMesh = UnityEngine.Object.Instantiate(weaponMesh.gameObject, __instance.m_weapon.m_lastProjectile.transform, false);
                    newMesh.AddComponent<ExpMethods.Flip>();
                }
            }
        }
        
        
        
        
        
        
        
        
        
        

    }
}