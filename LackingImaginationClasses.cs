using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using LocalizationManager;
using LocationManager;
using PieceManager;
using ServerSync;
using SkillManager;
using StatusEffectManager;
using UnityEngine;
using PrefabManager = ItemManager.PrefabManager;
using System.Collections.Generic;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using ItemDataManager;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.Animations;



namespace LackingImaginationV2
{
    public class LackingImaginationClasses
    {
         public static Dictionary<Heightmap.Biome, List<string>> biomeDictionary = new Dictionary<Heightmap.Biome, List<string>>
        {
            { Heightmap.Biome.Meadows, new List<string> {"Meadows_Exp", "1"} },
            { Heightmap.Biome.BlackForest, new List<string> {"BlackForest_Exp", "2"} },
            { Heightmap.Biome.Swamp, new List<string> {"Swamp_Exp", "3"} },
            { Heightmap.Biome.Ocean, new List<string> {"Ocean_Exp", "4"} },
            { Heightmap.Biome.Mountain, new List<string> {"Mountain_Exp", "4"} },
            { Heightmap.Biome.Plains, new List<string> {"Plains_Exp", "5"} },
            { Heightmap.Biome.Mistlands, new List<string> {"Mistlands_Exp", "6"} },
            { Heightmap.Biome.AshLands, new List<string> {"AshLands_Exp", "7"} },
            { Heightmap.Biome.DeepNorth, new List<string> {"DeepNorth_Exp", "8"} }
        };
        
        public static Dictionary<string, List<string>> locationDictionary = new Dictionary<string,List<string>>
        {
            // dungeaonx2
            { "Crypt2(Clone)",  new List<string> {"BurialChambers_Exp", "4"} },
            { "Crypt3(Clone)", new List<string> {"BurialChambers_Exp", "4"} },
            { "Crypt4(Clone)", new List<string> {"BurialChambers_Exp", "4"} },
            { "TrollCave02(Clone)", new List<string> {"TrollCave_Exp", "4"} },
            { "SunkenCrypt4(Clone)", new List<string> {"SunkenCrypt_Exp", "6"} },
            { "MountainCave02(Clone)", new List<string> {"FrostCave_Exp", "8"} },
            { "GoblinCamp2(Clone)", new List<string> {"GoblinCamp_Exp", "10"} },
            { "Mistlands_DvergrTownEntrance1(Clone)", new List<string> {"InfectedMine_Exp", "12"} },
            { "Mistlands_DvergrTownEntrance2(Clone)", new List<string> {"InfectedMine_Exp", "12"} },
            //Vendors x2
            { "Vendor_BlackForest(Clone)", new List<string> {"Haldor_Exp", "4"} },
            { "Hildir_camp(Clone)", new List<string> {"Hildir_Exp", "4"} },
            //special x1
            { "InfestedTree01(Clone)", new List<string> {"InfestedTree_Exp", "3"} }, //swamp
            { "DrakeNest01(Clone)", new List<string> {"DrakeNest_Exp", "4"} }, // mountain
            { "TarPit1(Clone)", new List<string> {"TarPit_Exp", "5"} }, // plains
            { "TarPit2(Clone)", new List<string> {"TarPit_Exp", "5"} }, // plains
            { "TarPit3(Clone)", new List<string> {"TarPit_Exp", "5"} }, // plains
            { "Mistlands_Harbour1(Clone)", new List<string> {"DvergrHarbour_Exp", "6"} }, // mist
            // Runestones x1
            { "Runestone_Meadows(Clone)",  new List<string> {"MeadowRune_Exp", "1"} }, //meadow
            { "Runestone_Boars(Clone)", new List<string> {"BoarRune_Exp", "1"} }, //meadow
            { "Runestone_BlackForest(Clone)", new List<string> {"BlackForestRune_Exp", "2"} }, //black
            { "Runestone_Greydwarfs(Clone)", new List<string> {"GreydwarfRune_Exp", "2"} }, //black
            { "Runestone_Swamps(Clone)", new List<string> {"SwampRune_Exp", "3"} }, //swamp
            { "Runestone_Draugr(Clone)", new List<string> {"DraugrRune_Exp", "3"} }, //swamp
            { "Runestone_Mountains(Clone)", new List<string> {"MountainRune_Exp", "4"} }, // mountain
            { "DrakeLorestone(Clone)", new List<string> {"DrakeRune_Exp", "4"} }, // mountain
            { "Runestone_Plains(Clone)", new List<string> {"PlainsRune_Exp", "5"} }, // plains
            { "Runestone_Mistlands(Clone)",  new List<string> {"MistRune_Exp", "6"} }, // mist
            //mini boss locationsx3
            { "Hildir_crypt(Clone)", new List<string> {"ForestCryptHildir_Exp", "6"} },
            { "Hildir_cave(Clone)", new List<string> {"CaveHildir_Exp", "12"} },
            { "Hildir_plainsfortress(Clone)", new List<string> {"PlainsFortHildir_Exp", "15"} },
            { "Mistlands_GuardTower1_new(Clone)", new List<string> {"DvergrTower_Exp", "18"} },
            { "Mistlands_GuardTower2_new(Clone)", new List<string> {"DvergrTower_Exp", "18"} },
            { "Mistlands_GuardTower3_new(Clone)", new List<string> {"DvergrTower_Exp", "18"} },
            { "Mistlands_Excavation1(Clone)", new List<string> {"DvergrExcavation_Exp", "18"} },
            { "Mistlands_Excavation2(Clone)", new List<string> {"DvergrExcavation_Exp", "18"} },
            { "Mistlands_Excavation3(Clone)", new List<string> {"DvergrExcavation_Exp", "18"} },
            //Altar x3
            { "Eikthyrnir(Clone)",  new List<string> {"EikthyrSacrifice_Exp", "3"} },
            { "GDKing(Clone)", new List<string> {"TheElderSacrifice_Exp", "6"} },
            { "Bonemass(Clone)", new List<string> {"BoneMassSacrifice_Exp", "9"} },
            { "Dragonqueen(Clone)", new List<string> {"ModerSacrifice_Exp", "12"} },
            { "GoblinKing(Clone)", new List<string> {"YagluthSacrifice_Exp", "15"} },
            { "Mistlands_DvergrBossEntrance1(Clone)", new List<string> {"SeekerQueenSeal_Exp", "18"} },
        };
        
