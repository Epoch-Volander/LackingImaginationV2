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

    public class xDraugrEssence
    {
        public static string Ability_Name = "Forgotten";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"xDraugrEssence Button was pressed");
            
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xDraugrCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                
                
                
                
                
                
                
                
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        


    }

    [HarmonyPatch]
    public class xDraugrEssencePassive
    {
        
        public static float DraugrRot = LackingImaginationGlobal.c_draugrForgottenRot;
        
        public static List<string> DraugrStats = new List<string>(){"off"};

        [HarmonyPatch(typeof(Player), "UpdateEnvStatusEffects")]
        public static class Draugr_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence") && xDraugrRot.RotStats[0] == "100")
                {
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(EssenceItemData.equipedEssence.IndexOf("$item_draugr_essence"));
                    if (!__instance.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugr_essence"))))
                    {
                        __instance.GetSEMan().AddStatusEffect(se_cd);
                    }
                }
                else if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence") && xDraugrRot.RotStats[0] != "100")
                {
                    if (__instance.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugr_essence")))
                        && __instance.GetSEMan().GetStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugr_essence")).GetStableHashCode()).m_ttl == 0.0f)
                    {
                        __instance.GetSEMan().RemoveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugr_essence")).GetStableHashCode());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        class Draugr_GetTotalFoodValue_Patch
        {
            public static void Postfix(Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence"))
                {
                    if (DraugrStats[0] == "off")
                    {
                        __instance.m_maxCarryWeight += LackingImaginationGlobal.c_draugrForgottenPassiveCarry;
                        DraugrStats[0] = "on";
                    }
                }
                else if (DraugrStats[0] == "on" && !EssenceItemData.equipedEssence.Contains("$item_draugr_essence"))
                {
                    __instance.m_maxCarryWeight -= LackingImaginationGlobal.c_draugrForgottenPassiveCarry;
                    DraugrStats[0] = "off";
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        

    }
}