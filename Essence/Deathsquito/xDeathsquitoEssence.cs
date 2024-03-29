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

    public class xDeathsquitoEssence
    {
        public static string Ability_Name = "Relentless";

        public static GameObject[] Aura;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xDeathsquitoCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Lingering effects
                SE_Relentless se_relentless = (SE_Relentless)ScriptableObject.CreateInstance(typeof(SE_Relentless));
                se_relentless.m_ttl = SE_Relentless.m_baseTTL;
                
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                EffectList m_starteffect = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = LackingImaginationV2Plugin.fx_Relentless,
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = true,
                            m_follow = true,
                            m_childTransform = "Head",
                            // m_inheritParentScale = true,
                            // m_multiplyParentVisualScale = true,
                            // m_scale = true,
                            m_inheritParentRotation = true,
                        }
                    }
                };
                m_starteffect.m_effectPrefabs[0].m_prefab.GetComponent<TimedDestruction>().m_timeout = SE_Relentless.m_baseTTL;
                // Vector3 transformPosition = Aura.m_effectPrefabs[0].m_prefab.transform.position;
                // transformPosition.y += 0.4f;
                // Aura.m_effectPrefabs[0].m_prefab.transform.position = transformPosition;
                Aura = m_starteffect.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                
                // Aura = UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_Relentless, player.transform.position + player.transform.up * 2.2f, Quaternion.identity);
                // Aura.transform.parent = player.transform;
                

                //Apply effects
                player.GetSEMan().AddStatusEffect(se_relentless);

            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
    }

    [HarmonyPatch]
    public static class xDeathsquitoEssencePassive
    {
        private static List<Projectile> arrows = new List<Projectile>();
        private static List<GameObject[]> eyes = new List<GameObject[]>();

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.FixedUpdate))]
        public class Deathsquito_FixedUpdate_Patch
        {
            static void Prefix(Projectile __instance)
            {
                if (__instance.m_owner == Player.m_localPlayer && EssenceItemData.equipedEssence.Contains("$item_deathsquito_essence") && Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Relentless"))
                {
                    if (__instance.m_didHit)
                    {
                        if (arrows.Contains(__instance)) arrows.Remove(__instance);
                        return;
                    }// Skip if the projectile already hit

                    if (!arrows.Contains(__instance))
                    { 
                        // UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_Relentless, __instance.transform.position + __instance.transform.up * 0.3f, Quaternion.identity).transform.parent = __instance.transform;
                        EffectList m_starteffect = new EffectList
                        {
                            m_effectPrefabs = new EffectList.EffectData[]
                            {
                                new()
                                {
                                    m_prefab = LackingImaginationV2Plugin.fx_Relentless,
                                    m_enabled = true,
                                    m_variant = 0,
                                    m_attach = true,
                                    m_follow = true,
                                    // m_inheritParentScale = true,
                                    // m_multiplyParentVisualScale = true,
                                    // m_scale = true,
                                    m_inheritParentRotation = true,
                                }
                            }
                        };
                        
                        arrows.Add(__instance);
                        m_starteffect.Create(__instance.transform.position + __instance.transform.up * 0.3f, __instance.transform.rotation, __instance.transform);
                        // eyes.Add(m_starteffect.Create(__instance.transform.position + __instance.transform.up * 0.3f, __instance.transform.rotation, __instance.transform));

                    }
                    
                    if (__instance.m_owner != null && IsTargetInRange(__instance)) // Make sure the owner is a player
                    {
                        // homing logic
                        Vector3 targetPosition = GetHomingTargetPosition(__instance); // target logic
                        Vector3 toTarget = targetPosition - __instance.transform.position;
                        Vector3 newVelocity = toTarget.normalized * __instance.m_vel.magnitude;
                        // LackingImaginationV2Plugin.Log($"aim{__instance.m_vel}");
                        __instance.m_vel = Vector3.Lerp(__instance.m_vel, newVelocity, Time.fixedDeltaTime * LackingImaginationGlobal.c_deathsquitoRelentlessHoming);
                        // LackingImaginationV2Plugin.Log($"aim{__instance.m_vel}");
                    }
                    
                    bool IsTargetInRange(Projectile projectile)
                    {
                        List<Character> allCharacters = new List<Character>();
                        allCharacters.Clear();
                        Character.GetCharactersInRange(projectile.transform.position, LackingImaginationGlobal.c_deathsquitoRelentlessHomingRange, allCharacters);
                        foreach (Character ch in allCharacters)
                        {
                            if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer) || ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI) && LineOfSight.LOS(ch, projectile.transform.position))
                            {
                                return true;
                            }
                        }
                        return false;
                    }

                    Vector3 GetHomingTargetPosition(Projectile projectile)
                    {
                        List<Character> allCharacters = new List<Character>();
                        allCharacters.Clear();
                        Character.GetCharactersInRange(projectile.transform.position, LackingImaginationGlobal.c_deathsquitoRelentlessHomingRange + 1f, allCharacters);
                        Character closestCharacter = null;
                        Character backupCharacter = null;
                        float closestDistance = float.MaxValue;
                        foreach (Character ch in allCharacters)
                        {
                            if (ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer) || ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                            {
                                float distanceToCharacter = Vector3.Distance(projectile.transform.position, ch.transform.position);
                                if (distanceToCharacter < closestDistance)
                                {
                                    closestDistance = distanceToCharacter;
                                    backupCharacter = ch;
                                    if(LineOfSight.LOS(ch, projectile.transform.position)) closestCharacter = ch;

                                }
                            }
                        }
                        if(closestCharacter != null)
                        {
                            try
                            {
                                return closestCharacter.GetHeadPoint();
                            }
                            catch
                            {
                                try
                                {
                                    return closestCharacter.GetCenterPoint();
                                }
                                catch
                                {
                                    return closestCharacter.transform.position + Vector3.up * 0.5f;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                return backupCharacter.GetHeadPoint();
                            }
                            catch
                            {
                                try
                                {
                                    return backupCharacter.GetCenterPoint();
                                }
                                catch
                                {
                                    return backupCharacter.transform.position + Vector3.up * 0.5f;
                                }
                            } 
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.Setup))]
        public class Deathsquito_Setup_Patch
        {
            static void Prefix(ref HitData hitData)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_deathsquito_essence"))
                {
                    hitData.m_damage.m_pierce += hitData.GetTotalDamage() * LackingImaginationGlobal.c_deathsquitoRelentlessPassive;
                }
            }
        }
        
        // [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        // public static class Deathsquito_UpdateEnvStatusEffects_Patch
        // {
        //     public static void Prefix(Player __instance, ref float dt)
        //     {
        //         if (!__instance.GetSEMan().HaveStatusEffect("SE_Relentless") && xDeathsquitoEssence.Aura != null)
        //         {
        //             if(xDeathsquitoEssence.Aura.HasEffects())
        //             {
        //                 // if(xDeathsquitoEssence.Aura != null) xDeathsquitoEssence.Aura.m_effectPrefabs.;
        //                 foreach (EffectList.EffectData effectPrefab in xDeathsquitoEssence.Aura.m_effectPrefabs)
        //                 {
        //                     if (effectPrefab.m_enabled)
        //                         effectPrefab.m_enabled = false;
        //                 }
        //             }
        //         }
        //     }
        // }
        
        
        //add negative
        
        // [HarmonyPatch(typeof(Character), "AddNoise")]
        // public class Deathsquito_AddNoise_Patch
        // {
        //     static void Prefix(ref float range)
        //     {
        //         if (EssenceItemData.equipedEssence.Contains("$item_deathsquito_essence") && Player.m_localPlayer.m_nview.IsOwner())
        //         {
        //             range *= 3f;
        //         }
        //     }
        // }
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Deathsquito_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.Low)]
            public static void Postfix( ref float hp, ref float stamina, ref float eitr)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_deathsquito_essence"))
                {
                    hp -= hp * 0.5f; // hp ruduced by half
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        
    }

}