        public static Dictionary<string, List<string>> trophyDictionary = new Dictionary<string, List<string>>
        {
            //  item name                                 tutorial name, level gain
            //BOSS x5
            { "$item_trophy_eikthyr", new List<string> { "Eikthyr_Exp", "5" } },
            { "$item_trophy_elder", new List<string> { "TheElder_Exp", "10" } },
            { "$item_trophy_bonemass", new List<string> { "BoneMass_Exp", "15" } },
            { "$item_trophy_dragonqueen", new List<string> { "Moder_Exp", "20" } },
            { "$item_trophy_goblinking", new List<string> { "Yagluth_Exp", "25" } },
            { "$item_trophy_seekerqueen", new List<string> { "SeekerQueen_Exp", "30" } },
            //Meadow
            { "$item_trophy_boar", new List<string> { "Boar_Exp", "1" } }, 
            { "$item_trophy_deer", new List<string> { "Deer_Exp", "1" } },
            { "$item_trophy_greydwarf", new List<string> { "Greydwarf_Exp", "1" } },
            { "$item_trophy_neck", new List<string> { "Neck_Exp", "1" } },
            //Black
            { "$item_trophy_greydwarfbrute", new List<string> { "GreydwarfBrute_Exp", "2" } },
            { "$item_trophy_greydwarfshaman", new List<string> { "GreydwarfShaman_Exp", "2" } },
            { "$item_trophy_skeleton", new List<string> { "Skeleton_Exp", "2" } },
            //Area Black x2
            { "$item_trophy_troll", new List<string> { "Troll_Exp", "4" } },
            { "$item_trophy_skeletonpoison", new List<string> { "RancidRemains_Exp", "4" } },
            //Swamp
            { "$item_trophy_blob", new List<string> { "Blob_Exp", "3" } },
            { "$item_trophy_draugrelite", new List<string> { "DraugrElite_Exp", "3" } },
            { "$item_trophy_draugr", new List<string> { "Draugr_Exp", "3" } },
            { "$item_trophy_leech", new List<string> { "Leech_Exp", "3" } },
            { "$item_trophy_surtling", new List<string> { "Surtling_Exp", "3" } },
            //Area Swamp x2
            { "$item_trophy_abomination", new List<string> { "Abomination_Exp", "6" } },
            { "$item_trophy_wraith", new List<string> { "Wraith_Exp", "6" } },
            //Mountain
            { "$item_trophy_fenring", new List<string> { "Fenring_Exp", "4" } },
            { "$item_trophy_hatchling", new List<string> { "Drake_Exp", "4" } },
            { "$item_trophy_ulv", new List<string> { "Ulv_Exp", "4" } },
            { "$item_trophy_wolf", new List<string> { "Wolf_Exp", "4" } },
            //Area Mountain x2
            { "$item_trophy_sgolem", new List<string> { "StoneGolem_Exp", "8" } },
            { "$item_trophy_cultist", new List<string> { "Cultist_Exp", "8" } },
            //Ocean x2
            { "$item_trophy_serpent", new List<string> { "Serpent_Exp", "8" } },
            //Plains
            { "$item_trophy_deathsquito", new List<string> { "Deathsquito_Exp", "5" } },
            { "$item_trophy_goblin", new List<string> { "Fuling_Exp", "5" } },
            { "$item_trophy_growth", new List<string> { "Growth_Exp", "5" } },
            { "$item_trophy_lox", new List<string> { "Lox_Exp", "5" } },
            //Area Plains x2
            { "$item_trophy_goblinbrute", new List<string> { "FulingBerserker_Exp", "10" } }, 
            { "$item_trophy_goblinshaman", new List<string> { "FulingShaman_Exp", "10" } },
            //Mist
            { "$item_trophy_gjall", new List<string> { "Gjall_Exp", "6" } },
            { "$item_trophy_hare", new List<string> { "Hare_Exp", "6" } },
            { "$item_trophy_seeker", new List<string> { "Seeker_Exp", "6" } },
            { "$item_trophy_tick", new List<string> { "Tick_Exp", "6" } },
            //Area Mist x2
            { "$item_trophy_dvergr", new List<string> { "Dvergr_Exp", "12" } },
            { "$item_trophy_seeker_brute", new List<string> { "SeekerSoldier_Exp", "12" } },
            //Mini Boss x4
            { "$item_trophy_cultist_hildir", new List<string> { "Geirrhafa_Exp", "16" } },
            { "$item_trophy_skeleton_hildir", new List<string> { "Brenna_Exp", "8" } },
            { "$item_trophy_shamanbro", new List<string> { "Zil_Exp", "20" } },
            { "$item_trophy_brutebro", new List<string> { "Thungr_Exp", "20" } },
        };
        
