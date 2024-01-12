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

    public class xSeekerSoldierEssence
    {
        public static string Ability_Name = "Reverberation";
        
        
        public static void Process_Input(Player player, int position)
        {
                // LackingImaginationV2Plugin.Log($"Seeker Soldier Button was pressed");
                if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
                {
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xSeekerSoldierCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    
                    //Apply effects
                    SE_Reverberation se_reverberation = (SE_Reverberation)ScriptableObject.CreateInstance(typeof(SE_Reverberation));
                    se_reverberation.m_ttl = SE_Reverberation.m_baseTTL;
                    player.GetSEMan().AddStatusEffect(se_reverberation);

                    
                }
        }
    }

    [HarmonyPatch]
    public class xSeekerSoldierEssencePassive
    {
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        public static class SeekerSoldier_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_seeker_brute_essence"))
                {
                    __result += LackingImaginationGlobal.c_seekersoldierReverberationArmor;
                }
                if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Reverberation"))
                {
                    __result += __result;
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.AddStaggerDamage))]
        public static class SeekerSoldier_AddStaggerDamage_Patch
        {
            public static bool Prefix(ref bool __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_seeker_brute_essence"))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Character), (nameof(Character.RPC_Damage)))]
        class SeekerSoldier_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_seeker_brute_essence") && hit.GetAttacker() != null)
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    
                    if (__instance.GetSEMan().HaveStatusEffect("SE_Reverberation") && __instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        HitData hitData = new HitData();
                        hitData.m_damage.m_blunt = (__instance.GetBodyArmor() * LackingImaginationGlobal.c_seekersoldierReverberationReflection);
                        hitData.m_dir = attacker.transform.position - __instance.transform.position;
                        hitData.m_dodgeable = true;
                        hitData.m_blockable = true;
                        hitData.m_hitCollider = attacker.m_collider;
                        hitData.m_pushForce = 1f;
                        hitData.m_point = attacker.transform.position;
                        hitData.SetAttacker(__instance);
                        
                        attacker.Damage(hitData);
                        // hit vfx to make
                        
                        // UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_RavenousHunger, player.transform.position + player.transform.up * 2.2f, Quaternion.identity);
                        
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_BloodHit"), attacker.GetCenterPoint(), Quaternion.identity);
                    }
                }
            }
        }
        
        
        
    }
}