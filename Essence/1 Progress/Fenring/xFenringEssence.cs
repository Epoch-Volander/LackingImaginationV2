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

    public static class xFenringEssence // to do over, has leap animation to add
    {
        public static string Ability_Name = "Moonlit \nLeap";
        public static void Process_Input(Player player, int position)
        {
           
            // if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            // {
            //     LackingImaginationV2Plugin.Log($"xFenringEssence Button was pressed");
            //
            //     //Ability Cooldown
            //     StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
            //     se_cd.m_ttl = LackingImaginationUtilities.xFenringCooldownTime;
            //     player.GetSEMan().AddStatusEffect(se_cd);
            //     
            //     //Effects, animations, and sounds
            //     UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
            //     UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_fenring_death"), player.transform.position, Quaternion.identity);
            //
            //     //Lingering effects
            //     SE_MoonlitLeap se_moonlitleap = (SE_MoonlitLeap)ScriptableObject.CreateInstance(typeof(SE_MoonlitLeap));
            //     se_moonlitleap.m_ttl = SE_HorizonHaste.m_baseTTL;
            //
            //     //Apply effects
            //     player.GetSEMan().AddStatusEffect(se_moonlitleap);
            //     
            // }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }    
            
        }

    }

    [HarmonyPatch]
    public static class xFenringEssencePassive
    {
        
        // [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        // public class Fenring_Jump_Patch
        // {
        //     public static void Postfix(Character __instance) 
        //     {
        //         if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_MoonlitLeap"))
        //         {
        //             __instance.m_jumpForceForward = 20f;
        //         }
        //         else
        //         {
        //             __instance.m_jumpForceForward = 2f;
        //         }
        //     }
        // }
        //
        // [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        // class Fenring_RPC_Damage_Patch
        // {
        //     static void Prefix(Character __instance, ref HitData hit)
        //     {
        //         if (EssenceItemData.equipedEssence.Contains("$item_fenring_essence") && hit.GetAttacker() != null)
        //         {
        //             if (__instance.IsDebugFlying())
        //                 return;
        //             if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
        //             {
        //                 __instance.m_localPlayerHasHit = true;
        //             }
        //             if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
        //                 return;
        //             Character attacker = hit.GetAttacker();
        //             if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
        //                 return;
        //             if ((UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
        //             {
        //                 if (Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Unarmed)
        //                 {
        //                     UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_fenring_claw_hit"), Player.m_localPlayer.transform.position, Quaternion.identity);
        //                     
        //                     hit.m_damage.m_slash += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_fenringMoonlitLeapPassive;
        //                 }
        //             }
        //             if (__instance != null && __instance.IsPlayer() && !EnvMan.instance.IsNight())
        //             {
        //                 LackingImaginationV2Plugin.Log($"fenring you have been hit");
        //                 __instance.m_damageModifiers.m_fire = HitData.DamageModifier.Weak;
        //             }
        //         }
        //     }
        // }
        //
        
        
        
        
        
        
        
        
        
        
    }



}