using System;
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


namespace LackingImaginationV2
{

    public class xFulingBerserkerEssence // still have to fix bow arrow aim and proj spawn point
    {
        private static readonly int m_LOSMask = LayerMask.GetMask("piece", "piece_nonsolid", "Default", "static_solid", "Default_small", "terrain");
        
        public static string Ability_Name = "Giantization"; //all percentage based stat boosts, shield boost
        
        private static bool noRoof;
        
        public static void Process_Input(Player player, int position, ref float altitude)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)) )
            {
                Vector3 rayOrigin = player.transform.position;
                // Direction of the ray (in this case, shooting upwards)
                Vector3 rayDirection = Vector3.up;
                // Maximum distance for the ray
                float maxDistance = 3.8f; // You can adjust this as needed

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance, m_LOSMask))
                {
                    // A hit occurred, you can now access the distance to the hit point
                    float distanceToHit = hit.distance;
                    noRoof = false;
                    // Now you can use the distanceToHit for whatever you need
                    Debug.Log("Distance to the nearest solid object: " + distanceToHit);
                }
                else
                {
                    noRoof = true;
                }
                
                if (noRoof)
                {
                    LackingImaginationV2Plugin.Log($"FB Button was pressed");
                
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xFulingBerserkerCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                
                    //Effects, animations, and sounds
                
                    // sfx_goblinbrute_shout sfx_goblinbrute_taunt
                    //green lines on skin? hate me if you want trash XD
                    UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.sfx_Giantization, player.transform.position, Quaternion.identity);
                    LackingImaginationV2Plugin.UseGuardianPower = false;
                    
                    ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");

                    //Lingering effects
                    SE_Giantization se_giantization = (SE_Giantization)ScriptableObject.CreateInstance(typeof(SE_Giantization));
                    se_giantization.m_ttl = SE_Giantization.m_baseTTL;

                    //Apply effects
                    player.GetSEMan().AddStatusEffect(se_giantization);
                    
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Can't Cast Here");
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
    }


    [HarmonyPatch]
    public static class xFulingBerserkerEssencePassive 
    {
        private static bool giant;
        
        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class FulingBerserker_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                
                if (!giant && __instance.IsPlayer() && __instance.GetSEMan().HaveStatusEffect("SE_Giantization") )
                {
                    float targetSize = 2.0f;
                    float growthSpeed = 0.9f;
                    float currentSize = Player.m_localPlayer.transform.localScale.x;
                    
                    if (currentSize < targetSize)
                    {
                        currentSize += growthSpeed * Time.deltaTime;
                        currentSize = Mathf.Min(currentSize, targetSize); // Ensure we don't exceed the target size
                        __instance.transform.localScale = currentSize * Vector3.one;
                    }
                    if (currentSize == targetSize)
                    {
                        xFulingBerserkerEssencePassive.giant = true;

                        __instance.m_swimDepth *= 2f;
                    }
                }

                if (giant && __instance.IsPlayer() && !__instance.GetSEMan().HaveStatusEffect("SE_Giantization"))
                {
                    float targetSize = 1.0f;
                    float shrinkSpeed = 0.9f; // You can adjust the speed as needed
                    float currentSize = Player.m_localPlayer.transform.localScale.x; // Use the object's current scale
                    
                    if (currentSize > targetSize) // Assuming 1.0f is the original size, adjust as needed
                    {
                        currentSize -= shrinkSpeed * Time.deltaTime;
                        currentSize = Mathf.Max(currentSize, targetSize); // Ensure we don't go smaller than the original size
                        __instance.transform.localScale = currentSize * Vector3.one;
                    }
                    if (currentSize == targetSize)
                    {
                        xFulingBerserkerEssencePassive.giant = false;

                        __instance.m_swimDepth *= 0.5f;
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetRightHandEquipped))]
        public static class FulingBerserker_SetRightHandEquipped_Patch
        {
            static void Prefix(ref int hash, VisEquipment __instance)
            {
                if (hash != 0 && ZNetScene.instance)
                {
                    if (__instance != null && __instance.m_isPlayer && __instance.m_rightItemInstance != null)
                    {
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Giantization"))
                        {
                            __instance.m_rightItemInstance.transform.localScale = Vector3.one;
                            // __instance.m_rightItemInstance.GetComponentInChildren<BoxCollider>().size = 8 * Vector3.one;
                           
                        }
                    }
                }
            }
        }
        
        
        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLeftHandEquipped))]
        public static class FulingBerserker_SetLeftHandEquipped_Patch
        {
            static void Prefix(ref int hash, ref int variant, VisEquipment __instance)
            {
                if (hash != 0 && ZNetScene.instance)
                {
                    if (__instance != null && __instance.m_isPlayer && __instance.m_leftItemInstance != null)
                    {
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Giantization"))
                        {
                            __instance.m_leftItemInstance.transform.localScale = Vector3.one;
                            // __instance.m_leftItemInstance.GetComponentInChildren<BoxCollider>().transform.localScale = Vector3.one;
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(VisEquipment),nameof(VisEquipment.AttachBackItem))]
        public static class FulingBerserker_AttachBackItem_Patch
        {
            static void Postfix(GameObject __result)
            {
                if (ZNetScene.instance)
                {
                    if (__result != null)
                    {
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Giantization"))
                        {
                            // Double the size of the attached item
                            __result.transform.localScale *= 2f;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Attack), nameof(Attack.DoMeleeAttack))]//Attack GameObject m_spawnOnTrigger (projectile? start)
        public static class FulingBerserker_DoMeleeAttack_Patch
        {
            static void Prefix(Attack __instance)
            {
                if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Giantization"))
                {
                    __instance.m_attackRange *= 2f;
                    __instance.m_attackHeight *= 1.25f;
                    
                }
            }
        }
        // [HarmonyPatch(typeof(Attack))]
        // static class AttackPatch 
        // {
        //     [HarmonyPrefix]
        //     [HarmonyPatch(nameof(Attack.DoMeleeAttack))]
        //     static void DoMeleeAttackPrefix(ref Attack __instance) {
        //         
        //         __instance.m_maxYAngle = 180f;
        //         __instance.m_attackHeight = 1f;
        //     }
        // }
        // float attackRange = this.m_attackRange;
        
        
        
        // [HarmonyPatch(typeof(Projectile),"Setup")]
        // public static class FulingBerserker_Setup_Patch
        // {
        //     static void Postfix(Projectile __instance)
        //     {
        //         if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Giantization"))
        //         {
        //             if (__instance.m_haveStartPoint)
        //             {
        //                 //make arrows shoot from higher
        //                 // Double the height of m_startPoint
        //                 // __instance.m_startPoint = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 4f + Player.m_localPlayer.GetLookDir() * .5f;
        //             }
        //         }
        //     }
        // }

        // [HarmonyPatch(typeof(Attack),"GetProjectileSpawnPoint")]
        // public static class FulingBerserker_GetProjectileSpawnPoint_Patch
        // {
        //     public static void Prefix(Attack __instance, ref Vector3 spawnPoint, out Vector3 aimDir)
        //     {
        //         
        //         Debug.Log("DoubleProjectileSpawnHeightPatch Prefix called");
        //
        //         // Double the height where the projectile spawns
        //         Debug.Log($"Original spawnPoint: {spawnPoint}");
        //         // Double the height where the projectile spawns
        //         spawnPoint += Vector3.up * 8 * __instance.m_attackHeight;
        //         Debug.Log($"Modified spawnPoint: {spawnPoint}");
        //         aimDir = default;
        //         
        //     }
        // }
        
        // [HarmonyPatch(typeof(SpawnAbility),"SetupProjectile")]
        // public static class FulingBerserker_SetupProjectile_Patch
        // {
        //     static void Prefix(Projectile __instance, Vector3 targetPoint)
        //     {
        //         float heightMultiplier = 2.0f; // Adjust this value as needed to double the height
        //         Debug.Log($"Initial projectile spawn height: {__instance.transform.position}");
        //         Vector3 newPosition = __instance.transform.position;
        //         newPosition.y += heightMultiplier * (targetPoint.y - __instance.transform.position.y);
        //         targetPoint = newPosition;
        //         Debug.Log($"Adjusted projectile spawn height to {newPosition}");
        //     }
        // }
        
        // [HarmonyPatch(typeof(Attack),"DoAreaAttack")]
        // public static class FulingBerserker_DoAreaAttack_Patch
        // {
        //     // This is the prefix method that will be called before the original DoAreaAttack method
        //     public static void Prefix(Attack __instance)
        //     {
        //
        //         __instance.m_attackHeight *= 2.0f;
        //
        //     }
        // }

        
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class FulingBerserker_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix( ref float hp, ref float stamina, ref float eitr)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_goblinbrute_essence"))
                {
                    hp += 50f;
                    eitr -= (eitr * 0.75f);

                }
                if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Giantization"))
                {
                    hp += hp;
                    stamina -= (stamina * 0.5f);
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        
    }
}