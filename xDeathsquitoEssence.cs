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

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"xDeathsquitoEssence Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xDeathsquitoCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_deathsquito_death"), player.transform.position, Quaternion.identity);

                //Lingering effects
                SE_Relentless se_relentless = (SE_Relentless)ScriptableObject.CreateInstance(typeof(SE_Relentless));
                se_relentless.m_ttl = SE_Relentless.m_baseTTL;

                //Apply effects
                player.GetSEMan().AddStatusEffect(se_relentless);

            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
    }

    [HarmonyPatch]
    public static class xDeathsquitoEssencePassive
    {
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.FixedUpdate))]
        public class Deathsquito_FixedUpdate_Patch
        {
            static void Prefix(Projectile __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_deathsquito_essence") && Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Relentless"))
                {
                    if (__instance.m_didHit) return; // Skip if the projectile already hit

                    if (__instance.m_owner != null && __instance.m_owner.IsPlayer() && IsTaegetInRange(__instance)) // Make sure the owner is a player
                    {
                        // homing logic
                        Vector3 targetPosition = GetHomingTargetPosition(__instance); // target logic
                        Vector3 toTarget = targetPosition - __instance.transform.position;
                        Vector3 newVelocity = toTarget.normalized * __instance.m_vel.magnitude;
                        // LackingImaginationV2Plugin.Log($"aim{__instance.m_vel}");
                        __instance.m_vel = Vector3.Lerp(__instance.m_vel, newVelocity, Time.fixedDeltaTime * LackingImaginationGlobal.c_deathsquitoRelentlessHoming);
                        // LackingImaginationV2Plugin.Log($"aim{__instance.m_vel}");
                    }
                    
                    bool IsTaegetInRange(Projectile projectile)
                    {
                        List<Character> allCharacters = new List<Character>();
                        allCharacters.Clear();
                        Character.GetCharactersInRange(projectile.transform.position, 12f, allCharacters);
                        foreach (Character ch in allCharacters)
                        {
                            if (ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer) || ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                            {
                                // LackingImaginationV2Plugin.Log($"bool{ch.name}");
                                return true;
                            }
                        }
                        return false;
                    }

                    Vector3 GetHomingTargetPosition(Projectile projectile)
                    {
                        List<Character> allCharacters = new List<Character>();
                        allCharacters.Clear();
                        Character.GetCharactersInRange(projectile.transform.position, 12f, allCharacters);
                        Character closestCharacter = null;
                        float closestDistance = float.MaxValue;
                        foreach (Character ch in allCharacters)
                        {
                            if (ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer) || ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                            {
                                float distanceToCharacter = Vector3.Distance(projectile.transform.position, ch.transform.position);
                                if (distanceToCharacter < closestDistance)
                                {
                                    closestDistance = distanceToCharacter;
                                    closestCharacter = ch;
                                }
                            }
                        }
                        // LackingImaginationV2Plugin.Log($"target{closestCharacter.name}");
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
                                return closestCharacter.transform.position; 
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
        
        
        
        
        
        
        
        
        
        
        
        
        
    }

}