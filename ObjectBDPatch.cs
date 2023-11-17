using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;


namespace LackingImaginationV2
{
    public static class ObjectBDPatch
    {

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDBAwake
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddStatusEffect(__instance);
                AddCooldownStatusEffect(__instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDBCopyOtherDB
        {
            public static void Postfix(ObjectDB __instance)
            {
                AddStatusEffect(__instance);
                AddCooldownStatusEffect(__instance);
            }
        }
        
        
         [HarmonyPatch(typeof(ObjectDB), "GetStatusEffect", new Type[] { typeof(int) })]
        public static class ObjectDBGetStatusEffect
        {
            public static void Postfix(ObjectDB __instance, int nameHash, StatusEffect __result)
            {
                if (__result != null)
                {
                    if (nameHash == "SE_HorizonHaste".GetHashCode() )
                    {
                        (__result as SE_HorizonHaste).m_icon = SE_HorizonHaste.AbilityIcon;
                    }
                    else if (nameHash == "SE_MoonlitLeap".GetHashCode())
                    {
                        (__result as SE_MoonlitLeap).m_icon = SE_MoonlitLeap.AbilityIcon;
                    }
                    else if (nameHash == "SE_RavenousHunger".GetHashCode())
                    {
                        (__result as SE_RavenousHunger).m_icon = SE_RavenousHunger.AbilityIcon;
                    }
                    else if (nameHash == "SE_Ritual".GetHashCode())
                    {
                        (__result as SE_Ritual).m_icon = SE_Ritual.AbilityIcon;
                    }
                    else if (nameHash == "SE_Relentless".GetHashCode())
                    {
                        (__result as SE_Relentless).m_icon = SE_Relentless.AbilityIcon;
                    }
                    else if (nameHash == "SE_Giantization".GetHashCode())
                    {
                        (__result as SE_Giantization).m_icon = SE_Giantization.AbilityIcon;
                    }
                    else if (nameHash == "SE_Harbinger".GetHashCode())
                    {
                        (__result as SE_Harbinger).m_icon = SE_Harbinger.AbilityIcon;
                    }
                    else if (nameHash == "SE_ThreeFreeze".GetHashCode())
                    {
                        (__result as SE_ThreeFreeze).m_icon = SE_ThreeFreeze.AbilityIcon;
                    }
                    else if (nameHash == "SE_BloodSiphon".GetHashCode())
                    {
                        (__result as SE_BloodSiphon).m_icon = SE_BloodSiphon.AbilityIcon;
                    }
                    else if (nameHash == "SE_TimedDeath".GetHashCode())
                    {
                        (__result as SE_TimedDeath).m_icon = SE_TimedDeath.AbilityIcon;
                    }
                    else if (nameHash == "SE_Bash".GetHashCode())
                    {
                        (__result as SE_Bash).m_icon = SE_Bash.AbilityIcon;
                    }
                    else if (nameHash == "SE_Longinus".GetHashCode())
                    {
                        (__result as SE_Longinus).m_icon = SE_Longinus.AbilityIcon;
                    }
                    else if (nameHash == "SE_AncientAwe".GetHashCode())
                    {
                        (__result as SE_AncientAwe).m_icon = SE_AncientAwe.AbilityIcon;
                    }
                    else if (nameHash == "SE_FumesSplit".GetHashCode())
                    {
                        (__result as SE_FumesSplit).m_icon = SE_FumesSplit.AbilityIcon;
                    }
                    else if (nameHash == "SE_Vigil".GetHashCode())
                    {
                        (__result as SE_Vigil).m_icon = SE_Vigil.AbilityIcon;
                    }
                    else if (nameHash == "SE_TwinSouls".GetHashCode())
                    {
                        (__result as SE_TwinSouls).m_icon = SE_TwinSouls.AbilityIcon;
                    }
                    else if (nameHash == "SE_Forgotten".GetHashCode())
                    {
                        (__result as SE_Forgotten).m_icon = SE_Forgotten.AbilityIcon;
                    }
                    else if (nameHash == "SE_Rot".GetHashCode())
                    {
                        (__result as SE_Rot).m_icon = SE_Rot.AbilityIcon;
                    }
                    else if (nameHash == "SE_FallenHero".GetHashCode())
                    {
                        (__result as SE_FallenHero).m_icon = SE_FallenHero.AbilityIcon;
                    }
                    else if (nameHash == "SE_Satiated".GetHashCode())
                    {
                        (__result as SE_Satiated).m_icon = SE_Satiated.AbilityIcon;
                    }
                    // else if (nameHash == "SE_Craving".GetHashCode())
                    // {
                    //     (__result as SE_Craving).m_icon = SE_Craving.AbilityIcon;
                    // }
                    else if (nameHash == "SE_BloodWell".GetHashCode())
                    {
                        (__result as SE_BloodWell).m_icon = SE_BloodWell.AbilityIcon;
                    }
                    else if (nameHash == "SE_Calm".GetHashCode())
                    {
                        (__result as SE_Calm).m_icon = SE_Calm.AbilityIcon;
                    }
                    else if (nameHash == "SE_Courage".GetHashCode())
                    {
                        (__result as SE_Courage).m_icon = SE_Courage.AbilityIcon;
                    }
                    else if (nameHash == "SE_RecklessCharge".GetHashCode())
                    {
                        (__result as SE_RecklessCharge).m_icon = SE_RecklessCharge.AbilityIcon;
                    }
                    else if (nameHash == "SE_GolemCore".GetHashCode())
                    {
                        (__result as SE_GolemCore).m_icon = SE_GolemCore.AbilityIcon;
                    }
                    else if (nameHash == "SE_Culmination".GetHashCode())
                    {
                        (__result as SE_Culmination).m_icon = SE_Culmination.AbilityIcon;
                    }
                    else if (nameHash == "SE_TerritorialSlumber".GetHashCode())
                    {
                        (__result as SE_TerritorialSlumber).m_icon = SE_TerritorialSlumber.AbilityIcon;
                    }
                    else if (nameHash == "SE_Arrogance".GetHashCode())
                    {
                        (__result as SE_Arrogance).m_icon = SE_Arrogance.AbilityIcon;
                    }
                    else if (nameHash == "SE_Disdain".GetHashCode())
                    {
                        (__result as SE_Disdain).m_icon = SE_Disdain.AbilityIcon;
                    }
                    
                    
                    
                    
                    
                    if (nameHash == "Ability1_CoolDown".GetHashCode())
                    {
                        (__result as CooldownStatusEffects_A1).m_icon = CooldownStatusEffects_A1.AbilityIcon;
                    }
                    else if (nameHash == "Ability2_CoolDown".GetHashCode())
                    {
                        (__result as CooldownStatusEffects_A2).m_icon = CooldownStatusEffects_A2.AbilityIcon;
                    }
                    else if (nameHash == "Ability3_CoolDown".GetHashCode())
                    {
                        (__result as CooldownStatusEffects_A3).m_icon = CooldownStatusEffects_A3.AbilityIcon;
                    }
                    else if (nameHash == "Ability4_CoolDown".GetHashCode())
                    {
                        (__result as CooldownStatusEffects_A4).m_icon = CooldownStatusEffects_A4.AbilityIcon;
                    }
                    else if (nameHash == "Ability5_CoolDown".GetHashCode())
                    {
                        (__result as CooldownStatusEffects_A5).m_icon = CooldownStatusEffects_A5.AbilityIcon;
                    }
                }
            }
        }
        
        
        
        
        [HarmonyPatch(typeof(Hud), "Awake")]
        public static class HudAwake
        {
            public static void Postfix(Hud __instance)
            {
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish1").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish2").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish5").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish4_cave").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish3").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish8").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish6").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish7").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish12").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish9").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish11").GetComponent<ItemDrop>().m_itemData);
                xSeaSerpentEssencePassive.allCravings.Add(ZNetScene.instance.GetPrefab("Fish10").GetComponent<ItemDrop>().m_itemData);
                    

                SE_HorizonHaste.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyDeer").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_MoonlitLeap.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyFenring").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_RavenousHunger.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyWolf").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Ritual.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGoblinShaman").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Relentless.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyDeathsquito").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Harbinger.AbilityIcon = ZNetScene.instance.GetPrefab("TrophySurtling").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Giantization.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGoblinBrute").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_ThreeFreeze.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyHatchling").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_BloodSiphon.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyLeech").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_TimedDeath.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyBonemass").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Bash.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGreydwarfBrute").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Longinus.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGoblin").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_AncientAwe.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyTheElder").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_FumesSplit.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyBlob").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Vigil.AbilityIcon = ZNetScene.instance.GetPrefab("TrophySkeleton").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_TwinSouls.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyWraith").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Forgotten.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyDraugr").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Rot.AbilityIcon = ZNetScene.instance.GetPrefab("RottenMeat").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_FallenHero.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyDraugrElite").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Satiated.AbilityIcon = ZNetScene.instance.GetPrefab("TrophySerpent").GetComponent<ItemDrop>().m_itemData.GetIcon();
                // SE_Craving.AbilityIcon = ZNetScene.instance.GetPrefab("TrophySerpent").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_BloodWell.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyTick").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Calm.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyDragonQueen").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Courage.AbilityIcon = ZNetScene.instance.GetPrefab("Torch").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_RecklessCharge.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyBoar").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_GolemCore.AbilityIcon = ZNetScene.instance.GetPrefab("Crystal").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Culmination.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGoblinKing").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_TerritorialSlumber.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyUlv").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Arrogance.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGoblinBruteBrosBrute").GetComponent<ItemDrop>().m_itemData.GetIcon();
                SE_Disdain.AbilityIcon = ZNetScene.instance.GetPrefab("TrophyGoblinBruteBrosBrute").GetComponent<ItemDrop>().m_itemData.GetIcon();
            }
        }

        
        private static void AddCooldownStatusEffect(ObjectDB odb)
        {
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "Ability1_CoolDown"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<CooldownStatusEffects_A1>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "Ability2_CoolDown"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<CooldownStatusEffects_A2>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "Ability3_CoolDown"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<CooldownStatusEffects_A3>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "Ability4_CoolDown"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<CooldownStatusEffects_A4>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "Ability5_CoolDown"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<CooldownStatusEffects_A5>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "RitualProjectileCooldown"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<RitualProjectileCooldown>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "BrennaThrow"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<BrennaThrow>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "RancidThrow"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<RancidThrow>());
            } 
            
            
        }


        
        private static void AddStatusEffect(ObjectDB odb)
        {
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_HorizonHaste"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_HorizonHaste>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_MoonlitLeap"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_MoonlitLeap>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_RavenousHunger"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_RavenousHunger>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Ritual"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Shield>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Relentless"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Relentless>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Giantization"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Giantization>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Harbinger"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Harbinger>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_ThreeFreeze"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_ThreeFreeze>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_BloodSiphon"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_BloodSiphon>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_TimedDeath"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_TimedDeath>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Bash"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Bash>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Longinus"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Longinus>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_AncientAwe"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_AncientAwe>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_FumesSplit"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_FumesSplit>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Vigil"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Vigil>());
            }
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_TwinSouls"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_TwinSouls>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Forgotten"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Forgotten>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Rot"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Rot>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_FallenHero"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_FallenHero>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Satiated"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Satiated>());
            } 
            // if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Craving"))
            // {
            //     odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Craving>());
            // } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_BloodWell"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_BloodWell>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Calm"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Calm>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Courage"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Courage>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_RecklessCharge"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_RecklessCharge>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_GolemCore"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_GolemCore>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Culmination"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Culmination>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_TerritorialSlumber"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_TerritorialSlumber>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Arrogance"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Arrogance>());
            } 
            if (!(bool)odb.m_StatusEffects.Find((StatusEffect se) => se.name == "SE_Disdain"))
            {
                odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_Disdain>());
            } 
            
            
            
            
        }
        


    }
}