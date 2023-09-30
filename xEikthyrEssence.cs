using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
 
    public class xEikthyrEssence 
    {
        public static string Ability_Name = "Blitz";
        
        public static bool EikthyrController = false;
        
        private static int Script_Lightning_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");

        private static float lightningDelay = 1f;

        // public static float xxx = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.Clubs);
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Eikthyr Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xEikthyrCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_eikthyr_death"), player.transform.position, Quaternion.identity);
               
                LackingImaginationV2Plugin.UseGuardianPower = false;
                EikthyrController = true;
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                EikthyrController = false;
              
                
                ScheduleLightning(player);

                // // Assuming player is a reference to the player GameObject
                // Vector3 coneDirection = player.transform.forward;
                // float coneAngle = 70f; // Adjust the cone angle as needed
                // float coneLength = 150f; // Adjust the cone length as needed
                // int rayCount = 20; // Adjust the number of rays vertically
                //
                // RaycastHit[] hits;
                // List<Vector3> rayDirections = GenerateRayDirections(coneDirection, rayCount, coneAngle * 0.5f);
                //
                // foreach (Vector3 rayDir in rayDirections)
                // {
                //     hits = Physics.RaycastAll(player.GetEyePoint(), rayDir, coneLength);
                //
                //     foreach (RaycastHit hit in hits)
                //     {
                //         Vector3 toObject = hit.transform.position - player.transform.position;
                //         float angle = Vector3.Angle(coneDirection, toObject);
                //         if (angle <= coneAngle * 0.5f)
                //         {
                //             // Object is within the cone's angle
                //             // Do something with the detected object (hit.transform)
                //             if (hit.transform.gameObject.GetComponent<Character>() != null)
                //             {
                //                 Character ch = hit.transform.gameObject.GetComponent<Character>();
                //                    
                //                 HitData hitData = new HitData();
                //                 hitData.m_damage.m_lightning = UnityEngine.Random.Range(2f, 5f);
                //                 hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_eikthyrBlitz));
                //                 hitData.m_pushForce = 0f;
                //                 hitData.SetAttacker(player);
                //                 hitData.m_point = ch.GetEyePoint();
                //                     
                //                 ch.Damage(hitData);
                //                     
                //             }
                //         }
                //     }
                // }
                //
                // List<Character> allCharacters = new List<Character>();
                // allCharacters.Clear();
                // Character.GetCharactersInRange(player.transform.position, 4f, allCharacters);
                // foreach (Character ch in allCharacters)
                // {
                //     if (BaseAI.IsEnemy(player, ch))
                //     {
                //         HitData hitData = new HitData();
                //         hitData.m_damage.m_lightning = UnityEngine.Random.Range(10f, 15f);
                //         hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_eikthyrBlitz));
                //         hitData.SetAttacker(player);
                //         hitData.m_point = ch.GetEyePoint();
                //         ch.Damage(hitData);
                //     }
                // }
                //
                // // Generates ray directions within the specified cone angle
                // List<Vector3> GenerateRayDirections(Vector3 coneDirection, int rayCount, float maxAngle)
                // {
                //     List<Vector3> rayDirections = new List<Vector3>();
                //
                //     for (int i = 0; i < rayCount; i++)
                //     {
                //         float angle = Mathf.Lerp(-maxAngle, maxAngle, (float)i / (float)(rayCount - 1));
                //         Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                //         rayDirections.Add(rotation * coneDirection);
                //     }
                //
                //     return rayDirections;
                // }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        private static void ScheduleLightning(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleLightningCoroutine(player));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleLightningCoroutine(Player player)
        {
            yield return new WaitForSeconds(lightningDelay);
            
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_eikthyr_forwardshockwave"), player.transform.position, player.transform.rotation, player.transform);

            HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

            Vector3 capsuleCenter = player.GetCenterPoint() + player.transform.forward * 0.5f;
            float capsuleRadius = 2f; // Radius of the capsule
            float capsuleHeight = 30f; // Height of the capsule (equals the ray length)

            // Perform the capsule overlap check with the specified layer mask
            Collider[] collidersRight = Physics.OverlapCapsule(capsuleCenter, capsuleCenter + (player.transform.forward * capsuleHeight) + (player.transform.right * capsuleRadius) , capsuleRadius, Script_Lightning_Layermask);
            Collider[] collidersLeft = Physics.OverlapCapsule(capsuleCenter, capsuleCenter + (player.transform.forward * capsuleHeight) + -(player.transform.right * capsuleRadius) , capsuleRadius, Script_Lightning_Layermask);
            // Combine the results into a single array
            Collider[] combinedColliders = collidersRight.Concat(collidersLeft).ToArray();

             foreach (Collider collider in combinedColliders)
            {
                IDestructible destructibleComponent = collider.gameObject.GetComponent<IDestructible>();
                if (destructibleComponent != null)
                {
                    // This is a valid target (creature) if it hasn't been detected before.
                    if (!detectedObjects.Contains(collider.gameObject))
                    {
                        detectedObjects.Add(collider.gameObject);
                        
                        HitData hitData = new HitData();
                        hitData.m_hitCollider = collider;
                        hitData.m_dodgeable = true;
                        hitData.m_blockable = true;
                        hitData.m_ranged = true;
                        hitData.m_damage.m_lightning = LackingImaginationGlobal.c_moderDraconicFrostDragonBreath;
                        hitData.m_dir = collider.gameObject.transform.position - player.transform.position;
                        // hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_loxWildTremor));
                        hitData.m_pushForce = 10f;
                        hitData.m_backstabBonus = 2f;
                        hitData.m_staggerMultiplier = 2f;
                        hitData.m_point = collider.gameObject.transform.position;
                        hitData.SetAttacker(player);
                        hitData.m_hitType = HitData.HitType.PlayerHit;
                        destructibleComponent.Damage(hitData);
                    }
                }
                Character characterComponent = collider.gameObject.GetComponent<Character>();
                if (characterComponent != null && !characterComponent.IsOwner())
                {
                    // This is a valid target (creature) if it hasn't been detected before.
                    if (!detectedObjects.Contains(collider.gameObject))
                    {
                        detectedObjects.Add(collider.gameObject);
                        
                        HitData hitData = new HitData();
                        hitData.m_hitCollider = collider;
                        hitData.m_dodgeable = true;
                        hitData.m_blockable = true;
                        hitData.m_ranged = true;
                        hitData.m_damage.m_lightning = LackingImaginationGlobal.c_moderDraconicFrostDragonBreath;
                        hitData.m_dir = collider.gameObject.transform.position - player.transform.position;
                        // hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_loxWildTremor));
                        hitData.m_pushForce = 10f;
                        hitData.m_backstabBonus = 2f;
                        hitData.m_staggerMultiplier = 2f;
                        hitData.m_point = collider.gameObject.transform.position;
                        hitData.SetAttacker(player);
                        hitData.m_hitType = HitData.HitType.PlayerHit;
                        characterComponent.Damage(hitData);
                    }
                }
            }
             
             
        }
    }

    [HarmonyPatch]
    public static class xEikthyrEssencePassive
    {

        [HarmonyPatch(typeof(Character), (nameof(Character.RPC_Damage)))]
        class Eikthyr_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_eikthyr_essence") && hit.GetAttacker() != null)
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
                    if ((UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
                    {
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_himminafl_hit"), hit.m_point, Quaternion.identity, __instance.transform);
                        hit.m_damage.m_lightning += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_eikthyrBlitzPassive;
                    }
                    if (__instance.IsStaggering() && !__instance.IsPlayer())
                    {
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_himminafl_aoe"), hit.m_point, Quaternion.identity, __instance.transform);
                    }
                    if (__instance.GetSEMan().HaveStatusEffect("Wet") && __instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        // LackingImaginationV2Plugin.Log($"Eikthyr you have been hit");
                        hit.m_damage.m_lightning += hit.GetTotalDamage() * 0.05f;
                    }
                }
            }
        }
    }


}