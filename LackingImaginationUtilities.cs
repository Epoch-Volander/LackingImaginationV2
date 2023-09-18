using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using System.IO;
using UnityEngine.UI;



namespace LackingImaginationV2
{

    public static class LackingImaginationUtilities
    {
        
        // private static int m_interactMask = LayerMask.GetMask("item", "piece", "piece_nonsolid", "Default", "static_solid", "Default_small", "character", "character_net", "terrain", "vehicle");
        // private static int m_LOSMask = LayerMask.GetMask("piece", "piece_nonsolid", "Default", "static_solid", "Default_small", "terrain", "vehicle");


        public static Dictionary<string, string> abilityNameDictionary = new Dictionary<string, string>
        {
            { "$item_eikthyr_essence", xEikthyrEssence.Ability_Name },
            { "$item_elder_essence",xElderEssence.Ability_Name},
            { "$item_bonemass_essence", xBoneMassEssence.Ability_Name },
            { "$item_dragonqueen_essence", xModerEssence.Ability_Name },
            { "$item_yagluth_essence", xYagluthEssence.Ability_Name },
            { "$item_seekerqueen_essence", xSeekerQueenEssence.Ability_Name },
            
            { "$item_abomination_essence", xAbominationEssence.Ability_Name },
            { "$item_stonegolem_essence", xStoneGolemEssence.Ability_Name },
            { "$item_troll_essence", xTrollEssence.Ability_Name },
            
            
            { "$item_blob_essence", xBlobEssence.Ability_Name },            
            { "$item_deathsquito_essence", xDeathsquitoEssence.Ability_Name },            
            { "$item_draugrelite_essence", xDraugrEliteEssence.Ability_Name },            
            { "$item_draugr_essence", xDraugrEssence.Ability_Name },            
            { "$item_fenring_essence", xFenringEssence.Ability_Name },            
            { "$item_gjall_essence", xGjallEssence.Ability_Name },
            { "$item_goblin_essence", xFulingEssence.Ability_Name },    
            { "$item_greydwarf_essence", xGreydwarfEssence.Ability_Name },
            { "$item_greydwarfbrute_essence", xGreydwarfBruteEssence.Ability_Name },
            { "$item_greydwarfshaman_essence", xGreydwarfShamanEssence.Ability_Name },
            { "$item_growth_essence", xGrowthEssence.Ability_Name },
            { "$item_hatchling_essence", xDrakeEssence.Ability_Name },
            { "$item_leech_essence", xLeechEssence.Ability_Name },
            { "$item_seeker_essence", xSeekerEssence.Ability_Name },
            { "$item_seeker_brute_essence", xSeekerSoldierEssence.Ability_Name },
            { "$item_skeleton_essence", xSkeletonEssence.Ability_Name },
            { "$item_surtling_essence", xSurtlingEssence.Ability_Name },
            { "$item_tick_essence", xTickEssence.Ability_Name },

            { "$item_goblinbrute_essence", xFulingBerserkerEssence.Ability_Name },
            { "$item_goblinshaman_essence", xFulingShamanEssence.Ability_Name },
            { "$item_cultist_essence", xCultistEssence.Ability_Name },
            { "$item_dvergr_essence", xDvergrEssence.Ability_Name },
            { "$item_serpent_essence", xSeaSerpentEssence.Ability_Name },
            { "$item_skeletonpoison_essence", xRancidRemainsEssence.Ability_Name },
            { "$item_ulv_essence", xUlvEssence.Ability_Name },
            { "$item_wraith_essence", xWraithEssence.Ability_Name },

            { "$item_boar_essence", xBoarEssence.Ability_Name },
            { "$item_deer_essence", xDeerEssence.Ability_Name },
            { "$item_hare_essence", xHareEssence.Ability_Name },
            { "$item_lox_essence", xLoxEssence.Ability_Name },
            { "$item_neck_essence", xNeckEssence.Ability_Name },
            { "$item_wolf_essence", xWolfEssence.Ability_Name },
        };
        
        
        
            
        
        
         public static void InitiateEssenceStatus(Hud hud)
        {
            float xMod = (float)(Screen.width / 1920f);
            float yMod = (float)(Screen.height / 1080f);
            float xStep = 90f * xMod;                
            float yStep = 0f;
            float yOffset = (106f * yMod) + LackingImaginationV2Plugin.icon_Y_Offset.Value;
            // float xOffset = (209f * xMod) + LackingImaginationV2Plugin.icon_X_Offset.Value;
            float xOffset = (220f * xMod) + LackingImaginationV2Plugin.icon_X_Offset.Value;
            if(LackingImaginationV2Plugin.iconAlignment.Value.ToLower() == "vertical")
            {
                xStep = 0f; 
                yStep = 100f * yMod;
            }
            
            Vector3 pos = new Vector3(xOffset + xStep, yOffset + yStep, 0);
            Quaternion rot = new Quaternion(0, 0, 0, 1);
            Transform t = hud.m_statusEffectListRoot;
            EssenceItemData.equipedEssence = EssenceItemData.GetEquippedEssence();
            for (int i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; i++)
            {
                if (EssenceItemData.equipedEssence[i] != null && LackingImaginationV2Plugin.abilitiesStatus[i] == null)
                {
                    RectTransform rectTransform = UnityEngine.Object.Instantiate(hud.m_statusEffectTemplate, pos, rot, t);
                    rectTransform.gameObject.SetActive(value: true);
                    rectTransform.GetComponentInChildren<Text>().text = Localization.instance.Localize((LackingImaginationV2Plugin.AbilityNames[i]).ToString());
                    LackingImaginationV2Plugin.abilitiesStatus[i] = rectTransform;
                }
                pos.x += xStep;
                pos.y += yStep;
            }
        }
         
