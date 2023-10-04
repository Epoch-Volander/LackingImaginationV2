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

    public class xUlvEssence
    {
        public static string Ability_Name = "Territorial \nSlumber";
        public static void Process_Input(Player player, int position)
        {
            //Ability Cooldown
            StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
            se_cd.m_ttl = LackingImaginationUtilities.xUlvCooldownTime;
            player.GetSEMan().AddStatusEffect(se_cd);
            
            LackingImaginationV2Plugin.Log($"Ulv Button was pressed");

            UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_TerritorialSlumber, player.transform.position + player.transform.up * 0.5f, Quaternion.identity);

            // make circles higher, coroutine while / summon ulv pal on kill in circle/ stats scale with Comfort
            
        }
    }

    [HarmonyPatch]
    public static class xUlvEssencePassive
    {
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Ulv_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ulv_essence"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_TerritorialSlumber"))
                    {
                        __instance.GetSEMan().AddStatusEffect("SE_TerritorialSlumber".GetStableHashCode());
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_TerritorialSlumber"))
                {
                    SE_TerritorialSlumber.Comfort = 0;
                    __instance.GetSEMan().RemoveStatusEffect("SE_TerritorialSlumber".GetStableHashCode());
                }
            }
        }
        [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.CalculateComfortLevel), new Type[] {typeof(Player)})]
        public static class Ulv_CalculateComfortLevel_Patch
        {
            public static void Postfix(SE_Rested __instance, ref int __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ulv_essence"))
                {
                     __result += (int)LackingImaginationGlobal.c_ulvTerritorialSlumberComfort;
                }
            }
        }
        [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.UpdateTTL))]
        public static class Ulv_UpdateTTL_Patch
        {
            public static void Prefix(SE_Rested __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ulv_essence"))
                {
                    __instance.m_TTLPerComfortLevel = 30f;
                }
                else if (__instance.m_TTLPerComfortLevel == 30f)
                {
                    __instance.m_TTLPerComfortLevel = 60f;
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Wolf_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.High)]
            public static void Postfix(ref float stamina, Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ULV_essence"))
                {
                    stamina *= (1f + ((float)SE_TerritorialSlumber.Comfort * LackingImaginationGlobal.c_ulvTerritorialSlumberStamina));
                }
            }
        }
        
        
        
        
    }
}