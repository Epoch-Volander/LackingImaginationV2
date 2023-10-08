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


namespace LackingImaginationV2
{

    public class xBrennaEssence
    {
        public static string Ability_Name = "Vulcan";

        public static bool Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
        private static float equipDelay = 0.5f;
        private static float deleteDelay = 60f;
        public static bool Throwable;

        // public static  GameObject FireSword = Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                 GameObject FireSword = Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                
                LackingImaginationV2Plugin.Log($"Brenna Button was pressed");
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);

                if (!player.m_inventory.ContainsItemByName(FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name))
                {
                    if (!player.m_inventory.CanAddItem(FireSword))
                    {
                        player.Message(MessageHud.MessageType.TopLeft, $"Inventory Full");
                        return;
                    }
                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                    
                    player.m_inventory.AddItem(FireSword, 1);
                    ItemDrop.ItemData sword = player.m_inventory.GetItem(FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
                    ScheduleEquip(player, ref sword);
                    ScheduleDelete(player, ref sword);
                }
                else
                {
                    if (!player.IsItemEquiped(player.m_inventory.GetItem(FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)))
                    {
                        player.Message(MessageHud.MessageType.TopLeft, $"{FireSword.name}Not Equipped");
                        return;
                    }

                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime * 2f;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    Throwable = true;
                    //add aura, maybe brenna head flames
                    
                    
                }
                
                //Effects, animations, and sounds
                // LackingImaginationV2Plugin.Log($"Brenna {player.transform.position}");
                
             
              
                   
                // brenna swords fall from the sky and do the Aoe, on kill spawn a brenna 

                //summon brenna sword, last for a set duration, you can use it like a sword or throw it with recast, spawns the aoe around hit spot and in random spots nearby and summons a brenna ally at the location, can only have one

                //destroy if dropped, destroy if essence removed, destroy on death// destroy on logout
                
                // broken base form, sacrifice fully upgraded krom, to unlock true version, just a stats list to say yes or no to the version of the sword summoned
                
                // synergy, add element ot the vigil spirts
                
                


            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        private static void ScheduleEquip(Player player, ref ItemDrop.ItemData item)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleEquipCoroutine(player, item));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleEquipCoroutine(Player player, ItemDrop.ItemData item)
        {
            yield return new WaitForSeconds(equipDelay);
            
            player.EquipItem(item);
        }
        private static void ScheduleDelete(Player player, ref ItemDrop.ItemData item)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleDeleteCoroutine(player, item));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleDeleteCoroutine(Player player, ItemDrop.ItemData item)
        {
            yield return new WaitForSeconds(deleteDelay);
            
            if (player.m_inventory.ContainsItem(item))
            {
                player.m_inventory.RemoveItem(item);
            }
        }    
        
    }

    
    
    [HarmonyPatch]
    public static class xBrennaEssencePassive
    {
        public static List<string> BrennaStats = new List<string>() { "false" };

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
        public static class Brenna_Throwable_StartAttack_Patch
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
            
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (currentWeapon.m_shared.m_name != FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name || !xBrennaEssence.Throwable)
                {
                    return true;
                }
               
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
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Attack), nameof(Attack.ProjectileAttackTriggered))]
        public static class Brenna_Throwable_ProjectileAttackTriggered_Patch
        {
            public static void Prefix(Attack __instance, ref EffectList __state)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_weapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name && xBrennaEssence.Throwable)
                {
                    __state = __instance.m_weapon.m_shared.m_triggerEffect;
                    __instance.m_weapon.m_shared.m_triggerEffect = new EffectList();
                }
            }
		      
            public static void Postfix(Attack __instance, EffectList __state)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_weapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name && xBrennaEssence.Throwable)
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
        public static class Brenna_Throwable_FireProjectileBurst_Patch
        {
            public static void Postfix(Attack __instance)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_weapon.m_lastProjectile != null && __instance.m_weapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name && xBrennaEssence.Throwable)
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
                    newMesh.AddComponent<Flip>();
                }
            }
        }
        
        public class Flip : MonoBehaviour
        {
            private const float RotationSpeed = 720;
            public void Awake()
            {
                transform.Rotate(-90, 270, 0);
            }
            public void Update()
            {
                transform.Rotate(0, -RotationSpeed * Time.deltaTime, 0);
            }
        }
        
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.SpawnOnHit))]
        public static class Brenna_SpawnOnHit_Patch
        {
            public static void Prefix(Projectile __instance)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_owner == Player.m_localPlayer && xBrennaEssence.Throwable && __instance.m_spawnItem?.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    if (__instance.m_spawnItem != null)
                    {
                        Vector3 vector3 = __instance.transform.position + __instance.transform.TransformDirection(__instance.m_spawnOffset);
                        Quaternion rotation = __instance.transform.rotation;
                        rotation = Quaternion.Euler(0.0f, __instance.transform.rotation.eulerAngles.y, 0.0f);
                        
                        //sphere radius 9.5
                        // fx_fireskeleton_nova
                        GameObject Aoe = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_fireskeleton_nova"), vector3, rotation);
                        DelayRemoval(Aoe);
                        
                        __instance.m_spawnItem = null;
                    }
                }
            }
            static void DelayRemoval(GameObject Gobject)
            {
                ParticleSystem[] particleSystems = Gobject.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    var main = ps.main;
                    main.startDelay = 0f;
                }
            }
            
        }

        
        
        
        
        
        
        
        

    }
}