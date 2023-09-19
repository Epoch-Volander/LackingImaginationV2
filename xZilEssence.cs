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

    public class xZilEssence
    {
        public static string Ability_Name = "";

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Zil Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                // se_cd.m_ttl = LackingImaginationUtilities.xGeirrhafaCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                
                
                



            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

    }

    
    
    [HarmonyPatch]
    public static class xZilEssencePassive
    {
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}