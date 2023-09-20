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
using UnityEngine.UI;



namespace LackingImaginationV2
{

    public class xGeirrhafaEssence
    {
        public static string Ability_Name = "Ice Age";

        public static bool GeirrhafaController = false;
        
        private static float effectDelay = 4f;
        private static int effectCast = 0;

        private static float AoeDelay = 1.46f;
        private static int AoeCast = 0;
        private static float minDistanceBetweenCharacters = 2f;

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Geirrhafa Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                 se_cd.m_ttl = LackingImaginationUtilities.xGeirrhafaCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                
                
                // //caverock_ice_stalagtite_falling  have these droop on enemies and a few random spots or maybe fall in groups of three
                // Fenring_attack_IceNova
                // fx_fenring_icenova
                //     fx_fenring_frost_hand_aoestart
                // Frost AoE Spell Attack 3 Burst
                
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_fenring_icenova"), player.transform.position, Quaternion.identity);

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
                ScheduleEffect(player, m_HandEffectsRight, m_HandEffectsLeft);
                
                System.Threading.Timer timerAoe = new System.Threading.Timer
                    (_ => { ScheduleAoe(player); }, null, (int)(AoeDelay * 1000), System.Threading.Timeout.Infinite);


            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

         private static void ScheduleEffect(Player player, EffectList Right, EffectList Left)
        {
            if (effectCast < 3)
            {
               
                Right.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                Left.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                effectCast++;
                effectDelay *= 0.5f;
                
                System.Threading.Timer timer = new System.Threading.Timer
                (_ => { ScheduleEffect(player, Right, Left); }, null, (int)(effectDelay * 1000), System.Threading.Timeout.Infinite);
                
            }
            else
            {
                effectCast = 0;
                effectDelay = 4f;
            }
        }
         private static void ScheduleAoe(Player player)
         {
             if (AoeCast < 3)
             {
                 List<Character> allCharacters = new List<Character>();
                 allCharacters.Clear();
                 Character.GetCharactersInRange(player.transform.position, 8f, allCharacters);
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
                         hitData.m_pushForce = 100f;
                         hitData.m_point = ch.transform.position;
                         hitData.SetAttacker(player);
                         ch.Damage(hitData);
                     }
                 }
                 
                 AoeCast++;
                 
                 System.Threading.Timer timerAoe = new System.Threading.Timer
                     (_ => { ScheduleAoe(player); }, null, (int)(AoeDelay * 1000), System.Threading.Timeout.Infinite);
                
             }
             else
             {
                 AoeCast = 0;
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
                             SummonIce(currentCharacter, 2f);
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
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}