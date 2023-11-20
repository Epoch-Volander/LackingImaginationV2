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
        
        
        
        
    }
}