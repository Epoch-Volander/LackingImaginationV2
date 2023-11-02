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

    public class xThungrEssence
    {
        public static string Ability_Name = "Tyrant";

        private static string Mode1 = "Tyrant\n(Arrogance)";

        private static string Mode2 = "Tyrant\n(Disdain)";
        
        public static GameObject Aura;
        public static GameObject AuraEnemy;
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                // se_cd.m_ttl = LackingImaginationUtilities.xThungrCooldownTime;
                // player.GetSEMan().AddStatusEffect(se_cd);
                
                if (Ability_Name != Mode2 && Ability_Name != Mode1)
                {
                    Ability_Name = Mode1;
                }

                if (Ability_Name == Mode1 && !player.GetSEMan().HaveStatusEffect("SE_Arrogance".GetStableHashCode()))
                {
                    if (player.IsCrouching() && xThungrEssencePassive.ThungrStats.Any(item => item != "0"))
                    {
                        Ability_Name = Mode2;
                        se_cd.m_ttl = 1f;
                        player.GetSEMan().AddStatusEffect(se_cd);
                        return;
                    } 
                    se_cd.m_ttl = LackingImaginationUtilities.xThungrCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                    
                    SE_Arrogance se_arrogance = (SE_Arrogance)ScriptableObject.CreateInstance(typeof(SE_Arrogance));
                    player.GetSEMan().AddStatusEffect(se_arrogance);


                    Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Longinus, player.GetCenterPoint(), Quaternion.identity); /// change effect
                    Aura.transform.parent = player.transform;
                    
                }

                if (Ability_Name == Mode2)
                {
                    if (player.IsCrouching() && xThungrEssencePassive.ThungrStats.Any(item => item != "0"))
                    {
                        Ability_Name = Mode1;
                        se_cd.m_ttl = 1f;
                        player.GetSEMan().AddStatusEffect(se_cd);
                        return;
                    }
                    
                    
                    
                }
                
                
                se_cd.m_ttl = LackingImaginationUtilities.xThungrCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds


                
                
                
                
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }

    }

    
    
    [HarmonyPatch]
    public static class xThungrEssencePassive
    {
        public static List<string> ThungrStats = new List<string>(){"0","0","0","0","0","0","0","0",};

        private static Character Mark = null;
        

        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        class Thungr_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_thungr_essence") && hit.GetAttacker() != null)
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object)hit.GetAttacker() == (UnityEngine.Object)Player.m_localPlayer)
                    {
                        Game.instance.IncrementPlayerStat(__instance.IsPlayer() ? PlayerStatType.PlayerHits : PlayerStatType.EnemyHits);
                        __instance.m_localPlayerHasHit = true;
                    }
                    
                    if (!__instance.m_nview.IsOwner() || (double)__instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if ((UnityEngine.Object)__instance.m_baseAI != (UnityEngine.Object)null && (bool)(UnityEngine.Object)attacker && attacker.IsPlayer())
                    {
                        if (attacker.GetSEMan().HaveStatusEffect("SE_Arrogance") && Mark == null)
                        {
                            Mark = __instance;
                            __instance.GetSEMan().AddStatusEffect("SE_Arrogance".GetStableHashCode());
                            
                            //add effect to the creature instance
                            
                            xThungrEssence.AuraEnemy = UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_BloodSiphon, __instance.transform.position, Quaternion.identity);/// change effect
                            xThungrEssence.AuraEnemy.transform.parent = __instance.transform;
                            
                        }
                    }

                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class Leech_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (__instance.IsPlayer() && Mark != null)
                {
                    if (Mark.IsDead() || (double)Mark.GetHealth() <= 0.0)
                    {
                        ThungrStats[0] = (Mark.m_lastHit.m_damage.m_blunt * 0.25f).ToString();
                        ThungrStats[1] = (Mark.m_lastHit.m_damage.m_pierce * 0.25f).ToString();
                        ThungrStats[2] = (Mark.m_lastHit.m_damage.m_slash * 0.25f).ToString();
                        ThungrStats[3] = (Mark.m_lastHit.m_damage.m_fire * 0.25f).ToString();
                        ThungrStats[4] = (Mark.m_lastHit.m_damage.m_frost * 0.25f).ToString();
                        ThungrStats[5] = (Mark.m_lastHit.m_damage.m_lightning * 0.25f).ToString();
                        ThungrStats[6] = (Mark.m_lastHit.m_damage.m_poison * 0.25f).ToString();
                        ThungrStats[7] = (Mark.m_lastHit.m_damage.m_spirit * 0.25f).ToString();

                        Mark = null;
                    }
                }
                
                
                
                
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}