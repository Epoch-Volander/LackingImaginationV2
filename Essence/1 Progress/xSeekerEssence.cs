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

    public class xSeekerEssence
    {
        public static string Ability_Name = "PH";//Become invisible and ride the seeker
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {

                // LackingImaginationV2Plugin.Log($"Seeker Button was pressed");
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("SeekerPlayer"), player.transform.position, Quaternion.identity);
                
                
                
            }
        }
        


    }

    [HarmonyPatch]
    public class xSeekerEssencePassive
    {
        
        
        
        
        
    }
}