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
using UnityEngine.UI;



namespace LackingImaginationV2
{

    public class xGeirrhafaEssence
    {
        public static string Ability_Name = "Ice \nAge";

        public static bool GeirrhafaController = false;

        private static float AoeDelay = 1.46f;
        private static float minDistanceBetweenCharacters = 2f;
        
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                 se_cd.m_ttl = LackingImaginationUtilities.xGeirrhafaCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                EffectList m_startEffects = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_icenova"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = true,
                        }
                    }
                };
                EffectList m_HandEffectsRight = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_frost_hand_aoestart"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = false,
                            m_follow = true,
                            m_childTransform = "RightHandMiddle3_end"
                        }
                    }
                };
                EffectList m_HandEffectsLeft = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_frost_hand_aoestart"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = false,
                            m_follow = true,
                            m_childTransform = "LeftHandMiddle3_end"
                        }
                    }
                };
                LackingImaginationV2Plugin.UseGuardianPower = false;
                GeirrhafaController = true;
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                GeirrhafaController = false;

                m_startEffects.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                ScheduleEffects(player, m_HandEffectsRight, m_HandEffectsLeft);
                
                System.Threading.Timer timerAoe = new System.Threading.Timer
                    (_ => { ScheduleAoe(player); }, null, (int)(AoeDelay * 1000), System.Threading.Timeout.Infinite);
                
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }

        
        private static void ScheduleEffects(Player player, EffectList Right, EffectList Left)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleEffectsCoroutine(player, Right, Left));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleEffectsCoroutine(Player player, EffectList Right, EffectList Left)
        {
            float effectDelay = 4f;
            int effectCast = 0;
            
            while (effectCast < 3)
            {
                Right.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                Left.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                effectCast++;
                effectDelay *= 0.5f;
                
                yield return new WaitForSeconds(effectDelay);
            }
        }
         
         private static void ScheduleAoe(Player player)
         {
             CoroutineRunner.Instance.StartCoroutine(ScheduleAoeCoroutine(player));
         }
         // ReSharper disable Unity.PerformanceAnalysis
         private static IEnumerator ScheduleAoeCoroutine(Player player)
         {
             int AoeCast = 0;
             
             while (AoeCast < 3)
             {
                 List<Character> allCharacters = new List<Character>();
                 allCharacters.Clear();
                 Character.GetCharactersInRange(player.transform.position, 7f, allCharacters);
                 foreach (Character ch in allCharacters)
                 {
                     if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer)) 
                         && !ch.m_tamed ||ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                     {
                         HitData hitData = new HitData();
                         hitData.m_damage.m_frost = UnityEngine.Random.Range(3f, 5f);
                         hitData.m_damage.m_blunt = 3f;
                         hitData.m_dir = ch.transform.position - player.transform.position;
                         hitData.ApplyModifier( LackingImaginationGlobal.c_geirrhafaIceAgeAoe);
                         hitData.m_point = ch.transform.position;
                         hitData.SetAttacker(player);
                         ch.Damage(hitData);
                     }
                 } 
                 HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

                Vector3 capsuleCenter = player.transform.position;
                float capsuleRadius = 7f; // Radius of the capsule

                // Perform the capsule overlap check with the specified layer mask
                Collider[] colliders = Physics.OverlapSphere(capsuleCenter, capsuleRadius, Script_Breath_Layermask);

                foreach (Collider collider in colliders)
                {
                    IDestructible destructibleComponent = collider.gameObject.GetComponent<IDestructible>();
                    Character characterComponent = collider.gameObject.GetComponent<Character>();
                    if (destructibleComponent != null || (characterComponent != null && !characterComponent.IsOwner()))
                    {
                        // This is a valid target (creature) if it hasn't been detected before.
                        if (!detectedObjects.Contains(collider.gameObject))
                        {
                            detectedObjects.Add(collider.gameObject);
                            
                            HitData hitData = new HitData();
                            hitData.m_damage.m_frost = UnityEngine.Random.Range(3f, 5f);
                            hitData.m_damage.m_blunt = 3f;
                            hitData.m_dodgeable = true;
                            hitData.m_blockable = true;
                            hitData.m_hitCollider = collider;
                            hitData.m_dir = collider.transform.position - player.transform.position;
                            hitData.ApplyModifier( LackingImaginationGlobal.c_geirrhafaIceAgeAoe);
                            hitData.m_point = collider.transform.position;
                            hitData.SetAttacker(player);
                            destructibleComponent.Damage(hitData);
                        }
                    }
                }
                AoeCast++;
                 
                 yield return new WaitForSeconds(AoeDelay);
             }
             {
                 List<Character> allCharacters = new List<Character>();
                 allCharacters.Clear();
                 Character.GetCharactersInRange(Player.m_localPlayer.GetCenterPoint(), 15f, allCharacters);
                 List<Character> affectedCharacters = new List<Character>();
                 affectedCharacters.Clear();
                 foreach (Character currentCharacter in allCharacters)
                 {
                     if ((currentCharacter.GetBaseAI() is MonsterAI && currentCharacter.GetBaseAI().IsEnemy(Player.m_localPlayer)) && !currentCharacter.m_tamed ||
                         currentCharacter.GetBaseAI() is AnimalAI)
                     {
                         bool tooClose = false;
                         // Check if the current character is too close to any affected character
                         foreach (Character affectedCharacter in affectedCharacters)
                         {
                             float distance = Vector3.Distance(currentCharacter.transform.position, affectedCharacter.transform.position);
                             if (distance < minDistanceBetweenCharacters)
                             {
                                 tooClose = true;
                                 break; // No need to check other affected characters if one is too close
                             }
                         }
                         if (!tooClose)
                         {
                             // Add the current character to the list of affected characters
                             affectedCharacters.Add(currentCharacter);
                             SummonIce(currentCharacter, 0.1f);
                             SummonIce(currentCharacter, 8f);
                             SummonIce(currentCharacter, 8f);
                         }
                     }
                 }
             }
         }
         
         static void SummonIce(Character currentCharacter, float range)
         {
             Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * range;
             Vector3 randomPosition = currentCharacter.transform.position + new Vector3(randomCirclePoint.x, 5f, randomCirclePoint.y);
             GameObject fallingIce = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("caverock_ice_stalagtite_falling"), randomPosition, Quaternion.identity);

             fallingIce.GetComponent<ImpactEffect>().m_damagePlayers = false;
             fallingIce.GetComponent<ImpactEffect>().m_damages.m_frost = 5f;
             fallingIce.GetComponent<ImpactEffect>().m_damages.m_pierce = 5f;
             fallingIce.GetComponent<ImpactEffect>().m_damages.Modify(LackingImaginationGlobal.c_geirrhafaIceAgeAoe);
         }
    }

    
    
    [HarmonyPatch]
    public static class xGeirrhafaEssencePassive
    {
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        class Geirrhafa_GetTotalFoodValue_Patch
        {
            public static void Postfix(Player __instance, ref float eitr)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_geirrhafa_essence"))
                {
                    eitr += LackingImaginationGlobal.c_geirrhafaIceAgePassiveEitr;
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Geirrhafa_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_geirrhafa_essence") && hit.GetAttacker() != null)
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
                    if (__instance != null && __instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        int Dubious = UnityEngine.Random.Range(1, 21); // 1-20 inclusive
                        if (Dubious == 1)
                        {
                            hit.m_damage.m_frost = hit.GetTotalDamage() * 0.1f;
                        }
                    }
                    if ((UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
                    {
                        hit.m_damage.m_frost += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_geirrhafaIceAgePassive;
                    }
                }
            }
        }

    }
    
    
}