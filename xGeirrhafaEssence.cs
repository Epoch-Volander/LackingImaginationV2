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

    public class xGeirrhafaEssence
    {
        public static string Ability_Name = "Ice Age";

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Geirrhafa Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                // se_cd.m_ttl = LackingImaginationUtilities.xGeirrhafaCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                
                
                // //caverock_ice_stalagtite_falling  have these droop on enemies and a few random spots or maybe fall in groups of three



            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

    }

    
    
    [HarmonyPatch]
    public static class xGeirrhafaEssencePassive
    {
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}