         public static void NameEssence()
         {
             for (int i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; i++)
             {
                 if (EssenceItemData.equipedEssence[i] != null)
                 {
                     if (abilityNameDictionary.ContainsKey(EssenceItemData.equipedEssence[i]))
                     {
                         LackingImaginationV2Plugin.AbilityNames[i] = abilityNameDictionary[EssenceItemData.equipedEssence[i]];
                         LackingImaginationV2Plugin.AbilitySprites[i] = EssenceItemData.equipedEssenceData[i].GetIcon();
                     }
                 }
             }
         }
         
         public static void AbilityInputPlugin(int position, Player player, Rigidbody body, float altitude, float ground)
        {
            if (EssenceItemData.equipedEssence[position] == "$item_eikthyr_essence")
            {
                xEikthyrEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_elder_essence")
            {
                xElderEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_bonemass_essence")
            {
                xBoneMassEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_dragonqueen_essence")
            {
                xModerEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_yagluth_essence")
            {
                xYagluthEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_seekerqueen_essence")
            {
                xSeekerQueenEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_wolf_essence")
            {
                xWolfEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_goblinbrute_essence")
            {
                xFulingBerserkerEssence.Process_Input(Player.m_localPlayer, position, ref altitude);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_neck_essence")
            {
                xNeckEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_lox_essence")
            {
                xLoxEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_hare_essence")
            {
                xHareEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_dvergr_essence")
            {
                xDvergrEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_deer_essence")
            {
                xDeerEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_boar_essence")
            {
                xBoarEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_wraith_essence")
            {
                xWraithEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_ulv_essence")
            {
                xUlvEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_serpent_essence")
            {
                xSeaSerpentEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_skeletonpoison_essence")
            {
                xRancidRemainsEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_growth_essence")
            {
                xGrowthEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_goblinshaman_essence")
            {
                xFulingShamanEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_greydwarfbrute_essence")
            {
                xGreydwarfBruteEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_cultist_essence")
            {
                xCultistEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_tick_essence")
            {
                xTickEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_surtling_essence")
            {
                xSurtlingEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_skeleton_essence")
            {
                xSkeletonEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_seeker_essence")
            {
                xSeekerEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_seeker_brute_essence")
            {
                xSeekerSoldierEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_leech_essence")
            {
                xLeechEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_goblin_essence")
            {
                xFulingEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_greydwarf_essence")
            {
                xGreydwarfEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_greydwarfshaman_essence")
            {
                xGreydwarfShamanEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_abomination_essence")
            {
                xAbominationEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_stonegolem_essence")
            {
                xStoneGolemEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_troll_essence")
            {
                xTrollEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_blob_essence")
            {
                xBlobEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_deathsquito_essence")
            {
                xDeathsquitoEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_draugrelite_essence")
            {
                xDraugrEliteEssence.Process_Input(Player.m_localPlayer);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_draugr_essence")
            {
                xDraugrEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_fenring_essence")
            {
                xFenringEssence.Process_Input(Player.m_localPlayer,position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_gjall_essence")
            {
                xGjallEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
            if (EssenceItemData.equipedEssence[position] == "$item_hatchling_essence")
            {
                xDrakeEssence.Process_Input(Player.m_localPlayer, position);
                return;
            }
        }

        
         
         public static string CooldownString(int position)
         {
             if (position == 0)
             {
                 return "Ability1_CoolDown";
             }
             if (position == 1)
             {
                 return "Ability2_CoolDown";
             }
             if (position == 2)
             {
                 return "Ability3_CoolDown";
             }
             if (position == 3)
             {
                 return "Ability4_CoolDown";
             }
             if (position == 4)
             {
                 return "Ability5_CoolDown";
             }
             return null;
         }
         
         public static StatusEffect CDEffect(int position)
         {
             if (position == 0)
             {
                 StatusEffect se_cd = (CooldownStatusEffects_A1)ScriptableObject.CreateInstance(typeof(CooldownStatusEffects_A1));
                 return se_cd;
             }
             if (position == 1)
             {
                 StatusEffect se_cd = (CooldownStatusEffects_A2)ScriptableObject.CreateInstance(typeof(CooldownStatusEffects_A2));
                 return se_cd;
             }
             if (position == 2)
             {
                 StatusEffect se_cd = (CooldownStatusEffects_A3)ScriptableObject.CreateInstance(typeof(CooldownStatusEffects_A3));
                 return se_cd;
             }
             if (position == 3)
             {
                 StatusEffect se_cd = (CooldownStatusEffects_A4)ScriptableObject.CreateInstance(typeof(CooldownStatusEffects_A4));
                 return se_cd;
             }
             if (position == 4)
             {
                 StatusEffect se_cd = (CooldownStatusEffects_A5)ScriptableObject.CreateInstance(typeof(CooldownStatusEffects_A5));
                 return se_cd;
             }
             return null;
         }
         
         

         
         /*
          *Cooldown for all essenc
          */
         public static float xEikthyrCooldownTime
         {
             get
             {
                 return 45f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         
         public static float xDeerCooldownTime
         {
             get
             {
                 return 30f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         
         public static float xFenringCooldownTime
         {
             get
             {
                 return 30f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         
         public static float xLoxCooldownTime
         {
             get
             {
                 return 25f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         
         public static float xWolfCooldownTime
         {
             get
             {
                 return 120f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         
         public static float xFulingShamanCooldownTime
         {
             get
             {
                 return 30f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xDeathsquitoCooldownTime
         {
             get
             {
                 return 60f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xSurtlingCooldownTime
         {
             get
             {
                 return 60f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xFulingBerserkerCooldownTime
         {
             get
             {
                 return 60f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xDrakeCooldownTime
         {
             get
             {
                 return 12f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xGrowthCooldownTime
         {
             get
             {
                 return 15f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xTrollCooldownTime
         {
             get
             {
                 return 10f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xGreydwarfShamanCooldownTime
         {
             get
             {
                 return 10f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xDvergrCooldownTime
         {
             get
             {
                 return 2f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xNeckCooldownTime
         {
             get
             {
                 return 3f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xLeechCooldownTime
         {
             get
             {
                 return 10f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xBoneMassCooldownTime
         {
             get
             {
                 return 20f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xGreydwarfBruteCooldownTime
         {
             get
             {
                 return 8f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xFulingCooldownTime
         {
             get
             {
                 return 8f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xGjallCooldownTime
         {
             get
             {
                 return 25f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xGreydwarfCooldownTime
         {
             get
             {
                 return 5f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xElderCooldownTime
         {
             get
             {
                 return 30f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }
         public static float xBlobCooldownTime
         {
             get
             {
                 return 10f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }         
         public static float xSkeletonCooldownTime
         {
             get
             {
                 return 50f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }    
         public static float xAbominationCooldownTime
         {
             get
             {
                 return 120f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }    
         public static float xWriathCooldownTime
         {
             get
             {
                 return 70f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }    
         public static float xDraugrCooldownTime
         {
             get
             {
                 return 10f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }    
         public static float xBoarCooldownTime
         {
             get
             {
                 return 10f * LackingImaginationGlobal.g_CooldownModifer;
             }
         }    
         
         
         
        
         
         
         
         
         
         
         



        public static bool Ability1_Input_Down
        {
            get
            {
                if(LackingImaginationV2Plugin.Ability1_Hotkey.Value == "")
                {
                    return false;
                }
                else if(LackingImaginationV2Plugin.Ability1_Hotkey_Combo.Value == "")
                {
                    return Input.GetKeyDown(LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToLower()) || Input.GetButtonDown(LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToLower());
                }
                else if((Input.GetKeyDown(LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToLower()) && Input.GetKey(LackingImaginationV2Plugin.Ability1_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetKey(LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToLower()) && Input.GetKeyDown(LackingImaginationV2Plugin.Ability1_Hotkey_Combo.Value.ToLower())) || 
                        (Input.GetButtonDown(LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToLower()) && Input.GetButton(LackingImaginationV2Plugin.Ability1_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetButton(LackingImaginationV2Plugin.Ability1_Hotkey.Value.ToLower()) && Input.GetButtonDown(LackingImaginationV2Plugin.Ability1_Hotkey_Combo.Value.ToLower())))
                {
                    return true;
                }
                return false;
            }
        }
        
        
        public static bool Ability2_Input_Down
        {
            get
            {
                if(LackingImaginationV2Plugin.Ability2_Hotkey.Value == "")
                {
                    return false;
                }
                else if(LackingImaginationV2Plugin.Ability2_Hotkey_Combo.Value == "")
                {
                    return Input.GetKeyDown(LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToLower()) || Input.GetButtonDown(LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToLower());
                }
                else if((Input.GetKeyDown(LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToLower()) && Input.GetKey(LackingImaginationV2Plugin.Ability2_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetKey(LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToLower()) && Input.GetKeyDown(LackingImaginationV2Plugin.Ability2_Hotkey_Combo.Value.ToLower())) || 
                        (Input.GetButtonDown(LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToLower()) && Input.GetButton(LackingImaginationV2Plugin.Ability2_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetButton(LackingImaginationV2Plugin.Ability2_Hotkey.Value.ToLower()) && Input.GetButtonDown(LackingImaginationV2Plugin.Ability2_Hotkey_Combo.Value.ToLower())))
                {
                    return true;
                }
                return false;
            }
        }
        
        
        public static bool Ability3_Input_Down
        {
            get
            {
                if(LackingImaginationV2Plugin.Ability3_Hotkey.Value == "")
                {
                    return false;
                }
                else if(LackingImaginationV2Plugin.Ability3_Hotkey_Combo.Value == "")
                {
                    return Input.GetKeyDown(LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToLower()) || Input.GetButtonDown(LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToLower());
                }
                else if((Input.GetKeyDown(LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToLower()) && Input.GetKey(LackingImaginationV2Plugin.Ability3_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetKey(LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToLower()) && Input.GetKeyDown(LackingImaginationV2Plugin.Ability3_Hotkey_Combo.Value.ToLower())) || 
                        (Input.GetButtonDown(LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToLower()) && Input.GetButton(LackingImaginationV2Plugin.Ability3_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetButton(LackingImaginationV2Plugin.Ability3_Hotkey.Value.ToLower()) && Input.GetButtonDown(LackingImaginationV2Plugin.Ability3_Hotkey_Combo.Value.ToLower())))
                {
                    return true;
                }
                return false;
            }
        }
        
        public static bool Ability4_Input_Down
        {
            get
            {
                if(LackingImaginationV2Plugin.Ability4_Hotkey.Value == "")
                {
                    return false;
                }
                else if(LackingImaginationV2Plugin.Ability4_Hotkey_Combo.Value == "")
                {
                    return Input.GetKeyDown(LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToLower()) || Input.GetButtonDown(LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToLower());
                }
                else if((Input.GetKeyDown(LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToLower()) && Input.GetKey(LackingImaginationV2Plugin.Ability4_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetKey(LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToLower()) && Input.GetKeyDown(LackingImaginationV2Plugin.Ability4_Hotkey_Combo.Value.ToLower())) || 
                        (Input.GetButtonDown(LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToLower()) && Input.GetButton(LackingImaginationV2Plugin.Ability4_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetButton(LackingImaginationV2Plugin.Ability4_Hotkey.Value.ToLower()) && Input.GetButtonDown(LackingImaginationV2Plugin.Ability4_Hotkey_Combo.Value.ToLower())))
                {
                    return true;
                }
                return false;
            }
        }
        
        public static bool Ability5_Input_Down
        {
            get
            {
                if(LackingImaginationV2Plugin.Ability5_Hotkey.Value == "")
                {
                    return false;
                }
                else if(LackingImaginationV2Plugin.Ability5_Hotkey_Combo.Value == "")
                {
                    return Input.GetKeyDown(LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToLower()) || Input.GetButtonDown(LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToLower());
                }
                else if((Input.GetKeyDown(LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToLower()) && Input.GetKey(LackingImaginationV2Plugin.Ability5_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetKey(LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToLower()) && Input.GetKeyDown(LackingImaginationV2Plugin.Ability5_Hotkey_Combo.Value.ToLower())) || 
                        (Input.GetButtonDown(LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToLower()) && Input.GetButton(LackingImaginationV2Plugin.Ability5_Hotkey_Combo.Value.ToLower())) ||
                        (Input.GetButton(LackingImaginationV2Plugin.Ability5_Hotkey.Value.ToLower()) && Input.GetButtonDown(LackingImaginationV2Plugin.Ability5_Hotkey_Combo.Value.ToLower())))
                {
                    return true;
                }
                return false;
            }
        }
        
        
        
        
        
    }



}