         public static bool TakeInput(Player p)
        {
            bool result = (!(bool)Chat.instance || !Chat.instance.HasFocus()) && !Console.IsVisible() && !TextInput.IsVisible() && !StoreGui.IsVisible() && !InventoryGui.IsVisible() && !Menu.IsVisible() && (!(bool)TextViewer.instance || !TextViewer.instance.IsVisible()) && !Minimap.IsOpen() && !GameCamera.InFreeFly();
            if (p.IsDead() || p.InCutscene() || p.IsTeleporting())
            {
                result = false;
            }
            return result;
        }


        private static int jumpCount = 0;

        [HarmonyPatch(typeof(Player), (nameof(Player.Update)), null)]
        public class AbilityInput_Postfix
        {
            public static bool Prefix(Player __instance, ref float ___m_maxAirAltitude, ref Rigidbody ___m_body, ref float ___m_lastGroundTouch /*,ref Animator ___m_animator, float ___m_waterLevel*/)
            {
                
                Player localPlayer = Player.m_localPlayer;
                if (localPlayer != null && LackingImaginationV2Plugin.playerEnabled)
                {
                    if (TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability1_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(0, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability2_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(1, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability3_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(2, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability4_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(3, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability5_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(4, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    
                    int maxJumps = xBlobEssencePassive.canDoubleJump + xGrowthEssencePassive.canDoubleJump + xHareEssencePassive.canDoubleJump;

                    if (__instance.GetSEMan().HaveStatusEffect("SE_LuckyFoot")) maxJumps *= 2;
                    
                    if (ZInput.GetButtonDown("Jump") && !__instance.IsDead() && !__instance.InAttack() && !__instance.IsEncumbered() 
                        && !__instance.InDodge() && !__instance.IsKnockedBack())
                    {
                        // LackingImaginationV2Plugin.Log($"x {maxJumps}");
                        if (!__instance.IsOnGround() && jumpCount < maxJumps) // Check if there are remaining jumps
                        {
                            Vector3 velVec = __instance.GetVelocity();
                            velVec.y = 0f;                    
                            ___m_body.velocity = (velVec * 2f) + new Vector3(0, 10f, 0f); // You can adjust the jump velocity as needed
                            jumpCount++;
                            ___m_maxAirAltitude = 0;
                            ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance)).SetTrigger("jump");
                        }
                        else if (__instance.IsOnGround())
                        {
                            jumpCount = 0; // Reset jump count when landing
                        }
                    }
                }
                return true;
            }
        }
        

        
        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStatusEffects))]
        public static class Skill_Icon_Patch
        {
            public static void Postfix(Hud __instance)
            {
                if(__instance != null && LackingImaginationV2Plugin.showAbilityIcons.Value)
                {
                    if(LackingImaginationV2Plugin.abilitiesStatus == null)
                    { 
                        Debug.Log("== null plug");
                        LackingImaginationV2Plugin.abilitiesStatus = new List<RectTransform>();
                        LackingImaginationV2Plugin.abilitiesStatus.Clear(); 
                        for (int i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; i++) 
                        {
                            LackingImaginationV2Plugin.abilitiesStatus.Add(null);
                        }
                    }
                    if( Player.m_localPlayer != null && Player.m_localPlayer.GetEssenceSlotInventory() != null)
                    {
                        EssenceItemData.equipedEssence = EssenceItemData.GetEquippedEssence();
                        EssenceItemData.equipedEssenceData = EssenceItemData.GetEquippedEssenceData();
                        LackingImaginationUtilities.NameEssence();
                        LackingImaginationUtilities.InitiateEssenceStatus(Hud.m_instance);
                    }
                    if (InventoryGui.IsVisible())
                    {
                        for (int i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; i++)
                        {
                            if (EssenceItemData.equipedEssence[i] != null)
                            {
                                if(LackingImaginationV2Plugin.abilitiesStatus[i] != null)
                                {
                                    UnityEngine.Object.Destroy(LackingImaginationV2Plugin.abilitiesStatus[i].gameObject);
                                }
                            }
                        }
                    }
                    if (LackingImaginationV2Plugin.abilitiesStatus != null && !InventoryGui.IsVisible())
                    {
                        for (int j = 0; j < LackingImaginationV2Plugin.abilitiesStatus.Count; j++)
                        {
                            if (LackingImaginationV2Plugin.abilitiesStatus[j] != null)
                            {
                                RectTransform rectTransform2 = LackingImaginationV2Plugin.abilitiesStatus[j];
                                Image component = rectTransform2.Find("Icon").GetComponent<Image>();
                                string iconText = "";
                                if (j == 0)
                                {
                                    component.sprite = LackingImaginationV2Plugin.AbilitySprites[0];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability1_CoolDown"))
                                    {
                                        component.color = LackingImaginationV2Plugin.abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability1_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToString();
                                        if (LackingImaginationV2Plugin.Ability1_Combokey.Value != KeyCode.None)
                                        {
                                            iconText += " + " + LackingImaginationV2Plugin.Ability1_Combokey.Value.ToString();
                                        }
                                    }
                                }
                                else if (j == 1)
                                {

                                    component.sprite = LackingImaginationV2Plugin.AbilitySprites[1];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability2_CoolDown"))
                                    {
                                        component.color = LackingImaginationV2Plugin.abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability2_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToString();
                                        if (LackingImaginationV2Plugin.Ability2_Combokey.Value != KeyCode.None)
                                        {
                                            iconText += " + " + LackingImaginationV2Plugin.Ability2_Combokey.Value.ToString();
                                        }
                                    }
                                }
                                else if (j == 2)
                                {
                                    component.sprite = LackingImaginationV2Plugin.AbilitySprites[2];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability3_CoolDown"))
                                    {
                                        component.color = LackingImaginationV2Plugin.abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability3_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToString();
                                        if (LackingImaginationV2Plugin.Ability3_Combokey.Value != KeyCode.None)
                                        {
                                            iconText += " + " + LackingImaginationV2Plugin.Ability3_Combokey.Value.ToString();
                                        }
                                    }
                                }
                                else if (j == 3)
                                {
                                    component.sprite = LackingImaginationV2Plugin.AbilitySprites[3];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability4_CoolDown"))
                                    {
                                        component.color = LackingImaginationV2Plugin.abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability4_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToString();
                                        if (LackingImaginationV2Plugin.Ability4_Combokey.Value != KeyCode.None)
                                        {
                                            iconText += " + " + LackingImaginationV2Plugin.Ability4_Combokey.Value.ToString();
                                        }
                                    }
                                }
                                else if (j == 4)
                                {
                                    component.sprite = LackingImaginationV2Plugin.AbilitySprites[4];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability5_CoolDown"))
                                    {
                                        component.color = LackingImaginationV2Plugin.abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability5_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToString();
                                        if (LackingImaginationV2Plugin.Ability5_Combokey.Value != KeyCode.None)
                                        {
                                            iconText += " + " + LackingImaginationV2Plugin.Ability5_Combokey.Value.ToString();
                                        }
                                    }
                                }
                                //rectTransform2.GetComponentInChildren<Text>().text = Localization.instance.Localize((Ability1.Name).ToString());
                                TMP_Text component2 = rectTransform2.Find("TimeText").GetComponent<TMP_Text>();
                                if (!string.IsNullOrEmpty(iconText))
                                {
                                    component2.gameObject.SetActive(value: true);
                                    component2.text = iconText;
                                }
                                else
                                {
                                    component2.gameObject.SetActive(value: false);
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        public static class AbilityInput_Prefix
        {
            public static bool Prefix(Player __instance)
            {
                if (ZInput.GetButtonDown("GP") || ZInput.GetButtonDown("JoyGP"))
                {
                    LackingImaginationV2Plugin.UseGuardianPower = true;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.ActivateGuardianPower), null)]
        public class ActivatePowerPrevention_Patch
        {
            public static bool Prefix(Player __instance, ref bool __result)
            {                
                if (!LackingImaginationV2Plugin.UseGuardianPower)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.StartGuardianPower), null)]
        public class StartPowerPrevention_Patch
        {
            public static bool Prefix(Player __instance, ref bool __result)
            {
                if (!LackingImaginationV2Plugin.UseGuardianPower)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }



        
        
        
        // public enum Theme
        // {
        //     None = 0,
        //     Crypt = 1,
        //     SunkenCrypt = 2,
        //     Cave = 4,
        //     ForestCrypt = 8,
        //     GoblinCamp = 16, // 0x00000010
        //     MeadowsVillage = 32, // 0x00000020
        //     MeadowsFarm = 64, // 0x00000040
        //     DvergerTown = 128, // 0x00000080
        //     DvergerBoss = 256, // 0x00000100
        //     ForestCryptHildir = 512, // 0x00000200
        //     CaveHildir = 1024, // 0x00000400
        //     PlainsFortHildir = 2048, // 0x00000800
        // }
        
        
        
        
        // public static Location GetLocation(Vector3 point, bool checkDungeons = true)
        // {
        //     if (Character.InInterior(point))
        //         return Location.GetZoneLocation(point);
        //     foreach (Location allLocation in Location.m_allLocations)
        //     {
        //         if (allLocation.IsInside(point, 0.0f))
        //             return allLocation;
        //     }
        //     return (Location) null;
        // }
        
        
        
        
        
       
        
        //Trophy Exp
        [HarmonyPatch(typeof(Player), nameof(Player.OnInventoryChanged))]
        public static class OnInventoryChangedLackingImagination
        {
            public static void Postfix(Player __instance)
            {
                if (__instance.m_isLoading)
                    return;
                foreach (ItemDrop.ItemData allItem in __instance.m_inventory.GetAllItems())
                {
                    __instance.AddKnownItem(allItem);

                    foreach (KeyValuePair<string, List<string>> kvp in trophyDictionary)
                    {
                        ExpMethods.TrophyExpMethod(kvp.Value, kvp.Key, allItem.m_shared.m_name);
                    }
                }
                __instance.UpdateKnownRecipesList();
                __instance.UpdateAvailablePiecesList();
            }
        }
        
        // biomeDictionary //locationDictionary
        //Biome Exp //All exp now
        private static float m_biomeTimer;
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateBiome))]
        public class BiomeExp
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (__instance.InIntro())
                    return;
                if ((double) m_biomeTimer == 0.0)
                {
                    Location location = Location.GetLocation(__instance.transform.position);
                    if(location != null && !string.IsNullOrEmpty(location.name))
                    {
                        if (locationDictionary.ContainsKey(location.name))
                        {
                            ExpMethods.DungeonExpMethod(locationDictionary[location.name]);
                        }
                    }
                }
                m_biomeTimer += dt;
                if ((double) m_biomeTimer <= 1.0)
                    return;
                m_biomeTimer = 0.0f;
                foreach (KeyValuePair<Heightmap.Biome, List<string>> kvp in biomeDictionary)
                {
                    ExpMethods.BiomeExpMethod(kvp.Key, kvp.Value);
                    // Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
        }

        [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.ShowText), new Type[] { typeof(TextsDialog.TextInfo)})]
        public class TextsDialog_Header_Fix
        {
            public static void Postfix(TextsDialog __instance, ref TextsDialog.TextInfo text)
            {
                string info = text.m_topic;
                if (Player.m_localPlayer.m_knownTexts.ContainsKey(info))
                {
                    Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_label == info));
                    if (tutorialText != null)
                    {
                        if(tutorialText.m_topic != "") __instance.m_textAreaTopic.text = Localization.instance.Localize(tutorialText.m_topic);
                    }
                }
            }
        }
        
        
      
    }
}