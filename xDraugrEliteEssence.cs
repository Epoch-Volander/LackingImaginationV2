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

    public class xDraugrEliteEssence
    {
        public static string Ability_Name = "Fallen Hero";
        public static void Process_Input(Player player)
        {
            System.Random rnd = new System.Random();
            Vector3 pVec = default(Vector3);
            
                LackingImaginationV2Plugin.Log($"xDraugrEliteEssence Button was pressed");
            
            
        }
        


    }


}