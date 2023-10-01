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

    public class xYagluthEssence
    {
        public static string Ability_Name = "PH";
        public static void Process_Input(Player player, int position)
        {
            //Ability Cooldown
            StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
            se_cd.m_ttl = LackingImaginationUtilities.xYagluthCooldownTime;
            player.GetSEMan().AddStatusEffect(se_cd);
            
            LackingImaginationV2Plugin.Log($"Yag Button was pressed");
            
            
            
            
        }
        


    }

    [HarmonyPatch]
    public class xYagluthEssencePassive
    {
        
        
        
        
    }
}