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

    public class xHareEssence
    {
        public static string Ability_Name = "Lucky Foot";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Hare Button was pressed");
            
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xHareCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_hare_alerted"), player.transform.position, Quaternion.identity);
                GameObject odin = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                ParticleSystem.MainModule mainModule = odin.transform.Find("smoke_expl").GetComponent<ParticleSystem>().main;
                mainModule.startColor = new Color(1f,0.941771f, 0.0f,0.4980392f);
                
                SE_LuckyFoot se_LuckyFoot = (SE_LuckyFoot)ScriptableObject.CreateInstance(typeof(SE_LuckyFoot));
                player.GetSEMan().AddStatusEffect(se_LuckyFoot);
                
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }   
        }
        


    }
    [HarmonyPatch]
    public class xHareEssencePassive
    {
        public static int canDoubleJump;
        
        [HarmonyPatch(typeof(Player), "Update", null)]
        public class Hare_AbilityInput_Postfix
        {
            public static void Postfix(Player __instance)
            {
                Player localPlayer = Player.m_localPlayer;
                if (localPlayer != null && LackingImaginationV2Plugin.playerEnabled && EssenceItemData.equipedEssence.Contains("$item_hare_essence"))
                {
                    canDoubleJump = 1;
                }
                else
                {
                    canDoubleJump = 0;
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), "GetBodyArmor")]
        public static class Hare_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_hare_essence"))
                {
                    __result *= (1f - LackingImaginationGlobal.c_hareLuckyFootArmor);
                }
            }
        }
        [HarmonyPatch(typeof(Player), "UpdateMovementModifier")]
        public static class Hare_UpdateMovementModifier_Patch
        {
            public static void Postfix(Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_hare_essence"))
                {
                    __instance.m_equipmentMovementModifier += 0.1f;
                }
            }
        }
        
        
        
        
        
        
        
        
    }

}