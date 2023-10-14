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
        public static string Ability_Name = "Fallen \nHero";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                
                if (float.Parse(xDraugrRot.RotStats[0]) <= 94f)
                {
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xDraugrEliteCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                
                    xDraugrRot.RotStats[0] = (float.Parse(xDraugrRot.RotStats[0]) + 5f).ToString();
                
                    //Effects, animations, and sounds
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_draugr_alerted"), player.transform.position, Quaternion.identity);
                    GameObject activateEffect =UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_DraugrSpawn"), player.transform.position + player.transform.up * 1.5f, Quaternion.identity);
                    ParticleSystem.MainModule mainModule = activateEffect.transform.Find("wetsplsh").GetComponent<ParticleSystem>().main;
                    mainModule.startColor = Color.blue + Color.red;

                    SE_FallenHero se_fallenhero = (SE_FallenHero)ScriptableObject.CreateInstance(typeof(SE_FallenHero));
                    se_fallenhero.m_ttl = SE_FallenHero.m_baseTTL;
                
                    player.GetSEMan().AddStatusEffect(se_fallenhero);
                    
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Will Break This One's Limit");
                }
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // } 
        }
        


    }

    [HarmonyPatch]
    public class xDraugrEliteEssencePassive
    {

        public static float DraugrEliteRot = LackingImaginationGlobal.c_draugreliteFallenHeroRot;

        public static List<string> DraugrEliteStats = new List<string>(){"off"};

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class DraugrElite_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence") && xDraugrRot.RotStats[0] == "100")
                {
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(EssenceItemData.equipedEssence.IndexOf("$item_draugrelite_essence"));
                    if (!__instance.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugrelite_essence"))))
                    {
                        __instance.GetSEMan().AddStatusEffect(se_cd);
                    }
                }
                else if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence") && xDraugrRot.RotStats[0] != "100")
                {
                    if (__instance.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugrelite_essence")))
                        && __instance.GetSEMan().GetStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugrelite_essence")).GetStableHashCode()).m_ttl == 0.0f)
                    {
                        __instance.GetSEMan().RemoveStatusEffect(LackingImaginationUtilities.CooldownString(EssenceItemData.equipedEssence.IndexOf("$item_draugrelite_essence")).GetStableHashCode());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        class DraugrElite_GetTotalFoodValue_Patch
        {
            public static void Postfix(Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence"))
                {
                    if (DraugrEliteStats[0] == "off")
                    {
                        __instance.m_maxCarryWeight += LackingImaginationGlobal.c_draugreliteFallenHeroPassiveCarry;
                        DraugrEliteStats[0] = "on";
                    }
                }
                else if (DraugrEliteStats[0] == "on" && !EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence"))
                {
                    __instance.m_maxCarryWeight -= LackingImaginationGlobal.c_draugreliteFallenHeroPassiveCarry;
                    DraugrEliteStats[0] = "off";
                }
            }
        }

    }
}