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

    public class xZilEssence
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
        
        public static string Ability_Name = "Soulmass";

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xZilCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                for(int i = 0; i < 5; i++)
                {
                    Vector3 vector = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * .5f; // player.GetLookDir() * 2f;
                    GameObject prefab = ZNetScene.instance.GetPrefab("GoblinShaman_projectile_fireball");
                    GameObject GO_SoulmassProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
                    Projectile P_SoulmassProjectile = GO_SoulmassProjectile.GetComponent<Projectile>();
                    P_SoulmassProjectile.name = "Soulmass" + i;
                    P_SoulmassProjectile.m_respawnItemOnHit = false;
                    P_SoulmassProjectile.m_spawnOnHit = null;
                    P_SoulmassProjectile.m_ttl = 60f;
                    P_SoulmassProjectile.m_gravity = 0f;
                    P_SoulmassProjectile.m_rayRadius = .2f;
                    P_SoulmassProjectile.m_aoe = 3f;
                    P_SoulmassProjectile.m_owner = player;
                    P_SoulmassProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
                    P_SoulmassProjectile.transform.localScale = Vector3.one;

                    RaycastHit hitInfo = default(RaycastHit);
                    Vector3 player_position = player.transform.position;
                    Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
                    HitData hitData = new HitData();
                    hitData.m_damage.m_fire = UnityEngine.Random.Range(1f, 2f);
                    hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f, 2f);
                    hitData.m_damage.m_spirit = UnityEngine.Random.Range(1f, 2f);
                    hitData.ApplyModifier(LackingImaginationGlobal.c_fulingshamanRitualProjectile);
                    hitData.m_pushForce = 2f;
                    hitData.SetAttacker(player);
                    Vector3 a = Vector3.MoveTowards(GO_SoulmassProjectile.transform.position, target, 1f);
                    P_SoulmassProjectile.Setup(player, (a - GO_SoulmassProjectile.transform.position) * 25f, -1f, hitData, null, null);
                    GO_SoulmassProjectile = null;
                }
                
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }

    }

    
    
    [HarmonyPatch]
    public static class xZilEssencePassive
    {
        
        
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.FixedUpdate))]
        class Projectile_FixedUpdate_Patch
        {
            static void Prefix(Projectile __instance)
            {
                if (__instance.m_owner == Player.m_localPlayer && EssenceItemData.equipedEssence.Contains("$item_zil_essence") && __instance.name.Substring(0, __instance.name.Length - 1) == "Soulmass")
                {
                    if (!__instance.m_nview.IsValid() || !__instance.m_nview.IsOwner())
                        return;

                    if (__instance.m_didHit)
                    {
                        return;
                    }// Skip if the projectile already hit
                    
                    // Calculate the new position for hovering
                    Vector3 newPosition = CalculateHoverPosition(__instance);

                    // Move the projectile to the new position
                    __instance.transform.position = newPosition;
                    
                    bool shouldFire = IsTargetInRange(__instance);

                    if (shouldFire && !__instance.m_didHit)
                    {
                        __instance.name = "SoulmassReleased";
                        // Modify the behavior to fire the projectile
                        Vector3 targetPosition = GetTargetPosition(__instance);
                        Vector3 toTarget = targetPosition - __instance.transform.position;
                        Vector3 newVelocity = toTarget.normalized * __instance.m_vel.magnitude;
                        __instance.m_vel = newVelocity * 0.5f;
                        // __instance.m_didHit = true;
                    }
                }
            }
            
            static bool IsTargetInRange(Projectile projectile)
            {
                List<Character> allCharacters = new List<Character>();
                allCharacters.Clear();
                Character.GetCharactersInRange(projectile.transform.position, 20f, allCharacters);
                foreach (Character ch in allCharacters)
                {
                    if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer) || ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI) && LineOfSight.LOS(ch, projectile.transform.position))
                    {
                        return true;
                    }
                }
                return false;
            }
            static Vector3 GetTargetPosition(Projectile projectile)
            {
                List<Character> allCharacters = new List<Character>();
                allCharacters.Clear();
                Character.GetCharactersInRange(projectile.transform.position, 21f, allCharacters);
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
            
            // Calculate the new position for hovering
            static Vector3 CalculateHoverPosition(Projectile projectile)
            {
                Vector3 currentVelocity = projectile.GetVelocity();

                // Assuming you have a reference to the player (e.g., player variable)
                if (projectile.m_owner != null && !projectile.m_didHit)
                {
                    Vector3 playerPosition = projectile.m_owner.transform.position;
                    Quaternion playerRotation = projectile.m_owner.transform.rotation;
                    Vector3 hoverOffset;
                    switch (projectile.name)
                    {
                        case "Soulmass0":
                            hoverOffset = playerRotation * (Vector3.up * 3.2f); // Adjust the hover height 
                            break;
                        case "Soulmass1":
                            hoverOffset = playerRotation * ((Vector3.up * 2.45f) + (Vector3.left * 0.6f)); 
                            break;
                        case "Soulmass2":
                            hoverOffset = playerRotation * ((Vector3.up * 2f) + (Vector3.left * 1.1f)); 
                            break;
                        case "Soulmass3":
                            hoverOffset = playerRotation * ((Vector3.up * 2.45f) + (Vector3.right * 0.6f)); 
                            break;
                        case "Soulmass4":
                            hoverOffset = playerRotation * ((Vector3.up * 2f) + (Vector3.right * 1.1f));
                            break;
                        
                        default:
                            hoverOffset = Vector3.zero; // Default value 
                            break;
                    }

                    return playerPosition + hoverOffset - currentVelocity * Time.fixedDeltaTime;
                }
                else
                {
                    // If the player reference is not available, keep the current position
                    return projectile.transform.position;
                }
                
                
                
            }

        }

    }
    
    
    
    
    
    
    
    
}