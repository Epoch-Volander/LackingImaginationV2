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
        
        public static Dictionary<string, List<string>> ItemBundleUnwrapDict = new Dictionary<string, List<string>>()
        {
            // { "Essence_Drop", new List<string> { "Essence", "An erie glow.", "Greydwarf", "0.5" } }
            //bosses
            { "$item_eikthyr_essence", new List<string> { "Eikthyr_Essence_Drop","Eikthyr Essence", "Eikthyr", "0.1", "Blitz", "EssenceTrophyEikthyr"} },
            { "$item_elder_essence", new List<string> { "TheElder_Essence_Drop", "The Elder Essence", "gd_king", "0.1", "Ancient Awe", "EssenceTrophyTheElder" } },
            { "$item_bonemass_essence", new List<string> { "BoneMass_Essence_Drop", "Bone Mass Essence", "Bonemass", "0.1", "Mass Release", "EssenceTrophyBonemass" } },
            { "$item_dragonqueen_essence", new List<string> { "Moder_Essence_Drop", "Moder Essence", "Dragon", "0.1", "Draconic Frost", "EssenceTrophyDragonQueen" } },
            { "$item_yagluth_essence", new List<string> { "Yagluth_Essence_Drop", "Yagluth Essence", "GoblinKing", "0.1", "Culmination", "EssenceTrophyGoblinKing" } },
            { "$item_seekerqueen_essence", new List<string> { "SeekerQueen_Essence_Drop", "Seeker Queen Essence", "SeekerQueen", "0.1", "An erie glow.", "EssenceTrophySeekerQueen" } },
            
            { "$item_abomination_essence", new List<string> { "Abomination_Essence_Drop", "Abomination Essence", "Abomination", "0.01", "Bane", "EssenceTrophyAbomination" } },
            { "$item_stonegolem_essence", new List<string> { "StoneGolem_Essence_Drop", "Stone Golem Essence", "StoneGolem", "0.01", "Core Overdrive", "EssenceTrophySGolem" } },
            { "$item_troll_essence", new List<string> { "Troll_Essence_Drop", "Troll Essence", "Troll", "0.01", "Troll Toss", "EssenceTrophyFrostTroll" } },
            
            { "$item_blob_essence", new List<string> { "Blob_Essence_Drop", "Blob Essence", "Blob", "0.005", "Fumes", "EssenceTrophyBlob" } },
            { "$item_boar_essence", new List<string> { "Boar_Essence_Drop", "Boar Essence", "Boar", "0.005", "Reckless Charge", "EssenceTrophyBoar" } },
            { "$item_cultist_essence", new List<string> { "Cultist_Essence_Drop", "Cultist Essence", "Fenring_Cultist", "0.01", "Lone Sun", "EssenceTrophyCultist" } },//1
            { "$item_deathsquito_essence", new List<string> { "Deathsquito_Essence_Drop", "Deathsquito Essence", "Deathsquito", "0.005", "Relentless", "EssenceTrophyDeathsquito" } },
            { "$item_deer_essence", new List<string> { "Deer_Essence_Drop", "Deer Essence", "Deer", "0.005", "Horizon Haste", "EssenceTrophyDeer" } },
            { "$item_draugrelite_essence", new List<string> { "DraugrElite_Essence_Drop", "Draugr Elite Essence", "Draugr_Elite", "0.005", "Fallen Hero", "EssenceTrophyDraugrElite" } },
            { "$item_draugr_essence", new List<string> { "Draugr_Essence_Drop", "Draugr Essence", "Draugr", "0.005", "Forgotten", "EssenceTrophyDraugr" } },
            { "$item_dvergr_essence", new List<string> { "Dvergr_Essence_Drop", "Dvergr Essence", "Dverger", "0.005", "Randomize", "EssenceTrophyDvergr"} },
            { "$item_fenring_essence", new List<string> { "Fenring_Essence_Drop", "Fenring Essence", "Fenring", "0.01", "Moonlit Leap", "EssenceTrophyFenring" } },//1
            { "$item_gjall_essence", new List<string> { "Gjall_Essence_Drop", "Gjall Essence", "Gjall", "0.01", "Gjallarhorn", "EssenceTrophyGjall" } },//1
            { "$item_goblin_essence", new List<string> { "Goblin_Essence_Drop", "Fuling Essence", "Goblin", "0.005", "Longinus", "EssenceTrophyGoblin" } },
            { "$item_goblinbrute_essence", new List<string> { "GoblinBrute_Essence_Drop", "Fuling Berserker Essence", "GoblinBrute", "0.01", "Giantization", "EssenceTrophyGoblinBrute" } },//1
            { "$item_goblinshaman_essence", new List<string> { "GoblinShaman_Essence_Drop", "Fuling Shaman Essence", "GoblinShaman", "0.01", "Ritual", "EssenceTrophyGoblinShaman" } },//1
            { "$item_greydwarf_essence", new List<string> { "Greydwarf_Essence_Drop", "Greydwarf Essence", "Greydwarf", "0.005", "Pebble", "EssenceTrophyGreydwarf" } },
            { "$item_greydwarfbrute_essence", new List<string> { "GreydwarfBrute_Essence_Drop", "Greydwarf Brute Essence", "Greydwarf_Elite", "0.005", "Bash", "EssenceTrophyGreydwarfBrute" } },
            { "$item_greydwarfshaman_essence", new List<string> { "GreydwarfShaman_Essence_Drop", "Greydwarf Shaman Essence", "Greydwarf_Shaman", "0.005", "Dubious Heal", "EssenceTrophyGreydwarfShaman" } },
            { "$item_growth_essence", new List<string> { "Growth_Essence_Drop", "Growth Essence", "BlobTar", "0.01", "Ancient Tar", "EssenceTrophyGrowth" } },//
            { "$item_hare_essence", new List<string> { "Hare_Essence_Drop", "Hare Essence", "Hare", "0.005", "Lucky Foot", "EssenceTrophyHare" } },
            { "$item_hatchling_essence", new List<string> { "Drake_Essence_Drop", "Drake Essence", "Hatchling", "0.005", "Three Freeze", "EssenceTrophyHatchling" } },
            { "$item_leech_essence", new List<string> { "Leech_Essence_Drop", "Leech Essence", "Leech", "0.005", "Blood Siphon", "EssenceTrophyLeech" } },
            { "$item_lox_essence", new List<string> { "Lox_Essence_Drop", "Lox Essence", "Lox", "0.005", "Wild Tremor", "EssenceTrophyLox" } },
            { "$item_neck_essence", new List<string> { "Neck_Essence_Drop", "Neck Essence", "Neck", "0.005", "Splash", "EssenceTrophyNeck" } },
            { "$item_seeker_essence", new List<string> { "Seeker_Essence_Drop","Seeker Essence", "Seeker", "0.005", "An erie glow.", "EssenceTrophySeeker" } },
            { "$item_seeker_brute_essence", new List<string> { "SeekerSoldier_Essence_Drop","Seeker Soldier Essence", "SeekerBrute", "0.01", "An erie glow.", "EssenceTrophySeekerBrute" } },//1
            { "$item_serpent_essence", new List<string> { "Serpent_Essence_Drop", "Sea Serpent Essence", "Serpent", "0.05", "Sea King", "EssenceTrophySerpent" } },//5
            { "$item_skeleton_essence", new List<string> { "Skeleton_Essence_Drop", "Skeleton Essence", "Skeleton", "0.005", "Vigil", "EssenceTrophySkeleton" } },
            { "$item_skeletonpoison_essence", new List<string> { "SkeletonPoison_Essence_Drop", "Rancid Remains Essence", "Skeleton_Poison", "0.05", "Rancorous", "EssenceTrophySkeletonPoison" } },//5
            { "$item_surtling_essence", new List<string> { "Surtling_Essence_Drop", "Surtling Essence", "Surtling", "0.005", "Harbinger", "EssenceTrophySurtling" } },
            { "$item_tick_essence", new List<string> { "Tick_Essence_Drop", "Tick Essence", "Tick", "0.005", "Blood Well", "EssenceTrophyTick" } },
            { "$item_ulv_essence", new List<string> { "Ulv_Essence_Drop", "Ulv Essence", "Ulv", "0.01", "Territorial Slumber", "EssenceTrophyUlv" } },//1
            { "$item_wolf_essence", new List<string> { "Wolf_Essence_Drop", "Wolf Essence", "Wolf", "0.005", "Ravenous Hunger", "EssenceTrophyWolf" } },
            { "$item_wraith_essence", new List<string> { "Wraith_Essence_Drop", "Wraith Essence", "Wraith", "0.01", "Twin Souls", "EssenceTrophyWraith" } },//1
            
            { "$item_brenna_essence", new List<string> { "Brenna_Essence_Drop", "Brenna Essence", "Skeleton_Hildir", "1.0", "Vulcan", "EssenceTrophySkeletonHildir" } },
            { "$item_geirrhafa_essence", new List<string> { "Geirrhafa_Essence_Drop", "Geirrhafa Essence", "Fenring_Cultist_Hildir", "1.0", "Ice Age.", "EssenceTrophyCultist_Hildir" } },
            { "$item_zil_essence", new List<string> { "Zil_Essence_Drop", "Zil Essence", "GoblinShaman_Hildir", "1.0", "An erie glow.", "EssenceTrophyGoblinBruteBrosShaman" } },
            { "$item_thungr_essence", new List<string> { "Thungr_Essence_Drop", "Thungr Essence", "GoblinBrute_Hildir", "1.0", "An erie glow.", "EssenceTrophyGoblinBruteBrosBrute" } },
        };

        public static Dictionary<string, string> EssenceTrophyMaterials = new Dictionary<string, string>()
        {
            //Essence prefab, Trophy prefab
            {"Eikthyr_Essence_Drop", "TrophyEikthyr"},
            {"TheElder_Essence_Drop", "TrophyTheElder"},
            {"BoneMass_Essence_Drop", "TrophyBonemass"},
            {"Moder_Essence_Drop", "TrophyDragonQueen"},
            {"Yagluth_Essence_Drop", "TrophyGoblinKing"},
            {"SeekerQueen_Essence_Drop", "TrophySeekerQueen"},
            {"Abomination_Essence_Drop", "TrophyAbomination"},
            {"StoneGolem_Essence_Drop", "TrophySGolem"},
            {"Troll_Essence_Drop", "TrophyFrostTroll"},
            {"Blob_Essence_Drop", "TrophyBlob"},
            {"Boar_Essence_Drop", "TrophyBoar"},
            {"Cultist_Essence_Drop", "TrophyCultist"},
            {"Deathsquito_Essence_Drop", "TrophyDeathsquito"},
            {"Deer_Essence_Drop", "TrophyDeer"},
            {"DraugrElite_Essence_Drop", "TrophyDraugrElite"},
            {"Draugr_Essence_Drop", "TrophyDraugr"},
            {"Dvergr_Essence_Drop", "TrophyDvergr"},
            {"Fenring_Essence_Drop", "TrophyFenring"},
            {"Gjall_Essence_Drop", "TrophyGjall"},
            {"Goblin_Essence_Drop", "TrophyGoblin"},
            {"GoblinBrute_Essence_Drop", "TrophyGoblinBrute"},
            {"GoblinShaman_Essence_Drop", "TrophyGoblinShaman"},
            {"Greydwarf_Essence_Drop", "TrophyGreydwarf"},
            {"GreydwarfBrute_Essence_Drop", "TrophyGreydwarfBrute"},
            {"GreydwarfShaman_Essence_Drop", "TrophyGreydwarfShaman"},
            {"Growth_Essence_Drop", "TrophyGrowth"},
            {"Hare_Essence_Drop", "TrophyHare"},
            {"Drake_Essence_Drop", "TrophyHatchling"},
            {"Leech_Essence_Drop", "TrophyLeech"},
            {"Lox_Essence_Drop", "TrophyLox"},
            {"Neck_Essence_Drop", "TrophyNeck"},
            {"Seeker_Essence_Drop", "TrophySeeker"},
            {"SeekerSoldier_Essence_Drop", "TrophySeekerBrute"},
            {"Serpent_Essence_Drop", "TrophySerpent"},
            {"Skeleton_Essence_Drop", "TrophySkeleton"},
            {"SkeletonPoison_Essence_Drop", "TrophySkeletonPoison"},
            {"Surtling_Essence_Drop", "TrophySurtling"},
            {"Tick_Essence_Drop", "TrophyTick"},
            {"Ulv_Essence_Drop", "TrophyUlv"},
            {"Wolf_Essence_Drop", "TrophyWolf"},
            {"Wraith_Essence_Drop", "TrophyWraith"},
            {"Brenna_Essence_Drop", "TrophySkeletonHildir"},
            {"Geirrhafa_Essence_Drop", "TrophyCultist_Hildir"},
            {"Zil_Essence_Drop", "TrophyGoblinBruteBrosShaman"},
            {"Thungr_Essence_Drop", "TrophyGoblinBruteBrosBrute"},
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

        public static ConfigEntry<bool>? EssenceSlotsEnabled;
        public static bool UseGuardianPower = true;
        
        // public static ConfigEntry<KeyboardShortcut> Ability1_Hotkey { get; set; }
        public static ConfigEntry<KeyCode>? Sprintkey { get; set; }
        public static ConfigEntry<KeyCode>? Ability1_Hotkey { get; set; }
        public static ConfigEntry<KeyCode>? Ability1_Combokey { get; set; }
        public static ConfigEntry<KeyCode>? Ability2_Hotkey { get; set; }
        public static ConfigEntry<KeyCode>? Ability2_Combokey { get; set; }
        public static ConfigEntry<KeyCode>? Ability3_Hotkey { get; set; }
        public static ConfigEntry<KeyCode>? Ability3_Combokey { get; set; }
        public static ConfigEntry<KeyCode>? Ability4_Hotkey { get; set; }
        public static ConfigEntry<KeyCode>? Ability4_Combokey { get; set; }
        public static ConfigEntry<KeyCode>? Ability5_Hotkey { get; set; }
        public static ConfigEntry<KeyCode>? Ability5_Combokey { get; set; }

        public static ConfigEntry<float>? li_cooldownMultiplier;

        public static ConfigEntry<float>? icon_X_Offset;
        public static ConfigEntry<float>? icon_Y_Offset;

        public static ConfigEntry<bool>? showAbilityIcons;
        public static ConfigEntry<string>? iconAlignment;

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


        private static ConfigEntry<float>? li_draugrSynergyRot;
        private static ConfigEntry<float>? li_skeletonSynergyBrenna;
        private static ConfigEntry<float>? li_skeletonSynergyRancid;
        
        //Cooldown
        private static ConfigEntry<float>? li_eikthyrBlitzCD;
        private static ConfigEntry<float>? li_deerHorizonHasteCD;
        private static ConfigEntry<float>? li_fenringMoonlitLeapCD;
        private static ConfigEntry<float>? li_loxWildTremorCD;
        private static ConfigEntry<float>? li_wolfRavenousHungerCD;
        private static ConfigEntry<float>? li_fulingshamanRitualCD;
        private static ConfigEntry<float>? li_deathsquitoRelentlessCD;
        private static ConfigEntry<float>? li_surtlingHarbingerCD;
        private static ConfigEntry<float>? li_fulingberserkerGiantizationCD;
        private static ConfigEntry<float>? li_drakeThreeFreezeCD;
        private static ConfigEntry<float>? li_growthAncientTarCD;
        private static ConfigEntry<float>? li_trollTrollTossCD;
        private static ConfigEntry<float>? li_greydwarfshamanDubiousHealCD;
        private static ConfigEntry<float>? li_dvergrRandomizeCD;
        private static ConfigEntry<float>? li_neckSplashCD;
        private static ConfigEntry<float>? li_leechBloodSiphonCD;
        private static ConfigEntry<float>? li_bonemassMassReleaseCD;
        private static ConfigEntry<float>? li_greydwarfbruteBashCD;
        private static ConfigEntry<float>? li_fulingLonginusCD;
        private static ConfigEntry<float>? li_gjallGjallarhornCD;
        private static ConfigEntry<float>? li_greydwarfPebbleCD;
        private static ConfigEntry<float>? li_elderAncientAweCD;
        private static ConfigEntry<float>? li_blobFumesCD;
        private static ConfigEntry<float>? li_skeletonVigilCD;
        private static ConfigEntry<float>? li_abominationBaneCD;
        private static ConfigEntry<float>? li_wraithTwinSoulsCD;
        private static ConfigEntry<float>? li_draugrForgottenCD;
        private static ConfigEntry<float>? li_draugreliteFallenHeroCD;
        private static ConfigEntry<float>? li_geirrhafaIceAgeCD;
        private static ConfigEntry<float>? li_cultistLoneSunCD;
        private static ConfigEntry<float>? li_hareLuckyFootCD;
        private static ConfigEntry<float>? li_seaserpentSeaKingCD;
        private static ConfigEntry<float>? li_tickBloodWellCD;
        private static ConfigEntry<float>? li_moderDraconicFrostCD;
        private static ConfigEntry<float>? li_boarRecklessChargeCD;
        private static ConfigEntry<float>? li_stonegolemCoreOverdriveCD;
        private static ConfigEntry<float>? li_yagluthCulminationCD;
        private static ConfigEntry<float>? li_ulvTerritorialSlumberCD;
        private static ConfigEntry<float>? li_brennaVulcanCD;
        private static ConfigEntry<float>? li_rancidremainsRancorousCD;
        private static ConfigEntry<float>? li_thungrTyrantCD;
        private static ConfigEntry<float>? li_zilSoulmassCD;
        private static ConfigEntry<float>? li_seekersoldierReverberationCD;
        
        //Status Duration
        private static ConfigEntry<float>? li_deerHorizonHasteSED;
        private static ConfigEntry<float>? li_fenringMoonlitLeapSED;
        private static ConfigEntry<float>? li_wolfRavenousHungerSED;
        private static ConfigEntry<float>? li_deathsquitoRelentlessSED;
        private static ConfigEntry<float>? li_surtlingHarbingerSED;
        private static ConfigEntry<float>? li_fulingberserkerGiantizationSED;
        private static ConfigEntry<float>? li_elderAncientAweSED;
        private static ConfigEntry<float>? li_wraithTwinSoulsSED;
        private static ConfigEntry<float>? li_draugrForgottenSED;
        private static ConfigEntry<float>? li_draugreliteFallenHeroSED;
        private static ConfigEntry<float>? li_hareLuckyFootSED;
        private static ConfigEntry<float>? li_boarRecklessChargeSED;
        private static ConfigEntry<float>? li_ulvTerritorialSlumberSED;
        private static ConfigEntry<float>? li_thungrTyrantSED;
        private static ConfigEntry<float>? li_seekersoldierReverberationSED;
        
        //Essence
        private static ConfigEntry<float>? li_deerHorizonHaste;
        private static ConfigEntry<float>? li_deerHorizonHastePassive;
        private static ConfigEntry<float>? li_eikthyrBlitzPassive;
        private static ConfigEntry<float>? li_eikthyrBlitz;
        private static ConfigEntry<float>? li_fenringMoonlitLeap;
        private static ConfigEntry<float>? li_fenringMoonlitLeapPassive;
        private static ConfigEntry<float>? li_loxWildTremor;
        private static ConfigEntry<float>? li_loxWildTremorPassive;
        private static ConfigEntry<float>? li_wolfRavenousHunger;
        private static ConfigEntry<float>? li_wolfRavenousHungerStaminaPassive;
        private static ConfigEntry<float>? li_wolfRavenousHungerPassive;
        private static ConfigEntry<float>? li_fulingshamanRitualShield;
        private static ConfigEntry<float>? li_fulingshamanRitualShieldGrowthCap;
        private static ConfigEntry<float>? li_fulingshamanRitualProjectile;
        private static ConfigEntry<float>? li_fulingshamanRitualPassiveEitr;
        private static ConfigEntry<float>? li_fulingshamanRitualPassiveCarry;
        private static ConfigEntry<float>? li_deathsquitoRelentlessHoming;
        private static ConfigEntry<float>? li_deathsquitoRelentlessHomingRange;
        private static ConfigEntry<float>? li_deathsquitoRelentlessPassive;
        private static ConfigEntry<float>? li_surtlingHarbingerCharges;
        private static ConfigEntry<float>? li_surtlingHarbingerBurn;
        private static ConfigEntry<float>? li_surtlingHarbingerMinDistance;
        private static ConfigEntry<float>? li_fulingberserkerGiantizationHealth;
        private static ConfigEntry<float>? li_drakeThreeFreezeProjectile;
        private static ConfigEntry<float>? li_growthAncientTarProjectile;
        private static ConfigEntry<float>? li_growthAncientTarPassive;
        private static ConfigEntry<float>? li_greydwarfshamanDubiousHealPlayer;
        private static ConfigEntry<float>? li_greydwarfshamanDubiousHealCreature;
        private static ConfigEntry<float>? li_greydwarfshamanDubiousHealPassive;
        private static ConfigEntry<float>? li_greydwarfshamanDubiousHealPassiveEitr;
        private static ConfigEntry<float>? li_trollTrollTossProjectile;
        private static ConfigEntry<float>? li_trollTrollTossPassive;
        private static ConfigEntry<float>? li_dvergrRandomizeIceProjectile;
        private static ConfigEntry<float>? li_dvergrRandomizeFireProjectile;
        private static ConfigEntry<float>? li_dvergrRandomizeHealPlayer;
        private static ConfigEntry<float>? li_dvergrRandomizeHealCreature;
        private static ConfigEntry<float>? li_dvergrRandomizeCost;
        private static ConfigEntry<float>? li_dvergrRandomizePassiveEitr;
        private static ConfigEntry<float>? li_dvergrRandomizePassive;
        private static ConfigEntry<float>? li_leechBloodSiphonStack;
        private static ConfigEntry<float>? li_leechBloodSiphonStackCap;
        private static ConfigEntry<float>? li_bonemassMassReleaseSummonDuration;
        private static ConfigEntry<float>? li_bonemassMassReleaseProjectile;
        private static ConfigEntry<float>? li_greydwarfbruteBashMultiplier;
        private static ConfigEntry<float>? li_greydwarfbruteRangedReductionPassive;
        private static ConfigEntry<float>? li_greydwarfbruteHealthPassive;
        private static ConfigEntry<float>? li_fulingLonginusMultiplier;
        private static ConfigEntry<float>? li_fulingLonginusPassiveBlockMultiplier;
        private static ConfigEntry<float>? li_fulingLonginusPassiveMotivated;
        private static ConfigEntry<float>? li_fulingLonginusPassiveDemotivated;
        private static ConfigEntry<float>? li_gjallGjallarhornSummonDuration;
        private static ConfigEntry<float>? li_gjallGjallarhornProjectile;
        private static ConfigEntry<float>? li_gjallGjallarhornArmor;
        private static ConfigEntry<float>? li_greydwarfPebbleProjectile;
        private static ConfigEntry<float>? li_greydwarfPebblePassiveCarry;
        private static ConfigEntry<float>? li_greydwarfPebbleForestAnger;
        private static ConfigEntry<float>? li_elderAncientAwePassive;
        private static ConfigEntry<float>? li_blobFumes;
        private static ConfigEntry<float>? li_skeletonVigilSummons;
        private static ConfigEntry<float>? li_skeletonVigilSummonDuration;
        private static ConfigEntry<float>? li_skeletonVigilSoulCap;
        private static ConfigEntry<float>? li_abominationBaneArmor;
        private static ConfigEntry<float>? li_abominationBaneHealth;
        private static ConfigEntry<float>? li_abominationBaneAllySpeed;
        private static ConfigEntry<float>? li_abominationBaneAllyHealth;
        private static ConfigEntry<float>? li_abominationBaneAllyAttack;
        private static ConfigEntry<float>? li_wraithTwinSoulsArmor;
        private static ConfigEntry<float>? li_wraithTwinSoulsPassive;
        private static ConfigEntry<float>? li_wraithTwinSoulsAllySpeed;
        private static ConfigEntry<float>? li_wraithTwinSoulsAllyHealth;
        private static ConfigEntry<float>? li_wraithTwinSoulsAllyAttack;
        private static ConfigEntry<float>? li_draugrForgottenRot;
        private static ConfigEntry<float>? li_draugrForgottenPassiveCarry;
        private static ConfigEntry<float>? li_draugrForgottenActive;
        private static ConfigEntry<float>? li_draugreliteFallenHeroRot;
        private static ConfigEntry<float>? li_draugreliteFallenHeroPassiveCarry;
        private static ConfigEntry<float>? lidraugreliteFallenHeroActive;
        private static ConfigEntry<float>? li_geirrhafaIceAgeAoe;
        private static ConfigEntry<float>? li_geirrhafaIceAgePassiveEitr;
        private static ConfigEntry<float>? li_geirrhafaIceAgePassive;
        private static ConfigEntry<float>? li_cultistLoneSunAoe;
        private static ConfigEntry<float>? li_cultistLoneSunPassive;
        private static ConfigEntry<float>? li_hareLuckyFoot;
        private static ConfigEntry<float>? li_hareLuckyFootArmor;
        private static ConfigEntry<float>? li_seaserpentSeaKingProjectile;
        private static ConfigEntry<float>? li_tickBloodWellLifeSteal;
        private static ConfigEntry<float>? li_tickBloodWellArmor;
        private static ConfigEntry<float>? li_tickBloodWellStackCap;
        private static ConfigEntry<float>? li_moderDraconicFrostProjectile;
        private static ConfigEntry<float>? li_moderDraconicFrostPassive;
        private static ConfigEntry<float>? li_moderDraconicFrostDragonBreath;
        private static ConfigEntry<float>? li_boarRecklessCharge;
        private static ConfigEntry<float>? li_boarRecklessChargePassive;
        private static ConfigEntry<float>? li_stonegolemCoreOverdriveStacks;
        private static ConfigEntry<float>? li_stonegolemCoreOverdriveArmor;
        private static ConfigEntry<float>? li_yagluthCulminationStaticCap;
        private static ConfigEntry<float>? li_yagluthCulmination;
        private static ConfigEntry<float>? li_ulvTerritorialSlumberComfort;
        private static ConfigEntry<float>? li_ulvTerritorialSlumberStamina;
        private static ConfigEntry<float>? li_ulvTerritorialSlumberSummonDuration;
        private static ConfigEntry<float>? li_ulvTerritorialSlumberSummonHealth;
        private static ConfigEntry<float>? li_brennaVulcanArmor;
        private static ConfigEntry<float>? li_rancidremainsRancorousArmor;
        private static ConfigEntry<float>? li_thungrTyrantArmor;
        private static ConfigEntry<float>? li_thungrTyrantArroganceEnemyBuff;
        private static ConfigEntry<float>? li_thungrTyrantArrogancePlayerDebuff;
        private static ConfigEntry<float>? li_thungrTyrantDisdain;
        private static ConfigEntry<float>? li_zilSoulmassProjectile;
        private static ConfigEntry<float>? li_zilSoulmassPassiveEitr;
        private static ConfigEntry<float>? li_seekersoldierReverberationArmor;
        private static ConfigEntry<float>? li_seekersoldierReverberationReflection;
        
        
        // public static List<string> equipedEssence = new();
        private static LackingImaginationV2Plugin _instance;

        // Prefabs type 1 //Pulled from in game
        public static GameObject fx_Harbinger;
        
        //Weapons
        public static GameObject? GO_VulcanSwordBroken;
        public static GameObject? GO_VulcanSword;
        public static GameObject? fx_Vulcan;
        public static GameObject? GO_RancorousMaceBroken;
        public static GameObject? GO_RancorousMace;
        public static GameObject? fx_Rancorous;
        
        //Prefabs type 2 //Pulled from assets
        public static GameObject? fx_Giantization;
        public static GameObject? fx_Bash;
        public static GameObject? fx_Longinus;
        public static GameObject? fx_TwinSouls;
        public static GameObject? fx_BloodWell;
        public static GameObject? fx_TerritorialSlumber;
        public static GameObject? fx_BloodSiphon;
        public static GameObject? fx_RavenousHunger;
        public static GameObject? fx_Relentless;
        public static GameObject? fx_Brenna;
        public static GameObject? fx_Rancid;

        public static GameObject? fx_Disdain;
        public static GameObject? fx_ArroBuff;
        public static GameObject? fx_ArroDebuff;

        public static GameObject? fx_Reverberation;
        
        //Prefab3 //originals
        public static GameObject? p_SeaKing;
        public static GameObject? fx_ocean_hit;
        public static GameObject? WhirlPool2;
            
        public static GameObject? fx_RecklessCharge;
        public static GameObject? fx_RecklessChargeHit;
        
        
        //Sounds
        public static GameObject? sfx_Giantization;
        
        //Prefab4 new bundle
        public static GameObject SG_Spiked_Arms;
        public static GameObject StoneGolem_Player;
        public static GameObject SG_Club_Arms;
        public static GameObject SG_Hat;
        
        
        // Animation Clips // Pulled from in game
        private static Animator? creatureAnimatorGeirrhafa;
        private static AnimationClip? creatureAnimationClipGeirrhafaIceNova;
        private static AnimationClip? creatureAnimationClipCultistSpray; // Flame Spell Attack
        private static Animator? creatureAnimatorElder;
        private static AnimationClip? creatureAnimationClipElderSummon;
        private static Animator? creatureAnimatorFenring;
        private static AnimationClip? creatureAnimationClipFenringLeapAttack; //Leap Attack//
        private static Animator? creatureAnimatorGreyShaman;
        private static AnimationClip? creatureAnimationClipGreyShamanHeal; //Standing 1H Cast Spell 01 
        private static Animator? creatureAnimatorHaldor;
        private static AnimationClip? creatureAnimationClipHaldorGreet; //Greet//
        private static Animator? creatureAnimatorPlayer;
        private static AnimationClip? creatureAnimationClipPlayerEmoteCower; 
        private static AnimationClip? creatureAnimationClipPlayerMace2; 
        private static AnimationClip? creatureAnimationClipPlayerEmotePoint; 
        private static AnimationClip? creatureAnimationClipPlayerEmoteDespair; 
        private static Animator? creatureAnimatorBrenna;
        private static AnimationClip? creatureAnimationClipBrennaGroundStab;
        private static Animator? creatureAnimatorDvergr;
        private static AnimationClip? creatureAnimationClipDvergrStaffRaise;
        
        
        // Animation Clip swappers
        private static readonly Dictionary<string, AnimationClip> OutsideAnimations = new();
        private static readonly Dictionary<string, RuntimeAnimatorController> CustomizedRuntimeControllers = new();
        private static readonly Dictionary<string, Dictionary<string, string>> replacementMap = new();
        private static bool _firstInit;



       
        
        
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


            // Creature SeekerPlayer = new Creature("seekerp", "SeekerPlayer");
            

            
            
            
            
            

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
            fx_Vulcan = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "VulcanFloor");
            fx_Rancorous = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "RancorousFloor");
            fx_RecklessCharge = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "BoarGuard");
            fx_RecklessChargeHit = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "BoarHit");
            
            fx_Brenna = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "Skeleton_Hildir_Fire");
            fx_Rancid = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "Skeleton_poison_drip");
            
            fx_Disdain = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "ThungrDisdain");
            fx_ArroBuff = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "ThungrBuff");
            fx_ArroDebuff = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "ThungrDebuff");
            
            fx_Reverberation = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "Thorns");
       
            //Prefab 3
            p_SeaKing = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "Serpent_projectile");
            fx_ocean_hit = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "fx_ocean_hit");
            WhirlPool2 = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "WhirlPool2");
            
            
            //Prefab 4
            // SG_Spiked_Arms = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_spikes_Player");
            // StoneGolem_Player = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_Player");
            // SG_Hat = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_hat_Player");
            // SG_Club_Arms = ItemManager.PrefabManager.RegisterPrefab("sgasset", "StoneGolem_clubs_Player");
            // ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack1_spike_Player");
            // ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack2_left_groundslam_Player");
            // ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack3_spikesweep_Player");
            // ItemManager.PrefabManager.RegisterPrefab("sgasset", "stonegolem_attack_doublesmash_Player");
            
            
            //Sound Prefabs
            sfx_Giantization = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", "sfx_goblinbrute_rage");
            
            
            
            abilitiesStatus.Clear();
            for (int i = 0; i < 5; i++)
            {
                abilitiesStatus.Add(null);
            }
            



            EssenceSlotsEnabled = Config.Bind("Toggles", "Enable Essence Slots", true, "Disabling this while items are in the slots will attempt to move them to your inventory.");

            Sprintkey = config("Keybinds", "Sprintkey", KeyCode.LeftShift, "This does not change the sprint key, \nit is to let me know what yours is for better casting.");
            Ability1_Hotkey = config("Keybinds", "Ability1_Hotkey", KeyCode.LeftAlt, "Ability 1 Hotkey");
            Ability1_Combokey = config("Keybinds", "Ability1_Combokey", KeyCode.Alpha1, "Ability 1 Combokey");
            Ability2_Hotkey = config("Keybinds", "Ability2_Hotkey", KeyCode.LeftAlt, "Ability 2 Hotkey");
            Ability2_Combokey = config("Keybinds", "Ability2_Combokey", KeyCode.Alpha2, "Ability 2 Combokey");
            Ability3_Hotkey = config("Keybinds", "Ability3_Hotkey", KeyCode.LeftAlt, "Ability 3 Hotkey");
            Ability3_Combokey = config("Keybinds", "Ability3_Combokey", KeyCode.Alpha3, "Ability 3 Combokey");
            Ability4_Hotkey = config("Keybinds", "Ability4_Hotkey", KeyCode.LeftAlt, "Ability 4 Hotkey");
            Ability4_Combokey = config("Keybinds", "Ability4_Combokey", KeyCode.Alpha4, "Ability 4 Combokey");
            Ability5_Hotkey = config("Keybinds", "Ability5_Hotkey", KeyCode.LeftAlt, "Ability 5 Hotkey");
            Ability5_Combokey = config("Keybinds", "Ability5_Combokey", KeyCode.Alpha5, "Ability 5 Combokey");

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
            //Thungr
            li_thungrTyrantCD = config("Essence Thungr Modifiers", "li_thungrTyrantCD", 180f, "Cooldown");
            li_thungrTyrantSED = config("Essence Thungr Modifiers", "li_thungrTyrantSED", 90f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_thungrTyrantArmor = config("Essence Thungr Modifiers", "li_thungrTyrantArmor", 5f, "Armor gained per 1% of movement speed reduced by equipment or lost per 1% of movement speed increased by equipment");
            li_thungrTyrantArroganceEnemyBuff = config("Essence Thungr Modifiers", "li_thungrTyrantArroganceEnemyBuff", 75f, new ConfigDescription("Percentage of damage increase to Marked enemy", new AcceptableValueRange<float>(0f, 100f)));
            li_thungrTyrantArrogancePlayerDebuff = config("Essence Thungr Modifiers", "li_thungrTyrantArrogancePlayerDebuff", 25f, new ConfigDescription("Percentage player damage is decrease to", new AcceptableValueRange<float>(0f, 100f)));
            li_thungrTyrantDisdain = config("Essence Thungr Modifiers", "li_thungrTyrantDisdain", 50f, new ConfigDescription("Percentage of stats stolen from defeated enemy", new AcceptableValueRange<float>(0f, 100f)));
            //Zil
            li_zilSoulmassCD = config("Essence Zil Modifiers", "li_zilSoulmassCD", 30f, "Cooldown");
            li_zilSoulmassProjectile = config("Essence Zil Modifiers", "li_zilSoulmassProjectile", 150f, "Modifies the damage of Soulmass Projectile");
            li_zilSoulmassPassiveEitr = config("Essence Zil Modifiers", "li_zilSoulmassPassiveEitr", 50f, "Modifies bonus Eitr passive");
            //Seeker Soldier
            li_seekersoldierReverberationCD = config("Essence Seeker Soldier Modifiers", "li_seekersoldierReverberationCD", 30f, "Cooldown");
            li_seekersoldierReverberationSED = config("Essence Seeker Soldier Modifiers", "li_seekersoldierReverberationSED", 80f, new ConfigDescription("Percentage of Cooldown that the status effect will last", new AcceptableValueRange<float>(0f, 100f)));
            li_seekersoldierReverberationArmor = config("Essence Seeker Soldier Modifiers", "li_seekersoldierReverberationArmor", 75f, "Bonus Armor passive");
            li_seekersoldierReverberationReflection = config("Essence Seeker Soldier Modifiers", "li_seekersoldierReverberationReflection", 75f, new ConfigDescription("Percentage of Armor Returned as Damage", new AcceptableValueRange<float>(0f, 1000f)));

            
            
            
            
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
            LackingImaginationGlobal.ConfigStrings.Add("li_thungrTyrantCD", li_thungrTyrantCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_zilSoulmassCD", li_zilSoulmassCD.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_seekersoldierReverberationCD", li_seekersoldierReverberationCD.Value);
            
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
            LackingImaginationGlobal.ConfigStrings.Add("li_thungrTyrantSED", li_thungrTyrantSED.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_seekersoldierReverberationSED", li_seekersoldierReverberationSED.Value);
            
            
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
            LackingImaginationGlobal.ConfigStrings.Add("li_thungrTyrantArmor", li_thungrTyrantArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_thungrTyrantArroganceEnemyBuff", li_thungrTyrantArroganceEnemyBuff.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_thungrTyrantArrogancePlayerDebuff", li_thungrTyrantArrogancePlayerDebuff.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_thungrTyrantDisdain", li_thungrTyrantDisdain.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_zilSoulmassProjectile", li_zilSoulmassProjectile.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_zilSoulmassPassiveEitr", li_zilSoulmassPassiveEitr.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_seekersoldierReverberationArmor", li_seekersoldierReverberationArmor.Value);
            LackingImaginationGlobal.ConfigStrings.Add("li_seekersoldierReverberationReflection", li_seekersoldierReverberationReflection.Value);
            
            
            _ = ConfigSync.AddConfigEntry(Sprintkey);
            _ = ConfigSync.AddConfigEntry(Ability1_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability1_Combokey);
            _ = ConfigSync.AddConfigEntry(Ability2_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability2_Combokey);
            _ = ConfigSync.AddConfigEntry(Ability3_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability3_Combokey);
            _ = ConfigSync.AddConfigEntry(Ability4_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability4_Combokey);
            _ = ConfigSync.AddConfigEntry(Ability5_Hotkey);
            _ = ConfigSync.AddConfigEntry(Ability5_Combokey);

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
            _ = ConfigSync.AddConfigEntry(li_thungrTyrantCD);
            _ = ConfigSync.AddConfigEntry(li_zilSoulmassCD);
            _ = ConfigSync.AddConfigEntry(li_seekersoldierReverberationCD);
            
            
            
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
            _ = ConfigSync.AddConfigEntry(li_thungrTyrantSED);
            _ = ConfigSync.AddConfigEntry(li_seekersoldierReverberationSED);
                
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
            _ = ConfigSync.AddConfigEntry(li_thungrTyrantArmor);
            _ = ConfigSync.AddConfigEntry(li_thungrTyrantArroganceEnemyBuff); 
            _ = ConfigSync.AddConfigEntry(li_thungrTyrantArrogancePlayerDebuff);
            _ = ConfigSync.AddConfigEntry(li_thungrTyrantDisdain); 
            _ = ConfigSync.AddConfigEntry(li_zilSoulmassProjectile);
            _ = ConfigSync.AddConfigEntry(li_zilSoulmassPassiveEitr); 
            _ = ConfigSync.AddConfigEntry(li_seekersoldierReverberationArmor);
            _ = ConfigSync.AddConfigEntry(li_seekersoldierReverberationReflection); 
            
            
                
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
        

        [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.Start))]
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
                        LackingImaginationV2Plugin.fx_Harbinger.AddComponent<ZNetView>();
                        LackingImaginationV2Plugin.fx_Harbinger.AddComponent<ZSyncTransform>();
                        LackingImaginationV2Plugin.fx_Harbinger.AddComponent<TimedDestruction>();
                        LackingImaginationV2Plugin.fx_Harbinger.GetComponent<ZSyncTransform>().m_syncPosition = true;
                        LackingImaginationV2Plugin.fx_Harbinger.GetComponent<ZSyncTransform>().m_syncRotation = true;
                        LackingImaginationV2Plugin.fx_Harbinger.GetComponent<ZSyncTransform>().m_syncScale = true;
                        break;
                    }
                }

                //Fire Head Effect
                {
                    GameObject ClonePrefab = fx_Brenna.transform.Find("fx_Torch_Carried").gameObject;
                    GameObject OriginalPrefab = ZNetScene.instance.GetPrefab("Skeleton_Hildir").transform.Find("Visual").transform.Find("_skeleton_base").transform.Find("Armature").transform.Find("Hips").transform.Find("Spine").transform.Find("Spine1").transform.Find("Spine2").transform.Find("Neck").transform.Find("Head").transform.Find("fx_Torch_Carried").gameObject;
                    if (ClonePrefab != null && OriginalPrefab != null)
                    {
                        ExpMethods.CopyMaterialsRecursively(ClonePrefab.transform, OriginalPrefab.transform);
                    }
                }
                //Poison drip Effect
                {
                    GameObject ClonePrefab = fx_Rancid.transform.Find("vfx_drippingwater").gameObject;
                    GameObject OriginalPrefab =  ZNetScene.instance.GetPrefab("Skeleton_Poison").transform.Find("Visual").transform.Find("vfx_drippingwater").gameObject;
                    if (ClonePrefab != null && OriginalPrefab != null)
                    {
                        ExpMethods.CopyMaterialsRecursively(ClonePrefab.transform, OriginalPrefab.transform);
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
                
                
                //Material Fix
                {
                    foreach (KeyValuePair<string, string> kvp in EssenceTrophyMaterials)
                    {
                        ExpMethods.FixMaterials(kvp.Key, kvp.Value);
                    }
                }
            }
        }
         
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        private static class Patch_Player_Start
        {
            private static void Postfix(Player __instance)
            {
                if (__instance == Player.m_localPlayer)
                {
                    if (_firstInit) return;

                    if (creatureAnimationClipGeirrhafaIceNova != null && creatureAnimationClipElderSummon != null &&
                        creatureAnimationClipCultistSpray != null && creatureAnimationClipFenringLeapAttack != null &&
                        creatureAnimationClipGreyShamanHeal != null && creatureAnimationClipHaldorGreet != null &&
                        creatureAnimationClipPlayerEmoteCower != null && creatureAnimationClipPlayerEmotePoint != null &&
                        creatureAnimationClipPlayerMace2 != null && creatureAnimationClipBrennaGroundStab != null &&
                        creatureAnimationClipDvergrStaffRaise != null && creatureAnimationClipPlayerEmoteDespair != null &&
                        creatureAnimationClipBrennaGroundStab != null && creatureAnimationClipDvergrStaffRaise != null) // ADD REST
                    {
                        // LogWarning($"animations good");
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
                            CustomizedRuntimeControllers["FenringLeapControl"] = MakeAOC(replacementMap["FenringLeap"], __instance.m_animator.runtimeAnimatorController); // unused, use it
                            CustomizedRuntimeControllers["GreyShamanHealControl"] = MakeAOC(replacementMap["GreyShamanHeal"], __instance.m_animator.runtimeAnimatorController);
                            CustomizedRuntimeControllers["HaldorGreetControl"] = MakeAOC(replacementMap["HaldorGreet"], __instance.m_animator.runtimeAnimatorController); // unused
                            CustomizedRuntimeControllers["PlayerCowerControl"] = MakeAOC(replacementMap["PlayerCowerEmote"], __instance.m_animator.runtimeAnimatorController);
                            CustomizedRuntimeControllers["PlayerPointControl"] = MakeAOC(replacementMap["PlayerPointEmote"], __instance.m_animator.runtimeAnimatorController);
                            CustomizedRuntimeControllers["PlayerMace2Control"] = MakeAOC(replacementMap["PlayerMace2"], __instance.m_animator.runtimeAnimatorController);
                            CustomizedRuntimeControllers["PlayerDespairControl"] = MakeAOC(replacementMap["PlayerDespair"], __instance.m_animator.runtimeAnimatorController);
                            CustomizedRuntimeControllers["SkeletonGroundStabControl"] = MakeAOC(replacementMap["GroundStab"], __instance.m_animator.runtimeAnimatorController); // upgrade anim, equip first
                            CustomizedRuntimeControllers["DvergrRaiseStaffControl"] = MakeAOC(replacementMap["RaiseStaff"], __instance.m_animator.runtimeAnimatorController); // upgrade anim, equip first

                        }
                    }

                    _firstInit = true;
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
                    if (xFenringEssence.FenringController)
                    {
                        controllerName = "FenringLeapControl";
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