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
using Object = System.Object;


namespace LackingImaginationV2
{
    public class xDeerEssence
    {
        public static string Ability_Name = "Horizon Haste";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {

                LackingImaginationV2Plugin.Log($"Deer Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xDeerCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_deer_death"), player.transform.position, Quaternion.identity);
                GameObject odin = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                ParticleSystem.MainModule mainModule = odin.transform.Find("smoke_expl").GetComponent<ParticleSystem>().main;
                mainModule.startColor = new Color(1f,0.490099f, 0.0f,0.4980392f);

                //Lingering effects
                SE_HorizonHaste se_horizonhaste = (SE_HorizonHaste)ScriptableObject.CreateInstance(typeof(SE_HorizonHaste));
                se_horizonhaste.m_ttl = SE_HorizonHaste.m_baseTTL;

                //Apply effects
                player.GetSEMan().AddStatusEffect(se_horizonhaste);
                
                List<Character> allCharacters = new List<Character>();
                allCharacters.Clear();
                Character.GetCharactersInRange(player.GetCenterPoint(), 500f, allCharacters);
                foreach (Character ch in allCharacters)
                {
                    if (ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI && ch.name == "Deer(Clone)")
                    {
                        AnimalAI ai = ch.GetBaseAI() as AnimalAI;
                        if (ai != null )
                        {
                            Traverse.Create(root: ai).Field("m_alerted").SetValue(false);
                            Traverse.Create(root: ai).Field("m_target").SetValue(null);
                            Traverse.Create(root: ai).Field("m_inDangerTimer").SetValue(0f);
                            Traverse.Create(root: ai).Field("m_updateTargetTimer").SetValue(se_horizonhaste.m_ttl);
                        }
                    }
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
    }


    [HarmonyPatch]
    public static class xDeerEssencePassive
    {
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        public static class Deer_GetTotalFoodValue_Patch
        {
            public static void Postfix(ref float stamina, ref float hp)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_deer_essence"))
                {
                    stamina += LackingImaginationGlobal.c_deerHorizonHastePassive;
                    hp *= 0.9f;
                }
            }
        }
        [HarmonyPatch(typeof(Player), "UpdateMovementModifier")]
        public static class Deer_UpdateMovementModifier_Patch
        {
            public static void Postfix(Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_deer_essence"))
                {
                    __instance.m_equipmentMovementModifier += 0.05f;
                }
            }
        }
    }

    
    
    
    
}