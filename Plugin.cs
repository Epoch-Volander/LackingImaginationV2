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
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.Animations;


namespace LackingImaginationV2
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class LackingImaginationV2Plugin : BaseUnityPlugin
    {
        internal const string ModName = "LackingImaginationV2";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Epoch-Volander";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource LackingImaginationV2Logger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        // // Location Manager variables
        // public Texture2D tex;
        // private Sprite mySprite;
        // private SpriteRenderer sr;


        public static Dictionary<string, Heightmap.Biome> biomeDictionary = new Dictionary<string, Heightmap.Biome>
        {
            { "BlackForest_Exp", Heightmap.Biome.BlackForest },
            { "Meadows_Exp", Heightmap.Biome.Meadows },
            { "Mistlands_Exp", Heightmap.Biome.Mistlands },
            { "Mountain_Exp", Heightmap.Biome.Mountain },
            { "Ocean_Exp", Heightmap.Biome.Ocean },
            { "Plains_Exp", Heightmap.Biome.Plains },
            { "Swamp_Exp", Heightmap.Biome.Swamp },
            { "AshLands_Exp", Heightmap.Biome.AshLands },
            { "DeepNorth_Exp", Heightmap.Biome.DeepNorth }
        };
        
        public static Dictionary<string, string> dungeonDictionary = new Dictionary<string, string>
        {
            { "InfectedMine", "InfectedMine_Exp" },
            { "Caves", "FrostCave_Exp" },
            { "Crypt", "BurialChambers_TrollCave_Exp" },
            { "SunkenCrypt", "SunkenCrypt_Exp" },
        };
        
        public static Dictionary<string, string> dungeonMusicDictionary = new Dictionary<string, string>
        {
            { "GoblinCamp", "GoblinCamp_Exp" },
            { "ForestCryptHildir", "ForestCryptHildir_Exp" },
            { "CaveHildir", "CaveHildir_Exp" },
            { "PlainsFortHildir", "PlainsFortHildir_Exp" },
        };

        public static Dictionary<string, List<string>> trophyDictionary = new Dictionary<string, List<string>>
        {
            //  item name                                 tutorial name, level gain
            { "$item_trophy_eikthyr", new List<string> { "Eikthyr_Exp", "3" } },
            { "$item_trophy_elder", new List<string> { "TheElder_Exp", "3" } },
            { "$item_trophy_bonemass", new List<string> { "BoneMass_Exp", "3" } },
            { "$item_trophy_dragonqueen", new List<string> { "Moder_Exp", "3" } },
            { "$item_trophy_goblinking", new List<string> { "Yagluth_Exp", "3" } },
            { "$item_trophy_seekerqueen", new List<string> { "SeekerQueen_Exp", "3" } },

            { "$item_trophy_abomination", new List<string> { "Abomination_Exp", "2" } },
            { "$item_trophy_sgolem", new List<string> { "StoneGolem_Exp", "2" } },
            { "$item_trophy_troll", new List<string> { "Troll_Exp", "2" } },

            { "$item_trophy_blob", new List<string> { "Blob_Exp", "1" } },
            { "$item_trophy_deathsquito", new List<string> { "Deathsquito_Exp", "1" } },
            { "$item_trophy_draugrelite", new List<string> { "DraugrElite_Exp", "1" } },
            { "$item_trophy_draugr", new List<string> { "Draugr_Exp", "1" } },
            { "$item_trophy_fenring", new List<string> { "Fenring_Exp", "1" } },
            { "$item_trophy_gjall", new List<string> { "Gjall_Exp", "1" } },
            { "$item_trophy_goblin", new List<string> { "Fuling_Exp", "1" } },
            { "$item_trophy_greydwarf", new List<string> { "Greydwarf_Exp", "1" } },
            { "$item_trophy_greydwarfbrute", new List<string> { "GreydwarfBrute_Exp", "1" } },
            { "$item_trophy_greydwarfshaman", new List<string> { "GreydwarfShaman_Exp", "1" } },
            { "$item_trophy_growth", new List<string> { "Growth_Exp", "1" } },
            { "$item_trophy_hatchling", new List<string> { "Drake_Exp", "1" } },
            { "$item_trophy_leech", new List<string> { "Leech_Exp", "1" } },
            { "$item_trophy_seeker", new List<string> { "Seeker_Exp", "1" } },
            { "$item_trophy_seeker_brute", new List<string> { "SeekerSoldier_Exp", "1" } },
            { "$item_trophy_skeleton", new List<string> { "Skeleton_Exp", "1" } },
            { "$item_trophy_surtling", new List<string> { "Surtling_Exp", "1" } },
            { "$item_trophy_tick", new List<string> { "Tick_Exp", "1" } },

            { "$item_trophy_goblinbrute", new List<string> { "FulingBerserker_Exp", "1" } },
            { "$item_trophy_goblinshaman", new List<string> { "FulingShaman_Exp", "1" } },
            { "$item_trophy_cultist", new List<string> { "Cultist_Exp", "1" } },
            { "$item_trophy_dvergr", new List<string> { "Dvergr_Exp", "1" } },
            { "$item_trophy_serpent", new List<string> { "Serpent_Exp", "1" } },
            { "$item_trophy_skeletonpoison", new List<string> { "RancidRemains_Exp", "1" } },
            { "$item_trophy_ulv", new List<string> { "Ulv_Exp", "1" } },
            { "$item_trophy_wraith", new List<string> { "Wraith_Exp", "1" } },

            { "$item_trophy_boar", new List<string> { "Boar_Exp", "1" } },
            { "$item_trophy_deer", new List<string> { "Deer_Exp", "1" } },
            { "$item_trophy_hare", new List<string> { "Hare_Exp", "1" } },
            { "$item_trophy_lox", new List<string> { "Lox_Exp", "1" } },
            { "$item_trophy_neck", new List<string> { "Neck_Exp", "1" } },
            { "$item_trophy_wolf", new List<string> { "Wolf_Exp", "1" } },
            
            { "$item_trophy_cultist_hildir", new List<string> { "Geirrhafa_Exp", "2" } },
            { "$item_trophy_skeleton_hildir", new List<string> { "Brenna_Exp", "2" } },
            { "$item_trophy_shamanbro", new List<string> { "Zil_Exp", "2" } },
            { "$item_trophy_brutebro", new List<string> { "Thungr_Exp", "2" } },
        };
        
        public static Dictionary<string, List<string>> ItemBundleUnwrapDict = new Dictionary<string, List<string>>()
        {
            // { "Essence_Drop", new List<string> { "Essence", "An erie glow.", "Greydwarf", "0.5" } }
            //bosses
            // { "$item_eikthyr_essence", new List<string> { "Eikthyr_Essence_Drop","Eikthyr Essence", "Eikthyr", "0.5", "An erie glow.", "TrophyEikthyrEssence"} },
            { "$item_eikthyr_essence", new List<string> { "Eikthyr_Essence_Drop", "Eikthyr Essence", "Eikthyr", "0.5", "An erie glow." } },
            { "$item_elder_essence", new List<string> { "TheElder_Essence_Drop", "The Elder Essence", "gd_king", "0.5", "An erie glow." } },
            { "$item_bonemass_essence", new List<string> { "BoneMass_Essence_Drop", "Bone Mass Essence", "Bonemass", "0.5", "An erie glow." } },
            { "$item_dragonqueen_essence", new List<string> { "Moder_Essence_Drop", "Moder Essence", "Dragon", "0.5", "An erie glow." } },
            { "$item_yagluth_essence", new List<string> { "Yagluth_Essence_Drop", "Yagluth Essence", "GoblinKing", "0.5", "An erie glow." } },
            { "$item_seekerqueen_essence", new List<string> { "SeekerQueen_Essence_Drop", "Seeker Queen Essence", "SeekerQueen", "0.5", "An erie glow." } },

            { "$item_abomination_essence", new List<string> { "Abomination_Essence_Drop", "Abomination Essence", "Abomination", "0.5", "An erie glow." } },
            { "$item_stonegolem_essence", new List<string> { "StoneGolem_Essence_Drop", "Stone Golem Essence", "StoneGolem", "0.5", "An erie glow." } },
            { "$item_troll_essence", new List<string> { "Troll_Essence_Drop", "Troll Essence", "Troll", "0.5", "An erie glow." } },

            { "$item_blob_essence", new List<string> { "Blob_Essence_Drop", "Blob Essence", "Blob", "0.5", "An erie glow." } },
            { "$item_boar_essence", new List<string> { "Boar_Essence_Drop", "Boar Essence", "Boar", "0.5", "An erie glow." } },
            { "$item_cultist_essence", new List<string> { "Cultist_Essence_Drop", "Cultist Essence", "Fenring_Cultist", "0.5", "An erie glow." } },
            { "$item_deathsquito_essence", new List<string> { "Deathsquito_Essence_Drop", "Deathsquito Essence", "Deathsquito", "0.5", "An erie glow." } },
            { "$item_deer_essence", new List<string> { "Deer_Essence_Drop", "Deer Essence", "Deer", "0.5", "An erie glow." } },
            { "$item_draugrelite_essence", new List<string> { "DraugrElite_Essence_Drop", "Draugr Elite Essence", "Draugr_Elite", "0.5", "An erie glow." } },
            { "$item_draugr_essence", new List<string> { "Draugr_Essence_Drop", "Draugr Essence", "Draugr", "0.5", "An erie glow." } },
            { "$item_dvergr_essence", new List<string> { "Dvergr_Essence_Drop", "Dvergr Essence", "Dverger", "0.5", "An erie glow." } },
            { "$item_fenring_essence", new List<string> { "Fenring_Essence_Drop", "Fenring Essence", "Fenring", "0.5", "An erie glow." } },
            { "$item_gjall_essence", new List<string> { "Gjall_Essence_Drop", "Gjall Essence", "Gjall", "0.5", "An erie glow." } },
            { "$item_goblin_essence", new List<string> { "Goblin_Essence_Drop", "Fuling Essence", "Goblin", "0.5", "An erie glow." } },
            { "$item_goblinbrute_essence", new List<string> { "GoblinBrute_Essence_Drop", "Fuling Berserker Essence", "GoblinBrute", "0.5", "An erie glow." } },
            { "$item_goblinshaman_essence", new List<string> { "GoblinShaman_Essence_Drop", "Fuling Shaman Essence", "GoblinShaman", "0.5", "An erie glow." } },
            { "$item_greydwarf_essence", new List<string> { "Greydwarf_Essence_Drop", "Greydwarf Essence", "Greydwarf", "0.5", "An erie glow." } },
            { "$item_greydwarfbrute_essence", new List<string> { "GreydwarfBrute_Essence_Drop", "Greydwarf Brute Essence", "Greydwarf_Elite", "0.5", "An erie glow." } },
            { "$item_greydwarfshaman_essence", new List<string> { "GreydwarfShaman_Essence_Drop", "Greydwarf Shaman Essence", "Greydwarf_Shaman", "0.5", "An erie glow." } },
            { "$item_growth_essence", new List<string> { "Growth_Essence_Drop", "Growth Essence", "BlobTar", "0.5", "An erie glow." } },
            { "$item_hare_essence", new List<string> { "Hare_Essence_Drop", "Hare Essence", "Hare", "0.5", "An erie glow." } },
            { "$item_hatchling_essence", new List<string> { "Drake_Essence_Drop", "Drake Essence", "Hatchling", "0.5", "An erie glow." } },
            { "$item_leech_essence", new List<string> { "Leech_Essence_Drop", "Leech Essence", "Leech", "0.5", "An erie glow." } },
            { "$item_lox_essence", new List<string> { "Lox_Essence_Drop", "Lox Essence", "Lox", "0.5", "An erie glow." } },
            { "$item_neck_essence", new List<string> { "Neck_Essence_Drop", "Neck Essence", "Neck", "0.5", "An erie glow." } },
            { "$item_seeker_essence", new List<string> { "Seeker_Essence_Drop","Seeker Essence", "Seeker", "0.5", "An erie glow." } },
            { "$item_seeker_brute_essence", new List<string> { "SeekerSoldier_Essence_Drop","Seeker Soldier Essence", "SeekerBrute", "0.5", "An erie glow." } },
            { "$item_serpent_essence", new List<string> { "Serpent_Essence_Drop", "Sea Serpent Essence", "Serpent", "0.5", "An erie glow." } },
            { "$item_skeleton_essence", new List<string> { "Skeleton_Essence_Drop", "Skeleton Essence", "Skeleton", "0.5", "An erie glow." } },
            { "$item_skeletonpoison_essence", new List<string> { "SkeletonPoison_Essence_Drop", "Rancid Remains Essence", "Skeleton_Poison", "0.5", "An erie glow." } },
            { "$item_surtling_essence", new List<string> { "Surtling_Essence_Drop", "Surtling Essence", "Surtling", "0.5", "An erie glow." } },
            { "$item_tick_essence", new List<string> { "Tick_Essence_Drop", "Tick Essence", "Tick", "0.5", "An erie glow." } },
            { "$item_ulv_essence", new List<string> { "Ulv_Essence_Drop", "Ulv Essence", "Ulv", "0.5", "An erie glow." } },
            { "$item_wolf_essence", new List<string> { "Wolf_Essence_Drop", "Wolf Essence", "Wolf", "0.5", "An erie glow." } },
            { "$item_wraith_essence", new List<string> { "Wraith_Essence_Drop", "Wraith Essence", "Wraith", "0.5", "An erie glow." } },
            
            { "$item_brenna_essence", new List<string> { "Brenna_Essence_Drop", "Brenna Essence", "Skeleton_Hildir", "0.5", "An erie glow." } },
            { "$item_geirrhafa_essence", new List<string> { "Geirrhafa_Essence_Drop", "Geirrhafa Essence", "Fenring_Cultist_Hildir", "0.5", "An erie glow." } },
            { "$item_zil_essence", new List<string> { "Zil_Essence_Drop", "Zil Essence", "GoblinShaman_Hildir", "0.5", "An erie glow." } },
            { "$item_thungr_essence", new List<string> { "Thungr_Essence_Drop", "Thungr Essence", "GoblinBrute_Hildir", "0.5", "An erie glow." } },
        };


        // public static int EquipSlotCount => EquipSlotTypes.Count; // link this to imagination skill
        // public static int EquipSlotCount; // link this to imagination skill

        public static int EquipSlotCount = 5;

        public static readonly List<ItemDrop.ItemData.ItemType> EquipSlotTypes = new List<ItemDrop.ItemData.ItemType>()
        {
            ItemDrop.ItemData.ItemType.Customization,
            ItemDrop.ItemData.ItemType.Customization,
            ItemDrop.ItemData.ItemType.Customization,
            ItemDrop.ItemData.ItemType.Customization,
            ItemDrop.ItemData.ItemType.Customization
        };

        
        public static bool playerEnabled = true;

        public static ConfigEntry<bool> EssenceSlotsEnabled;
        public static bool UseGuardianPower = true;

        public static ConfigEntry<string> Ability1_Hotkey;
        public static ConfigEntry<string> Ability1_Hotkey_Combo;
        public static ConfigEntry<string> Ability2_Hotkey;
        public static ConfigEntry<string> Ability2_Hotkey_Combo;
        public static ConfigEntry<string> Ability3_Hotkey;
        public static ConfigEntry<string> Ability3_Hotkey_Combo;
        public static ConfigEntry<string> Ability4_Hotkey;
        public static ConfigEntry<string> Ability4_Hotkey_Combo;
        public static ConfigEntry<string> Ability5_Hotkey;
        public static ConfigEntry<string> Ability5_Hotkey_Combo;

        public static ConfigEntry<float> li_cooldownMultiplier;

        public static ConfigEntry<float> icon_X_Offset;
        public static ConfigEntry<float> icon_Y_Offset;

        public static ConfigEntry<bool> showAbilityIcons;
        public static ConfigEntry<string> iconAlignment;

        public static readonly Color abilityCooldownColor = new Color(1f, .3f, .3f, .5f);

        public static List<Sprite> AbilitySprites = new List<Sprite>
        {
            null,null,null,null,null,
        };

        public static List<string> AbilityNames = new List<string>
        {
            null,null,null,null,null,
        };

        public static List<RectTransform> abilitiesStatus = new List<RectTransform> { };
        
        public static ConfigEntry<float> li_deerHorizonHaste;
        public static ConfigEntry<float> li_deerHorizonHastePassive;
        public static ConfigEntry<float> li_eikthyrBlitzPassive;
        public static ConfigEntry<float> li_eikthyrBlitz;
        public static ConfigEntry<float> li_fenringMoonlitLeap;
        public static ConfigEntry<float> li_fenringMoonlitLeapPassive;
        public static ConfigEntry<float> li_loxWildTremor;
        public static ConfigEntry<float> li_loxWildTremorPassive;
        public static ConfigEntry<float> li_wolfRavenousHunger;
        public static ConfigEntry<float> li_wolfRavenousHungerStaminaPassive;
        public static ConfigEntry<float> li_wolfRavenousHungerPassive;
        public static ConfigEntry<float> li_fulingshamanRitualShield;
        public static ConfigEntry<float> li_fulingshamanRitualShieldGrowthCap;
        public static ConfigEntry<float> li_fulingshamanRitualProjectile;
        public static ConfigEntry<float> li_fulingshamanRitualPassiveEitr;
        public static ConfigEntry<float> li_fulingshamanRitualPassiveCarry;
        public static ConfigEntry<float> li_deathsquitoRelentlessHoming;
        public static ConfigEntry<float> li_deathsquitoRelentlessPassive;
        public static ConfigEntry<float> li_surtlingHarbingerCharges;
        public static ConfigEntry<float> li_surtlingHarbingerBurn;
        public static ConfigEntry<float> li_surtlingHarbingerMinDistance;
        public static ConfigEntry<float> li_drakeThreeFreezeProjectile;
        public static ConfigEntry<float> li_growthAncientTarProjectile;
        public static ConfigEntry<float> li_growthAncientTarPassive;
        public static ConfigEntry<float> li_greydwarfshamanDubiousHealPlayer;
        public static ConfigEntry<float> li_greydwarfshamanDubiousHealCreature;
        public static ConfigEntry<float> li_greydwarfshamanDubiousHealPassive;
        public static ConfigEntry<float> li_greydwarfshamanDubiousHealPassiveEitr;
        public static ConfigEntry<float> li_trollTrollTossProjectile;
        public static ConfigEntry<float> li_trollTrollTossPassive;
        public static ConfigEntry<float> li_dvergrRandomizeIceProjectile;
        public static ConfigEntry<float> li_dvergrRandomizeFireProjectile;
        public static ConfigEntry<float> li_dvergrRandomizeHealPlayer;
        public static ConfigEntry<float> li_dvergrRandomizeHealCreature;
        public static ConfigEntry<float> li_dvergrRandomizeCost;
        public static ConfigEntry<float> li_dvergrRandomizePassiveEitr;
        public static ConfigEntry<float> li_dvergrRandomizePassive;
        public static ConfigEntry<float> li_leechBloodSiphonStack;
        public static ConfigEntry<float> li_leechBloodSiphonStackCap;
        public static ConfigEntry<float> li_bonemassMassReleaseSummonDuration;
        public static ConfigEntry<float> li_bonemassMassReleaseProjectile;
        public static ConfigEntry<float> li_greydwarfbruteBashMultiplier;
        public static ConfigEntry<float> li_greydwarfbruteRangedReductionPassive;
        public static ConfigEntry<float> li_fulingLonginusMultiplier;
        public static ConfigEntry<float> li_fulingLonginusPassiveBlockMultiplier;
        public static ConfigEntry<float> li_fulingLonginusPassiveMotivated;
        public static ConfigEntry<float> li_fulingLonginusPassiveDemotivated;
        public static ConfigEntry<float> li_gjallGjallarhornSummonDuration;
        public static ConfigEntry<float> li_gjallGjallarhornProjectile;
        public static ConfigEntry<float> li_gjallGjallarhornArmor;
        public static ConfigEntry<float> li_greydwarfPebbleProjectile;
        public static ConfigEntry<float> li_greydwarfPebblePassiveCarry;
        public static ConfigEntry<float> li_greydwarfPebbleForestAnger;
        public static ConfigEntry<float> li_elderAncientAwePassive;
        public static ConfigEntry<float> li_blobFumes;
        public static ConfigEntry<float> li_skeletonVigilSummons;
        public static ConfigEntry<float> li_skeletonVigilSummonDuration;
        public static ConfigEntry<float> li_skeletonVigilSoulCap;
        public static ConfigEntry<float> li_abominationBaneArmor;
        public static ConfigEntry<float> li_abominationBaneHealth;
        public static ConfigEntry<float> li_wraithTwinSoulsArmor;
        public static ConfigEntry<float> li_wraithTwinSoulsPassive;
        public static ConfigEntry<float> li_draugrForgottenRot;
        public static ConfigEntry<float> li_draugrForgottenPassiveCarry;
        public static ConfigEntry<float> li_draugrForgottenActive;
        public static ConfigEntry<float> li_draugreliteFallenHeroRot;
        public static ConfigEntry<float> li_draugreliteFallenHeroPassiveCarry;
        public static ConfigEntry<float> lidraugreliteFallenHeroActive;
        public static ConfigEntry<float> li_geirrhafaIceAgeAoe;
        public static ConfigEntry<float> li_geirrhafaIceAgePassiveEitr;
        public static ConfigEntry<float> li_geirrhafaIceAgePassive;
        public static ConfigEntry<float> li_cultistLoneSunAoe;
        public static ConfigEntry<float> li_cultistLoneSunPassive;
      
        
        
        
        
        // public static List<string> equipedEssence = new();
        private static LackingImaginationV2Plugin _instance;

        // Prefabs type 1 //Pulled from in game
        public static GameObject fx_Harbinger;
        
        //Prefabs type 2 //Pulled from assets
        public static GameObject fx_Giantization;
        public static GameObject fx_Bash;
        public static GameObject fx_Longinus;
        public static GameObject fx_TwinSouls;
        //Sounds
        public static GameObject sfx_Giantization;

        // Animation Clips // Pulled from in game
        public static Animator creatureAnimatorGeirrhafa;
        public static AnimationClip creatureAnimationClipGeirrhafaIceNova;
        public static Animator creatureAnimatorElder;
        public static AnimationClip creatureAnimationClipElderSummon;
       
        
        // Animation Clip swappers
        public static readonly Dictionary<string, AnimationClip> ExternalAnimations = new();
        public static readonly Dictionary<string, RuntimeAnimatorController> CustomRuntimeControllers = new();
        private static readonly Dictionary<string, Dictionary<string, string>> replacementMap = new();




        public static List<List<string>> li_stringList = new List<List<string>>();

        [HarmonyPatch(typeof(PlayerProfile), "SavePlayerToDisk", null)]
        public static class Save_LI_StringList_Patch
        {
            public static void Postfix(PlayerProfile __instance, string ___m_filename, string ___m_playerName)
            {
                LackingImaginationV2Plugin.Log($"sav{LackingImaginationV2Plugin.li_stringList.Count}");
                foreach (List<string> subList in LackingImaginationV2Plugin.li_stringList)
                {
                    LackingImaginationV2Plugin.Log($"Sub-list count: {subList.Count}");
                    foreach (string item in subList)
                    {
                        LackingImaginationV2Plugin.Log($"Item: {item}");
                    }

                    LackingImaginationV2Plugin.Log("End of sub-list");
                }

                try
                {
                    Directory.CreateDirectory(Utils.GetSaveDataPath(FileHelpers.FileSource.Local) +
                                              "/characters/LackingI");
                    string text = Utils.GetSaveDataPath(FileHelpers.FileSource.Local) + "/characters/LackingI/" +
                                  ___m_filename + "_li_strings.fch";
                    string text3 = Utils.GetSaveDataPath(FileHelpers.FileSource.Local) + "/characters/LackingI/" +
                                   ___m_filename + "_li_strings.fch.new";
                    ZPackage zPackage = new ZPackage();

                    // Serialize the string list data
                    zPackage.Write(LackingImaginationV2Plugin.li_stringList.Count);
                    foreach (var strList in LackingImaginationV2Plugin.li_stringList)
                    {
                        zPackage.Write(strList.Count);
                        foreach (var str in strList)
                        {
                            zPackage.Write(str);
                        }
                    }

                    byte[] array = zPackage.GenerateHash();
                    byte[] array2 = zPackage.GetArray();
                    FileStream fileStream = File.Create(text3);
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                    binaryWriter.Write(array2.Length);
                    binaryWriter.Write(array2);
                    binaryWriter.Write(array.Length);
                    binaryWriter.Write(array);
                    binaryWriter.Flush();
                    fileStream.Flush(flushToDisk: true);
                    fileStream.Close();
                    fileStream.Dispose();
                    if (File.Exists(text))
                    {
                        File.Delete(text);
                    }

                    File.Move(text3, text);
                }
                catch (NullReferenceException ex)
                {
                    LackingImaginationV2Plugin.Log($"failed to save");
                }
            }
        }

        [HarmonyPatch(typeof(PlayerProfile), "LoadPlayerFromDisk", null)]
        public class Load_LI_StringList_Patch
        {
            public static void Postfix(PlayerProfile __instance, string ___m_filename, string ___m_playerName)
            {
                try
                {
                    if (LackingImaginationV2Plugin.li_stringList == null)
                    {
                        LackingImaginationV2Plugin.li_stringList = new List<List<string>>();
                    }

                    LackingImaginationV2Plugin.li_stringList.Clear();

                    LackingImaginationV2Plugin.li_stringList.Add(xLoxEssencePassive.LoxEaten);
                    LackingImaginationV2Plugin.li_stringList.Add(xWolfEssencePassive.WolfStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xFulingShamanEssencePassive.FulingShamanStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xSurtlingEssencePassive.SurtlingStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xNeckEssencePassive.NeckStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xLeechEssencePassive.LeechStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xGreydwarfEssencePassive.GreydwarfStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xSkeletonEssencePassive.SkeletonStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xDraugrRot.RotStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xDraugrEssencePassive.DraugrStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xDraugrEliteEssencePassive.DraugrEliteStats);
                    
                    ZPackage zPackage = LoadStringDataFromDisk(___m_filename);
                    if (zPackage == null)
                    {
                        // No data for the string lists
                        goto LoadExit;
                    }

                    int listCount = zPackage.ReadInt();
                    for (int i = 0; i < listCount; i++)
                    {
                        int strCount = zPackage.ReadInt();
                        if (li_stringList[i].Count == 0)
                        {
                            for (int j = 0; j < strCount; j++)
                            {
                                string str = zPackage.ReadString();
                                li_stringList[i].Add(str);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < strCount; j++)
                            {
                                string str = zPackage.ReadString();
                                li_stringList[i][j] = str;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Exception while loading string list: " + ex.ToString());
                }

                LoadExit: ;
            }

            private static ZPackage LoadStringDataFromDisk(string m_filename)
            {
                string text = Utils.GetSaveDataPath(FileHelpers.FileSource.Local) + "/characters/LackingI/" +
                              m_filename + "_li_strings.fch";
                FileStream fileStream = null;
                try
                {
                    fileStream = File.OpenRead(text);
                }
                catch
                {
                    return null;
                }

                if (fileStream == null)
                {
                    return null;
                }

                byte[] data;

                BinaryReader binaryReader = null;
                try
                {
                    binaryReader = new BinaryReader(fileStream);
                    int count = binaryReader.ReadInt32();
                    data = binaryReader.ReadBytes(count);
                    int count2 = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(count2);
                }
                catch
                {
                    if (binaryReader != null)
                    {
                        binaryReader.Close();
                    }

                    fileStream.Dispose();
                    return null;
                }
                finally
                {
                    if (binaryReader != null)
                    {
                        binaryReader.Close();
                    }
                }

                fileStream.Dispose();
                return new ZPackage(data);
            }
        }

        
        
        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            // Uncomment the line below to use the LocalizationManager for localizing your mod.
            //Localizer.Load(); // Use this to initialize the LocalizationManager (for more information on LocalizationManager, see the LocalizationManager documentation https://github.com/blaxxun-boop/LocalizationManager#example-project).

            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
                "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            _instance = this;

            {
                //creates imagination skill to do, slots per 20 lvls
                Skill imagination = new("Imagination", "YagluthDrop_icon.png"); // Skill name along with the skill icon. By default the icon is found in the icons folder. Put it there if you wish to load one.
                imagination.Description.English("Records Player Achievements.");
                imagination.Configurable = true;
                
                
            }
            
            //Uploads item prefabs
            foreach (KeyValuePair<string, List<string>> kvp in ItemBundleUnwrapDict)
            {
                ExpMethods.CreateEssenceBase(kvp.Value);
            }
            
            // PrefabManager.RegisterPrefab("essence_bundle_2","TrophyEikthyrEssence");
            // MaterialReplacer.RegisterGameObjectForShaderSwap(ZNetScene.instance.GetPrefab("TrophyEikthyrEssence"), MaterialReplacer.ShaderType.CustomCreature);
        
            
            //Prefabs 2
            fx_Giantization = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "RotVariant1");
            fx_Bash = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantRed");
            fx_Longinus = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantYellow");
            fx_TwinSouls = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantWraith");
            
            //Sound Prefabs
            sfx_Giantization = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "sfx_goblinbrute_rage");
            
            abilitiesStatus.Clear();
            for (int i = 0; i < 5; i++)
            {
                abilitiesStatus.Add(null);
            }
            



            EssenceSlotsEnabled = Config.Bind("Toggles", "Enable Essence Slots", true, "Disabling this while items are in the slots will attempt to move them to your inventory.");

            //Ability1_Hotkey = ConfigManager.RegisterModConfigVariable<string>(ModName, "Ability1_Hotkey", "1", "Keybinds", "Ability 1 Hotkey", true); //\nUse mouse # to bind an ability to a mouse button\nThe # represents the mouse button; mouse 0 is left click, mouse 1 right click, etc", true);
            Ability1_Hotkey = config("Keybinds", "Ability1_Hotkey", "1", "Ability 1 Hotkey\nUse mouse # to bind an ability to a mouse button");
            //Ability1_Hotkey_Combo = ConfigManager.RegisterModConfigVariable<string>(ModName, "Ability1_Hotkey_Combo", "left alt", "Keybinds", "Ability 1 Combination Key - entering a value will trigger the ability only when both the Hotkey and Hotkey_Combo buttons are pressed", true); //\nAllows input from a combination of keys when a value is entered for the combo key\nIf only one key is used, leave the combo key blank\nExamples: space, Q, left shift, left ctrl, right alt, right cmd", true);
            Ability1_Hotkey_Combo = config("Keybinds", "Ability1_Hotkey_Combo", "left alt", "Ability 1 Combo Key - entering a value will trigger the ability only when both the Hotkey and Hotkey_Combo buttons are pressed\nAllows input from a combination of keys when a value is entered for the combo key\nIf only one key is used, leave the combo key blank");
            Ability2_Hotkey = config("Keybinds", "Ability2_Hotkey", "2", "Ability 2 Hotkey\nUse mouse # to bind an ability to a mouse button");
            Ability2_Hotkey_Combo = config("Keybinds", "Ability2_Hotkey_Combo", "left alt", "Ability 2 Combo Key - entering a value will trigger the ability only when both the Hotkey and Hotkey_Combo buttons are pressed\nAllows input from a combination of keys when a value is entered for the combo key\nIf only one key is used, leave the combo key blank");
            Ability3_Hotkey = config("Keybinds", "Ability3_Hotkey", "3", "Ability 3 Hotkey\nUse mouse # to bind an ability to a mouse button");
            Ability3_Hotkey_Combo = config("Keybinds", "Ability3_Hotkey_Combo", "left alt", "Ability 3 Combo Key - entering a value will trigger the ability only when both the Hotkey and Hotkey_Combo buttons are pressed\nAllows input from a combination of keys when a value is entered for the combo key\nIf only one key is used, leave the combo key blank");
            Ability4_Hotkey = config("Keybinds", "Ability4_Hotkey", "4", "Ability 4 Hotkey\nUse mouse # to bind an ability to a mouse button");
            Ability4_Hotkey_Combo = config("Keybinds", "Ability4_Hotkey_Combo", "left alt", "Ability 4 Combo Key - entering a value will trigger the ability only when both the Hotkey and Hotkey_Combo buttons are pressed\nAllows input from a combination of keys when a value is entered for the combo key\nIf only one key is used, leave the combo key blank");
            Ability5_Hotkey = config("Keybinds", "Ability5_Hotkey", "5", "Ability 5 Hotkey\nUse mouse # to bind an ability to a mouse button");
            Ability5_Hotkey_Combo = config("Keybinds", "Ability5_Hotkey_Combo", "left alt", "Ability 5 Combo Key - entering a value will trigger the ability only when both the Hotkey and Hotkey_Combo buttons are pressed\nAllows input from a combination of keys when a value is entered for the combo key\nIf only one key is used, leave the combo key blank");

            //li_cooldownMultiplier = ConfigManager.RegisterModConfigVariable<float>(ModName, "vl_mce_cooldownMultiplier", 1f, "Modifiers", "This value multiplied on overall cooldown time of abilities", false);
            li_cooldownMultiplier = config("Modifiers", "li_cooldownMultiplier", 100f, "This value multiplied on overall cooldown time of abilities");


            //showAbilityIcons = ConfigManager.RegisterModConfigVariable<bool>(ModName, "showAbilityIcons", true, "Display", "Displays Icons on Hud for each ability", true);
            showAbilityIcons = config("Display", "showAbilityIcons", true, "Displays Icons on Hud for each ability");
            //iconAlignment = ConfigManager.RegisterModConfigVariable<string>(ModName, "iconAlignment", "horizontal", "Display", "Aligns icons horizontally or vertically off the guardian power icon; options are horizontal or vertical", true);
            iconAlignment = config("Display", "iconAlignment", "horizontal", "Aligns icons horizontally or vertically off the guardian power icon; options are horizontal or vertical");
            //icon_X_Offset = ConfigManager.RegisterModConfigVariable<float>(ModName, "icon_X_Offset", 0f, "Display", "Offsets the icon bar horizontally. The icon bar is anchored to the Guardian power icon.", true);
            icon_X_Offset = config("Display", "icon_X_Offset", 0f, "Offsets the icon bar horizontally. The icon bar is anchored to the Guardian power icon.");
            //icon_Y_Offset = ConfigManager.RegisterModConfigVariable<float>(ModName, "icon_Y_Offset", 0f, "Display", "Offsets the icon bar vertically. The icon bar is anchored to the Guardian power icon.", true);
            icon_Y_Offset = config("Display", "icon_Y_Offset", 0f, "Offsets the icon bar vertically. The icon bar is anchored to the Guardian power icon.");

            //deer
            li_deerHorizonHaste = config("Essence Deer Modifiers", "li_deerHorizonHaste", 100f, "Modifies the movement speed for Horizon Haste");
            li_deerHorizonHastePassive = config("Essence Deer Modifiers", "li_deerHorizonHastePassive", 25f, "Bonus Stamina from Horizon Haste");
            // eikthyr
            li_eikthyrBlitzPassive = config("Essence Eikthyr Modifiers", "li_eikthyrBlitzPassive", 10f, "Modifies the lightning damage passive for Weapons");
            li_eikthyrBlitz = config("Essence Eikthyr Modifiers", "li_eikthyrBlitz", 100f, "Modifies the lightning damage for Blitz");
            // fenring
            li_fenringMoonlitLeap = config("Essence Fenring Modifiers", "li_fenringMoonlitLeap", 100f, "Modifies the jump force for Moonlit Leap");
            li_fenringMoonlitLeapPassive = config("Essence Fenring Modifiers", "li_fenringMoonlitLeapPassive", 10f, "Modifies the % slash damage passive for Fist Weapons");
            // lox
            li_loxWildTremor = config("Essence Lox Modifiers", "li_loxWildTremor", 100f, "Modifies the blunt damage for Wild Tremor");
            li_loxWildTremorPassive = config("Essence Lox Modifiers", "li_loxWildTremorPassive", 30f, "Bonus Health from Wild Tremor on eat");
            // wolf
            li_wolfRavenousHunger = config("Essence Wolf Modifiers", "li_wolfRavenousHunger", 5f, "Modifies % max HP dmg for Ravenous Hunger");
            li_wolfRavenousHungerStaminaPassive = config("Essence Wolf Modifiers", "li_wolfRavenousHungerStaminaPassive", 65f, "Bonus Stamina per stage of Ravenous Hunger");
            li_wolfRavenousHungerPassive = config("Essence Wolf Modifiers", "li_wolfRavenousHungerPassive", 100f, "Modifies the Damage multiplier of Ravenous Hunger");
            // fulingshaman
            li_fulingshamanRitualShield = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualShield", 200f, "Modifies health of Ritual Shield");
            li_fulingshamanRitualShieldGrowthCap = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualShieldGrowthCap", 800f, "Modifies Max health growth, increases by 1 per cast");
            li_fulingshamanRitualProjectile = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualProjectile", 100f, "Modifies the damage of Ritual Projectile");
            li_fulingshamanRitualPassiveEitr = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualPassiveEitr", 50f, "Modifies bonus Eitr passive");
            li_fulingshamanRitualPassiveCarry = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualPassiveCarry", 100f, "Modifies Carry Weight reduction");
            // deathsquito
            li_deathsquitoRelentlessHoming = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessHoming", 100f, "Modifies projectile Homing aggression");
            li_deathsquitoRelentlessPassive = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessPassive", 10f, "Modifies % bonus pierce damage on projectiles");
            //fuling brute unchangeable? xD
            // surtling
            li_surtlingHarbingerCharges = config("Essence Surtling Modifiers", "li_surtlingHarbingerCharges", 10f, "Modifies the number of casts per Core");
            li_surtlingHarbingerBurn = config("Essence Surtling Modifiers", "li_surtlingHarbingerBurn", 100f, "Modifies the Aoe Dmg");
            li_surtlingHarbingerMinDistance = config("Essence Surtling Modifiers", "li_surtlingHarbingerMinDistance", 8f, "Modifies the distance a creature has to be from another for a rift to spawn");
            // drake
            li_drakeThreeFreezeProjectile = config("Essence Drake Modifiers", "li_drakeThreeFreezeProjectile", 50f, "Modifies the % weapon damage for Three Freeze");
            // growth
            li_growthAncientTarProjectile = config("Essence Growth Modifiers", "li_growthAncientTarProjectile", 50f, "Modifies the % weapon damage for Ancient Tar");
            li_growthAncientTarPassive = config("Essence Growth Modifiers", "li_growthAncientTarPassive", 10f, "Modifies the % weapon damage for Ancient Tar Passive for Weapons");
            // greydwarfshaman
            li_greydwarfshamanDubiousHealPlayer = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealPlayer", 50f, "Modifies the Aoe heal Players receive");
            li_greydwarfshamanDubiousHealCreature = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealCreature", 20f, "Modifies the % max health Aoe heal ally creatures receive");
            li_greydwarfshamanDubiousHealPassive = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealPassive", 2f, "Modifies the Passive regen multiplier ");
            li_greydwarfshamanDubiousHealPassiveEitr = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealPassiveEitr", 20f, "Modifies bonus Eitr passive");
            // troll
            li_trollTrollTossProjectile = config("Essence Troll Modifiers", "li_trollTrollTossProjectile", 100f, "Modifies the damage of Toss Projectile");
            li_trollTrollTossPassive = config("Essence Troll Modifiers", "li_trollTrollTossPassive", 100f, "Modifies the bonus health passive");
            // dvergr
            li_dvergrRandomizeIceProjectile = config("Essence Dvergr Modifiers", "li_dvergrRandomizeIceProjectile", 5f, "Modifies the % weapon damage of Randomize Ice Projectile");
            li_dvergrRandomizeFireProjectile = config("Essence Dvergr Modifiers", "li_dvergrRandomizeFireProjectile", 50f, "Modifies the % weapon damage of Randomize Fire Projectile");
            li_dvergrRandomizeHealPlayer = config("Essence Dvergr Modifiers", "li_dvergrRandomizeHealPlayer", 110f, "Modifies the Aoe heal Players receive");
            li_dvergrRandomizeHealCreature = config("Essence Dvergr Modifiers", "li_dvergrRandomizeHealCreature", 50f, "Modifies the % max health Aoe heal ally creatures receive");
            li_dvergrRandomizeCost = config("Essence Dvergr Modifiers", "li_dvergrRandomizeCost", 50f, "Modifies the Eitr cost to cast");
            li_dvergrRandomizePassiveEitr = config("Essence Dvergr Modifiers", "li_dvergrRandomizePassiveEitr", 80f, "Modifies bonus Eitr passive");
            li_dvergrRandomizePassive = config("Essence Dvergr Modifiers", "li_dvergrRandomizePassive", 60f, "Modifies the % pierce damage passive for Crossbow Weapons");
            // leech
            li_leechBloodSiphonStack = config("Essence Leech Modifiers", "li_leechBloodSiphonStack", 10f, "Modifies the Blood Stacks per marked kill");
            li_leechBloodSiphonStackCap = config("Essence Leech Modifiers", "li_leechBloodSiphonStackCap", 500f, "Modifies the Max Blood Stacks you can hold");
            // bonemass
            li_bonemassMassReleaseSummonDuration = config("Essence BoneMass Modifiers", "li_bonemassMassReleaseSummonDuration", 70f, "Modifies the time before summons die");
            li_bonemassMassReleaseProjectile = config("Essence BoneMass Modifiers", "li_bonemassMassReleaseProjectile", 100f, "Modifies the Damage of the Mass Release Projectile");
            // greydwarfbrute
            li_greydwarfbruteBashMultiplier = config("Essence Greydwarf Brute Modifiers", "li_greydwarfbruteBashMultiplier", 2f, "Modifies the Damage multiplier");
            li_greydwarfbruteRangedReductionPassive = config("Essence Greydwarf Brute Modifiers", "li_greydwarfbruteRangedReductionPassive", 2f, "Modifies the Damage multiplier of active melee hit");
            // fuling
            li_fulingLonginusMultiplier = config("Essence Fuling Modifiers", "li_fulingLonginusMultiplier", 3f, "Modifies the Damage multiplier of active spear throw");
            li_fulingLonginusPassiveBlockMultiplier = config("Essence Fuling Modifiers", "li_fulingLonginusPassiveBlockMultiplier", 2f, "Modifies the Block force multiplier passive");
            li_fulingLonginusPassiveMotivated = config("Essence Fuling Modifiers", "li_fulingLonginusPassiveMotivated", 60f, "Modifies the Bonus Stamina when holding gold");
            li_fulingLonginusPassiveDemotivated = config("Essence Fuling Modifiers", "li_fulingLonginusPassiveDemotivated", 50f, "Percentage Stamina reduction when not holding gold");
            // gjall
            li_gjallGjallarhornSummonDuration = config("Essence Gjall Modifiers", "li_gjallGjallarhornSummonDuration", 70f, "Modifies the time before summons die");
            li_gjallGjallarhornProjectile = config("Essence Gjall Modifiers", "li_gjallGjallarhornProjectile", 130f, "Modifies the Damage of the Gjallarhorn Projectile");
            li_gjallGjallarhornArmor = config("Essence Gjall Modifiers", "li_gjallGjallarhornArmor", 50f, "Bonus Armor passive");
            // greydwarf
            li_greydwarfPebbleProjectile = config("Essence Greydwarf Modifiers", "li_greydwarfPebbleProjectile", 20f, "Modifies the Damage of the Pubble Projectile");
            li_greydwarfPebblePassiveCarry = config("Essence Greydwarf Modifiers", "li_greydwarfPebblePassiveCarry", 50f, "Modifies the bonus Carry Weight Passive");
            li_greydwarfPebbleForestAnger = config("Essence Greydwarf Modifiers", "li_greydwarfPebbleForestAnger", 10f, "Modifies % Bonus Damage Forest creature do to you");
            // elder
            li_elderAncientAwePassive = config("Essence Elder Modifiers", "li_elderAncientAwePassive", 3f, "Modifies the Passive regen multiplier");
            // blob
            li_blobFumes = config("Essence Blob Modifiers", "li_blobFumes", 50f, "Modifies the % weapon damage added to Fumes");
            // skeleton
            li_skeletonVigilSummons = config("Essence Skeleton Modifiers", "li_skeletonVigilSummons", 10f, "Modifies the number of ghosts summoned");
            li_skeletonVigilSummonDuration = config("Essence Skeleton Modifiers", "li_skeletonVigilSummonDuration", 70f, "Modifies the time before summons die");
            li_skeletonVigilSoulCap = config("Essence Skeleton Modifiers", "li_skeletonVigilSoulCap", 300f, "Modifies the number of souls you can store");
            // abomination
            li_abominationBaneArmor = config("Essence Abomination Modifiers", "li_abominationBaneArmor", 30f, "Modifies bonus armor passive");
            li_abominationBaneHealth = config("Essence Abomination Modifiers", "li_abominationBaneHealth", 5f, "Modifies % health reduction");
            //wraith
            li_wraithTwinSoulsArmor = config("Essence Wraith Modifiers", "li_wraithTwinSoulsArmor", 10f, "Modifies armor reduction amount");
            li_wraithTwinSoulsPassive = config("Essence Wraith Modifiers", "li_wraithTwinSoulsPassive", 15f, "Modifies the % weapon spirit damage passive");
            //draugr
            li_draugrForgottenRot = config("Essence Draugr Modifiers", "li_draugrForgottenRot", 5f, "Modifies % dmg reduction system");
            li_draugrForgottenPassiveCarry = config("Essence Draugr Modifiers", "li_draugrForgottenPassiveCarry", 60f, "Modifies the bonus Carry Weight Passive");
            li_draugrForgottenActive = config("Essence Draugr Modifiers", "li_draugrForgottenActive", 10f, "Modifies % bonus dmg active to Axes and Bows");
            //draugr elite
            li_draugreliteFallenHeroRot = config("Essence Draugr Elite Modifiers", "li_draugreliteFallenHeroRot", 10f, "Modifies % dmg reduction system");
            li_draugreliteFallenHeroPassiveCarry = config("Essence Draugr Elite Modifiers", "li_draugreliteFallenHeroPassiveCarry", 70f, "Modifies the bonus Carry Weight Passive");
            lidraugreliteFallenHeroActive = config("Essence Draugr Elite Modifiers", "lidraugreliteFallenHeroActive", 20f, "Modifies % bonus dmg active to Swords and Polearms");
            //geirrhafa
            li_geirrhafaIceAgeAoe = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgeAoe", 70f, "Modifies Active Damage");
            li_geirrhafaIceAgePassiveEitr = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgePassiveEitr", 70f, "Modifies bonus Eitr Passive");
            li_geirrhafaIceAgePassive = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgePassive", 30f, "Modifies the frost damage passive for Weapons");
            //cultist
            li_cultistLoneSunAoe = config("Essence Cultist Modifiers", "li_cultistLoneSunAoe", 50f, "Modifies Active Damage");
            li_cultistLoneSunPassive = config("Essence Cultist Modifiers", "li_cultistLoneSunPassive", 15f, "Modifies the fire damage passive for Weapons");

            
            
            
               
            
            LackingImaginationGlobal.ConfigStrings = new Dictionary<string, float>();
            LackingImaginationGlobal.ConfigStrings.Clear();

            //Global
            LackingImaginationGlobal.ConfigStrings.Add("li_cooldownMultiplier", li_cooldownMultiplier.Value);

            
            // Essence
            LackingImaginationGlobal.ConfigStrings.Add("li_deerHorizonHaste", li_deerHorizonHaste.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deerHorizonHastePassive", li_deerHorizonHastePassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_eikthyrBlitzPassive", li_eikthyrBlitzPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_eikthyrBlitz", li_eikthyrBlitz.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fenringMoonlitLeap", li_fenringMoonlitLeap.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fenringMoonlitLeapPassive", li_fenringMoonlitLeapPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_loxWildTremor", li_loxWildTremor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_loxWildTremorPassive", li_loxWildTremorPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wolfRavenousHunger", li_wolfRavenousHunger.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wolfRavenousHungerStaminaPassive", li_wolfRavenousHungerStaminaPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wolfRavenousHungerPassive", li_wolfRavenousHungerPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingshamanRitualShield", li_fulingshamanRitualShield.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingshamanRitualShieldGrowthCap", li_fulingshamanRitualShieldGrowthCap.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingshamanRitualProjectile", li_fulingshamanRitualProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingshamanRitualPassiveEitr", li_fulingshamanRitualPassiveEitr.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingshamanRitualPassiveCarry", li_fulingshamanRitualPassiveCarry.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deathsquitoRelentlessHoming", li_deathsquitoRelentlessHoming.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deathsquitoRelentlessPassive", li_deathsquitoRelentlessPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerCharges", li_surtlingHarbingerCharges.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerBurn", li_surtlingHarbingerBurn.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerMinDistance", li_surtlingHarbingerMinDistance.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_drakeThreeFreezeProjectile", li_drakeThreeFreezeProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_growthAncientTarProjectile", li_growthAncientTarProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_growthAncientTarPassive", li_growthAncientTarPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfshamanDubiousHealPlayer", li_greydwarfshamanDubiousHealPlayer.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfshamanDubiousHealCreature", li_greydwarfshamanDubiousHealCreature.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfshamanDubiousHealPassive", li_greydwarfshamanDubiousHealPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfshamanDubiousHealPassiveEitr", li_greydwarfshamanDubiousHealPassiveEitr.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_trollTrollTossProjectile", li_trollTrollTossProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_trollTrollTossPassive", li_trollTrollTossPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizeIceProjectile", li_dvergrRandomizeIceProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizeFireProjectile", li_dvergrRandomizeFireProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizeHealPlayer", li_dvergrRandomizeHealPlayer.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizeHealCreature", li_dvergrRandomizeHealCreature.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizeCost", li_dvergrRandomizeCost.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizePassiveEitr", li_dvergrRandomizePassiveEitr.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizePassive", li_dvergrRandomizePassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_leechBloodSiphonStack", li_leechBloodSiphonStack.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_leechBloodSiphonStackCap", li_leechBloodSiphonStackCap.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_bonemassMassReleaseSummonDuration", li_bonemassMassReleaseSummonDuration.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_bonemassMassReleaseProjectile", li_bonemassMassReleaseProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfbruteBashMultiplier", li_greydwarfbruteBashMultiplier.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfbruteRangedReductionPassive", li_greydwarfbruteRangedReductionPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingLonginusMultiplier", li_fulingLonginusMultiplier.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingLonginusPassiveBlockMultiplier", li_fulingLonginusPassiveBlockMultiplier.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingLonginusPassiveMotivated", li_fulingLonginusPassiveMotivated.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingLonginusPassiveDemotivated", li_fulingLonginusPassiveDemotivated.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_gjallGjallarhornSummonDuration", li_gjallGjallarhornSummonDuration.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_gjallGjallarhornProjectile", li_gjallGjallarhornProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_gjallGjallarhornArmor", li_gjallGjallarhornArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfPebbleProjectile", li_greydwarfPebbleProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfPebblePassiveCarry", li_greydwarfPebblePassiveCarry.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfPebbleForestAnger", li_greydwarfPebbleForestAnger.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_elderAncientAwePassive", li_elderAncientAwePassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_blobFumes", li_blobFumes.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_skeletonVigilSummons", li_skeletonVigilSummons.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_skeletonVigilSummonDuration", li_skeletonVigilSummonDuration.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_skeletonVigilSoulCap", li_skeletonVigilSoulCap.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_abominationBaneArmor", li_abominationBaneArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_abominationBaneHealth", li_abominationBaneHealth.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsArmor", li_wraithTwinSoulsArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsPassive", li_wraithTwinSoulsPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugrForgottenRot", li_draugrForgottenRot.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugrForgottenPassiveCarry", li_draugrForgottenPassiveCarry.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugrForgottenActive", li_draugrForgottenActive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugreliteFallenHeroRot", li_draugreliteFallenHeroRot.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugreliteFallenHeroPassiveCarry", li_draugreliteFallenHeroPassiveCarry.Value);
            LackingImaginationGlobal.ConfigStrings.Add("lidraugreliteFallenHeroActive", lidraugreliteFallenHeroActive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_geirrhafaIceAgeAoe", li_geirrhafaIceAgeAoe.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_geirrhafaIceAgePassiveEitr", li_geirrhafaIceAgePassiveEitr.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_geirrhafaIceAgePassive", li_geirrhafaIceAgePassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_cultistLoneSunAoe", li_cultistLoneSunAoe.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_cultistLoneSunPassive", li_cultistLoneSunPassive.Value);
            
            
            
            
            
            
            _ = ConfigSync.AddConfigEntry(Ability1_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability1_Hotkey_Combo);
            _ = ConfigSync.AddConfigEntry(Ability2_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability2_Hotkey_Combo);
            _ = ConfigSync.AddConfigEntry(Ability3_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability3_Hotkey_Combo);
            _ = ConfigSync.AddConfigEntry(Ability4_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability4_Hotkey_Combo);
            _ = ConfigSync.AddConfigEntry(Ability5_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability5_Hotkey_Combo);
            
            _ = ConfigSync.AddConfigEntry(li_cooldownMultiplier);
            _ = ConfigSync.AddConfigEntry(showAbilityIcons);
            _ = ConfigSync.AddConfigEntry(iconAlignment);
            _ = ConfigSync.AddConfigEntry(icon_X_Offset);
            _ = ConfigSync.AddConfigEntry(icon_Y_Offset);

            _ = ConfigSync.AddConfigEntry(li_deerHorizonHaste);
            _ = ConfigSync.AddConfigEntry(li_deerHorizonHastePassive);
            _ = ConfigSync.AddConfigEntry(li_eikthyrBlitzPassive);
            _ = ConfigSync.AddConfigEntry(li_eikthyrBlitz);
            _ = ConfigSync.AddConfigEntry(li_fenringMoonlitLeap);
            _ = ConfigSync.AddConfigEntry(li_fenringMoonlitLeapPassive);
            _ = ConfigSync.AddConfigEntry(li_loxWildTremor);
            _ = ConfigSync.AddConfigEntry(li_loxWildTremorPassive);
            _ = ConfigSync.AddConfigEntry(li_wolfRavenousHunger);
            _ = ConfigSync.AddConfigEntry(li_wolfRavenousHungerStaminaPassive);
            _ = ConfigSync.AddConfigEntry(li_wolfRavenousHungerPassive);
            _ = ConfigSync.AddConfigEntry(li_fulingshamanRitualShield);
            _ = ConfigSync.AddConfigEntry(li_fulingshamanRitualShieldGrowthCap);
            _ = ConfigSync.AddConfigEntry(li_fulingshamanRitualProjectile);
            _ = ConfigSync.AddConfigEntry(li_fulingshamanRitualPassiveEitr);
            _ = ConfigSync.AddConfigEntry(li_fulingshamanRitualPassiveCarry);
            _ = ConfigSync.AddConfigEntry(li_deathsquitoRelentlessHoming);
            _ = ConfigSync.AddConfigEntry(li_deathsquitoRelentlessPassive);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerCharges);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerBurn);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerMinDistance);
            _ = ConfigSync.AddConfigEntry(li_drakeThreeFreezeProjectile);
            _ = ConfigSync.AddConfigEntry(li_growthAncientTarProjectile);
            _ = ConfigSync.AddConfigEntry(li_growthAncientTarPassive);
            _ = ConfigSync.AddConfigEntry(li_greydwarfshamanDubiousHealPlayer);
            _ = ConfigSync.AddConfigEntry(li_greydwarfshamanDubiousHealCreature);
            _ = ConfigSync.AddConfigEntry(li_greydwarfshamanDubiousHealPassive);
            _ = ConfigSync.AddConfigEntry(li_greydwarfshamanDubiousHealPassiveEitr);
            _ = ConfigSync.AddConfigEntry(li_trollTrollTossProjectile);
            _ = ConfigSync.AddConfigEntry(li_trollTrollTossPassive);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizeIceProjectile);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizeFireProjectile);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizeHealPlayer);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizeHealCreature);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizeCost);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizePassiveEitr);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizePassive);
            _ = ConfigSync.AddConfigEntry(li_leechBloodSiphonStack);
            _ = ConfigSync.AddConfigEntry(li_leechBloodSiphonStackCap);
            _ = ConfigSync.AddConfigEntry(li_bonemassMassReleaseSummonDuration);
            _ = ConfigSync.AddConfigEntry(li_bonemassMassReleaseProjectile);
            _ = ConfigSync.AddConfigEntry(li_greydwarfbruteBashMultiplier);
            _ = ConfigSync.AddConfigEntry(li_greydwarfbruteRangedReductionPassive);
            _ = ConfigSync.AddConfigEntry(li_fulingLonginusMultiplier);
            _ = ConfigSync.AddConfigEntry(li_fulingLonginusPassiveBlockMultiplier);
            _ = ConfigSync.AddConfigEntry(li_fulingLonginusPassiveMotivated);
            _ = ConfigSync.AddConfigEntry(li_fulingLonginusPassiveDemotivated);
            _ = ConfigSync.AddConfigEntry(li_gjallGjallarhornSummonDuration);
            _ = ConfigSync.AddConfigEntry(li_gjallGjallarhornProjectile);
            _ = ConfigSync.AddConfigEntry(li_gjallGjallarhornArmor);
            _ = ConfigSync.AddConfigEntry(li_greydwarfPebbleProjectile);
            _ = ConfigSync.AddConfigEntry(li_greydwarfPebblePassiveCarry);
            _ = ConfigSync.AddConfigEntry(li_greydwarfPebbleForestAnger);
            _ = ConfigSync.AddConfigEntry(li_elderAncientAwePassive);
            _ = ConfigSync.AddConfigEntry(li_blobFumes);
            _ = ConfigSync.AddConfigEntry(li_skeletonVigilSummons);
            _ = ConfigSync.AddConfigEntry(li_skeletonVigilSummonDuration);
            _ = ConfigSync.AddConfigEntry(li_skeletonVigilSoulCap);
            _ = ConfigSync.AddConfigEntry(li_abominationBaneArmor);
            _ = ConfigSync.AddConfigEntry(li_abominationBaneHealth);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsArmor);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsPassive);
            _ = ConfigSync.AddConfigEntry(li_draugrForgottenRot);
            _ = ConfigSync.AddConfigEntry(li_draugrForgottenPassiveCarry);
            _ = ConfigSync.AddConfigEntry(li_draugrForgottenActive);
            _ = ConfigSync.AddConfigEntry(li_draugreliteFallenHeroRot);
            _ = ConfigSync.AddConfigEntry(li_draugreliteFallenHeroPassiveCarry);
            _ = ConfigSync.AddConfigEntry(lidraugreliteFallenHeroActive);
            _ = ConfigSync.AddConfigEntry(li_geirrhafaIceAgeAoe);
            _ = ConfigSync.AddConfigEntry(li_geirrhafaIceAgePassiveEitr);
            _ = ConfigSync.AddConfigEntry(li_geirrhafaIceAgePassive);
            _ = ConfigSync.AddConfigEntry(li_cultistLoneSunAoe);
            _ = ConfigSync.AddConfigEntry(li_cultistLoneSunPassive);     
                
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();

        }

        
        private static void InitAnimation()
        {
            replacementMap["IceNova"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "IceNova",
                // ["Block idle"] = "BlockExternal",
            };
            replacementMap["RootSummon"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "RootSummon",
            };
        }
        

        [HarmonyPatch(typeof(ZoneSystem), "Start")]
        public static class ZoneSystem_Awake_Patch
        {
            public static void Postfix(ZoneSystem __instance)
            {
                GameObject location_harbinger = ExpMethods.GetLocationFromZoneSystem(ZoneSystem.m_instance, "FireHole");
                foreach (Transform transform_harbinger in location_harbinger.transform)
                {
                    if (transform_harbinger.name == "gas_flame")
                    {
                         LackingImaginationV2Plugin.fx_Harbinger = transform_harbinger.gameObject;
                        break;
                    }
                    
                    creatureAnimatorGeirrhafa = ZNetScene.instance.GetPrefab("Fenring_Cultist").gameObject.transform.Find("Visual").GetComponent<Animator>();
                    foreach (AnimationClip clip in creatureAnimatorGeirrhafa.runtimeAnimatorController.animationClips)
                    {
                        if (clip.name == "Frost AoE Spell Attack 3 Burst") // Replace with the actual name of the clip you're looking for.
                        {
                            creatureAnimationClipGeirrhafaIceNova = clip;
                            break; // Exit the loop once you've found the clip.
                        }
                    }
                    // if (creatureAnimationClipGeirrhafaIceNova != null)
                    // {
                    //     LogWarning($"T1.");
                    // }
                    
                    creatureAnimatorElder = ZNetScene.instance.GetPrefab("gd_king").gameObject.transform.Find("Visual").GetComponent<Animator>();
                    foreach (AnimationClip clip in creatureAnimatorElder.runtimeAnimatorController.animationClips)
                    {
                        if (clip.name == "Standing 1H Magic Attack 03") // Replace with the actual name of the clip you're looking for.
                        {
                            creatureAnimationClipElderSummon = clip;
                            break; // Exit the loop once you've found the clip.
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        private static class Patch_Player_Start
        {
            private static void Postfix(Player __instance)
            {
                
                if (creatureAnimationClipGeirrhafaIceNova !=null && creatureAnimationClipElderSummon !=null) //ZNetScene.instance
                {
                    LogWarning($"animations good");
                    AnimationClip copyOfCreatureAnimationClipGeirrhafaIceNova = Instantiate(creatureAnimationClipGeirrhafaIceNova);
                    ExternalAnimations["IceNova"] = copyOfCreatureAnimationClipGeirrhafaIceNova;
                    AnimationClip copyOfcreatureAnimationClipElderSummon = Instantiate(creatureAnimationClipElderSummon);
                    ExternalAnimations["RootSummon"] = copyOfcreatureAnimationClipElderSummon;
                  
                    LackingImaginationV2Plugin.InitAnimation();

                    if (CustomRuntimeControllers.Count == 0 && Player.m_localPlayer is not null)
                    {
                        CustomRuntimeControllers["Original"] = MakeAOC(new Dictionary<string, string>(), __instance.m_animator.runtimeAnimatorController);
                        CustomRuntimeControllers["IceNovaControl"] = MakeAOC(replacementMap["IceNova"], __instance.m_animator.runtimeAnimatorController);
                        CustomRuntimeControllers["RootSummonControl"] = MakeAOC(replacementMap["RootSummon"], __instance.m_animator.runtimeAnimatorController);
                       
                    }
                }
            }
        }
        private static RuntimeAnimatorController MakeAOC(Dictionary<string, string> replacement, RuntimeAnimatorController ORIGINAL)
        {
            AnimatorOverrideController aoc = new(ORIGINAL);
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new();
            foreach (AnimationClip animation in aoc.animationClips)
            {
                string name = animation.name;
                if (replacement.TryGetValue(name, out string value))
                {
                    // LogWarning($"T8.{name}");
                    AnimationClip newClip = Instantiate(ExternalAnimations[value]);
                    newClip.name = name;
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, newClip));
                }
                else
                {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, animation));
                }
            }
            aoc.ApplyOverrides(anims);
            return aoc;
        }
        
        public static void FastReplaceRAC(Player player, RuntimeAnimatorController replace)
        {
            if (player.m_animator.runtimeAnimatorController == replace)
            {
                return;
            }
            player.m_animator.runtimeAnimatorController = replace;
            player.m_animator.Update(Time.deltaTime);
        }
        
        [HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.RPC_SetTrigger))]
        private static class Patch_ZSyncAnimation_RPC_SetTrigger
        {
            private static void Prefix(ZSyncAnimation __instance, string name)
            {
                if (__instance.GetComponent<Player>() is { } player)
                {
                    string controllerName = "Original";
                    if (xGeirrhafaEssence.GeirrhafaController)
                    {
                         controllerName = "IceNovaControl";
                    }
                    if (xElderEssence.ElderController)
                    {
                        controllerName = "RootSummonControl";
                    }
                   
                    // in case this is called before the first Player.Start
                    if (CustomRuntimeControllers.TryGetValue(controllerName, out RuntimeAnimatorController controller))
                    {
                        FastReplaceRAC(player, controller);
                    }
                }
            }
        }
        
        
        
        
        
        
        
        
        public static bool TakeInput(Player p)
        {
            bool result = (!(bool)Chat.instance || !Chat.instance.HasFocus()) && !Console.IsVisible() && !TextInput.IsVisible() && !StoreGui.IsVisible() && !InventoryGui.IsVisible() && !Menu.IsVisible() && (!(bool)TextViewer.instance || !TextViewer.instance.IsVisible()) && !Minimap.IsOpen() && !GameCamera.InFreeFly();
            if (p.IsDead() || p.InCutscene() || p.IsTeleporting())
            {
                result = false;
            }
            return result;
        }

        int maxJumps = xBlobEssencePassive.canDoubleJump + xGrowthEssencePassive.canDoubleJump;
        public static int jumpCount = 0;
        
        
        [HarmonyPatch(typeof(Player), "Update", null)]
        public class AbilityInput_Postfix
        {
            public static void Postfix(Player __instance, ref float ___m_maxAirAltitude, ref Rigidbody ___m_body, ref float ___m_lastGroundTouch /*,ref Animator ___m_animator, float ___m_waterLevel*/)
            {
                
                Player localPlayer = Player.m_localPlayer;
                if (localPlayer != null && playerEnabled)
                {
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability1_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(0, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability2_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(1, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability3_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(2, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability4_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(3, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability5_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(4, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                    }
                    
                    int maxJumps = xBlobEssencePassive.canDoubleJump + xGrowthEssencePassive.canDoubleJump + xHareEssencePassive.canDoubleJump;

                    if (__instance.GetSEMan().HaveStatusEffect("SE_LuckyFoot")) maxJumps *= 2;
                    
                    if (ZInput.GetButtonDown("Jump") && !__instance.IsDead() && !__instance.InAttack() && !__instance.IsEncumbered() 
                        && !__instance.InDodge() && !__instance.IsKnockedBack())
                    {
                        // LackingImaginationV2Plugin.Log($"x {maxJumps}");
                        if (!__instance.IsOnGround() && LackingImaginationV2Plugin.jumpCount < maxJumps) // Check if there are remaining jumps
                        {
                            Vector3 velVec = __instance.GetVelocity();
                            velVec.y = 0f;                    
                            ___m_body.velocity = (velVec * 2f) + new Vector3(0, 10f, 0f); // You can adjust the jump velocity as needed
                            LackingImaginationV2Plugin.jumpCount++;
                            ___m_maxAirAltitude = 0;
                            ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance)).SetTrigger("jump");
                        }
                        else if (__instance.IsOnGround())
                        {
                            LackingImaginationV2Plugin.jumpCount = 0; // Reset jump count when landing
                        }
                    }
                }
            }
        }
        

        
        [HarmonyPatch(typeof(Hud), "UpdateStatusEffects")]
        public static class SkillIcon_Patch
        {
            public static void Postfix(Hud __instance)
            {
                if(__instance != null && showAbilityIcons.Value)
                {
                    if(abilitiesStatus == null)
                    { 
                        Debug.Log("== null plug");
                        abilitiesStatus = new List<RectTransform>();
                        abilitiesStatus.Clear(); 
                        for (int i = 0; i < EquipSlotCount; i++) 
                        {
                            LackingImaginationV2Plugin.abilitiesStatus.Add(null);
                        }
                    }
                    if (abilitiesStatus != null)
                    {
                        for (int j = 0; j < abilitiesStatus.Count; j++)
                        {
                            if (abilitiesStatus[j] != null)
                            {
                                RectTransform rectTransform2 = abilitiesStatus[j];
                                Image component = rectTransform2.Find("Icon").GetComponent<Image>();
                                string iconText = "";
                                if (j == 0)
                                {
                                    component.sprite = AbilitySprites[0];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability1_CoolDown"))
                                    {
                                        component.color = abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability1_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = Ability1_Hotkey.Value;
                                        if (LackingImaginationV2Plugin.Ability1_Hotkey_Combo.Value != "")
                                        {
                                            iconText += " + " + Ability1_Hotkey_Combo.Value;
                                        }
                                    }
                                }
                                else if (j == 1)
                                {

                                    component.sprite = AbilitySprites[1];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability2_CoolDown"))
                                    {
                                        component.color = abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability2_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = Ability2_Hotkey.Value;
                                        if (LackingImaginationV2Plugin.Ability2_Hotkey_Combo.Value != "")
                                        {
                                            iconText += " + " + Ability2_Hotkey_Combo.Value;
                                        }
                                    }
                                }
                                else if (j == 2)
                                {
                                    component.sprite = AbilitySprites[2];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability3_CoolDown"))
                                    {
                                        component.color = abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability3_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = Ability3_Hotkey.Value;
                                        if (LackingImaginationV2Plugin.Ability3_Hotkey_Combo.Value != "")
                                        {
                                            iconText += " + " + Ability3_Hotkey_Combo.Value;
                                        }
                                    }
                                }
                                else if (j == 3)
                                {
                                    component.sprite = AbilitySprites[3];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability4_CoolDown"))
                                    {
                                        component.color = abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability4_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = Ability4_Hotkey.Value;
                                        if (LackingImaginationV2Plugin.Ability4_Hotkey_Combo.Value != "")
                                        {
                                            iconText += " + " + Ability4_Hotkey_Combo.Value;
                                        }
                                    }
                                }
                                else if (j == 4)
                                {
                                    component.sprite = AbilitySprites[4];
                                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Ability5_CoolDown"))
                                    {
                                        component.color = abilityCooldownColor;
                                        iconText = StatusEffect.GetTimeString(Player.m_localPlayer.GetSEMan()
                                            .GetStatusEffect("Ability5_CoolDown".GetStableHashCode())
                                            .GetRemaningTime());
                                    }
                                    else
                                    {
                                        component.color = Color.white;
                                        iconText = Ability5_Hotkey.Value;
                                        if (LackingImaginationV2Plugin.Ability5_Hotkey_Combo.Value != "")
                                        {
                                            iconText += " + " + Ability5_Hotkey_Combo.Value;
                                        }
                                    }
                                }
                                //rectTransform2.GetComponentInChildren<Text>().text = Localization.instance.Localize((Ability1.Name).ToString());
                                Text component2 = rectTransform2.Find("TimeText").GetComponent<Text>();
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

        [HarmonyPatch(typeof(Player), "Update")]
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

        [HarmonyPatch(typeof(Player), "ActivateGuardianPower", null)]
        public class ActivatePowerPrevention_Patch
        {
            public static bool Prefix(Player __instance, ref bool __result)
            {                
                if (!UseGuardianPower)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Player), "StartGuardianPower", null)]
        public class StartPowerPrevention_Patch
        {
            public static bool Prefix(Player __instance, ref bool __result)
            {
                if (!UseGuardianPower)
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
        [HarmonyPatch(typeof(Player), "OnInventoryChanged")]
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
        
        // biomeDictionary
        //Biome Exp
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateBiome))]
        public class BiomeExp
        {
            public static void Prefix()
            {
                foreach (KeyValuePair<string, Heightmap.Biome> kvp in biomeDictionary)
                {
                    ExpMethods.BiomeExpMethod(kvp.Key, kvp.Value);
                    // Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
        }
        
        // dungeonDictionary
        //Dungeon Exp
        [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.SetForceEnvironment))]
        public  class DungeonEnterDetection
        {
            public static void Postfix(string env)
            {
                if(dungeonDictionary.ContainsKey(env))
                {
                    ExpMethods.dungeonExpMethod(dungeonDictionary[env]);
                }
            }
        }
        [HarmonyPatch(typeof(MusicMan), nameof(MusicMan.HandleLocationMusic))]
        public class DungeonMusicDetection
        {
            public static void Postfix(ref string currentMusic)
            {
                if(dungeonMusicDictionary.ContainsKey(currentMusic))
                {
                    ExpMethods.dungeonExpMethod(dungeonMusicDictionary[currentMusic]);
                }
            }
        }
        
        // [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        [HarmonyPatch(typeof(Player), "OnSpawned")]
        public static class PlayerModNotification
        {
            public static void Postfix(Player __instance)
            {
                Tutorial.TutorialText LI = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label ="1Lacking Imagination",
                    m_name = "Lacking_Imagination",
                    m_text = "This World is vast and dangerous. \n Explore, Kill and Conquer to grow your power!",
                    
                    m_topic = "Broaden your horizons!"
                };
                if (!Tutorial.instance.m_texts.Contains(LI))
                {
                    Tutorial.instance.m_texts.Add(LI);
                }

                __instance.ShowTutorial("Lacking_Imagination");
                
                //Biome Entries
                Tutorial.TutorialText _blackForestExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xBlack Forest",
                    m_name = "BlackForest_Exp",
                    m_text = "The Black Forest, home of the wood spirits, invaded by the lesser undead.",
                     
                    m_topic = "Black Forest"
                };
                if (!Tutorial.instance.m_texts.Contains(_blackForestExp))
                {
                    Tutorial.instance.m_texts.Add(_blackForestExp);
                }

                Tutorial.TutorialText _medowsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xMeadows",
                    m_name = "Meadows_Exp",
                    m_text = "A peaceful land, yet untouched by evil.",
                     
                    m_topic = "Meadows"
                };
                if (!Tutorial.instance.m_texts.Contains(_medowsExp))
                {
                    Tutorial.instance.m_texts.Add(_medowsExp);
                }
                Tutorial.TutorialText _plainsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xPlains",
                    m_name = "Plains_Exp",
                    m_text = "A vast land claimed under the rule of the Eternal Blaze.",
                     
                    m_topic = "Plains"
                };
                if (!Tutorial.instance.m_texts.Contains(_plainsExp))
                {
                    Tutorial.instance.m_texts.Add(_plainsExp);
                }
                Tutorial.TutorialText _mountainExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xMountain",
                    m_name = "Mountain_Exp",
                    m_text = "Frigid spears piercing out from the lands, dominated by the Sky Empresses kin.",
                     
                    m_topic = "Mountain"
                };
                if (!Tutorial.instance.m_texts.Contains(_mountainExp))
                {
                    Tutorial.instance.m_texts.Add(_mountainExp);
                }
                Tutorial.TutorialText _oceanExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xOcean",
                    m_name = "Ocean_Exp",
                    m_text = "Turbulent winds and deep waters, the shadow of death rises from below.",
                     
                    m_topic = "Ocean"
                };
                if (!Tutorial.instance.m_texts.Contains(_oceanExp))
                {
                    Tutorial.instance.m_texts.Add(_oceanExp);
                }
                Tutorial.TutorialText _swampExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xSwamp",
                    m_name = "Swamp_Exp",
                    m_text = "",
                     
                    m_topic = "Swamp"
                };
                if (!Tutorial.instance.m_texts.Contains(_swampExp))
                {
                    Tutorial.instance.m_texts.Add(_swampExp);
                }
                Tutorial.TutorialText _mistLandsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xMistlands",
                    m_name = "Mistlands_Exp",
                    m_text = "",
                     
                    m_topic = "Mistlands"
                };
                if (!Tutorial.instance.m_texts.Contains(_mistLandsExp))
                {
                    Tutorial.instance.m_texts.Add(_mistLandsExp);
                } 
                Tutorial.TutorialText _ashLandsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xAshLands",
                    m_name = "AshLands_Exp",
                    m_text = "",
                     
                    m_topic = "AshLands"
                };
                if (!Tutorial.instance.m_texts.Contains(_ashLandsExp))
                {
                    Tutorial.instance.m_texts.Add(_ashLandsExp);
                } 
                Tutorial.TutorialText _deepNorthExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xDeep North",
                    m_name = "DeepNorth_Exp",
                    m_text = "",
                     
                    m_topic = "Deep North"
                };
                if (!Tutorial.instance.m_texts.Contains(_deepNorthExp))
                {
                    Tutorial.instance.m_texts.Add(_deepNorthExp);
                }

                // Boss Trophy Entries
                Tutorial.TutorialText _eikthyrExp = new Tutorial.TutorialText
                {
                    m_label = "xEikthyr",
                    m_name = "Eikthyr_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Eikthyr"
                };
                if (!Tutorial.instance.m_texts.Contains(_eikthyrExp))
                {
                    Tutorial.instance.m_texts.Add(_eikthyrExp);
                }
                Tutorial.TutorialText _bonemassExp = new Tutorial.TutorialText
                {
                    m_label = "xBoneMass",
                    m_name = "BoneMass_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "BoneMass"
                };
                if (!Tutorial.instance.m_texts.Contains(_bonemassExp))
                {
                    Tutorial.instance.m_texts.Add(_bonemassExp);
                }
                Tutorial.TutorialText _moderExp = new Tutorial.TutorialText
                {
                    m_label = "xModer",
                    m_name = "Moder_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Moder"
                };
                if (!Tutorial.instance.m_texts.Contains(_moderExp))
                {
                    Tutorial.instance.m_texts.Add(_moderExp);
                } 
                Tutorial.TutorialText _seekerQueenExp = new Tutorial.TutorialText
                {
                    m_label = "xSeeker Queen",
                    m_name = "SeekerQueen_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Seeker Queen"
                };
                if (!Tutorial.instance.m_texts.Contains(_seekerQueenExp))
                {
                    Tutorial.instance.m_texts.Add(_seekerQueenExp);
                }
                Tutorial.TutorialText _theElderExp = new Tutorial.TutorialText
                {
                    m_label = "xThe Elder",
                    m_name = "TheElder_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "The Elder"
                };
                if (!Tutorial.instance.m_texts.Contains(_theElderExp))
                {
                    Tutorial.instance.m_texts.Add(_theElderExp);
                }
                Tutorial.TutorialText _yagluthrExp = new Tutorial.TutorialText
                {
                    m_label = "xYagluth",
                    m_name = "Yagluth_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Yagluth"
                };
                if (!Tutorial.instance.m_texts.Contains(_yagluthrExp))
                {
                    Tutorial.instance.m_texts.Add(_yagluthrExp);
                }

                //Mini-Boss Trophy Entries
                Tutorial.TutorialText _abominationExp = new Tutorial.TutorialText
                {
                    m_label = "xAbomination",
                    m_name = "Abomination_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Abomination"
                };
                if (!Tutorial.instance.m_texts.Contains(_abominationExp))
                {
                    Tutorial.instance.m_texts.Add(_abominationExp);
                }
                Tutorial.TutorialText _stoneGolemExp = new Tutorial.TutorialText
                {
                    m_label = "xStone Golem",
                    m_name = "StoneGolem_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Stone Golem"
                };
                if (!Tutorial.instance.m_texts.Contains(_stoneGolemExp))
                {
                    Tutorial.instance.m_texts.Add(_stoneGolemExp);
                }
                Tutorial.TutorialText _trollExp = new Tutorial.TutorialText
                {
                    m_label = "xTroll",
                    m_name = "Troll_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Troll"
                };
                if (!Tutorial.instance.m_texts.Contains(_trollExp))
                {
                    Tutorial.instance.m_texts.Add(_trollExp);
                }
                
                //Enemy Trophy Entries
                Tutorial.TutorialText _blobExp = new Tutorial.TutorialText
                {
                    m_label = "xBlob",
                    m_name = "Blob_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Blob"
                };
                if (!Tutorial.instance.m_texts.Contains(_blobExp))
                {
                    Tutorial.instance.m_texts.Add(_blobExp);
                }
                Tutorial.TutorialText _deathsquitoExp = new Tutorial.TutorialText
                {
                    m_label = "xDeathsquito",
                    m_name = "Deathsquito_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Deathsquito"
                };
                if (!Tutorial.instance.m_texts.Contains(_deathsquitoExp))
                {
                    Tutorial.instance.m_texts.Add(_deathsquitoExp);
                }
                Tutorial.TutorialText _fenringExp = new Tutorial.TutorialText
                {
                    m_label = "xFenring",
                    m_name = "Fenring_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Fenring"
                };
                if (!Tutorial.instance.m_texts.Contains(_fenringExp))
                {
                    Tutorial.instance.m_texts.Add(_fenringExp);
                }
                Tutorial.TutorialText _drakeExp = new Tutorial.TutorialText
                {
                    m_label = "xDrake",
                    m_name = "Drake_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Drake"
                };
                if (!Tutorial.instance.m_texts.Contains(_drakeExp))
                {
                    Tutorial.instance.m_texts.Add(_drakeExp);
                }
                Tutorial.TutorialText _draugrEliteExp = new Tutorial.TutorialText
                {
                    m_label = "xDraugr Elite",
                    m_name = "DraugrElite_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Draugr Elite"
                };
                if (!Tutorial.instance.m_texts.Contains(_draugrEliteExp))
                {
                    Tutorial.instance.m_texts.Add(_draugrEliteExp);
                }
                Tutorial.TutorialText _draugrExp = new Tutorial.TutorialText
                {
                    m_label = "xDraugr",
                    m_name = "Draugr_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Draugr"
                };
                if (!Tutorial.instance.m_texts.Contains(_draugrExp))
                {
                    Tutorial.instance.m_texts.Add(_draugrExp);
                }
                Tutorial.TutorialText _fulingExp = new Tutorial.TutorialText
                {
                    m_label ="xFuling",
                    m_name = "Fuling_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Fuling"
                };
                if (!Tutorial.instance.m_texts.Contains(_fulingExp))
                {
                    Tutorial.instance.m_texts.Add(_fulingExp);
                } 
                Tutorial.TutorialText _gjallExp = new Tutorial.TutorialText
                {
                    m_label ="xGjall",
                    m_name = "Gjall_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Gjall"
                };
                if (!Tutorial.instance.m_texts.Contains(_gjallExp))
                {
                    Tutorial.instance.m_texts.Add(_gjallExp);
                }
                Tutorial.TutorialText _greydwarfBruteExp = new Tutorial.TutorialText
                {
                    m_label ="xGreydwarf Brute",
                    m_name = "GreydwarfBrute_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Greydwarf Brute"
                };
                if (!Tutorial.instance.m_texts.Contains(_greydwarfBruteExp))
                {
                    Tutorial.instance.m_texts.Add(_greydwarfBruteExp);
                }
                Tutorial.TutorialText _greydwarfShamanExp = new Tutorial.TutorialText
                {
                    m_label ="xGreydwarf Shaman",
                    m_name = "GreydwarfShaman_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Greydwarf Shaman"
                };
                if (!Tutorial.instance.m_texts.Contains(_greydwarfShamanExp))
                {
                    Tutorial.instance.m_texts.Add(_greydwarfShamanExp);
                }
                Tutorial.TutorialText _greydwarfExp = new Tutorial.TutorialText
                {
                    m_label ="xGreydwarf",
                    m_name = "Greydwarf_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Greydwarf"
                };
                if (!Tutorial.instance.m_texts.Contains(_greydwarfExp))
                {
                    Tutorial.instance.m_texts.Add(_greydwarfExp);
                }
                Tutorial.TutorialText _leechfExp = new Tutorial.TutorialText
                {
                    m_label ="xLeech",
                    m_name = "Leech_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Leech"
                };
                if (!Tutorial.instance.m_texts.Contains(_leechfExp))
                {
                    Tutorial.instance.m_texts.Add(_leechfExp);
                }
                Tutorial.TutorialText _seekerSoldierExp = new Tutorial.TutorialText
                {
                    m_label ="xSeeker Soldier",
                    m_name = "SeekerSoldier_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Seeker Soldier"
                };
                if (!Tutorial.instance.m_texts.Contains(_seekerSoldierExp))
                {
                    Tutorial.instance.m_texts.Add(_seekerSoldierExp);
                }    
                Tutorial.TutorialText _seekerExp = new Tutorial.TutorialText
                {
                    m_label ="xSeeker",
                    m_name = "Seeker_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Seeker"
                };
                if (!Tutorial.instance.m_texts.Contains(_seekerExp))
                {
                    Tutorial.instance.m_texts.Add(_seekerExp);
                } 
                Tutorial.TutorialText _skeletonExp = new Tutorial.TutorialText
                {
                    m_label ="xSkeleton",
                    m_name = "Skeleton_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Skeleton"
                };
                if (!Tutorial.instance.m_texts.Contains(_skeletonExp))
                {
                    Tutorial.instance.m_texts.Add(_skeletonExp);
                } 
                Tutorial.TutorialText _surtlingExp = new Tutorial.TutorialText
                {
                    m_label ="xSurtling",
                    m_name = "Surtling_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Surtling"
                };
                if (!Tutorial.instance.m_texts.Contains(_surtlingExp))
                {
                    Tutorial.instance.m_texts.Add(_surtlingExp);
                }
                Tutorial.TutorialText _tickExp = new Tutorial.TutorialText
                {
                    m_label ="xTick",
                    m_name = "Tick_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Tick"
                };
                if (!Tutorial.instance.m_texts.Contains(_tickExp))
                {
                    Tutorial.instance.m_texts.Add(_tickExp);
                }
                    
                    
                //Dungeon Enemy Trophy Entries
                Tutorial.TutorialText _cultistExp = new Tutorial.TutorialText
                {
                    m_label = "xCultist",
                    m_name = "Cultist_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Cultist"
                };
                if (!Tutorial.instance.m_texts.Contains(_cultistExp))
                {
                    Tutorial.instance.m_texts.Add(_cultistExp);
                }
                Tutorial.TutorialText _fulingBerserkerExp = new Tutorial.TutorialText
                {
                    m_label ="xFuling Berserker",
                    m_name = "FulingBerserker_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Fuling Berserker"
                };
                if (!Tutorial.instance.m_texts.Contains(_fulingBerserkerExp))
                {
                    Tutorial.instance.m_texts.Add(_fulingBerserkerExp);
                }
                Tutorial.TutorialText _fulingShamanExp = new Tutorial.TutorialText
                {
                    m_label ="xFuling Shaman",
                    m_name = "FulingShaman_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Fuling Shaman"
                };
                if (!Tutorial.instance.m_texts.Contains(_fulingShamanExp))
                {
                    Tutorial.instance.m_texts.Add(_fulingShamanExp);
                }
                Tutorial.TutorialText _growthExp = new Tutorial.TutorialText
                {
                    m_label ="xGrowth",
                    m_name = "Growth_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Growth"
                };
                if (!Tutorial.instance.m_texts.Contains(_growthExp))
                {
                    Tutorial.instance.m_texts.Add(_growthExp);
                }
                Tutorial.TutorialText _rancidRemainsExp = new Tutorial.TutorialText
                {
                    m_label ="xRancid Remains",
                    m_name = "RancidRemains_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Rancid Remains"
                };
                if (!Tutorial.instance.m_texts.Contains(_rancidRemainsExp))
                {
                    Tutorial.instance.m_texts.Add(_rancidRemainsExp);
                }
                Tutorial.TutorialText _serpentExp = new Tutorial.TutorialText
                {
                    m_label ="xSerpent",
                    m_name = "Serpent_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Serpent"
                };
                if (!Tutorial.instance.m_texts.Contains(_serpentExp))
                {
                    Tutorial.instance.m_texts.Add(_serpentExp);
                }
                Tutorial.TutorialText _ulvExp = new Tutorial.TutorialText
                {
                    m_label ="xUlv",
                    m_name = "Ulv_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Ulv"
                };
                if (!Tutorial.instance.m_texts.Contains(_ulvExp))
                {
                    Tutorial.instance.m_texts.Add(_ulvExp);
                }
                Tutorial.TutorialText _wraithExp = new Tutorial.TutorialText
                {
                    m_label ="xWraith",
                    m_name = "Wraith_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Wraith"
                };
                if (!Tutorial.instance.m_texts.Contains(_wraithExp))
                {
                    Tutorial.instance.m_texts.Add(_wraithExp);
                }
                    
                //Animal Trophy Entries
                Tutorial.TutorialText _dvergrExp = new Tutorial.TutorialText
                {
                    m_label = "xDvergr",
                    m_name = "Dvergr_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Dvergr"
                };
                if (!Tutorial.instance.m_texts.Contains(_dvergrExp))
                {
                    Tutorial.instance.m_texts.Add(_dvergrExp);
                }
                Tutorial.TutorialText _boarExp = new Tutorial.TutorialText
                {
                    m_label = "xBoar",
                    m_name = "Boar_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Boar"
                };
                if (!Tutorial.instance.m_texts.Contains(_boarExp))
                {
                    Tutorial.instance.m_texts.Add(_boarExp);
                }
                Tutorial.TutorialText _deerExp = new Tutorial.TutorialText
                {
                    m_label = "xDeer",
                    m_name = "Deer_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Deer"
                };
                if (!Tutorial.instance.m_texts.Contains(_deerExp))
                {
                    Tutorial.instance.m_texts.Add(_deerExp);
                } 
                Tutorial.TutorialText _hareExp = new Tutorial.TutorialText
                {
                    m_label = "xHare",
                    m_name = "Hare_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Hare"
                };
                if (!Tutorial.instance.m_texts.Contains(_hareExp))
                {
                    Tutorial.instance.m_texts.Add(_hareExp);
                }
                Tutorial.TutorialText _loxExp = new Tutorial.TutorialText
                {
                    m_label = "xLox",
                    m_name = "Lox_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Lox"
                };
                if (!Tutorial.instance.m_texts.Contains(_loxExp))
                {
                    Tutorial.instance.m_texts.Add(_loxExp);
                }
                Tutorial.TutorialText _neckExp = new Tutorial.TutorialText
                {
                    m_label = "xNeck",
                    m_name = "Neck_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Neck"
                };
                if (!Tutorial.instance.m_texts.Contains(_neckExp))
                {
                    Tutorial.instance.m_texts.Add(_neckExp);
                }
                Tutorial.TutorialText _wolfExp = new Tutorial.TutorialText
                {
                    m_label = "xWolf",
                    m_name = "Wolf_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Wolf"
                };
                if (!Tutorial.instance.m_texts.Contains(_wolfExp))
                {
                    Tutorial.instance.m_texts.Add(_wolfExp);
                }
                
                //Hildr Enemies
                Tutorial.TutorialText _brennaExp = new Tutorial.TutorialText
                {
                    m_label = "xBrenna",
                    m_name = "Brenna_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Brenna"
                };
                if (!Tutorial.instance.m_texts.Contains(_brennaExp))
                {
                    Tutorial.instance.m_texts.Add(_brennaExp);
                }
                Tutorial.TutorialText _geirrhafaExp = new Tutorial.TutorialText
                {
                    m_label = "xGeirrhafa",
                    m_name = "Geirrhafa_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Geirrhafa"
                };
                if (!Tutorial.instance.m_texts.Contains(_geirrhafaExp))
                {
                    Tutorial.instance.m_texts.Add(_geirrhafaExp);
                }
                Tutorial.TutorialText _zilExp = new Tutorial.TutorialText
                {
                    m_label = "xZil",
                    m_name = "Zil_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Zil"
                };
                if (!Tutorial.instance.m_texts.Contains(_zilExp))
                {
                    Tutorial.instance.m_texts.Add(_zilExp);
                }
                Tutorial.TutorialText _thungrExp = new Tutorial.TutorialText
                {
                    m_label = "xThungr",
                    m_name = "Thungr_Exp",
                    m_text = "Essence Power: ",
                     
                    m_topic = "Thungr"
                };
                if (!Tutorial.instance.m_texts.Contains(_thungrExp))
                {
                    Tutorial.instance.m_texts.Add(_thungrExp);
                }

                //Dungeon Entries
                Tutorial.TutorialText _infectedMineExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xInfected Mine",
                    m_name = "InfectedMine_Exp",
                    m_text = " ",
                     
                    m_topic = "Infected Mine"
                };
                if (!Tutorial.instance.m_texts.Contains(_infectedMineExp))
                {
                    Tutorial.instance.m_texts.Add(_infectedMineExp);
                }
                Tutorial.TutorialText _frostCaveExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xFrost Cave Mine",
                    m_name = "FrostCave_Exp",
                    m_text = " ",
                     
                    m_topic = "Frost Cave"
                };
                if (!Tutorial.instance.m_texts.Contains(_frostCaveExp))
                {
                    Tutorial.instance.m_texts.Add(_frostCaveExp);
                }
                Tutorial.TutorialText _sunkenCryptExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xSunken Crypt",
                    m_name = "SunkenCrypt_Exp",
                    m_text = " ",
                     
                    m_topic = "Sunken Crypt"
                };
                if (!Tutorial.instance.m_texts.Contains(_sunkenCryptExp))
                {
                    Tutorial.instance.m_texts.Add(_sunkenCryptExp);
                }
                Tutorial.TutorialText _burialChambersTrollCaveExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xBurial Chambers & Troll Cave",
                    m_name = "BurialChambers_TrollCave_Exp",
                    m_text = " ",
                     
                    m_topic = "Burial Chambers & Troll Cave"
                };
                if (!Tutorial.instance.m_texts.Contains(_burialChambersTrollCaveExp))
                {
                    Tutorial.instance.m_texts.Add(_burialChambersTrollCaveExp);
                }
                //Open Dungeons
                Tutorial.TutorialText _goblinCampExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xGoblin Camp",
                    m_name = "GoblinCamp_Exp",
                    m_text = " ",
                     
                    m_topic = "Goblin Camp"
                };
                if (!Tutorial.instance.m_texts.Contains(_goblinCampExp))
                {
                    Tutorial.instance.m_texts.Add(_goblinCampExp);
                }
                Tutorial.TutorialText _forestCryptHildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xHildir Forest Crypt",
                    m_name = "ForestCryptHildir_Exp",
                    m_text = " ",
                     
                    m_topic = "Hildir Forest Crypt"
                };
                if (!Tutorial.instance.m_texts.Contains(_forestCryptHildirExp))
                {
                    Tutorial.instance.m_texts.Add(_forestCryptHildirExp);
                }
                Tutorial.TutorialText _caveHildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xHildir Cave",
                    m_name = "CaveHildir_Exp",
                    m_text = " ",
                     
                    m_topic = "Hildir Cave"
                };
                if (!Tutorial.instance.m_texts.Contains(_caveHildirExp))
                {
                    Tutorial.instance.m_texts.Add(_caveHildirExp);
                }
                Tutorial.TutorialText _plainsFortHildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xHildir Plains Fort",
                    m_name = "PlainsFortHildir_Exp",
                    m_text = " ",
                     
                    m_topic = "Hildir Plains Fort"
                };
                if (!Tutorial.instance.m_texts.Contains(_plainsFortHildirExp))
                {
                    Tutorial.instance.m_texts.Add(_plainsFortHildirExp);
                }
            }
        }
        
      
        
       // Essence Slots ////////////////////////////////////////////
       public static void Log(string message)
       {
           _instance.Logger.LogMessage(message);
       }
       public static void LogWarning(string message)
       {
           _instance.Logger.LogWarning(message);
       }
       public static void LogError(string message)
       {
           _instance.Logger.LogError(message);
       }
      
         
         public static CodeInstruction LogMessage(CodeInstruction instruction, int counter)
         {
             LackingImaginationV2Plugin.Log($"IL_{counter}: Opcode: {instruction.opcode} Operand: {instruction.operand}");
             return instruction;
         }
            
         public static CodeInstruction FindInstructionWithLabel(List<CodeInstruction> codeInstructions, int index, Label label)
         {
             if (index >= codeInstructions.Count)
                 return null;
                
             if (codeInstructions[index].labels.Contains(label))
                 return codeInstructions[index];
                
             return FindInstructionWithLabel(codeInstructions, index + 1, label);
         }
         
         // /////////////////////////////////////////////////////////

         


          private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                LackingImaginationV2Logger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                LackingImaginationV2Logger.LogError($"There was an issue loading your {ConfigFileName}");
                LackingImaginationV2Logger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable;
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}