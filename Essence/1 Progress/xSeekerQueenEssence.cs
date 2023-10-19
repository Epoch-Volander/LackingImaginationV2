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

    public class xSeekerQueenEssence
    {
        public static string Ability_Name = "PH";
        public static void Process_Input(Player player, int position)
        {
            // xBrennaEssencePassive.BrennaStats[0] = "false";
            // xRancidRemainsEssencePassive.RancidRemainsStats[0] = "false";
            //
                LackingImaginationV2Plugin.Log($"SeekerQueen Button was pressed");
            
                // Location location = Location.GetLocation(Player.m_localPlayer.transform.position);
                // LackingImaginationV2Plugin.Log($"{location.ToString()}");
                
                
                
        }
        


    }


}