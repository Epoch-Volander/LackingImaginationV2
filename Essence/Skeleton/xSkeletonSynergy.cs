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
using ItemManager;
using TMPro;
using UnityEngine.UI;


namespace LackingImaginationV2
{
    
    [HarmonyPatch]
    public class xSkeletonSynergy
    {
        public static bool SkeletonSynergyBrennaController = false;
        public static bool SkeletonSynergyRancidController = false;
        private static float equipDelay;

        private static List<string> boundVulkanList = new List<string>()
        {
            LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name,
            LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name
        };
        private static List<string> boundRancorousList = new List<string>()
        {
            LackingImaginationV2Plugin.GO_RancorousMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_name,
            LackingImaginationV2Plugin.GO_RancorousMaceBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name
        };
        
        
        private static bool sword;
        private static bool mace;
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Skele_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_brenna_essence")) sword = true;
                if (!EssenceItemData.equipedEssence.Contains("$item_brenna_essence") && sword == true)
                {
                    foreach (string bound in boundVulkanList)
                    {
                        if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                        {
                            Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                        }
                    }
                    sword = false;
                }

                if (EssenceItemData.equipedEssence.Contains("$item_skeletonpoison_essence")) mace = true;
                if (!EssenceItemData.equipedEssence.Contains("$item_skeletonpoison_essence") && mace == true)
                {
                    foreach (string bound in boundRancorousList)
                    {
                        if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                        {
                            Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                        }
                    }
                    mace = false;
                }
            }
        }
        
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveInventoryToGrave))]
        public class Skele_Remove_MoveInventoryToGrave_Patch
        {
            public static bool Prefix(Inventory __instance)
            {
                foreach (string bound in boundVulkanList)
                {
                    if (__instance.ContainsItemByName(bound))
                    {
                        __instance.RemoveItem(bound, 1);
                    }
                }
                foreach (string bound in boundRancorousList)
                {
                    if (__instance.ContainsItemByName(bound))
                    {
                        __instance.RemoveItem(bound, 1);
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Menu), nameof(Menu.OnLogout), null)]
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
                foreach (string bound in boundRancorousList)
                {
                    if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                    {
                        Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                    }
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Menu),  nameof(Menu.OnQuit), null)]
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
                foreach (string bound in boundRancorousList)
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

                if (boundRancorousList.Contains(item.m_shared.m_name))
                {
                    if(__instance.IsItemEquiped(item)) __instance.UnequipItem(item);
                    inventory.RemoveItem(item);
                    if(xRancidRemainsEssence.Aura != null) UnityEngine.GameObject.Destroy(xRancidRemainsEssence.Aura);
                    xRancidRemainsEssence.Throwable = false;
                    InventoryGui.instance.SetupDragItem((ItemDrop.ItemData) null, (Inventory) null, 1);
                    inventory.Changed();
                    return false;
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

                if (__instance != fromInventory && boundRancorousList.Contains(item.m_shared.m_name))
                {
                    if(Player.m_localPlayer.IsItemEquiped(item)) Player.m_localPlayer.UnequipItem(item);
                    fromInventory.RemoveItem(item);
                    InventoryGui.instance.SetupDragItem((ItemDrop.ItemData) null, (Inventory) null, 1);
                    // __instacne.RemoveItem(item);
                    if(xRancidRemainsEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xRancidRemainsEssence.Throwable = false;
                    
                    __instance.Changed();
                    fromInventory.Changed();
                    return false;
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

                if (__instance != fromInventory && boundRancorousList.Contains(item.m_shared.m_name))
                {
                    __instance.RemoveItem(__instance.GetItemAt(x, y));

                    if(xRancidRemainsEssence.Aura != null) UnityEngine.GameObject.Destroy(xRancidRemainsEssence.Aura);
                    xRancidRemainsEssence.Throwable = false;
                    
                    __instance.Changed();
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

                if (!boundVulkanList.Contains(currentWeapon.m_shared.m_name) && !boundRancorousList.Contains(currentWeapon.m_shared.m_name))
                {
                    return true;
                }
                
                xBrennaEssence.Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (currentWeapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name )
                {
                    if(!xBrennaEssence.Throwable) return true;
                }

                xRancidRemainsEssence.Awakened = Boolean.Parse(xRancidRemainsEssencePassive.RancidRemainsStats[0]);
                GameObject PoisonMace = xRancidRemainsEssence.Awakened ? LackingImaginationV2Plugin.GO_RancorousMace : LackingImaginationV2Plugin.GO_RancorousMaceBroken;
                if (currentWeapon.m_shared.m_name == PoisonMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_name )
                {
                    if(!xRancidRemainsEssence.Throwable) return true;
                }

                string weapon = "";

                if (boundVulkanList.Contains(currentWeapon.m_shared.m_name)) weapon = "fire";
                if (boundRancorousList.Contains(currentWeapon.m_shared.m_name)) weapon = "poison";
                
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
                    xRancidRemainsEssence.Throwable = false;
                    ScheduleDelete(xRancidRemainsEssence.Aura);
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
            
                if(aura != null) UnityEngine.GameObject.Destroy(aura);
            }    
        }
        
        [HarmonyPatch(typeof(Attack), nameof(Attack.ProjectileAttackTriggered))]
        public static class Skele_Throwable_ProjectileAttackTriggered_Patch
        {
            public static void Prefix(Attack __instance, ref EffectList __state)
            {
                if(boundVulkanList.Contains(__instance.m_weapon.m_shared.m_name) || boundRancorousList.Contains(__instance.m_weapon.m_shared.m_name))
                {
                    __state = __instance.m_weapon.m_shared.m_triggerEffect;
                    __instance.m_weapon.m_shared.m_triggerEffect = new EffectList();
                }
            }
		      
            public static void Postfix(Attack __instance, EffectList __state)
            {
                if(boundVulkanList.Contains(__instance.m_weapon.m_shared.m_name) || boundRancorousList.Contains(__instance.m_weapon.m_shared.m_name))
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
                if (__instance.m_weapon.m_lastProjectile != null && (boundVulkanList.Contains(__instance.m_weapon.m_shared.m_name) || boundRancorousList.Contains(__instance.m_weapon.m_shared.m_name)))
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
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Skele_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_skeleton_essence") && hit.GetAttacker() != null)
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        Game.instance.IncrementPlayerStat(__instance.IsPlayer() ? PlayerStatType.PlayerHits : PlayerStatType.EnemyHits);
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if ((UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null && attacker.m_name == "Ghost(Ally)")
                    {
                        if (EssenceItemData.equipedEssence.Contains("$item_brenna_essence")) hit.m_damage.m_fire += LackingImaginationGlobal.c_skeletonSynergyBrenna;
                        if (EssenceItemData.equipedEssence.Contains("$item_skeletonpoison_essence")) hit.m_damage.m_poison += LackingImaginationGlobal.c_skeletonSynergyRancid;
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UseItem))]
        public static class Skele_UseItem_Patch
        {
            public static bool Prefix(Humanoid __instance, ref Inventory inventory, ref ItemDrop.ItemData item, ref bool fromInventoryGui)
            {
                if (inventory == null)
                    inventory = __instance.m_inventory;
                if (!inventory.ContainsItem(item))
                    return true;
                if (EssenceItemData.equipedEssence.Contains("$item_brenna_essence") && 
                    (xBrennaEssencePassive.BrennaStats[0] == "false" && item.m_shared.m_name == "$item_sword_krom" && item.m_shared.m_maxQuality == item.m_quality))
                {
                    if(!Player.m_localPlayer.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_brenna_essence"))))
                    {
                        __instance.EquipItem(item);
                        LackingImaginationV2Plugin.UseGuardianPower = false;
                        SkeletonSynergyBrennaController = true;
                        ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance)).SetTrigger("gpower");
                        SkeletonSynergyBrennaController = false;

                        inventory.RemoveItem(item);
                        xBrennaEssencePassive.BrennaStats[0] = "true";
                        foreach (string bound in boundVulkanList)
                        {
                            if (inventory.ContainsItemByName(bound))
                            {
                                inventory.RemoveItem(bound, 1);
                            }
                        }
                        ScheduleDelay(__instance, 0.5f, true);
                        return false;
                    }
                }
                if (EssenceItemData.equipedEssence.Contains("$item_skeletonpoison_essence") && 
                    (xRancidRemainsEssencePassive.RancidRemainsStats[0] == "false" && item.m_shared.m_name == "$item_mace_iron" && item.m_shared.m_maxQuality == item.m_quality))
                {
                    if(!Player.m_localPlayer.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_skeletonpoison_essence"))))
                    {
                        __instance.EquipItem(item);
                        LackingImaginationV2Plugin.UseGuardianPower = false;
                        SkeletonSynergyRancidController = true;
                        ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance)).SetTrigger("gpower");
                        SkeletonSynergyRancidController = false;

                        inventory.RemoveItem(item);
                        xRancidRemainsEssencePassive.RancidRemainsStats[0] = "true";
                        foreach (string bound in boundRancorousList)
                        {
                            if (inventory.ContainsItemByName(bound))
                            {
                                inventory.RemoveItem(bound, 1);
                            }
                        }
                        ScheduleDelay(__instance, 1.5f, false);
                        return false;
                    }
                }
                return true;
            }
        }
        public static void ScheduleDelay(Humanoid human, float equipDelay, bool swordmace)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleDelayCoroutine(human, equipDelay, swordmace));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleDelayCoroutine(Humanoid human, float equipDelay, bool swordmace)
        {
            if(swordmace)
            {
                yield return new WaitForSeconds(equipDelay);
                xBrennaEssence.Process_Input(Player.m_localPlayer, EssenceItemData.equipedEssence.IndexOf("$item_brenna_essence"));
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_fireskeleton_nova"), human.transform.position + human.transform.up * 0.6f, Quaternion.identity);
            }
            else
            {
                yield return new WaitForSeconds(equipDelay);
                xRancidRemainsEssence.Process_Input(Player.m_localPlayer, EssenceItemData.equipedEssence.IndexOf("$item_skeletonpoison_essence"));
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_skeleton_mace_hit"), human.transform.position, Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_skeleton_mace_hit"), human.transform.position, Quaternion.identity);
            }
        }
        public static void ScheduleEquip(Player player, ref ItemDrop.ItemData item, float equipDelay)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleEquipCoroutine(player, item, equipDelay));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleEquipCoroutine(Player player, ItemDrop.ItemData item, float equipDelay)
        {
            yield return new WaitForSeconds(equipDelay);
            if(player.GetCurrentWeapon() != item)player.EquipItem(item);
        }
        public static void ScheduleDelete(Player player, ref ItemDrop.ItemData item, float deleteDelay)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleDeleteCoroutine(player, item, deleteDelay));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleDeleteCoroutine(Player player, ItemDrop.ItemData item, float deleteDelay)
        {
            yield return new WaitForSeconds(deleteDelay);
            if (player.m_inventory.ContainsItem(item))
            {
                if(player.IsItemEquiped(item)) player.UnequipItem(item);
                player.m_inventory.RemoveItem(item);
            }
        }
    }
}