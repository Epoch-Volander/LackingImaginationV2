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

    public class xSeaSerpentEssence
    {
        public static string Ability_Name = "PH5";// Aoe vortex that pulls enemies in, ref fuling berserker for smooth movement
        public static void Process_Input(Player player)//a class like ROT, will select a random fish, use that fish as icon for status, if eaten, will double the range of vortex for 1 day
        {
            System.Random rnd = new System.Random();
            Vector3 pVec = default(Vector3);
            
                LackingImaginationV2Plugin.Log($"Serpent Button was pressed");
            
            
        }
        


    }


}