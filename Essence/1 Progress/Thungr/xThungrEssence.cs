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

        public static string Mode1 = "Tyrant\n(Arrogance)";
        public static string Mode2 = "Tyrant\n(Disdain)";
        
        // public static GameObject AuraA;
        // public static GameObject AuraD;
        // public static GameObject AuraEnemy;
        
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

                if (Ability_Name == Mode1)
                {
                    if (player.IsCrouching() && xThungrEssencePassive.ThungrStats.Any(item => item != "0"))
                    {
                        Ability_Name = Mode2;
                        se_cd.m_ttl = 1f;
                        player.GetSEMan().AddStatusEffect(se_cd);

                        if (player.GetSEMan().HaveStatusEffect("SE_Arrogance".GetStableHashCode()))
                        {
                            player.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetStableHashCode());
                            xThungrEssencePassive.Mark.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetStableHashCode());
                            xThungrEssencePassive.Mark = null;

                            // if (xThungrEssence.AuraA != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraA);
                            // if (xThungrEssence.AuraEnemy != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraEnemy);
                        }

                        return;
                    }

                    if(!player.GetSEMan().HaveStatusEffect("SE_Arrogance".GetStableHashCode()))
                    {
                        se_cd.m_ttl = LackingImaginationUtilities.xThungrCooldownTime;
                        player.GetSEMan().AddStatusEffect(se_cd);

                        SE_Arrogance se_arrogance = (SE_Arrogance)ScriptableObject.CreateInstance(typeof(SE_Arrogance));
                        se_arrogance.m_startEffects.m_effectPrefabs[0].m_prefab = LackingImaginationV2Plugin.fx_ArroDebuff;
                        se_arrogance.m_startEffects.m_effectPrefabs[0].m_inheritParentScale = false;
                        se_arrogance.m_startEffects.m_effectPrefabs[0].m_childTransform = "Hips";
                        player.GetSEMan().AddStatusEffect(se_arrogance);
                        
                        // AuraA = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Longinus, player.GetCenterPoint(), Quaternion.identity); /// change effect Arrogance
                        // AuraA.transform.parent = player.transform;
                    }
                    
                    else if (player.GetSEMan().HaveStatusEffect("SE_Arrogance".GetStableHashCode()))
                    {
                        se_cd.m_ttl = LackingImaginationUtilities.xThungrCooldownTime * 0.1f;
                        player.GetSEMan().AddStatusEffect(se_cd);
                        
                        player.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetStableHashCode());
                        xThungrEssencePassive.Mark.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetStableHashCode());
                        xThungrEssencePassive.Mark = null;

                        // if(xThungrEssence.AuraA != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraA);
                        // if(xThungrEssence.AuraEnemy != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraEnemy);
                    }

                }
                

                if (Ability_Name == Mode2)
                {
                    if (player.IsCrouching() && xThungrEssencePassive.ThungrStats.Any(item => item != "0"))
                    {
                        Ability_Name = Mode1;
                        se_cd.m_ttl = 1f;
                        player.GetSEMan().AddStatusEffect(se_cd);
                        
                        if (player.GetSEMan().HaveStatusEffect("SE_Disdain".GetStableHashCode()))
                        {
                            player.GetSEMan().RemoveStatusEffect("SE_Disdain".GetStableHashCode());
                           
                            // if (xThungrEssence.AuraD != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraD);
                        }
                        
                        return;
                    }
                    
                    //Lingering effects
                    SE_Disdain se_disdain = (SE_Disdain)ScriptableObject.CreateInstance(typeof(SE_Disdain));
                    se_disdain.m_ttl = SE_Disdain.m_baseTTL;
                    player.GetSEMan().AddStatusEffect(se_disdain);
                    
                    se_cd.m_ttl = LackingImaginationUtilities.xThungrCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                
                    //Effects, animations, and sounds

                    // AuraD = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Longinus, player.GetCenterPoint(), Quaternion.identity); /// change effect dISDAIN
                    // AuraD.transform.parent = player.transform;
                    
                }
                
                
                
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

        public static Character Mark = null;
        

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
                        __instance.m_localPlayerHasHit = true;
                    }
                    
                    if (!__instance.m_nview.IsOwner() || (double)__instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if ((UnityEngine.Object)__instance.m_baseAI != (UnityEngine.Object)null && (bool)(UnityEngine.Object)attacker && attacker.IsPlayer())
                    {
                        if (attacker.GetSEMan().HaveStatusEffect("SE_Arrogance") && Mark == null && __instance.GetBaseAI() != null /*&& __instance.GetBaseAI() is MonsterAI*/)
                        {
                            Mark = __instance;
                            __instance.GetSEMan().AddStatusEffect("SE_Arrogance".GetStableHashCode());
                            
                            __instance.Message(MessageHud.MessageType.Center, $"{__instance.m_name.Replace("(Clone)", "")} is Enraged");
                            //add effect to the creature instance
                            
                            // xThungrEssence.AuraEnemy = UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_BloodSiphon, __instance.GetCenterPoint(), Quaternion.identity);/// change effect
                            // xThungrEssence.AuraEnemy.transform.parent = __instance.transform;
                            
                        }
                    }

                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class Thungr_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (__instance.IsPlayer() && Mark != null && __instance.GetSEMan().HaveStatusEffect("SE_Arrogance"))
                {
                    if (Mark.IsDead() || (double)Mark.GetHealth() <= 0.0)
                    {
                        if (Mark.m_lastHit != null && Mark.m_lastHit.GetAttacker() == __instance)
                        {
                            Humanoid mon = (Mark.GetBaseAI() as MonsterAI).m_character as Humanoid;
                            if (mon != null)
                            {
                                ThungrStats[0] = (mon.GetCurrentWeapon().GetDamage().m_blunt * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[1] = (mon.GetCurrentWeapon().GetDamage().m_pierce * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[2] = (mon.GetCurrentWeapon().GetDamage().m_slash * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[3] = (mon.GetCurrentWeapon().GetDamage().m_fire * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[4] = (mon.GetCurrentWeapon().GetDamage().m_frost * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[5] = (mon.GetCurrentWeapon().GetDamage().m_lightning * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[6] = (mon.GetCurrentWeapon().GetDamage().m_poison * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                ThungrStats[7] = (mon.GetCurrentWeapon().GetDamage().m_spirit * LackingImaginationGlobal.c_thungrTyrantDisdain).ToString();
                                    
                                // Success effects and message
                                __instance.Message(MessageHud.MessageType.Center, $"Enemy Dominated");
                                
                                xThungrEssence.Ability_Name = xThungrEssence.Mode2;
                            }
                        }
                        Mark = null;
                        __instance.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetStableHashCode());
                        
                        // if(xThungrEssence.AuraA != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraA);
                        // if(xThungrEssence.AuraEnemy != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraEnemy);
                    }
                }
                
                
                
                
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Thungr_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (!EssenceItemData.equipedEssence.Contains("$item_thungr_essence"))
                {
                    if (__instance.GetSEMan().HaveStatusEffect("SE_Arrogance"))
                    {
                        __instance.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetHashCode());
                        Mark.GetSEMan().RemoveStatusEffect("SE_Arrogance".GetHashCode());
                        Mark = null;
                        
                        // if(xThungrEssence.AuraA != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraA);
                        // if(xThungrEssence.AuraEnemy != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraEnemy);
                    }
                    if (__instance.GetSEMan().HaveStatusEffect("SE_Disdain"))
                    {
                        __instance.GetSEMan().RemoveStatusEffect("SE_Disdain".GetHashCode());
                        
                        // if(xThungrEssence.AuraD != null) UnityEngine.GameObject.Destroy(xThungrEssence.AuraD);
                    }
                }
                
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        public static class Thungr_GetBodyArmor_Patch
        {
            public static void Postfix(Player __instance, ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_thungr_essence"))
                {
                    __result += (-1.0f * __instance.m_equipmentMovementModifier * 100f) * LackingImaginationGlobal.c_thungrTyrantArmor;
                    __result = Math.Max(__result, 0.0f);
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}