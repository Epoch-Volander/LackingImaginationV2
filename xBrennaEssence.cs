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
using TMPro;


namespace LackingImaginationV2
{

    public class xBrennaEssence
    {
        public static string Ability_Name = "Vulcan";

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Brenna Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                
                LackingImaginationV2Plugin.Log($"Brenna {player.transform.position}");
                
                
                
                Location location = Location.GetLocation(player.transform.position);
                if (location != null && !string.IsNullOrEmpty(location.m_discoverLabel))  
                    LackingImaginationV2Plugin.Log($"Brenna {location.m_discoverLabel}"); //$hud_pin_hildir3
                // if (player.m_shownTutorials.Contains(location.m_discoverLabel))
                //     LackingImaginationV2Plugin.Log($"Brenna tower was found"); 
                    // player.AddKnownLocationName(location.m_discoverLabel);
                    if(location != null && location.name != null)
                    {
                        LackingImaginationV2Plugin.Log($"Brenna1 {location.name}");
                    }//MountainCave02(Clone)//StoneTowerRuins05(Clone)//Hildir_cave(Clone)//Dragonqueen(Clone)//Eikthyrnir(Clone)
                
                   
                // brenna swords fall from the sky and do the Aoe, on kill spawn a brenna 

                //summon brenna sword, last for a set duration, you can use it like a sword or throw it with recast, spawns the aoe around hit spot and in random spots nearby and summons a brenna ally at the location, can only have one

                //destroy if dropped, destroy if essence removed, destroy on death// destroy on logout
                
                // broken base form, sacrifice fully upgraded krom, to unlock true version, just a stats list to say yes or no to the version of the sword summoned
                
                // synergy, add element ot the vigil spirts
                
                
                // UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.GO_VulcanSwordBroken, player.transform.position, Quaternion.identity);


            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

    }

    
    
    [HarmonyPatch]
    public static class xBrennaEssencePassive
    {
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}