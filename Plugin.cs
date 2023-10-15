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


        public static Dictionary<Heightmap.Biome, List<string>> biomeDictionary = new Dictionary<Heightmap.Biome, List<string>>
        {
            { Heightmap.Biome.Meadows, new List<string> {"Meadows_Exp", "2"} },
            { Heightmap.Biome.BlackForest, new List<string> {"BlackForest_Exp", "2"} },
            { Heightmap.Biome.Swamp, new List<string> {"Swamp_Exp", "2"} },
            { Heightmap.Biome.Ocean, new List<string> {"Ocean_Exp", "2"} },
            { Heightmap.Biome.Mountain, new List<string> {"Mountain_Exp", "2"} },
            { Heightmap.Biome.Plains, new List<string> {"Plains_Exp", "2"} },
            { Heightmap.Biome.Mistlands, new List<string> {"Mistlands_Exp", "2"} },
            { Heightmap.Biome.AshLands, new List<string> {"AshLands_Exp", "2"} },
            { Heightmap.Biome.DeepNorth, new List<string> {"DeepNorth_Exp", "2"} }
        };
        
        public static Dictionary<string, List<string>> locationDictionary = new Dictionary<string,List<string>>
        {
            { "Crypt2(Clone)",  new List<string> {"BurialChambers_Exp", "2"} },
            { "Crypt3(Clone)", new List<string> {"BurialChambers_Exp", "2"} },
            { "Crypt4(Clone)", new List<string> {"BurialChambers_Exp", "2"} },
            { "TrollCave02(Clone)", new List<string> {"TrollCave_Exp", "2"} },
            { "SunkenCrypt4(Clone)", new List<string> {"SunkenCrypt_Exp", "2"} },
            { "MountainCave02(Clone)", new List<string> {"FrostCave_Exp", "2"} },
            { "GoblinCamp2(Clone)", new List<string> {"GoblinCamp_Exp", "2"} },
            { "Mistlands_DvergrTownEntrance1(Clone)", new List<string> {"InfectedMine_Exp", "2"} },
            { "Mistlands_DvergrTownEntrance2(Clone)", new List<string> {"InfectedMine_Exp", "2"} },
            
            { "Hildir_crypt(Clone)", new List<string> {"ForestCryptHildir_Exp", "2"} },
            { "Hildir_cave(Clone)", new List<string> {"CaveHildir_Exp", "2"} },
            { "Hildir_plainsfortress(Clone)", new List<string> {"PlainsFortHildir_Exp", "2"} },
            
            { "Eikthyrnir(Clone)",  new List<string> {"EikthyrSacrifice_Exp", "2"} },
            { "GDKing(Clone)", new List<string> {"TheElderSacrifice_Exp", "2"} },
            { "Bonemass(Clone)", new List<string> {"BoneMassSacrifice_Exp", "2"} },
            { "Dragonqueen(Clone)", new List<string> {"ModerSacrifice_Exp", "2"} },
            { "GoblinKing(Clone)", new List<string> {"YagluthSacrifice_Exp", "2"} },
            { "Mistlands_DvergrBossEntrance1(Clone)", new List<string> {"SeekerQueenSeal_Exp", "2"} },
        };
        
        // public static Dictionary<string, string> dungeonMusicDictionary = new Dictionary<string, string>
        // {
        //     // { "GoblinCamp", "GoblinCamp_Exp" },
        //     // { "ForestCryptHildir", "ForestCryptHildir_Exp" },
        //     // // { "frostcaveshildir", "CaveHildir_Exp" }, // Setting forced environment CavesHildir //  Starting music frostcaveshildir
        //     // { "PlainsFortHildir", "PlainsFortHildir_Exp" },
        // };

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
            { "$item_eikthyr_essence", new List<string> { "Eikthyr_Essence_Drop","Eikthyr Essence", "Eikthyr", "0.5", "Blitz", "TrophyEikthyrEssence"} },
            { "$item_elder_essence", new List<string> { "TheElder_Essence_Drop", "The Elder Essence", "gd_king", "0.5", "Ancient Awe", "EssenceTrophyTheElder" } },
            { "$item_bonemass_essence", new List<string> { "BoneMass_Essence_Drop", "Bone Mass Essence", "Bonemass", "0.5", "Mass Release", "EssenceTrophyBonemass" } },
            { "$item_dragonqueen_essence", new List<string> { "Moder_Essence_Drop", "Moder Essence", "Dragon", "0.5", "Draconic Frost", "EssenceTrophyDragonQueen" } },
            { "$item_yagluth_essence", new List<string> { "Yagluth_Essence_Drop", "Yagluth Essence", "GoblinKing", "0.5", "Culmination", "EssenceTrophyGoblinKing" } },
            { "$item_seekerqueen_essence", new List<string> { "SeekerQueen_Essence_Drop", "Seeker Queen Essence", "SeekerQueen", "0.5", "An erie glow.", "EssenceTrophySeekerQueen" } },
            
            { "$item_abomination_essence", new List<string> { "Abomination_Essence_Drop", "Abomination Essence", "Abomination", "0.5", "Bane", "EssenceTrophyAbomination" } },
            { "$item_stonegolem_essence", new List<string> { "StoneGolem_Essence_Drop", "Stone Golem Essence", "StoneGolem", "0.5", "Core Overdrive", "EssenceTrophySGolem" } },
            { "$item_troll_essence", new List<string> { "Troll_Essence_Drop", "Troll Essence", "Troll", "0.5", "Troll Toss", "EssenceTrophyFrostTroll" } },
            
            { "$item_blob_essence", new List<string> { "Blob_Essence_Drop", "Blob Essence", "Blob", "0.5", "Fumes", "EssenceTrophyBlob" } },
            { "$item_boar_essence", new List<string> { "Boar_Essence_Drop", "Boar Essence", "Boar", "0.5", "Reckless Charge", "TrophyBoarEssence" } },
            { "$item_cultist_essence", new List<string> { "Cultist_Essence_Drop", "Cultist Essence", "Fenring_Cultist", "0.5", "Lone Sun", "EssenceTrophyCultist" } },
            { "$item_deathsquito_essence", new List<string> { "Deathsquito_Essence_Drop", "Deathsquito Essence", "Deathsquito", "0.5", "Relentless", "EssenceTrophyDeathsquito" } },
            { "$item_deer_essence", new List<string> { "Deer_Essence_Drop", "Deer Essence", "Deer", "0.5", "Horizon Haste", "EssenceTrophyDeer" } },
            { "$item_draugrelite_essence", new List<string> { "DraugrElite_Essence_Drop", "Draugr Elite Essence", "Draugr_Elite", "0.5", "Fallen Hero", "EssenceTrophyDraugrElite" } },
            { "$item_draugr_essence", new List<string> { "Draugr_Essence_Drop", "Draugr Essence", "Draugr", "0.5", "Forgotten", "EssenceTrophyDraugr" } },
            { "$item_dvergr_essence", new List<string> { "Dvergr_Essence_Drop", "Dvergr Essence", "Dverger", "0.5", "Randomize", "EssenceTrophyDvergr"} },
            { "$item_fenring_essence", new List<string> { "Fenring_Essence_Drop", "Fenring Essence", "Fenring", "0.5", "Moonlit Leap", "EssenceTrophyFenring" } },
            { "$item_gjall_essence", new List<string> { "Gjall_Essence_Drop", "Gjall Essence", "Gjall", "0.5", "Gjallarhorn", "EssenceTrophyGjall" } },
            { "$item_goblin_essence", new List<string> { "Goblin_Essence_Drop", "Fuling Essence", "Goblin", "0.5", "Longinus", "EssenceTrophyGoblin" } },
            { "$item_goblinbrute_essence", new List<string> { "GoblinBrute_Essence_Drop", "Fuling Berserker Essence", "GoblinBrute", "0.5", "Giantization", "TrophyGoblinBruteEssence" } },
            { "$item_goblinshaman_essence", new List<string> { "GoblinShaman_Essence_Drop", "Fuling Shaman Essence", "GoblinShaman", "0.5", "Ritual", "EssenceTrophyGoblinShaman" } },
            { "$item_greydwarf_essence", new List<string> { "Greydwarf_Essence_Drop", "Greydwarf Essence", "Greydwarf", "0.5", "Pebble", "EssenceTrophyGreydwarf" } },
            { "$item_greydwarfbrute_essence", new List<string> { "GreydwarfBrute_Essence_Drop", "Greydwarf Brute Essence", "Greydwarf_Elite", "0.5", "Bash", "EssenceTrophyGreydwarfBrute" } },
            { "$item_greydwarfshaman_essence", new List<string> { "GreydwarfShaman_Essence_Drop", "Greydwarf Shaman Essence", "Greydwarf_Shaman", "0.5", "Dubious Heal", "EssenceTrophyGreydwarfShaman" } },
            { "$item_growth_essence", new List<string> { "Growth_Essence_Drop", "Growth Essence", "BlobTar", "0.5", "Ancient Tar", "EssenceTrophyGrowth" } },
            { "$item_hare_essence", new List<string> { "Hare_Essence_Drop", "Hare Essence", "Hare", "0.5", "Lucky Foot", "EssenceTrophyHare" } },
            { "$item_hatchling_essence", new List<string> { "Drake_Essence_Drop", "Drake Essence", "Hatchling", "0.5", "Three Freeze", "EssenceTrophyHatchling" } },
            { "$item_leech_essence", new List<string> { "Leech_Essence_Drop", "Leech Essence", "Leech", "0.5", "Blood Siphon", "EssenceTrophyLeech" } },
            { "$item_lox_essence", new List<string> { "Lox_Essence_Drop", "Lox Essence", "Lox", "0.5", "Wild Tremor", "TrophyLoxEssence" } },
            { "$item_neck_essence", new List<string> { "Neck_Essence_Drop", "Neck Essence", "Neck", "0.5", "Splash", "TrophyNeckEssence" } },
            { "$item_seeker_essence", new List<string> { "Seeker_Essence_Drop","Seeker Essence", "Seeker", "0.5", "An erie glow.", "EssenceTrophySeeker" } },
            { "$item_seeker_brute_essence", new List<string> { "SeekerSoldier_Essence_Drop","Seeker Soldier Essence", "SeekerBrute", "0.5", "An erie glow.", "EsenceTrophySeekerBrute" } },
            { "$item_serpent_essence", new List<string> { "Serpent_Essence_Drop", "Sea Serpent Essence", "Serpent", "0.5", "Sea King", "EssenceTrophySerpent" } },
            { "$item_skeleton_essence", new List<string> { "Skeleton_Essence_Drop", "Skeleton Essence", "Skeleton", "0.5", "Vigil", "EssenceTrophySkeleton" } },
            { "$item_skeletonpoison_essence", new List<string> { "SkeletonPoison_Essence_Drop", "Rancid Remains Essence", "Skeleton_Poison", "0.5", "Rancorous", "EssenceTrophySkeletonPoison" } },
            { "$item_surtling_essence", new List<string> { "Surtling_Essence_Drop", "Surtling Essence", "Surtling", "0.5", "Harbinger", "EssenceTrophySurtling" } },
            { "$item_tick_essence", new List<string> { "Tick_Essence_Drop", "Tick Essence", "Tick", "0.5", "Blood Well", "EssenceTrophyTick" } },
            { "$item_ulv_essence", new List<string> { "Ulv_Essence_Drop", "Ulv Essence", "Ulv", "0.5", "Territorial Slumber", "EssenceTrophyUlv" } },
            { "$item_wolf_essence", new List<string> { "Wolf_Essence_Drop", "Wolf Essence", "Wolf", "0.5", "Ravenous Hunger", "TrophyWolfEssence" } },
            { "$item_wraith_essence", new List<string> { "Wraith_Essence_Drop", "Wraith Essence", "Wraith", "0.5", "Twin Souls", "EssenceTrophyWraith" } },
            
            { "$item_brenna_essence", new List<string> { "Brenna_Essence_Drop", "Brenna Essence", "Skeleton_Hildir", "0.5", "Vulcan", "EssenceTrophySkeletonHildir" } },
            { "$item_geirrhafa_essence", new List<string> { "Geirrhafa_Essence_Drop", "Geirrhafa Essence", "Fenring_Cultist_Hildir", "0.5", "Ice Age.", "EssenceTrophyCultist_Hildir" } },
            { "$item_zil_essence", new List<string> { "Zil_Essence_Drop", "Zil Essence", "GoblinShaman_Hildir", "0.5", "An erie glow.", "EssenceTrophyGoblinBruteBrosShaman" } },
            { "$item_thungr_essence", new List<string> { "Thungr_Essence_Drop", "Thungr Essence", "GoblinBrute_Hildir", "0.5", "An erie glow.", "EssenceTrophyGoblinBruteBrosBrute" } },
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
        
        // public static ConfigEntry<KeyboardShortcut> Ability1_Hotkey { get; set; }
        public static ConfigEntry<KeyCode> Ability1_Hotkey { get; set; }
        public static ConfigEntry<KeyCode> Ability1_Combokey { get; set; }
        public static ConfigEntry<KeyboardShortcut> Ability2_Hotkey { get; set; }
        public static ConfigEntry<KeyboardShortcut> Ability3_Hotkey { get; set; }
        public static ConfigEntry<KeyboardShortcut> Ability4_Hotkey { get; set; }
        public static ConfigEntry<KeyboardShortcut> Ability5_Hotkey { get; set; }

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
        
        
        public static ConfigEntry<float> li_draugrSynergyRot;
        public static ConfigEntry<float> li_skeletonSynergyBrenna;
        public static ConfigEntry<float> li_skeletonSynergyRancid;
        
        //Cooldown
        public static ConfigEntry<float> li_eikthyrBlitzCD;
        public static ConfigEntry<float> li_deerHorizonHasteCD;
        public static ConfigEntry<float> li_fenringMoonlitLeapCD;
        public static ConfigEntry<float> li_loxWildTremorCD;
        public static ConfigEntry<float> li_wolfRavenousHungerCD;
        public static ConfigEntry<float> li_fulingshamanRitualCD;
        public static ConfigEntry<float> li_deathsquitoRelentlessCD;
        public static ConfigEntry<float> li_surtlingHarbingerCD;
        public static ConfigEntry<float> li_fulingberserkerGiantizationCD;
        public static ConfigEntry<float> li_drakeThreeFreezeCD;
        public static ConfigEntry<float> li_growthAncientTarCD;
        public static ConfigEntry<float> li_trollTrollTossCD;
        public static ConfigEntry<float> li_greydwarfshamanDubiousHealCD;
        public static ConfigEntry<float> li_dvergrRandomizeCD;
        public static ConfigEntry<float> li_neckSplashCD;
        public static ConfigEntry<float> li_leechBloodSiphonCD;
        public static ConfigEntry<float> li_bonemassMassReleaseCD;
        public static ConfigEntry<float> li_greydwarfbruteBashCD;
        public static ConfigEntry<float> li_fulingLonginusCD;
        public static ConfigEntry<float> li_gjallGjallarhornCD;
        public static ConfigEntry<float> li_greydwarfPebbleCD;
        public static ConfigEntry<float> li_elderAncientAweCD;
        public static ConfigEntry<float> li_blobFumesCD;
        public static ConfigEntry<float> li_skeletonVigilCD;
        public static ConfigEntry<float> li_abominationBaneCD;
        public static ConfigEntry<float> li_wraithTwinSoulsCD;
        public static ConfigEntry<float> li_draugrForgottenCD;
        public static ConfigEntry<float> li_draugreliteFallenHeroCD;
        public static ConfigEntry<float> li_geirrhafaIceAgeCD;
        public static ConfigEntry<float> li_cultistLoneSunCD;
        public static ConfigEntry<float> li_hareLuckyFootCD;
        public static ConfigEntry<float> li_seaserpentSeaKingCD;
        public static ConfigEntry<float> li_tickBloodWellCD;
        public static ConfigEntry<float> li_moderDraconicFrostCD;
        public static ConfigEntry<float> li_boarRecklessChargeCD;
        public static ConfigEntry<float> li_stonegolemCoreOverdriveCD;
        public static ConfigEntry<float> li_yagluthCulminationCD;
        public static ConfigEntry<float> li_ulvTerritorialSlumberCD;
        public static ConfigEntry<float> li_brennaVulcanCD;
        public static ConfigEntry<float> li_rancidremainsRancorousCD;
        
        
        
        //Status Duration
        public static ConfigEntry<float> li_deerHorizonHasteSED;
        public static ConfigEntry<float> li_fenringMoonlitLeapSED;
        public static ConfigEntry<float> li_wolfRavenousHungerSED;
        public static ConfigEntry<float> li_deathsquitoRelentlessSED;
        public static ConfigEntry<float> li_surtlingHarbingerSED;
        public static ConfigEntry<float> li_fulingberserkerGiantizationSED;
        public static ConfigEntry<float> li_elderAncientAweSED;
        public static ConfigEntry<float> li_wraithTwinSoulsSED;
        public static ConfigEntry<float> li_draugrForgottenSED;
        public static ConfigEntry<float> li_draugreliteFallenHeroSED;
        public static ConfigEntry<float> li_hareLuckyFootSED;
        public static ConfigEntry<float> li_boarRecklessChargeSED;
        public static ConfigEntry<float> li_ulvTerritorialSlumberSED;
        
        
        //Essence
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
        public static ConfigEntry<float> li_deathsquitoRelentlessHomingRange;
        public static ConfigEntry<float> li_deathsquitoRelentlessPassive;
        public static ConfigEntry<float> li_surtlingHarbingerCharges;
        public static ConfigEntry<float> li_surtlingHarbingerBurn;
        public static ConfigEntry<float> li_surtlingHarbingerMinDistance;
        public static ConfigEntry<float> li_fulingberserkerGiantizationHealth;
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
        public static ConfigEntry<float> li_greydwarfbruteHealthPassive;
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
        public static ConfigEntry<float> li_abominationBaneAllySpeed;
        public static ConfigEntry<float> li_abominationBaneAllyHealth;
        public static ConfigEntry<float> li_abominationBaneAllyAttack;
        public static ConfigEntry<float> li_wraithTwinSoulsArmor;
        public static ConfigEntry<float> li_wraithTwinSoulsPassive;
        public static ConfigEntry<float> li_wraithTwinSoulsAllySpeed;
        public static ConfigEntry<float> li_wraithTwinSoulsAllyHealth;
        public static ConfigEntry<float> li_wraithTwinSoulsAllyAttack;
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
        public static ConfigEntry<float> li_hareLuckyFoot;
        public static ConfigEntry<float> li_hareLuckyFootArmor;
        public static ConfigEntry<float> li_seaserpentSeaKingProjectile;
        public static ConfigEntry<float> li_tickBloodWellLifeSteal;
        public static ConfigEntry<float> li_tickBloodWellArmor;
        public static ConfigEntry<float> li_tickBloodWellStackCap;
        public static ConfigEntry<float> li_moderDraconicFrostProjectile;
        public static ConfigEntry<float> li_moderDraconicFrostPassive;
        public static ConfigEntry<float> li_moderDraconicFrostDragonBreath;
        public static ConfigEntry<float> li_boarRecklessCharge;
        public static ConfigEntry<float> li_boarRecklessChargePassive;
        public static ConfigEntry<float> li_stonegolemCoreOverdriveStacks;
        public static ConfigEntry<float> li_stonegolemCoreOverdriveArmor;
        public static ConfigEntry<float> li_yagluthCulminationStaticCap;
        public static ConfigEntry<float> li_yagluthCulmination;
        public static ConfigEntry<float> li_ulvTerritorialSlumberComfort;
        public static ConfigEntry<float> li_ulvTerritorialSlumberStamina;
        public static ConfigEntry<float> li_ulvTerritorialSlumberSummonDuration;
        public static ConfigEntry<float> li_ulvTerritorialSlumberSummonHealth;
        public static ConfigEntry<float> li_brennaVulcanArmor;
        public static ConfigEntry<float> li_rancidremainsRancorousArmor;
        
       
        
        // public static List<string> equipedEssence = new();
        private static LackingImaginationV2Plugin _instance;

        // Prefabs type 1 //Pulled from in game
        public static GameObject fx_Harbinger;
        
        //Weapons
        public static GameObject GO_VulcanSwordBroken;
        public static GameObject GO_VulcanSword;
        public static GameObject fx_Vulcan;
        public static GameObject GO_RancorousMaceBroken;
        public static GameObject GO_RancorousMace;
        public static GameObject fx_Rancorous;
        
        //Prefabs type 2 //Pulled from assets
        public static GameObject fx_Giantization;
        public static GameObject fx_Bash;
        public static GameObject fx_Longinus;
        public static GameObject fx_TwinSouls;
        public static GameObject fx_BloodWell;
        public static GameObject fx_TerritorialSlumber;
        public static GameObject fx_BloodSiphon;
        public static GameObject fx_RavenousHunger;
        public static GameObject fx_Relentless;
        //Prefab3 //originals
        public static GameObject p_SeaKing;
        public static GameObject fx_RecklessCharge;
        public static GameObject fx_RecklessChargeHit;
        
        
        //Sounds
        public static GameObject sfx_Giantization;
        
        //Prefab4 new bundle
        public static GameObject SG_Spiked_Arms;
        public static GameObject StoneGolem_Player;
        public static GameObject SG_Club_Arms;
        public static GameObject SG_Hat;
        
        
        // Animation Clips // Pulled from in game
        public static Animator creatureAnimatorGeirrhafa;
        public static AnimationClip creatureAnimationClipGeirrhafaIceNova;
        public static AnimationClip creatureAnimationClipCultistSpray; // Flame Spell Attack
        public static Animator creatureAnimatorElder;
        public static AnimationClip creatureAnimationClipElderSummon;
        public static Animator creatureAnimatorFenring;
        public static AnimationClip creatureAnimationClipFenringLeapAttack; //Leap Attack//
        public static Animator creatureAnimatorGreyShaman;
        public static AnimationClip creatureAnimationClipGreyShamanHeal; //Standing 1H Cast Spell 01 
        public static Animator creatureAnimatorHaldor;
        public static AnimationClip creatureAnimationClipHaldorGreet; //Greet//
        public static Animator creatureAnimatorPlayer;
        public static AnimationClip creatureAnimationClipPlayerEmoteCower; 
        public static AnimationClip creatureAnimationClipPlayerMace2; 
        public static AnimationClip creatureAnimationClipPlayerEmotePoint; 
        public static AnimationClip creatureAnimationClipPlayerEmoteDespair; 
        public static Animator creatureAnimatorBrenna;
        public static AnimationClip creatureAnimationClipBrennaGroundStab;
        public static Animator creatureAnimatorDvergr;
        public static AnimationClip creatureAnimationClipDvergrStaffRaise;
        
        
        // Animation Clip swappers
        public static readonly Dictionary<string, AnimationClip> OutsideAnimations = new();
        public static readonly Dictionary<string, RuntimeAnimatorController> CustomizedRuntimeControllers = new();
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
                    LackingImaginationV2Plugin.li_stringList.Add(xSeaSerpentEssencePassive.SeaSerpentStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xTickEssencePassive.TickStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xStoneGolemEssencePassive.StoneGolemStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xYagluthEssencePassive.YagluthStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xBrennaEssencePassive.BrennaStats);
                    LackingImaginationV2Plugin.li_stringList.Add(xRancidRemainsEssencePassive.RancidRemainsStats);
                    
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
        
            Item VulkanB = new Item("essence_bundle_2", "VulcanBroken");
            VulkanB.Name.English("Broken Vulcan"); 
            VulkanB.Description.English("The flame is a mere fraction of Brenna's anguish.");
            Item Vulkan = new Item("essence_bundle_2", "Vulcan");
            Vulkan.Name.English("Vulcan"); 
            Vulkan.Description.English("The flame overflows with Brenna's anguish.");
            Item RancorousB = new Item("essence_bundle_2", "RancorousBroken");
            RancorousB.Name.English("Broken Rancorous"); 
            RancorousB.Description.English("The malice of a nameless sinner coats the weapon.");
            Item Rancorous = new Item("essence_bundle_2", "Rancorous");
            Rancorous.Name.English("Rancorous"); 
            Rancorous.Description.English("The malice of a nameless sinner permeates the weapon.");

            
            


            //Prefabs 2
            fx_Giantization = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "RotVariant1");
            fx_Bash = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantRed");
            fx_Longinus = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantYellow");
            fx_TwinSouls = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantWraith");
            fx_BloodWell = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "FireVariantTick");
            fx_TerritorialSlumber = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "UlvCircle");
            fx_BloodSiphon = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "LeechDebuff");
            fx_RavenousHunger = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "wolfHit");
            fx_Relentless = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "DeathEye");
            fx_Vulcan = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "VulkanFloor");
            fx_Rancorous = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "RancorousFloor");
            fx_RecklessCharge = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "BoarGuard");
            fx_RecklessChargeHit = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "BoarHit");
            
                
            //Prefab 3
            p_SeaKing = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "Serpent_projectile");
            
            //Prefab 4
            SG_Spiked_Arms = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_spikes_Player");
            StoneGolem_Player = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_Player");
            SG_Hat = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_hat_Player");
            SG_Club_Arms = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_clubs_Player");
            ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack1_spike_Player");
            ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack2_left_groundslam_Player");
            ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack3_spikesweep_Player");
            ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack_doublesmash_Player");
            
            
            //Sound Prefabs
            sfx_Giantization = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "sfx_goblinbrute_rage");
            
            
            
            abilitiesStatus.Clear();
            for (int i = 0; i < 5; i++)
            {
                abilitiesStatus.Add(null);
            }
            



            EssenceSlotsEnabled = Config.Bind("Toggles", "Enable Essence Slots", true, "Disabling this while items are in the slots will attempt to move them to your inventory.");

            Ability1_Hotkey = config("Keybinds", "Ability1_Hotkey", KeyCode.LeftAlt, "Ability 1 Hotkey");
            Ability1_Combokey = config("Keybinds", "Ability1_Combokey", KeyCode.Alpha1, "Ability 1 Combokey");

            Ability2_Hotkey = config("Keybinds", "Ability2_Hotkey", new KeyboardShortcut(KeyCode.LeftAlt, KeyCode.Alpha2), "Ability 2 Hotkey");
            Ability3_Hotkey = config("Keybinds", "Ability3_Hotkey", new KeyboardShortcut(KeyCode.LeftAlt, KeyCode.Alpha3), "Ability 3 Hotkey");
            Ability4_Hotkey = config("Keybinds", "Ability4_Hotkey", new KeyboardShortcut(KeyCode.LeftAlt, KeyCode.Alpha4), "Ability 4 Hotkey");
            Ability5_Hotkey = config("Keybinds", "Ability5_Hotkey", new KeyboardShortcut(KeyCode.LeftAlt, KeyCode.Alpha5), "Ability 5 Hotkey");
           
            //li_cooldownMultiplier = ConfigManager.RegisterModConfigVariable<float>(ModName, "vl_mce_cooldownMultiplier", 1f, "Modifiers", "This value multiplied on overall cooldown time of abilities", false);
            li_cooldownMultiplier = config("Modifiers", "CooldownMultiplier", 100f, "This value multiplied on overall cooldown time of abilities");
            
            //showAbilityIcons = ConfigManager.RegisterModConfigVariable<bool>(ModName, "showAbilityIcons", true, "Display", "Displays Icons on Hud for each ability", true);
            showAbilityIcons = config("Display", "showAbilityIcons", true, "Displays Icons on Hud for each ability");
            //iconAlignment = ConfigManager.RegisterModConfigVariable<string>(ModName, "iconAlignment", "horizontal", "Display", "Aligns icons horizontally or vertically off the guardian power icon; options are horizontal or vertical", true);
            iconAlignment = config("Display", "iconAlignment", "horizontal", "Aligns icons horizontally or vertically off the guardian power icon; options are horizontal or vertical");
            //icon_X_Offset = ConfigManager.RegisterModConfigVariable<float>(ModName, "icon_X_Offset", 0f, "Display", "Offsets the icon bar horizontally. The icon bar is anchored to the Guardian power icon.", true);
            icon_X_Offset = config("Display", "icon_X_Offset", 0f, "Offsets the icon bar horizontally. The icon bar is anchored to the Guardian power icon.");
            //icon_Y_Offset = ConfigManager.RegisterModConfigVariable<float>(ModName, "icon_Y_Offset", 0f, "Display", "Offsets the icon bar vertically. The icon bar is anchored to the Guardian power icon.", true);
            icon_Y_Offset = config("Display", "icon_Y_Offset", 0f, "Offsets the icon bar vertically. The icon bar is anchored to the Guardian power icon.");
            
            //Synergies
            //Draugr Synergy
            li_draugrSynergyRot = config("Essence Synergy Modifiers", "li_draugrSynergyRot", 5f, new ConfigDescription("Modifies % dmg reduction system when all Draugr essences are equipped", new AcceptableValueRange<float>(0f, 100f)));
            li_skeletonSynergyBrenna = config("Essence Synergy Modifiers", "li_skeletonSynergyBrenna", 25f, "Bonus fire damage Vigil Ghost do if Brenna essence equipped");
            li_skeletonSynergyRancid = config("Essence Synergy Modifiers", "li_skeletonSynergyRancid", 25f, "Bonus poison damage Vigil Ghost do if Rancid Remains essence equipped");

        
            
            
            //deer
            li_deerHorizonHasteCD = config("Essence Deer Modifiers", "li_deerHorizonHasteCD", 25f, "Cooldown");
            li_deerHorizonHasteSED = config("Essence Deer Modifiers", "li_deerHorizonHasteSED", 50f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_deerHorizonHaste = config("Essence Deer Modifiers", "li_deerHorizonHaste", 1.2f, "Modifies the movement speed for Horizon Haste");
            li_deerHorizonHastePassive = config("Essence Deer Modifiers", "li_deerHorizonHastePassive", 25f, "Bonus Stamina from Horizon Haste");
            // eikthyr
            li_eikthyrBlitzCD = config("Essence Eikthyr Modifiers", "li_eikthyrBlitzCD", 25f, "Cooldown");
            li_eikthyrBlitzPassive = config("Essence Eikthyr Modifiers", "li_eikthyrBlitzPassive", 10f, "Modifies the % lightning damage passive for Weapons");
            li_eikthyrBlitz = config("Essence Eikthyr Modifiers", "li_eikthyrBlitz", 20f, "Modifies % weapon lightning damage for Blitz");
            // fenring
            li_fenringMoonlitLeapCD = config("Essence Fenring Modifiers", "li_fenringMoonlitLeapCD", 30f, "Cooldown");
            li_fenringMoonlitLeapSED = config("Essence Fenring Modifiers", "li_fenringMoonlitLeapSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_fenringMoonlitLeap = config("Essence Fenring Modifiers", "li_fenringMoonlitLeap", 1.5f, "Modifies the jump force for Moonlit Leap");
            li_fenringMoonlitLeapPassive = config("Essence Fenring Modifiers", "li_fenringMoonlitLeapPassive", 10f, "Modifies the % slash damage passive for Fist Weapons");
            // lox
            li_loxWildTremorCD = config("Essence Lox Modifiers", "li_loxWildTremorCD", 25f, "Cooldown");
            li_loxWildTremor = config("Essence Lox Modifiers", "li_loxWildTremor", 1f, "Modifies the blunt damage for Wild Tremor");
            li_loxWildTremorPassive = config("Essence Lox Modifiers", "li_loxWildTremorPassive", 30f, "Bonus Health from Wild Tremor on eat");
            // wolf
            li_wolfRavenousHungerCD = config("Essence Wolf Modifiers", "li_wolfRavenousHungerCD", 120f, "Cooldown");
            li_wolfRavenousHungerSED = config("Essence Wolf Modifiers", "li_wolfRavenousHungerSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_wolfRavenousHunger = config("Essence Wolf Modifiers", "li_wolfRavenousHunger", 5f, new ConfigDescription("Modifies % max HP dmg for Ravenous Hunger", new AcceptableValueRange<float>(0f, 100f)));
            li_wolfRavenousHungerStaminaPassive = config("Essence Wolf Modifiers", "li_wolfRavenousHungerStaminaPassive", 65f, "Bonus Stamina per stage of Ravenous Hunger");
            li_wolfRavenousHungerPassive = config("Essence Wolf Modifiers", "li_wolfRavenousHungerPassive", 1f, "Modifies the Damage multiplier of Ravenous Hunger");
            // fulingshaman
            li_fulingshamanRitualCD = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualCD", 30f, "Cooldown");
            li_fulingshamanRitualShield = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualShield", 200f, "Modifies health of Ritual Shield");
            li_fulingshamanRitualShieldGrowthCap = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualShieldGrowthCap", 800f, "Modifies Max health growth, increases by 1 per cast");
            li_fulingshamanRitualProjectile = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualProjectile", 100f, "Modifies the damage of Ritual Projectile");
            li_fulingshamanRitualPassiveEitr = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualPassiveEitr", 50f, "Modifies bonus Eitr passive");
            li_fulingshamanRitualPassiveCarry = config("Essence Fuling Shaman Modifiers", "li_fulingshamanRitualPassiveCarry", 100f, "Modifies Carry Weight reduction");
            // deathsquito
            li_deathsquitoRelentlessCD = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessCD", 60f, "Cooldown");
            li_deathsquitoRelentlessSED = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_deathsquitoRelentlessHoming = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessHoming", 50f, "Modifies projectile Homing aggression");
            li_deathsquitoRelentlessHomingRange = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessHomingRange", 12f, "Modifies projectile Homing range");
            li_deathsquitoRelentlessPassive = config("Essence Deathsquito Modifiers", "li_deathsquitoRelentlessPassive", 10f, "Modifies % bonus pierce damage on projectiles");
            //surtling
            li_surtlingHarbingerCD = config("Essence Surtling Modifiers", "li_surtlingHarbingerCD", 60f, "Cooldown");
            li_surtlingHarbingerSED = config("Essence Surtling Modifiers", "li_surtlingHarbingerSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_surtlingHarbingerCharges = config("Essence Surtling Modifiers", "li_surtlingHarbingerCharges", 10f, "Modifies the number of casts per Core");
            li_surtlingHarbingerBurn = config("Essence Surtling Modifiers", "li_surtlingHarbingerBurn", 100f, "Modifies the Aoe Dmg");
            li_surtlingHarbingerMinDistance = config("Essence Surtling Modifiers", "li_surtlingHarbingerMinDistance", 8f, "Modifies the distance a creature has to be from another for a rift to spawn");
            // fuling brute unchangeable? xD
            li_fulingberserkerGiantizationCD = config("Essence Fuling Brute Modifiers", "li_fulingberserkerGiantizationCD", 60f, "Cooldown");
            li_fulingberserkerGiantizationSED = config("Essence Fuling Brute Modifiers", "li_fulingberserkerGiantizationSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_fulingberserkerGiantizationHealth = config("Essence Fuling Brute Modifiers", "li_fulingberserkerGiantizationHealth", 50f, "Modifies health increase passive");
            // drake
            li_drakeThreeFreezeCD = config("Essence Drake Modifiers", "li_drakeThreeFreezeCD", 12f, "Cooldown");
            li_drakeThreeFreezeProjectile = config("Essence Drake Modifiers", "li_drakeThreeFreezeProjectile", 50f, "Modifies the % weapon damage for Three Freeze");
            // growth
            li_growthAncientTarCD = config("Essence Growth Modifiers", "li_growthAncientTarCD", 15f, "Cooldown");
            li_growthAncientTarProjectile = config("Essence Growth Modifiers", "li_growthAncientTarProjectile", 50f, "Modifies the % weapon damage for Ancient Tar");
            li_growthAncientTarPassive = config("Essence Growth Modifiers", "li_growthAncientTarPassive", 10f, "Modifies the % weapon damage for Ancient Tar Passive for Weapons");
            // greydwarfshaman
            li_greydwarfshamanDubiousHealCD = config("Essence Greydwarf Modifiers", "li_greydwarfshamanDubiousHealCD", 60f, "Cooldown");
            li_greydwarfshamanDubiousHealPlayer = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealPlayer", 50f, "Modifies the Aoe heal Players receive");
            li_greydwarfshamanDubiousHealCreature = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealCreature", 20f, new ConfigDescription("Modifies the % max health Aoe heal ally creatures receive", new AcceptableValueRange<float>(0f, 100f)));
            li_greydwarfshamanDubiousHealPassive = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealPassive", 2f, "Modifies the Passive regen multiplier ");
            li_greydwarfshamanDubiousHealPassiveEitr = config("Essence Greydwarf Shaman Modifiers", "li_greydwarfshamanDubiousHealPassiveEitr", 20f, "Modifies bonus Eitr passive");
            // troll
            li_trollTrollTossCD = config("Essence Troll Modifiers", "li_trollTrollTossCD", 10f, "Cooldown");
            li_trollTrollTossProjectile = config("Essence Troll Modifiers", "li_trollTrollTossProjectile", 100f, "Modifies the damage of Toss Projectile");
            li_trollTrollTossPassive = config("Essence Troll Modifiers", "li_trollTrollTossPassive", 100f, "Modifies the bonus health passive");
            // dvergr
            li_dvergrRandomizeCD = config("Essence Dvergr Modifiers", "li_dvergrRandomizeCD", 2f, "Cooldown");
            li_dvergrRandomizeIceProjectile = config("Essence Dvergr Modifiers", "li_dvergrRandomizeIceProjectile", 5f, "Modifies the % weapon damage of Randomize Ice Projectile");
            li_dvergrRandomizeFireProjectile = config("Essence Dvergr Modifiers", "li_dvergrRandomizeFireProjectile", 50f, "Modifies the % weapon damage of Randomize Fire Projectile");
            li_dvergrRandomizeHealPlayer = config("Essence Dvergr Modifiers", "li_dvergrRandomizeHealPlayer", 110f, "Modifies the Aoe heal Players receive");
            li_dvergrRandomizeHealCreature = config("Essence Dvergr Modifiers", "li_dvergrRandomizeHealCreature", 50f, new ConfigDescription("Modifies the % max health Aoe heal ally creatures receive", new AcceptableValueRange<float>(0f, 100f)));
            li_dvergrRandomizeCost = config("Essence Dvergr Modifiers", "li_dvergrRandomizeCost", 50f, "Modifies the Eitr cost to cast");
            li_dvergrRandomizePassiveEitr = config("Essence Dvergr Modifiers", "li_dvergrRandomizePassiveEitr", 80f, "Modifies bonus Eitr passive");
            li_dvergrRandomizePassive = config("Essence Dvergr Modifiers", "li_dvergrRandomizePassive", 60f, "Modifies the % pierce damage passive for Crossbow Weapons");
            //neck
            li_neckSplashCD = config("Essence Neck Modifiers", "li_neckSplashCD", 3f, "Cooldown");
            // leech
            li_leechBloodSiphonCD = config("Essence Leech Modifiers", "li_leechBloodSiphonCD", 10f, "Cooldown");
            li_leechBloodSiphonStack = config("Essence Leech Modifiers", "li_leechBloodSiphonStack", 10f, "Modifies the Blood Stacks per marked kill");
            li_leechBloodSiphonStackCap = config("Essence Leech Modifiers", "li_leechBloodSiphonStackCap", 500f, "Modifies the Max Blood Stacks you can hold");
            // bonemass
            li_bonemassMassReleaseCD = config("Essence BoneMass Modifiers", "li_bonemassMassReleaseCD", 20f, "Cooldown");
            li_bonemassMassReleaseSummonDuration = config("Essence BoneMass Modifiers", "li_bonemassMassReleaseSummonDuration", 70f, "Modifies the time before summons die");
            li_bonemassMassReleaseProjectile = config("Essence BoneMass Modifiers", "li_bonemassMassReleaseProjectile", 100f, "Modifies the Damage of the Mass Release Projectile");
            // greydwarfbrute
            li_greydwarfbruteBashCD = config("Essence Greydwarf Brute Modifiers", "li_greydwarfbruteBashCD", 8f, "Cooldown");
            li_greydwarfbruteBashMultiplier = config("Essence Greydwarf Brute Modifiers", "li_greydwarfbruteBashMultiplier", 2f, "Modifies the Damage multiplier");
            li_greydwarfbruteRangedReductionPassive = config("Essence Greydwarf Brute Modifiers", "li_greydwarfbruteRangedReductionPassive", 2f, "Modifies the Damage multiplier of active melee hit");
            li_greydwarfbruteHealthPassive = config("Essence Greydwarf Brute Modifiers", "li_greydwarfbruteHealthPassive", 25f, "Modifies health increase passive");
            // fuling
            li_fulingLonginusCD = config("Essence Fuling Modifiers", "li_fulingLonginusCD", 8f, "Cooldown");
            li_fulingLonginusMultiplier = config("Essence Fuling Modifiers", "li_fulingLonginusMultiplier", 3f, "Modifies the Damage multiplier of active spear throw");
            li_fulingLonginusPassiveBlockMultiplier = config("Essence Fuling Modifiers", "li_fulingLonginusPassiveBlockMultiplier", 2f, "Modifies the Block force multiplier passive");
            li_fulingLonginusPassiveMotivated = config("Essence Fuling Modifiers", "li_fulingLonginusPassiveMotivated", 60f, "Modifies the Bonus Stamina when holding gold");
            li_fulingLonginusPassiveDemotivated = config("Essence Fuling Modifiers", "li_fulingLonginusPassiveDemotivated", 50f, "Percentage Stamina reduction when not holding coins");
            // gjall
            li_gjallGjallarhornCD = config("Essence Gjall Modifiers", "li_gjallGjallarhornCD", 25f, "Cooldown");
            li_gjallGjallarhornSummonDuration = config("Essence Gjall Modifiers", "li_gjallGjallarhornSummonDuration", 70f, "Modifies the time before summons die");
            li_gjallGjallarhornProjectile = config("Essence Gjall Modifiers", "li_gjallGjallarhornProjectile", 130f, "Modifies the Damage of the Gjallarhorn Projectile");
            li_gjallGjallarhornArmor = config("Essence Gjall Modifiers", "li_gjallGjallarhornArmor", 50f, "Bonus Armor passive");
            // greydwarf
            li_greydwarfPebbleCD = config("Essence Greydwarf Modifiers", "li_greydwarfPebbleCD", 5f, "Cooldown");
            li_greydwarfPebbleProjectile = config("Essence Greydwarf Modifiers", "li_greydwarfPebbleProjectile", 20f, "Modifies the Damage of the Pebble Projectile");
            li_greydwarfPebblePassiveCarry = config("Essence Greydwarf Modifiers", "li_greydwarfPebblePassiveCarry", 50f, "Modifies the bonus Carry Weight Passive");
            li_greydwarfPebbleForestAnger = config("Essence Greydwarf Modifiers", "li_greydwarfPebbleForestAnger", 10f, "Modifies % Bonus Damage Forest creature do to you");
            // elder
            li_elderAncientAweCD = config("Essence Elder Modifiers", "li_elderAncientAweCD", 30f, "Cooldown");
            li_elderAncientAweSED = config("Essence Elder Modifiers", "li_elderAncientAweSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_elderAncientAwePassive = config("Essence Elder Modifiers", "li_elderAncientAwePassive", 3f, "Modifies the Passive regen multiplier");
            // blob
            li_blobFumesCD = config("Essence Blob Modifiers", "li_blobFumesCD", 10f, "Cooldown");
            li_blobFumes = config("Essence Blob Modifiers", "li_blobFumes", 10f, "Modifies the % weapon damage added to Fumes");
            // skeleton
            li_skeletonVigilCD = config("Essence Skeleton Modifiers", "li_skeletonVigilCD", 50f, "Cooldown");
            li_skeletonVigilSummons = config("Essence Skeleton Modifiers", "li_skeletonVigilSummons", 10f, "Modifies the number of ghosts summoned");
            li_skeletonVigilSummonDuration = config("Essence Skeleton Modifiers", "li_skeletonVigilSummonDuration", 70f, "Modifies the time before summons die");
            li_skeletonVigilSoulCap = config("Essence Skeleton Modifiers", "li_skeletonVigilSoulCap", 300f, "Modifies the number of souls you can store");
            // abomination
            li_abominationBaneCD = config("Essence Abomination Modifiers", "li_abominationBaneCD", 60f, "Cooldown");
            li_abominationBaneArmor = config("Essence Abomination Modifiers", "li_abominationBaneArmor", 30f, "Modifies bonus armor passive");
            li_abominationBaneHealth = config("Essence Abomination Modifiers", "li_abominationBaneHealth", 5f, new ConfigDescription("Modifies % health reduction", new AcceptableValueRange<float>(0f, 100f)));
            li_abominationBaneAllySpeed = config("Essence Abomination Modifiers", "li_abominationBaneAllySpeed", 4f, "Modifies creature speed");
            li_abominationBaneAllyHealth = config("Essence Abomination Modifiers", "li_abominationBaneAllyHealth", 3f, "Modifies creature health");
            li_abominationBaneAllyAttack = config("Essence Abomination Modifiers", "li_abominationBaneAllyAttack", 1f, "Modifies creature attack");
            //wraith
            li_wraithTwinSoulsCD = config("Essence Wraith Modifiers", "li_wraithTwinSoulsCD", 70f, "Cooldown");
            li_wraithTwinSoulsSED = config("Essence Wraith Modifiers", "li_wraithTwinSoulsSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_wraithTwinSoulsArmor = config("Essence Wraith Modifiers", "li_wraithTwinSoulsArmor", 10f, "Modifies armor reduction amount");
            li_wraithTwinSoulsPassive = config("Essence Wraith Modifiers", "li_wraithTwinSoulsPassive", 15f, "Modifies the % weapon spirit damage passive");
            li_wraithTwinSoulsAllySpeed = config("Essence Abomination Modifiers", "li_wraithTwinSoulsAllySpeed", 4f, "Modifies creature speed");
            li_wraithTwinSoulsAllyHealth = config("Essence Abomination Modifiers", "li_wraithTwinSoulsAllyHealth", 2f, "Modifies creature health");
            li_wraithTwinSoulsAllyAttack = config("Essence Abomination Modifiers", "li_wraithTwinSoulsAllyAttack", 2f, "Modifies creature attack");
            //draugr
            li_draugrForgottenCD = config("Essence Draugr Modifiers", "li_draugrForgottenCD", 60f, "Cooldown");
            li_draugrForgottenSED = config("Essence Draugr Modifiers", "li_draugrForgottenSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_draugrForgottenRot = config("Essence Draugr Modifiers", "li_draugrForgottenRot", 5f, new ConfigDescription("Modifies % dmg reduction system", new AcceptableValueRange<float>(0f, 100f)));
            li_draugrForgottenPassiveCarry = config("Essence Draugr Modifiers", "li_draugrForgottenPassiveCarry", 60f, "Modifies the bonus Carry Weight Passive");
            li_draugrForgottenActive = config("Essence Draugr Modifiers", "li_draugrForgottenActive", 10f, "Modifies % bonus dmg active to Axes and Bows");
            //draugr elite
            li_draugreliteFallenHeroCD = config("Essence Draugr Elite Modifiers", "li_draugreliteFallenHeroCD", 60f, "Cooldown");
            li_draugreliteFallenHeroSED = config("Essence Draugr Elite Modifiers", "li_draugreliteFallenHeroSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_draugreliteFallenHeroRot = config("Essence Draugr Elite Modifiers", "li_draugreliteFallenHeroRot", 10f, new ConfigDescription("Modifies % dmg reduction system", new AcceptableValueRange<float>(0f, 100f)));
            li_draugreliteFallenHeroPassiveCarry = config("Essence Draugr Elite Modifiers", "li_draugreliteFallenHeroPassiveCarry", 70f, "Modifies the bonus Carry Weight Passive");
            lidraugreliteFallenHeroActive = config("Essence Draugr Elite Modifiers", "lidraugreliteFallenHeroActive", 20f, "Modifies % bonus dmg active to Swords and Polearms");
            //geirrhafa
            li_geirrhafaIceAgeCD = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgeCD", 45f, "Cooldown");
            li_geirrhafaIceAgeAoe = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgeAoe", 70f, "Modifies Active Damage");
            li_geirrhafaIceAgePassiveEitr = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgePassiveEitr", 70f, "Modifies bonus Eitr Passive");
            li_geirrhafaIceAgePassive = config("Essence Geirrhafa Modifiers", "li_geirrhafaIceAgePassive", 20f, "Modifies the frost damage passive for Weapons");
            //cultist
            li_cultistLoneSunCD = config("Essence Cultist Modifiers", "li_cultistLoneSunCD", 30f, "Cooldown");
            li_cultistLoneSunAoe = config("Essence Cultist Modifiers", "li_cultistLoneSunAoe", 50f, "Modifies Active Damage");
            li_cultistLoneSunPassive = config("Essence Cultist Modifiers", "li_cultistLoneSunPassive", 15f, "Modifies the fire damage passive for Weapons");
            //hare
            li_hareLuckyFootCD = config("Essence Hare Modifiers", "li_hareLuckyFootCD", 40f, "Cooldown");
            li_hareLuckyFootSED = config("Essence Hare Modifiers", "li_hareLuckyFootSED", 50f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_hareLuckyFoot = config("Essence Hare Modifiers", "li_hareLuckyFoot", 1.5f, "Modifies Bonus Speed Active");
            li_hareLuckyFootArmor = config("Essence Hare Modifiers", "li_hareLuckyFootArmor", 10f, new ConfigDescription("Modifies % Armor Reduction passive", new AcceptableValueRange<float>(0f, 100f)));
            //serpent
            li_seaserpentSeaKingCD = config("Essence Sea Serpent Modifiers", "li_seaserpentSeaKingCD", 30f, "Cooldown");
            li_seaserpentSeaKingProjectile = config("Essence Sea Serpent Modifiers", "li_seaserpentSeaKingProjectile", 20f, "Modifies the % weapon Damage of the Sea King Projectile");
            //tick
            li_tickBloodWellCD = config("Essence Tick Modifiers", "li_tickBloodWellCD", 10f, "Cooldown");
            li_tickBloodWellLifeSteal = config("Essence Tick Modifiers", "li_tickBloodWellLifeSteal", 10f, "Modifies % Lifesteal Passive");
            li_tickBloodWellArmor = config("Essence Tick Modifiers", "li_tickBloodWellArmor", 25f, "Modifies Armor Reduction passive");
            li_tickBloodWellStackCap = config("Essence Tick Modifiers", "li_tickBloodWellStackCap", 500f, "Modifies the maximum stored damage");
            //moder
            li_moderDraconicFrostCD = config("Essence Moder Modifiers", "li_moderDraconicFrostCD", 30f, "Cooldown");
            li_moderDraconicFrostProjectile = config("Essence Moder Modifiers", "li_moderDraconicFrostProjectile", 5f, "Modifies the % weapon Damage of the Draconic Frost Projectile");
            li_moderDraconicFrostPassive = config("Essence Moder Modifiers", "li_moderDraconicFrostPassive", 25f, "Modifies the % weapon bonus frost damage passive");
            li_moderDraconicFrostDragonBreath = config("Essence Moder Modifiers", "li_moderDraconicFrostDragonBreath", 200f, "Modifies the damage of Draconic Frost Dragon Breath");
            //boar
            li_boarRecklessChargeCD = config("Essence Boar Modifiers", "li_boarRecklessChargeCD", 30f, "Cooldown");
            li_boarRecklessChargeSED = config("Essence Boar Modifiers", "li_boarRecklessChargeSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_boarRecklessCharge = config("Essence Boar Modifiers", "li_boarRecklessCharge", 30f, "Modifies Active Damage");
            li_boarRecklessChargePassive = config("Essence Boar Modifiers", "li_boarRecklessChargePassive", 30f, "Modifies bonus stamina passive");
            //stone golem
            li_stonegolemCoreOverdriveCD = config("Essence Stone Golem Modifiers", "li_stonegolemCoreOverdriveCD", 5f, "Cooldown");
            li_stonegolemCoreOverdriveStacks = config("Essence Stone Golem Modifiers", "li_stonegolemCoreOverdriveStacks", 100f, "Modifies maximum bonus armor stacks");
            li_stonegolemCoreOverdriveArmor = config("Essence Stone Golem Modifiers", "li_stonegolemCoreOverdriveArmor", 2f, "Modifies bonus armor passive multiplier");
            //yagluth
            li_yagluthCulminationCD = config("Essence Yagluth Modifiers", "li_yagluthCulminationCD", 20f, "Cooldown");
            li_yagluthCulminationStaticCap = config("Essence Yagluth Modifiers", "li_yagluthCulminationStaticCap", 100f, "Modifies maximum static build up before punishment");
            li_yagluthCulmination = config("Essence Yagluth Modifiers", "li_yagluthCulmination", 1f, "Modifies active damage multipliers");
            //ulv
            li_ulvTerritorialSlumberCD = config("Essence Ulv Modifiers", "li_ulvTerritorialSlumberCD", 30f, "Cooldown");
            li_ulvTerritorialSlumberSED = config("Essence Ulv Modifiers", "li_ulvTerritorialSlumberSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_ulvTerritorialSlumberComfort = config("Essence Ulv Modifiers", "li_ulvTerritorialSlumberComfort", 5f, "Bonus comfort passive");
            li_ulvTerritorialSlumberStamina = config("Essence Ulv Modifiers", "li_ulvTerritorialSlumberStamina", 5f, new ConfigDescription("Percentage of Comfort that stamina is multiplied by", new AcceptableValueRange<float>(0f, 100f)));
            li_ulvTerritorialSlumberSummonDuration = config("Essence Ulv Modifiers", "li_ulvTerritorialSlumberSummonDuration", 45f, "Modifies the time before summons die");
            li_ulvTerritorialSlumberSummonHealth = config("Essence Ulv Modifiers", "li_ulvTerritorialSlumberSummonHealth", 1f, "Modifies creature health");
            //brenna
            li_brennaVulcanCD = config("Essence Brenna Modifiers", "li_brennaVulcanCD", 10f, "Cooldown");
            li_brennaVulcanArmor = config("Essence Brenna Modifiers", "li_brennaVulcanArmor", 25f, "Modifies armor reduction amount");
            //rancidremains
            li_rancidremainsRancorousCD = config("Essence Rancid Remains Modifiers", "li_rancidremainsRancorousCD", 10f, "Cooldown");
            li_rancidremainsRancorousArmor = config("Essence Rancid Remains Modifiers", "li_rancidremainsRancorousArmor", 15f, "Modifies armor reduction amount");





            
            LackingImaginationGlobal.ConfigStrings = new Dictionary<string, float>();
            LackingImaginationGlobal.ConfigStrings.Clear();

            //Global
            LackingImaginationGlobal.ConfigStrings.Add("li_cooldownMultiplier", li_cooldownMultiplier.Value);
            
            //Synergies
            LackingImaginationGlobal.ConfigStrings.Add("li_draugrSynergyRot", li_draugrSynergyRot.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_skeletonSynergyBrenna", li_skeletonSynergyBrenna.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_skeletonSynergyRancid", li_skeletonSynergyRancid.Value);
            
            //Cooldowns
             LackingImaginationGlobal.ConfigStrings.Add("li_eikthyrBlitzCD", li_eikthyrBlitzCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deerHorizonHasteCD", li_deerHorizonHasteCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fenringMoonlitLeapCD", li_fenringMoonlitLeapCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_loxWildTremorCD", li_loxWildTremorCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wolfRavenousHungerCD", li_wolfRavenousHungerCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingshamanRitualCD", li_fulingshamanRitualCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deathsquitoRelentlessCD", li_deathsquitoRelentlessCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerCD", li_surtlingHarbingerCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingberserkerGiantizationCD", li_fulingberserkerGiantizationCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_drakeThreeFreezeCD", li_drakeThreeFreezeCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_growthAncientTarCD", li_growthAncientTarCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_trollTrollTossCD", li_trollTrollTossCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfshamanDubiousHealCD", li_greydwarfshamanDubiousHealCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_dvergrRandomizeCD", li_dvergrRandomizeCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_neckSplashCD", li_neckSplashCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_leechBloodSiphonCD", li_leechBloodSiphonCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_bonemassMassReleaseCD", li_bonemassMassReleaseCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfbruteBashCD", li_greydwarfbruteBashCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingLonginusCD", li_fulingLonginusCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_gjallGjallarhornCD", li_gjallGjallarhornCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfPebbleCD", li_greydwarfPebbleCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_elderAncientAweCD", li_elderAncientAweCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_blobFumesCD", li_blobFumesCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_skeletonVigilCD", li_skeletonVigilCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_abominationBaneCD", li_abominationBaneCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsCD", li_wraithTwinSoulsCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugrForgottenCD", li_draugrForgottenCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugreliteFallenHeroCD", li_draugreliteFallenHeroCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_geirrhafaIceAgeCD", li_geirrhafaIceAgeCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_cultistLoneSunCD", li_cultistLoneSunCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_hareLuckyFootCD", li_hareLuckyFootCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_seaserpentSeaKingCD", li_seaserpentSeaKingCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_tickBloodWellCD", li_tickBloodWellCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_moderDraconicFrostCD", li_moderDraconicFrostCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_boarRecklessChargeCD", li_boarRecklessChargeCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_stonegolemCoreOverdriveCD", li_stonegolemCoreOverdriveCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_yagluthCulminationCD", li_yagluthCulminationCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_ulvTerritorialSlumberCD", li_ulvTerritorialSlumberCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_brennaVulcanCD", li_brennaVulcanCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_rancidremainsRancorousCD", li_rancidremainsRancorousCD.Value);
            
            //Status Duration
            LackingImaginationGlobal.ConfigStrings.Add("li_deerHorizonHasteSED", li_deerHorizonHasteSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fenringMoonlitLeapSED", li_fenringMoonlitLeapSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wolfRavenousHungerSED", li_wolfRavenousHungerSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deathsquitoRelentlessSED", li_deathsquitoRelentlessSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerSED", li_surtlingHarbingerSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingberserkerGiantizationSED", li_fulingberserkerGiantizationSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_elderAncientAweSED", li_elderAncientAweSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsSED", li_wraithTwinSoulsSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugrForgottenSED", li_draugrForgottenSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_draugreliteFallenHeroSED", li_draugreliteFallenHeroSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_hareLuckyFootSED", li_hareLuckyFootSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_boarRecklessChargeSED", li_boarRecklessChargeSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_ulvTerritorialSlumberSED", li_ulvTerritorialSlumberSED.Value);
            
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
            LackingImaginationGlobal.ConfigStrings.Add("li_deathsquitoRelentlessHomingRange", li_deathsquitoRelentlessHomingRange.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_deathsquitoRelentlessPassive", li_deathsquitoRelentlessPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerCharges", li_surtlingHarbingerCharges.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerBurn", li_surtlingHarbingerBurn.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_surtlingHarbingerMinDistance", li_surtlingHarbingerMinDistance.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_fulingberserkerGiantizationHealth", li_fulingberserkerGiantizationHealth.Value);
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
            LackingImaginationGlobal.ConfigStrings.Add("li_greydwarfbruteHealthPassive", li_greydwarfbruteHealthPassive.Value);
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
            LackingImaginationGlobal.ConfigStrings.Add("li_abominationBaneAllySpeed", li_abominationBaneAllySpeed.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_abominationBaneAllyHealth", li_abominationBaneAllyHealth.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_abominationBaneAllyAttack", li_abominationBaneAllyAttack.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsArmor", li_wraithTwinSoulsArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsPassive", li_wraithTwinSoulsPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsAllySpeed", li_wraithTwinSoulsAllySpeed.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsAllyHealth", li_wraithTwinSoulsAllyHealth.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_wraithTwinSoulsAllyAttack", li_wraithTwinSoulsAllyAttack.Value);
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
            LackingImaginationGlobal.ConfigStrings.Add("li_hareLuckyFoot", li_hareLuckyFoot.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_hareLuckyFootArmor", li_hareLuckyFootArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_seaserpentSeaKingProjectile", li_seaserpentSeaKingProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_tickBloodWellLifeSteal", li_tickBloodWellLifeSteal.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_tickBloodWellArmor", li_tickBloodWellArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_tickBloodWellStackCap", li_tickBloodWellStackCap.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_moderDraconicFrostProjectile", li_moderDraconicFrostProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_moderDraconicFrostPassive", li_moderDraconicFrostPassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_moderDraconicFrostDragonBreath", li_moderDraconicFrostDragonBreath.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_boarRecklessCharge", li_boarRecklessCharge.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_boarRecklessChargePassive", li_boarRecklessChargePassive.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_stonegolemCoreOverdriveStacks", li_stonegolemCoreOverdriveStacks.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_stonegolemCoreOverdriveArmor", li_stonegolemCoreOverdriveArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_yagluthCulminationStaticCap", li_yagluthCulminationStaticCap.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_yagluthCulmination", li_yagluthCulmination.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_ulvTerritorialSlumberComfort", li_ulvTerritorialSlumberComfort.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_ulvTerritorialSlumberStamina", li_ulvTerritorialSlumberStamina.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_ulvTerritorialSlumberSummonDuration", li_ulvTerritorialSlumberSummonDuration.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_ulvTerritorialSlumberSummonHealth", li_ulvTerritorialSlumberSummonHealth.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_brennaVulcanArmor", li_brennaVulcanArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_rancidremainsRancorousArmor", li_rancidremainsRancorousArmor.Value);
            
            
            _ = ConfigSync.AddConfigEntry(Ability1_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability1_Combokey);
            
            _ = ConfigSync.AddConfigEntry(Ability2_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability3_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability4_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability5_Hotkey);

            _ = ConfigSync.AddConfigEntry(li_cooldownMultiplier);
            _ = ConfigSync.AddConfigEntry(showAbilityIcons);
            _ = ConfigSync.AddConfigEntry(iconAlignment);
            _ = ConfigSync.AddConfigEntry(icon_X_Offset);
            _ = ConfigSync.AddConfigEntry(icon_Y_Offset);
            
            
            
            _ = ConfigSync.AddConfigEntry(li_draugrSynergyRot);
            _ = ConfigSync.AddConfigEntry(li_skeletonSynergyBrenna);
            _ = ConfigSync.AddConfigEntry(li_skeletonSynergyRancid);
            
            
            
            
            //Cooldowns
            _ = ConfigSync.AddConfigEntry(li_eikthyrBlitzCD);
            _ = ConfigSync.AddConfigEntry(li_deerHorizonHasteCD);
            _ = ConfigSync.AddConfigEntry(li_fenringMoonlitLeapCD);
            _ = ConfigSync.AddConfigEntry(li_loxWildTremorCD);
            _ = ConfigSync.AddConfigEntry(li_wolfRavenousHungerCD);
            _ = ConfigSync.AddConfigEntry(li_fulingshamanRitualCD);
            _ = ConfigSync.AddConfigEntry(li_deathsquitoRelentlessCD);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerCD);
            _ = ConfigSync.AddConfigEntry(li_fulingberserkerGiantizationCD);
            _ = ConfigSync.AddConfigEntry(li_drakeThreeFreezeCD);
            _ = ConfigSync.AddConfigEntry(li_growthAncientTarCD);
            _ = ConfigSync.AddConfigEntry(li_trollTrollTossCD);
            _ = ConfigSync.AddConfigEntry(li_greydwarfshamanDubiousHealCD);
            _ = ConfigSync.AddConfigEntry(li_dvergrRandomizeCD);
            _ = ConfigSync.AddConfigEntry(li_neckSplashCD);
            _ = ConfigSync.AddConfigEntry(li_leechBloodSiphonCD);
            _ = ConfigSync.AddConfigEntry(li_bonemassMassReleaseCD);
            _ = ConfigSync.AddConfigEntry(li_greydwarfbruteBashCD);
            _ = ConfigSync.AddConfigEntry(li_fulingLonginusCD);
            _ = ConfigSync.AddConfigEntry(li_gjallGjallarhornCD);
            _ = ConfigSync.AddConfigEntry(li_greydwarfPebbleCD);
            _ = ConfigSync.AddConfigEntry(li_elderAncientAweCD);
            _ = ConfigSync.AddConfigEntry(li_blobFumesCD);
            _ = ConfigSync.AddConfigEntry(li_skeletonVigilCD);
            _ = ConfigSync.AddConfigEntry(li_abominationBaneCD);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsCD);
            _ = ConfigSync.AddConfigEntry(li_draugrForgottenCD);
            _ = ConfigSync.AddConfigEntry(li_draugreliteFallenHeroCD);
            _ = ConfigSync.AddConfigEntry(li_geirrhafaIceAgeCD);
            _ = ConfigSync.AddConfigEntry(li_cultistLoneSunCD);
            _ = ConfigSync.AddConfigEntry(li_hareLuckyFootCD);
            _ = ConfigSync.AddConfigEntry(li_moderDraconicFrostCD);
            _ = ConfigSync.AddConfigEntry(li_boarRecklessChargeCD);
            _ = ConfigSync.AddConfigEntry(li_stonegolemCoreOverdriveCD);
            _ = ConfigSync.AddConfigEntry(li_yagluthCulminationCD);
            _ = ConfigSync.AddConfigEntry(li_ulvTerritorialSlumberCD);
            _ = ConfigSync.AddConfigEntry(li_brennaVulcanCD);
            _ = ConfigSync.AddConfigEntry(li_rancidremainsRancorousCD);
            
            //Status Duration
            _ = ConfigSync.AddConfigEntry(li_deerHorizonHasteSED);
            _ = ConfigSync.AddConfigEntry(li_fenringMoonlitLeapSED);
            _ = ConfigSync.AddConfigEntry(li_wolfRavenousHungerSED);
            _ = ConfigSync.AddConfigEntry(li_deathsquitoRelentlessSED);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerSED);
            _ = ConfigSync.AddConfigEntry(li_fulingberserkerGiantizationSED);
            _ = ConfigSync.AddConfigEntry(li_elderAncientAweSED);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsSED);
            _ = ConfigSync.AddConfigEntry(li_draugrForgottenSED);
            _ = ConfigSync.AddConfigEntry(li_draugreliteFallenHeroSED);
            _ = ConfigSync.AddConfigEntry(li_hareLuckyFootSED);
            _ = ConfigSync.AddConfigEntry(li_boarRecklessChargeSED);
            _ = ConfigSync.AddConfigEntry(li_ulvTerritorialSlumberSED);
            
            // Essence
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
            _ = ConfigSync.AddConfigEntry(li_deathsquitoRelentlessHomingRange);
            _ = ConfigSync.AddConfigEntry(li_deathsquitoRelentlessPassive);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerCharges);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerBurn);
            _ = ConfigSync.AddConfigEntry(li_surtlingHarbingerMinDistance);
            _ = ConfigSync.AddConfigEntry(li_fulingberserkerGiantizationHealth);
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
            _ = ConfigSync.AddConfigEntry(li_greydwarfbruteHealthPassive);
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
            _ = ConfigSync.AddConfigEntry(li_abominationBaneAllySpeed);
            _ = ConfigSync.AddConfigEntry(li_abominationBaneAllyHealth);
            _ = ConfigSync.AddConfigEntry(li_abominationBaneAllyAttack);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsArmor);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsPassive);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsAllySpeed);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsAllyHealth);
            _ = ConfigSync.AddConfigEntry(li_wraithTwinSoulsAllyAttack);
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
            _ = ConfigSync.AddConfigEntry(li_hareLuckyFoot);
            _ = ConfigSync.AddConfigEntry(li_hareLuckyFootArmor);
            _ = ConfigSync.AddConfigEntry(li_seaserpentSeaKingProjectile);
            _ = ConfigSync.AddConfigEntry(li_tickBloodWellLifeSteal);
            _ = ConfigSync.AddConfigEntry(li_tickBloodWellArmor);
            _ = ConfigSync.AddConfigEntry(li_tickBloodWellStackCap);
            _ = ConfigSync.AddConfigEntry(li_moderDraconicFrostProjectile);
            _ = ConfigSync.AddConfigEntry(li_moderDraconicFrostPassive);
            _ = ConfigSync.AddConfigEntry(li_moderDraconicFrostDragonBreath);
            _ = ConfigSync.AddConfigEntry(li_boarRecklessCharge);
            _ = ConfigSync.AddConfigEntry(li_boarRecklessChargePassive);
            _ = ConfigSync.AddConfigEntry(li_stonegolemCoreOverdriveStacks);
            _ = ConfigSync.AddConfigEntry(li_stonegolemCoreOverdriveArmor);
            _ = ConfigSync.AddConfigEntry(li_yagluthCulminationStaticCap);
            _ = ConfigSync.AddConfigEntry(li_yagluthCulmination);     
            _ = ConfigSync.AddConfigEntry(li_ulvTerritorialSlumberComfort);
            _ = ConfigSync.AddConfigEntry(li_ulvTerritorialSlumberStamina);
            _ = ConfigSync.AddConfigEntry(li_ulvTerritorialSlumberSummonDuration);
            _ = ConfigSync.AddConfigEntry(li_ulvTerritorialSlumberSummonHealth); 
            _ = ConfigSync.AddConfigEntry(li_brennaVulcanArmor);
            _ = ConfigSync.AddConfigEntry(li_rancidremainsRancorousArmor); 
            
            
                
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();

        }

        
        private static void InitAnimation()
        {
            replacementMap["IceNova"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "IceNova",
            };
            replacementMap["RootSummon"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "RootSummon",
            };
            replacementMap["AttackSpray"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "AttackSpray",
            };
            replacementMap["FenringLeap"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "FenringLeap",
            };
            replacementMap["GreyShamanHeal"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "GreyShamanHeal",
            };
            replacementMap["HaldorGreet"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "HaldorGreet",
            };
            replacementMap["PlayerCowerEmote"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "PlayerCower",
            };
            replacementMap["PlayerPointEmote"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "PlayerPoint",
            };
            replacementMap["PlayerMace2"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "PlayerMace2",
            };
            replacementMap["PlayerDespair"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "PlayerDespair",
            };
            replacementMap["GroundStab"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "GroundStab",
            };
            replacementMap["RaiseStaff"] = new Dictionary<string, string>
            {
                ["GuardianPower"] = "RaiseStaff",
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
                }
               
                //Brenna Sword
                {
                    GO_VulcanSword = ObjectDB.instance?.GetItemPrefab("Vulcan");
                    GO_VulcanSwordBroken = ObjectDB.instance?.GetItemPrefab("VulcanBroken");
                    GameObject krom = ObjectDB.instance?.GetItemPrefab("THSwordKrom");
                    ItemDrop.ItemData dataTOReplace = GO_VulcanSword.GetComponent<ItemDrop>().m_itemData;
                    ItemDrop.ItemData dataTOReplaceB = GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData;
                    ItemDrop.ItemData newData = krom.GetComponent<ItemDrop>().m_itemData;
                    if (dataTOReplaceB != null && newData != null && dataTOReplace != null)
                    {
                        dataTOReplace.m_shared.m_hitEffect = newData.m_shared.m_hitEffect;
                        dataTOReplace.m_shared.m_blockEffect = newData.m_shared.m_blockEffect;
                        dataTOReplace.m_shared.m_triggerEffect = newData.m_shared.m_triggerEffect;
                        dataTOReplace.m_shared.m_trailStartEffect = newData.m_shared.m_trailStartEffect;
                        dataTOReplaceB.m_shared.m_hitEffect = newData.m_shared.m_hitEffect;
                        dataTOReplaceB.m_shared.m_blockEffect = newData.m_shared.m_blockEffect;
                        dataTOReplaceB.m_shared.m_triggerEffect = newData.m_shared.m_triggerEffect;
                        dataTOReplaceB.m_shared.m_trailStartEffect = newData.m_shared.m_trailStartEffect;
                    }
                    GameObject fire = ObjectDB.instance?.GetItemPrefab("skeleton_sword_hildir");
                    Transform childToReplace = GO_VulcanSword.transform.Find("attach").transform.Find("KromV");
                    Transform childToReplaceB = GO_VulcanSwordBroken.transform.Find("attach").transform.Find("KromV");
                    GameObject newChild = ExpMethods.DeepCopy(fire.transform.Find("attach").transform.Find("Krom").gameObject);
                    GameObject newChildB = ExpMethods.DeepCopy(fire.transform.Find("attach").transform.Find("Krom").gameObject);
                    if (childToReplaceB != null && newChildB != null && newChild != null && childToReplace != null)
                    {
                        childToReplace.GetComponent<MeshRenderer>().materials = newChild.GetComponent<MeshRenderer>().materials;
                        childToReplaceB.GetComponent<MeshRenderer>().materials = newChildB.GetComponent<MeshRenderer>().materials;
                        newChild.transform.Find("fx_Torch_Carried").SetParent(childToReplace);
                        newChildB.transform.Find("fx_Torch_Carried").SetParent(childToReplaceB);
                    }
                    Transform fxTorchCarriedB = childToReplaceB.transform.Find("fx_Torch_Carried");
                    fxTorchCarriedB.localPosition = new Vector3(0f, 1.25f, 0f); // Example: Move it forward along the local Z-axis
                    Transform fxTorchCarried = childToReplace.transform.Find("fx_Torch_Carried");
                    fxTorchCarried.localScale = new Vector3(1f, 1f, 1.4f);
                    fxTorchCarried.transform.Find("Local Flames").localScale = new Vector3(1f, 1f, 1.4f);
                    fxTorchCarried.localPosition = new Vector3(0f, 1.45f, 0f);
                }
                
                //Rancorous Mace
                {
                    GO_RancorousMace = ObjectDB.instance?.GetItemPrefab("Rancorous");
                    GO_RancorousMaceBroken = ObjectDB.instance?.GetItemPrefab("RancorousBroken");
                    GameObject iron = ObjectDB.instance?.GetItemPrefab("MaceIron");
                    ItemDrop.ItemData dataTOReplace = GO_RancorousMace.GetComponent<ItemDrop>().m_itemData;
                    ItemDrop.ItemData dataTOReplaceB = GO_RancorousMaceBroken.GetComponent<ItemDrop>().m_itemData;
                    ItemDrop.ItemData newData = iron.GetComponent<ItemDrop>().m_itemData;
                    if (dataTOReplaceB != null && newData != null && dataTOReplace != null)
                    {
                        dataTOReplace.m_shared.m_hitEffect = newData.m_shared.m_hitEffect;
                        dataTOReplace.m_shared.m_blockEffect = newData.m_shared.m_blockEffect;
                        dataTOReplace.m_shared.m_triggerEffect = newData.m_shared.m_triggerEffect;
                        dataTOReplace.m_shared.m_trailStartEffect = newData.m_shared.m_trailStartEffect;
                        dataTOReplaceB.m_shared.m_hitEffect = newData.m_shared.m_hitEffect;
                        dataTOReplaceB.m_shared.m_blockEffect = newData.m_shared.m_blockEffect;
                        dataTOReplaceB.m_shared.m_triggerEffect = newData.m_shared.m_triggerEffect;
                        dataTOReplaceB.m_shared.m_trailStartEffect = newData.m_shared.m_trailStartEffect;
                    }
                    GameObject poison = ObjectDB.instance?.GetItemPrefab("skeleton_mace");
                    Transform childToReplace = GO_RancorousMace.transform.Find("attach").transform.Find("iron_mace_CubeR");
                    Transform childToReplaceB = GO_RancorousMaceBroken.transform.Find("attach").transform.Find("iron_mace_CubeR");
                    GameObject newChild = ExpMethods.DeepCopy(poison.transform.Find("attach").transform.Find("mace").gameObject);
                    GameObject newChildB = ExpMethods.DeepCopy(poison.transform.Find("attach").transform.Find("mace").gameObject);
                    if (childToReplaceB != null && newChildB != null && newChild != null && childToReplace != null)
                    {
                        childToReplace.GetComponent<MeshRenderer>().materials = newChild.GetComponent<MeshRenderer>().materials;
                        childToReplaceB.GetComponent<MeshRenderer>().materials = newChildB.GetComponent<MeshRenderer>().materials;
                        newChild.transform.Find("Particle System").SetParent(childToReplace);
                        newChildB.transform.Find("Particle System").SetParent(childToReplaceB);
                        newChild.transform.Find("vfx_drippingwater").SetParent(childToReplace);
                        newChildB.transform.Find("vfx_drippingwater").SetParent(childToReplaceB);
                        newChild.transform.Find("Point light").SetParent(childToReplace);
                        newChildB.transform.Find("Point light").SetParent(childToReplaceB);
                        newChild.transform.Find("Particle System (1)").SetParent(childToReplace);
                        newChildB.transform.Find("Particle System (1)").SetParent(childToReplaceB);
                    }
                    foreach (Transform child in childToReplaceB)
                    {
                        child.localPosition = new Vector3(0f, 2.5f, 0f);
                        child.localScale *= 0.5f;
                    }
                    foreach (Transform child in childToReplace)
                    {
                        child.localPosition = new Vector3(0f, 2.5f, 0f);
                        child.localScale *= 0.75f;
                    }
                    
                    
                }



                creatureAnimatorGeirrhafa = ZNetScene.instance.GetPrefab("Fenring_Cultist").gameObject.transform.Find("Visual").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorGeirrhafa.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Frost AoE Spell Attack 3 Burst") // Replace with the actual name of the clip you're looking for.
                    {
                        creatureAnimationClipGeirrhafaIceNova = clip;
                        // break; // Exit the loop once you've found the clip.
                    }
                    if (clip.name == "Flame Spell Attack") // Replace with the actual name of the clip you're looking for.
                    {
                        creatureAnimationClipCultistSpray = clip;
                    }
                    if (creatureAnimationClipGeirrhafaIceNova != null && creatureAnimationClipCultistSpray != null)
                    {
                        // LogWarning($"T1.");
                        break;
                    }
                }
                // if (creatureAnimationClipGeirrhafaIceNova != null)
                // {
                //     LogWarning($"T1.");
                // }
                creatureAnimatorElder = ZNetScene.instance.GetPrefab("gd_king").gameObject.transform.Find("Visual").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorElder.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Standing 1H Magic Attack 03") 
                    {
                        creatureAnimationClipElderSummon = clip;
                        break; 
                    }
                }
                creatureAnimatorFenring = ZNetScene.instance.GetPrefab("Fenring").gameObject.transform.Find("Visual").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorFenring.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "LeapAttack") 
                    {
                        creatureAnimationClipFenringLeapAttack = clip;
                        break; 
                    }
                }
                creatureAnimatorGreyShaman = ZNetScene.instance.GetPrefab("Greydwarf_Shaman").gameObject.transform.Find("Visual").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorGreyShaman.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Standing 1H Cast Spell 01") 
                    {
                        creatureAnimationClipGreyShamanHeal = clip;
                        break; 
                    }
                }
                creatureAnimatorHaldor = ZNetScene.instance.GetPrefab("Haldor").gameObject.transform.Find("HaldorTheTrader").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorHaldor.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Greet") 
                    {
                        creatureAnimationClipHaldorGreet = clip;
                        break; 
                    }
                }
                creatureAnimatorPlayer = ZNetScene.instance.GetPrefab("Player").gameObject.transform.Find("Visual").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorPlayer.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Cower") creatureAnimationClipPlayerEmoteCower = clip;
                    
                    if (clip.name == "Point") creatureAnimationClipPlayerEmotePoint = clip;
                    
                    if (clip.name == "MaceAltAttack") creatureAnimationClipPlayerMace2 = clip;

                    if (clip.name == "Despair") creatureAnimationClipPlayerEmoteDespair = clip;
                    
                    if (creatureAnimationClipPlayerEmoteCower != null 
                        && creatureAnimationClipPlayerEmotePoint != null
                        && creatureAnimationClipPlayerMace2 != null 
                        && creatureAnimationClipPlayerEmoteDespair != null)
                    {
                        break;
                    }
                }
                creatureAnimatorBrenna = ZNetScene.instance.GetPrefab("Skeleton_Hildir").gameObject.transform.Find("Visual").transform.Find("_skeleton_base").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorBrenna.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Skeleton Fire Attack") 
                    {
                        creatureAnimationClipBrennaGroundStab = clip;
                        break; 
                    }
                }
                creatureAnimatorDvergr = ZNetScene.instance.GetPrefab("DvergerMage").gameObject.transform.Find("Visual").GetComponent<Animator>();
                foreach (AnimationClip clip in creatureAnimatorDvergr.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "Staff Magic Heal") 
                    {
                        creatureAnimationClipDvergrStaffRaise = clip;
                        break; 
                    }
                }
            }
        }
         
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        private static class Patch_Player_Start
        {
            private static void Postfix(Player __instance)
            {
                
                if (creatureAnimationClipGeirrhafaIceNova != null && creatureAnimationClipElderSummon != null &&
                    creatureAnimationClipCultistSpray != null && creatureAnimationClipFenringLeapAttack != null &&
                    creatureAnimationClipGreyShamanHeal != null && creatureAnimationClipHaldorGreet != null &&
                    creatureAnimationClipPlayerEmoteCower != null && creatureAnimationClipPlayerEmotePoint != null &&
                    creatureAnimationClipPlayerMace2 != null && creatureAnimationClipBrennaGroundStab != null &&
                    creatureAnimationClipDvergrStaffRaise != null && creatureAnimationClipPlayerEmoteDespair != null) // ADD REST
                {
                    LogWarning($"animations good");
                    AnimationClip copyOfCreatureAnimationClipGeirrhafaIceNova = Instantiate(creatureAnimationClipGeirrhafaIceNova);
                    OutsideAnimations["IceNova"] = copyOfCreatureAnimationClipGeirrhafaIceNova;
                    AnimationClip copyOfcreatureAnimationClipElderSummon = Instantiate(creatureAnimationClipElderSummon);
                    OutsideAnimations["RootSummon"] = copyOfcreatureAnimationClipElderSummon;
                    AnimationClip copyOfcreatureAnimationClipCultistSpray = Instantiate(creatureAnimationClipCultistSpray);
                    OutsideAnimations["AttackSpray"] = copyOfcreatureAnimationClipCultistSpray;
                    AnimationClip copyOfcreatureAnimationClipFenringLeapAttack = Instantiate(creatureAnimationClipFenringLeapAttack);
                    OutsideAnimations["FenringLeap"] = copyOfcreatureAnimationClipFenringLeapAttack;
                    AnimationClip copyOfcreatureAnimationClipGreyShamanHeal = Instantiate(creatureAnimationClipGreyShamanHeal);
                    OutsideAnimations["GreyShamanHeal"] = copyOfcreatureAnimationClipGreyShamanHeal;
                    AnimationClip copyOfcreatureAnimationClipHaldorGreet = Instantiate(creatureAnimationClipHaldorGreet);
                    OutsideAnimations["HaldorGreet"] = copyOfcreatureAnimationClipHaldorGreet;
                    AnimationClip copyOfcreatureAnimationClipPlayerEmoteCower = Instantiate(creatureAnimationClipPlayerEmoteCower);
                    OutsideAnimations["PlayerCower"] = copyOfcreatureAnimationClipPlayerEmoteCower;
                    AnimationClip copyOfcreatureAnimationClipPlayerEmotePoint = Instantiate(creatureAnimationClipPlayerEmotePoint);
                    OutsideAnimations["PlayerPoint"] = copyOfcreatureAnimationClipPlayerEmotePoint;
                    AnimationClip copyOfcreatureAnimationClipPlayerMace2 = Instantiate(creatureAnimationClipPlayerMace2);
                    OutsideAnimations["PlayerMace2"] = copyOfcreatureAnimationClipPlayerMace2;
                    AnimationClip copyOfcreatureAnimationClipPlayerEmoteDespair = Instantiate(creatureAnimationClipPlayerEmoteDespair);
                    OutsideAnimations["PlayerDespair"] = copyOfcreatureAnimationClipPlayerEmoteDespair;
                    AnimationClip copyOfcreatureAnimationClipBrennaGroundStab = Instantiate(creatureAnimationClipBrennaGroundStab);
                    OutsideAnimations["GroundStab"] = copyOfcreatureAnimationClipBrennaGroundStab;
                    AnimationClip copyOfcreatureAnimationClipDvergrStaffRaise = Instantiate(creatureAnimationClipDvergrStaffRaise);
                    OutsideAnimations["RaiseStaff"] = copyOfcreatureAnimationClipDvergrStaffRaise;
                    
                    
                    LackingImaginationV2Plugin.InitAnimation();

                    if (CustomizedRuntimeControllers.Count == 0 && Player.m_localPlayer is not null)
                    {
                        CustomizedRuntimeControllers["Original"] = MakeAOC(new Dictionary<string, string>(), __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["IceNovaControl"] = MakeAOC(replacementMap["IceNova"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["RootSummonControl"] = MakeAOC(replacementMap["RootSummon"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["AttackSprayControl"] = MakeAOC(replacementMap["AttackSpray"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["FenringLeapControl"] = MakeAOC(replacementMap["FenringLeap"], __instance.m_animator.runtimeAnimatorController);// unused, use it
                        CustomizedRuntimeControllers["GreyShamanHealControl"] = MakeAOC(replacementMap["GreyShamanHeal"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["HaldorGreetControl"] = MakeAOC(replacementMap["HaldorGreet"], __instance.m_animator.runtimeAnimatorController);// unused
                        CustomizedRuntimeControllers["PlayerCowerControl"] = MakeAOC(replacementMap["PlayerCowerEmote"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["PlayerPointControl"] = MakeAOC(replacementMap["PlayerPointEmote"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["PlayerMace2Control"] = MakeAOC(replacementMap["PlayerMace2"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["PlayerDespairControl"] = MakeAOC(replacementMap["PlayerDespair"], __instance.m_animator.runtimeAnimatorController);
                        CustomizedRuntimeControllers["SkeletonGroundStabControl"] = MakeAOC(replacementMap["GroundStab"], __instance.m_animator.runtimeAnimatorController);// upgrade anim, equip first
                        CustomizedRuntimeControllers["DvergrRaiseStaffControl"] = MakeAOC(replacementMap["RaiseStaff"], __instance.m_animator.runtimeAnimatorController);// upgrade anim, equip first

                    }
                }
            }
        }
        private static RuntimeAnimatorController MakeAOC(Dictionary<string, string> replacement, RuntimeAnimatorController original)
        {
            AnimatorOverrideController aoc = new(original);
            List<KeyValuePair<AnimationClip, AnimationClip>> animations = new();
            foreach (AnimationClip animation in aoc.animationClips)
            {
                string name = animation.name;
                if (replacement.TryGetValue(name, out string value))
                {
                    AnimationClip newClip = Instantiate(OutsideAnimations[value]);
                    newClip.name = name;
                    animations.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, newClip));
                }
                else
                {
                    animations.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, animation));
                }
            }
            aoc.ApplyOverrides(animations);
            return aoc;
        }

        [HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.RPC_SetTrigger))]
        private static class Patch_ZSyncAnimation_RPC_SetTrigger
        {
            [HarmonyPriority(Priority.VeryLow)]
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
                    if (xModerEssence.ModerController)
                    {
                        controllerName = "AttackSprayControl";
                    }
                    if (xGreydwarfShamanEssence.GreydwarfShamanController || xYagluthEssence.YagluthController2)
                    {
                        controllerName = "GreyShamanHealControl";
                    }
                    if (xBoarEssence.BoarController)
                    {
                        controllerName = "PlayerCowerControl";
                    }
                    if (xEikthyrEssence.EikthyrController || xYagluthEssence.YagluthController1)
                    {
                        controllerName = "PlayerPointControl";
                    }
                    if (xCultistEssence.CultistController)
                    {
                        controllerName = "PlayerMace2Control";
                    }
                    if (xSurtlingEssence.SurtlingController)
                    {
                        controllerName = "PlayerDespairControl";
                    }
                    if (xSkeletonSynergy.SkeletonSynergyBrennaController)
                    {
                        controllerName = "SkeletonGroundStabControl";
                    }
                    if (xSkeletonSynergy.SkeletonSynergyRancidController)
                    {
                        controllerName = "DvergrRaiseStaffControl";
                    }
                    
                    
                    
                    // if called before the first Player Start
                    if (CustomizedRuntimeControllers.TryGetValue(controllerName, out RuntimeAnimatorController controller))
                    {
                        ReplaceAnim(player, controller);
                    }
                }
            }
        }
        
        public static void ReplaceAnim(Player player, RuntimeAnimatorController replace)
        {
            if (player.m_animator.runtimeAnimatorController == replace)
            {
                return;
            }
            player.m_animator.runtimeAnimatorController = replace;
            player.m_animator.Update(Time.deltaTime);
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

        [HarmonyPatch(typeof(Player), (nameof(Player.Update)), null)]
        public class AbilityInput_Postfix
        {
            public static bool Prefix(Player __instance, ref float ___m_maxAirAltitude, ref Rigidbody ___m_body, ref float ___m_lastGroundTouch /*,ref Animator ___m_animator, float ___m_waterLevel*/)
            {
                
                Player localPlayer = Player.m_localPlayer;
                if (localPlayer != null && playerEnabled)
                {
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability1_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(0, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability2_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(1, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability3_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(2, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability4_Input_Down)
                    {
                        LackingImaginationUtilities.AbilityInputPlugin(3, __instance, ___m_body, ___m_maxAirAltitude, ___m_lastGroundTouch);
                        return false;
                    }
                    if (LackingImaginationV2Plugin.TakeInput(localPlayer) && !localPlayer.InPlaceMode() && LackingImaginationUtilities.Ability5_Input_Down)
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
                return true;
            }
        }
        

        
        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStatusEffects))]
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
                                        iconText = Ability1_Hotkey.Value.ToString();
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
                                        iconText = Ability2_Hotkey.Value.ToString();
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
                                        iconText = Ability3_Hotkey.Value.ToString();
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
                                        iconText = Ability4_Hotkey.Value.ToString();
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
                                        iconText = Ability5_Hotkey.Value.ToString();
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
        
        // biomeDictionary //locationDictionary
        //Biome Exp //All exp now
        public static float m_biomeTimer;
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateBiome))]
        public class BiomeExp
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (__instance.InIntro())
                    return;
                if ((double) LackingImaginationV2Plugin.m_biomeTimer == 0.0)
                {
                    Location location = Location.GetLocation(__instance.transform.position);
                    if(location != null && !string.IsNullOrEmpty(location.name))
                    {
                        if (locationDictionary.ContainsKey(location.name))
                        {
                            ExpMethods.dungeonExpMethod(locationDictionary[location.name]);
                        }
                    }
                }
                LackingImaginationV2Plugin.m_biomeTimer += dt;
                if ((double) LackingImaginationV2Plugin.m_biomeTimer <= 1.0)
                    return;
                LackingImaginationV2Plugin.m_biomeTimer = 0.0f;
                foreach (KeyValuePair<Heightmap.Biome, List<string>> kvp in biomeDictionary)
                {
                    ExpMethods.BiomeExpMethod(kvp.Key, kvp.Value);
                    // Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
        }
        
        // locationDictionary
        //Dungeon Exp
        // [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.SetForceEnvironment))]
        // public  class DungeonEnterDetection
        // {
        //     public static void Postfix(string env)
        //     {
        //         if(locationDictionary.ContainsKey(env))
        //         {
        //             ExpMethods.dungeonExpMethod(locationDictionary[env]);
        //         }
        //     }
        // }
        // [HarmonyPatch(typeof(MusicMan), nameof(MusicMan.HandleLocationMusic))]
        // public class DungeonMusicDetection
        // {
        //     public static void Postfix(ref string currentMusic)
        //     {
        //         if(dungeonMusicDictionary.ContainsKey(currentMusic))
        //         {
        //             ExpMethods.dungeonExpMethod(dungeonMusicDictionary[currentMusic]);
        //         }
        //     }
        // }
        
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
                    m_text = "A poisoned land, rife with filth and the sobering truth of mortality, discovered all too late. \n Will you be the same?",
                     
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
                    m_text = "The fallen land, struggling to regrow while despite the darkness coiled aroun its neck",
                     
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
                    m_text = "Essence Power: Blitz \n " +
                             "Active: Shoot a lightning cone forward(Weapon scaling). \n " +
                             "Positive Passive: All attacks do bonus lightning damage. \n" +
                             "Negative Passive: When wet, you take added lightning damage. ",
                     
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
                    m_text = "Essence Power: Mass Release \n" +
                             "Active: Lob a projectile that summons ally skeletons and blobs on impact. \n " +
                             "Positive Passive: When hit you have a 20% chance to release a poison cloud. \n" +
                             "Positive Passive: You become resistant to pierce. \n" +
                             "Negative Passive: You become very weak to blunt. \n" +
                             "Negative Passive: You become very weak to spirit.",
                     
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
                    m_text = "Essence Power: Draconic Frost \n" +
                             "Active: You fire a cone of ice shards that spawn ice on the ground(Weapon scaling). \n " +
                             "Active(Block): You shoot an ice dragon breath. \n " +
                             "Positive Passive: All attacks do added frost damage. \n" +
                             "Negative Passive: The draconic essence makes you always feel cold. \n" +
                             "Consume freeze glands to calm & suppress the cold.",
                     
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
                    m_text = "Essence Power: Ancient Awe \n" +
                             "Active: Briefly root targets in place & grow the roots from under them. \n " +
                             "Positive Passive: Multiplied regen. \n" +
                             "Negative Passive: You become very weak to frost.",
                    
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
                    m_text = "Essence Power: Culmination \n" +
                             "Active: Shoot a line of lightningfire.(Builds Static) \n" +
                             "Active(Block): Call a rain of meteors.(Builds Static) \n" +
                             "Active(Crouch): At the cost of some health, create a nova(Reduces Static) \n" +
                             "Positive Passive: Static decreases over time. \n" +
                             "Positive Passive: Immune to burning.(Builds Static) \n" +
                             "Negative Passive: If Static becomes full you will be struck by a Lightning bolt.(Resets Static)",
                     
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
                    m_text = "Essence Power: Bane \n" +
                             "Active: Summons an ally Abomination.(Eats Wood) \n" +
                             "Positive Passive: Increased armor. \n" +
                             "Negative Passive: Reduced health during the day.",
                    
                     
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
                    m_text = "Essence Power: Troll Toss \n" +
                             "Active: Lob a boulder(Max Health scaling). \n" +
                             "Positive Passive: Bonus health. \n" +
                             "Negative Passive: You become very weak to pierce.",
                     
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
                    m_text = "Essence Power: Fumes \n" +
                             "Active: Release a cloud of poison. \n" +
                             "Positive Passive: 1 extra jump.\n" +
                             "Positive Passive: At 50% hp, spawn 2 ally blobs to aid you.\n" +
                             "Negative Passive: You become weak to blunt. \n" +
                             "Negative Passive: You become weak to lightning.",
                     
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
                    m_text = "Essence Power: Relentless \n" +
                             "Active: For a duration of time your projectiles gain homing.\n" +
                             "Positive Passive: Gain added pierce damage to projectiles.\n" +
                             "Negative Passive: Maximum health reduced by 50%.",
                     
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
                    m_text = "Essence Power: Three Freeze \n" +
                             "Active: Fires a burst of 3 ice shards that freeze enemies for 3 seconds and applies freezing.\n" +
                             "Positive Passive: You become very resistant to frost.\n" +
                             "Negative Passive: You become weak to blunt.\n",
                     
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
                    m_text = "Essence Power: Fallen Hero \n" +
                             "Active: For a duration, increase the damage of Swords and Polearms(builds 5% Rot).\n" +
                             "Positive Passive: Increase carry weight.\n" +
                             "Positive Passive: Increase movement speed by 10%.\n" +
                             "Positive Passive: A portion of damage received is instead gained as Rot.\n" +
                             "Positive Passive: Entrails can be eaten to reduce Rot build up.\n" +
                             "Negative Passive: If Rot build up reaches 100% the ability is locked, \n" +
                             "                  damage reduction is removed and movement speed is reduced by 50%.\n" +
                             "Synergy  Passive: If the Draugr Essence is also equipped, \n" +
                             "                  a bonus potion of damage received is instead gained as Rot.",
                    
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
                    m_text = "Essence Power: Forgotten \n" +
                             "Active: For a duration, increase the damage of bows and axes(builds 3% Rot).\n" +
                             "Positive Passive: Increase carry weight.\n" +
                             "Positive Passive: A portion of damage received is instead gained as Rot.\n" +
                             "Positive Passive: Entrails can be eaten to reduce Rot build up.\n" +
                             "Negative Passive: If Rot build up reaches 100% the ability is locked, \n" +
                             "                  damage reduction is removed and movement speed is reduced by 50%.\n" +
                             "Synergy  Passive: If the Draugr Elite Essence is also equipped, \n" +
                             "                  a bonus potion of damage received is instead gained as Rot.",
                     
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
                    m_text = "Essence Power: Longinus \n" +
                             "Active: Empower your next spear throw hit.\n" +
                             "Positive Passive: Spears return when thrown.\n" +
                             "Positive Passive: Gain increased block power.\n" +
                             "Positive Passive: Gain bonus stamina when you carry coins.\n" +
                             "Negative Passive: Lose a % of stamina when not carrying coins.\n",
                     
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
                    m_text = "Essence Power: Gjallarhorn \n" +
                             "Active: A pair of entangled projectiles are fired, one explosive and the other summons ally ticks.\n" +
                             "Positive Passive: Gain bonus armor.\n" +
                             "Negative Passive: You become very weak to pierce.\n" +
                             "Negative Passive: Your head becomes a weakpoint(double damage)(disabled).\n",
                     
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
                    m_text = "Essence Power: Bash \n" +
                             "Active: Empower your next melee hit.\n" +
                             "Positive Passive: Gain bonus health.\n" +
                             "Negative Passive: Reduce the damage of ranged attacks.\n",
                     
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
                    m_text = "Essence Power: Dubious Heal \n" +
                             "Active: Cast an Aoe heal.\n" +
                             "Positive Passive: Gain increased player regen.\n" +
                             "Positive Passive: Gain a little bonus eitr.\n" +
                             "Negative Passive: When hit you have a 1 in 20 chance to take 10% bonus poison damage.\n",
                     
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
                    m_text = "Essence Power: Pebble \n" +
                             "Active: Throw a rock from your inventory.\n" +
                             "Positive Passive: Gain increases carry weight.\n" +
                             "Positive Passive: Gain 5% bonus movement speed.\n" +
                             "Negative Passive: Forest Monsters do bonus damage to you.\n",
                     
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
                    m_text = "Essence Power: Blood Siphon \n" +
                             "Active: Mark nearby enemies and when you kill them, gain Siphon stacks.\n" +
                             "Positive Passive: When hit, regain health if you have stacks.\n" +
                             "Negative Passive: Eitr regen is halved and regen delay is doubled.\n",
                     
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
                    m_text = "Essence Power: Vigil \n" +
                             "Active: Summon ally ghosts to fight alongside you.(Cost 1 Soul to cast)\n" +
                             "Positive Passive: Gain bonus spirit damage equal to 10% of Souls.\n" +
                             "Negative Passive: Kill skeletons to gain souls.\n" +
                             "Synergy  Passive: If the Brenna Essence is also equipped, ally ghosts do bonus fire damage. \n" +
                             "Synergy  Passive: If the Rancid Remains Essence is also equipped, ally ghosts do bonus poison damage.",
                     
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
                    m_text = "Essence Power: Harbinger \n" +
                             "Active: Summon ally surtlings from the Ash Lands.(Sacrifice a surtling core to gain charges)\n" +
                             "Positive Passive: The essence is your own personal campfire.\n" +
                             "Negative Passive: You take damage when wet.\n",
                     
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
                    m_text = "Essence Power: Blood Well \n" +
                             "Active: Empower next hit to do bonus slash damage equal to Well stacks.\n" +
                             "Positive Passive: Gain % LifeSteal.\n" +
                             "Positive Passive: Gain Well stacks equal to life stolen.\n" +
                             "Negative Passive: Armor is reduced.\n",
                     
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
                    m_text = "Essence Power: Lone Sun\n " +
                             "Active: Cast an Aoe fire field.\n" +
                             "Positive Passive: You are immune to smoke.\n" +
                             "Positive Passive: All attacks do bonus fire damage.\n" +
                             "Negative Passive: When hit you have a 1 in 20 chance to take 10% bonus fire damage.\n",
                     
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
                    m_text = "Essence Power: Giantization \n" +
                             "Active: Double your size, gaining double health but halving stamina.\n" +
                             "Positive Passive: Gain bonus health.\n" +
                             "Negative Passive: Eitr reduced by 75%.\n",
                     
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
                    m_text = "Essence Power: Ritual\n " +
                             "Active: Create a shield that protects you.(Cost 5 coins)\n" +
                             "Active(Shielded): Shoot a fireball.(Cost 1 coin)\n" +
                             "Positive Passive: Gain bonus eitr.\n" +
                             "Negative Passive: Reduce carry weight.\n",
                     
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
                    m_text = "Essence Power: Ancient Tar\n " +
                             "Active: Shoot a burst of 4 tar balls.(Weapon scaling)\n" +
                             "Positive Passive: Attacks tar enemies.\n" +
                             "Positive Passive: 2 extra jumps.\n" +
                             "Negative Passive: You become very weak to fire.\n",
                     
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
                    m_text = "Essence Power: Rancorous\n" +
                             "Active: Summon the bound mace Rancorous.\n" +
                             "Active(Recast): Rancorous seconary attack chnages to a throw.\n" +
                             "Active(Re-Recast): Rancorous seconary attack returns to normal.\n" +
                             "Positive Passive: Sacrifice a fully upgraded Iron mace to Awaken Rancorous permanently.\n" +
                             "Negative Passive: Armor is reduced.\n",
                     
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
                    m_text = "Essence Power: Sea King\n" +
                             "Active: Shoots a Whirlpool that sucks enemies in.(Weapon scaling)\n" +
                             "Passive: The essence wil crave a random known fish and eating it will," +
                             "         increase the range and duration of Sea King.\n" +
                             "         A higher quality fish can be eaten after to get a bigger buff.\n" +
                             "         Perch>Pike>Trollfish>Tetra>Tuna>Coral Cod>Giant Herring>Grouper>\n" +
                             "         Pufferfish>Anglerfish>Magmafish>Northern Salmon",
                     
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
                    m_text = "Essence Power: Territorial Slumber\n" +
                             "Active: Create a zone with a high chance to summon an ally ulv when an enemy dies inside.\n" +
                             "Positive Passive: Gain bonus Comfort.\n" +
                             "Positive Passive: Gain bonus stamina based of max comfort.\n" +
                             "Negative Passive: Duration of rested reduced by half.",
                     
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
                    m_text = "Essence Power: Twin Souls\n" +
                             "Active: Become intangible for a duration, moving through structures and creatures.\n" +
                             "        Block and Crouch to move down.\n" +
                             "Active(Block): If cast at night, summon an ally wraith(Dies at dawn).\n" +
                             "Positive Passive: All attacks do bonus spirit damage during the day.\n" +
                             "Negative Passive: Armor reduced.\n",
                     
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
                    m_text = "Essence Power: Randomize\n" +
                             "Active: Cast one of three spells.(Cost 50 Eitr)\n" +
                             "Active(Fire): Cast 1 of 2 fire ball types.(Weapon scaling)\n" +
                             "Active(Ice): Cast a burst of ice shards.(Weapon scaling)\n" +
                             "Active(Heal): Cast an Aoe Heal.\n" +
                             "Positive Passive: Gain bonus eitr.\n" +
                             "Positive Passive: Gain bonus pierce damage with crossbows.\n" +
                             "Negative Passive: Health reduced by 10%.",
                     
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
                    m_text = "Essence Power: Reckless Charge\n" +
                             "Active: For a duration, after running for 3 seconds you collide with the next enemy.\n" +
                             "Positive Passive: When you gather your courage you gain bonus stamina.\n" +
                             "Negative Passive: When near a fire you will cower in fear.\n",
                     
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
                    m_text = "Essence Power: Horizon Haste\n" +
                             "Active: For a duration, movement speed is increased.\n" +
                             "Positive Passive: Stamina increased.\n" +
                             "Positive Passive: Movement speed increased by 5%.\n" +
                             "Negative Passive: Health reduced by 5%.",
                     
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
                    m_text = "Essence Power: Lucky Foot\n" +
                             "Active: For a duration, movement speed is increased & jumps are doubled.\n" +
                             "Positive Passive: Gain 1 bonus jump.\n" +
                             "Positive Passive: Movement speed increased by 10%.\n" +
                             "Negative Passive: Armor reduced.",
                     
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
                    m_text = "Essence Power: Wild Tremor\n" +
                             "Active: Stomp the ground causing an Aoe.\n" +
                             "Positive Passive: Food eaten will gave bonus health.\n" +
                             "Negative Passive: Duration of food reduced by 25%.\n",
                     
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
                    m_text = "Essence Power: Splash(Effect in progress)\n" + 
                             "Active: Dash forward while swimming.(Weapon scaling)\n" +
                             "Positive Passive: Swim speed doubled.\n" +
                             "Positive Passive: You become weak to fire while wet.\n" +
                             "Negative Passive: You become resistant to poison while wet.\n",
                     
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
                    m_text = "Essence Power: Ravenous Hunger\n" +
                             "Active: For a duration, every 5th hit will deal a % of max health in slash damage.\n" +
                             "Passive: Different effects based on number of foods eaten.\n" +
                             "         3 foods: damage reduced by 25%.\n" +
                             "         2 foods: damage increased by 25% & x bonus stamina.\n" +
                             "         1 foods: damage increased by 50% & 2x bonus stamina.\n" +
                             "         0 foods: damage increased by 100% & 3x bonus stamina.\n" +
                             "         Below 3 foods run, sneak, dodge & jump stamina drain reduced by half." ,
                     
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
                    m_text = "Essence Power: Vulcan\n" +
                             "Active: Summon the bound mace Vulcan.\n" +
                             "Active(Recast): Vulcan secondary attack changes to a throw.\n" +
                             "Active(Re-Recast): Vulcan secondary attack returns to normal.\n" +
                             "Positive Passive: Sacrifice a fully upgraded Krom to Awaken Vulcan permanently.\n" +
                             "Negative Passive: Armor is reduced.\n",
                     
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
                    m_text = "Essence Power: Ice Age\n" +
                             "Active: Case 3 ice waves & then summon icicles above enemies in range.\n" +
                             "Positive Passive: Gain bonus eitr.\n" +
                             "Positive Passive: All attacks do bonus frost damage.\n" +
                             "Negative Passive:  When hit you have a 1 in 20 chance to take 10% bonus frost damage.\n",
                     
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
                    m_text = "An echo of prosperity, now a den of malice.",
                     
                    m_topic = "Infected Mine"
                };
                if (!Tutorial.instance.m_texts.Contains(_infectedMineExp))
                {
                    Tutorial.instance.m_texts.Add(_infectedMineExp);
                }
                Tutorial.TutorialText _frostCaveExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xFrost Cave",
                    m_name = "FrostCave_Exp",
                    m_text = "To resist the madness of the frost, flame itself was deified in this place.",
                     
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
                    m_text = "The treasures within gather warriors far and wide to their demise.",
                     
                    m_topic = "Sunken Crypt"
                };
                if (!Tutorial.instance.m_texts.Contains(_sunkenCryptExp))
                {
                    Tutorial.instance.m_texts.Add(_sunkenCryptExp);
                }
                Tutorial.TutorialText _burialChambersExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xBurial Chambers",
                    m_name = "BurialChambers_Exp",
                    m_text = "A resting place for the fallen, though they seem rather restless.",
                     
                    m_topic = "Burial Chambers"
                };
                if (!Tutorial.instance.m_texts.Contains(_burialChambersExp))
                {
                    Tutorial.instance.m_texts.Add(_burialChambersExp);
                }
                Tutorial.TutorialText _trollCaveExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xTroll Cave",
                    m_name = "TrollCave_Exp",
                    m_text = "Large eyes peer out from the darkness.",
                     
                    m_topic = "Troll Cave"
                };
                if (!Tutorial.instance.m_texts.Contains(_trollCaveExp))
                {
                    Tutorial.instance.m_texts.Add(_trollCaveExp);
                }

                //Open Dungeons
                Tutorial.TutorialText _goblinCampExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xFuling Camp",
                    m_name = "GoblinCamp_Exp",
                    m_text = "The kin gather in worship of their king.",
                     
                    m_topic = "Fuling Camp"
                };
                if (!Tutorial.instance.m_texts.Contains(_goblinCampExp))
                {
                    Tutorial.instance.m_texts.Add(_goblinCampExp);
                }
                
                //Hildir Dungeons
                Tutorial.TutorialText _forestCryptHildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xSmouldering Tomb",
                    m_name = "ForestCryptHildir_Exp",
                    m_text = "The wails of suffering and hellfire can be heard even from here.",
                     
                    m_topic = "Smouldering Tomb"
                };
                if (!Tutorial.instance.m_texts.Contains(_forestCryptHildirExp))
                {
                    Tutorial.instance.m_texts.Add(_forestCryptHildirExp);
                }
                Tutorial.TutorialText _caveHildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xHowling Cavern",
                    m_name = "CaveHildir_Exp",
                    m_text = "Madness within madness, forsaken by the flame, this place embraced the frost.",
                    
                    m_topic = "Howling Cavern"
                };
                if (!Tutorial.instance.m_texts.Contains(_caveHildirExp))
                {
                    Tutorial.instance.m_texts.Add(_caveHildirExp);
                }
                Tutorial.TutorialText _plainsFortHildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xSealed Tower",
                    m_name = "PlainsFortHildir_Exp",
                    m_text = "A fortress of brotherhood, it's residents seek to overthrow their king.",
                     
                    m_topic = "Sealed Tower"
                };
                if (!Tutorial.instance.m_texts.Contains(_plainsFortHildirExp))
                {
                    Tutorial.instance.m_texts.Add(_plainsFortHildirExp);
                }
                
                //Boss Arenas
                Tutorial.TutorialText _eikthyrSacrificeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xEikthyr Altar",
                    m_name = "EikthyrSacrifice_Exp",
                    m_text = "The air around the altar sparks with electricity.",
                    
                    m_topic = "Eikthyr Altar"
                };
                if (!Tutorial.instance.m_texts.Contains(_eikthyrSacrificeExp))
                {
                    Tutorial.instance.m_texts.Add(_eikthyrSacrificeExp);
                }
                Tutorial.TutorialText _theElderSacrificeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xThe Elder Altar",
                    m_name = "TheElderSacrifice_Exp",
                    m_text = "The roots seem to shift below your feet.",
                    
                    m_topic = "The Elder Altar"
                };
                if (!Tutorial.instance.m_texts.Contains(_theElderSacrificeExp))
                {
                    Tutorial.instance.m_texts.Add(_theElderSacrificeExp);
                }
                Tutorial.TutorialText _boneMassSacrificeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xBoneMass Altar",
                    m_name = "BoneMassSacrifice_Exp",
                    m_text = "A powerful mage must have made this, but to do or create what?",
                    
                    m_topic = "BoneMass Altar"
                };
                if (!Tutorial.instance.m_texts.Contains(_boneMassSacrificeExp))
                {
                    Tutorial.instance.m_texts.Add(_boneMassSacrificeExp);
                }
                Tutorial.TutorialText _moderSacrificeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xModer Altar",
                    m_name = "ModerSacrifice_Exp",
                    m_text = "Something massive roosts here.",
                    
                    m_topic = "Moder Altar"
                };
                if (!Tutorial.instance.m_texts.Contains(_moderSacrificeExp))
                {
                    Tutorial.instance.m_texts.Add(_moderSacrificeExp);
                }
                Tutorial.TutorialText _yagluthSacrificeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xYagluth Altar",
                    m_name = "YagluthSacrifice_Exp",
                    m_text = "Not content with mastery of fire, here the king sought to take mastery of lightning.",
                    
                    m_topic = "Yagluth Altar"
                };
                if (!Tutorial.instance.m_texts.Contains(_yagluthSacrificeExp))
                {
                    Tutorial.instance.m_texts.Add(_yagluthSacrificeExp);
                }
                Tutorial.TutorialText _seekerQueenSealExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xSeeker Queen Seal",
                    m_name = "SeekerQueenSeal_Exp",
                    m_text = "The epitome of pestilence is sealed within these walls.",
                    
                    m_topic = "Seeker Queen Seal"
                };
                if (!Tutorial.instance.m_texts.Contains(_seekerQueenSealExp))
                {
                    Tutorial.instance.m_texts.Add(_seekerQueenSealExp);
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
            [UsedImplicitly] public int? Order = null!;
            [UsedImplicitly] public bool? Browsable = null!;
            [UsedImplicitly] public string? Category = null!;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer = null!;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }
}