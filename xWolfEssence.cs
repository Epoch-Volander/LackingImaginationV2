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

    public class xWolfEssence
    {
        public static string Ability_Name = "Ravenous Hunger";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Wolf Button was pressed");
                // status effect that lets you build up bleed (3 hit passive?) then does x% hp dmg to enemy hit and resets counter
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xWolfCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Lingering effects
                SE_RavenousHunger se_ravenoushunger = (SE_RavenousHunger)ScriptableObject.CreateInstance(typeof(SE_RavenousHunger));
                se_ravenoushunger.m_ttl = SE_RavenousHunger.m_baseTTL;

                //Apply effects
                player.GetSEMan().AddStatusEffect(se_ravenoushunger);
                
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), player.GetEyePoint(), Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), player.GetCenterPoint(), Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), player.GetHeadPoint(), Quaternion.identity);
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_wolf_haul"), player.transform.position, Quaternion.identity);
                //why is this not working?
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
    }

    [HarmonyPatch]
    public static class xWolfEssencePassive
    {
        public static List<string> WolfStats = new List<string>(){"off", "0", "off"};
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Wolf_GetTotalFoodValue_Patch
        {
            public static void Postfix(ref float stamina, Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_wolf_essence") && __instance.m_foods.Count != 3)
                {
                    if (WolfStats[0] == "off")
                    {
                        __instance.m_runStaminaDrain *= 0.5f;
                        __instance.m_sneakStaminaDrain *= 0.5f;
                        __instance.m_dodgeStaminaUsage *= 0.5f;
                        WolfStats[0] = "on";
                    }
                    
                    if (__instance.m_foods.Count == 2)
                    {
                        stamina += LackingImaginationGlobal.c_wolfRavenousHungerStaminaPassive;
                    }
                    else if (__instance.m_foods.Count == 1)
                    {
                        stamina += LackingImaginationGlobal.c_wolfRavenousHungerStaminaPassive * 2;
                    }
                    else if (__instance.m_foods.Count == 0)
                    {
                        stamina += LackingImaginationGlobal.c_wolfRavenousHungerStaminaPassive * 3;
                    }
                }
                else
                {
                    if (WolfStats[0] == "on")
                    {
                        __instance.m_runStaminaDrain *= 2f;
                        __instance.m_sneakStaminaDrain *= 2f;
                        __instance.m_dodgeStaminaUsage *= 2f; 
                        WolfStats[0] = "off";
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        public class Wolf_Jump_Patch
        {
            public static void Prefix(Character __instance) 
            {
                if (EssenceItemData.equipedEssence.Contains("$item_wolf_essence") && Player.m_localPlayer.m_foods.Count != 3)
                {
                    if (WolfStats[2] == "off")
                    {
                        __instance.m_jumpStaminaUsage *= 0.5f;
                        WolfStats[2] = "on";
                    }
                }
                else
                {
                    if (WolfStats[2] == "on")
                    {
                        __instance.m_jumpStaminaUsage *= 2f;
                        WolfStats[2] = "off";
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        class Wolf_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_wolf_essence") && hit.GetAttacker() != null)
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
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_RavenousHunger") && WolfStats[1] != "5")
                        {
                            bleedOut(ref __instance, ref attacker, hit.m_point);
                        }
                        if (Player.m_localPlayer.m_foods.Count == 3)
                        {
                            hit.ApplyModifier((0.75f));
                        }
                        if (Player.m_localPlayer.m_foods.Count == 2)
                        {
                            hit.ApplyModifier((1.25f * LackingImaginationGlobal.c_wolfRavenousHungerPassive));
                        }
                        if (Player.m_localPlayer.m_foods.Count == 1)
                        {
                            hit.ApplyModifier((1.5f * LackingImaginationGlobal.c_wolfRavenousHungerPassive));
                        }
                        if (Player.m_localPlayer.m_foods.Count == 0)
                        {
                            hit.ApplyModifier((2f * LackingImaginationGlobal.c_wolfRavenousHungerPassive));
                        }
                        
                    }
                    void bleedOut(ref Character enemy,ref Character wolf, Vector3 point)
                    {
                        int BloodHits = int.Parse(WolfStats[1]);
                        BloodHits++;
                        WolfStats[1] = BloodHits.ToString();
                        if (BloodHits == 5)
                        {
                            BloodHits = 0;
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), enemy.GetEyePoint(), Quaternion.identity);
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), enemy.GetCenterPoint(), Quaternion.identity);
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), enemy.GetHeadPoint(), Quaternion.identity);
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_hurt"), enemy.transform.position, Quaternion.identity);
                            HitData hitData = new HitData();
                            hitData.m_damage.m_slash = (enemy.GetMaxHealth() * LackingImaginationGlobal.c_wolfRavenousHunger);
                            hitData.m_point = point;
                            hitData.SetAttacker(wolf);
                            enemy.Damage(hitData);
                        }
                        WolfStats[1] = BloodHits.ToString();
                    }
                }
            }
        }
    }
}