using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LackingImaginationV2

{
    public class LackingImaginationGlobal
    {
        public static Dictionary<string, float> ConfigStrings;
        
        
        public static float g_CooldownModifer
        {
            get
            {
                try
                {
                    return ConfigStrings["li_cooldownMultiplier"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        
        
        
        
        
        
        
        //Synergies
        public static float c_draugrSynergyRot
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugrSynergyRot"]/100;
                }
                catch
                {
                    return 0.05f;
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        //Deer
        public static float c_deerHorizonHasteCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_deerHorizonHasteCD"];
                }
                catch
                {
                    return 25f;
                }
            }
        }
        public static float c_deerHorizonHaste
        {
            get
            {
                try
                {
                    return ConfigStrings["li_deerHorizonHaste"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_deerHorizonHastePassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_deerHorizonHastePassive"];
                }
                catch
                {
                    return 25f;
                }
            }
        }
        
        //Eikthyr
        public static float c_eikthyrBlitzCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_eikthyrBlitzCD"];
                }
                catch
                {
                    return 25f;
                }
            }
        }
        public static float c_eikthyrBlitzPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_eikthyrBlitzPassive"]/100f;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        public static float c_eikthyrBlitz
        {
            get
            {
                try
                {
                    return ConfigStrings["li_eikthyrBlitz"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        
        //Fenring
        public static float c_fenringMoonlitLeapCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fenringMoonlitLeapCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_fenringMoonlitLeap
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fenringMoonlitLeap"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_fenringMoonlitLeapPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fenringMoonlitLeapPassive"]/100f;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        
        //Lox
        public static float c_loxWildTremorCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_loxWildTremorCD"];
                }
                catch
                {
                    return 25f;
                }
            }
        }
        public static float c_loxWildTremor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_loxWildTremor"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_loxWildTremorPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_loxWildTremorPassive"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        
        // Wolf
        public static float c_wolfRavenousHungerCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wolfRavenousHungerCD"];
                }
                catch
                {
                    return 120f;
                }
            }
        }
        public static float c_wolfRavenousHunger
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wolfRavenousHunger"]/100f;
                }
                catch
                {
                    return 0.05f;
                }
            }
        }
        public static float c_wolfRavenousHungerStaminaPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wolfRavenousHungerStaminaPassive"];
                }
                catch
                {
                    return 65f;
                }
            }
        }
        public static float c_wolfRavenousHungerPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wolfRavenousHungerPassive"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        
        // Fuling Shaman
        public static float c_fulingshamanRitualCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingshamanRitualCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_fulingshamanRitualShield
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingshamanRitualShield"];
                }
                catch
                {
                    return 200f;
                }
            }
        }
        public static float c_fulingshamanRitualShieldGrowthCap
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingshamanRitualShieldGrowthCap"];
                }
                catch
                {
                    return 800f;
                }
            }
        }
        public static float c_fulingshamanRitualProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingshamanRitualProjectile"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        public static float c_fulingshamanRitualPassiveEitr
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingshamanRitualPassiveEitr"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        public static float c_fulingshamanRitualPassiveCarry
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingshamanRitualPassiveCarry"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        
        // Deathsquito
        public static float c_deathsquitoRelentlessCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_deathsquitoRelentlessCD"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float c_deathsquitoRelentlessHoming
        {
            get
            {
                try
                {
                    return ConfigStrings["li_deathsquitoRelentlessHoming"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        public static float c_deathsquitoRelentlessPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_deathsquitoRelentlessPassive"]/100;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        
        //Surtling
        public static float c_surtlingHarbingerCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_surtlingHarbingerCD"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float c_surtlingHarbingerCharges
        {
            get
            {
                try
                {
                    return ConfigStrings["li_surtlingHarbingerCharges"];
                }
                catch
                {
                    return 10;
                }
            }
        }
        public static float c_surtlingHarbingerBurn
        {
            get
            {
                try
                {
                    return ConfigStrings["li_surtlingHarbingerBurn"]/100;
                }
                catch
                {
                    return 1;
                }
            }
        }
        
        public static float c_surtlingHarbingerMinDistance
        {
            get
            {
                try
                {
                    return ConfigStrings["li_surtlingHarbingerMinDistance"];
                }
                catch
                {
                    return 8;
                }
            }
        }
        
        //FulingBerserker?
        public static float c_fulingberserkerGiantizationCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingberserkerGiantizationCD"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        //Drake
        public static float c_drakeThreeFreezeCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_drakeThreeFreezeCD"];
                }
                catch
                {
                    return 12f;
                }
            }
        }
        public static float  c_drakeThreeFreezeProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_drakeThreeFreezeProjectile"]/100f;
                }
                catch
                {
                    return 0.5f;
                }
            }
        }
        
        //Growth
        public static float c_growthAncientTarCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_growthAncientTarCD"];
                }
                catch
                {
                    return 15f;
                }
            }
        }
        public static float  c_growthAncientTarProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_growthAncientTarProjectile"]/100f;
                }
                catch
                {
                    return 0.5f;
                }
            }
        }
        public static float  c_growthAncientTarPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_growthAncientTarPassive"]/100f;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        
        //Greydwarf Shaman
        public static float c_greydwarfshamanDubiousHealCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfshamanDubiousHealCD"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float  c_greydwarfshamanDubiousHealPlayer
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfshamanDubiousHealPlayer"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        public static float  c_greydwarfshamanDubiousHealCreature
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfshamanDubiousHealCreature"]/100f;
                }
                catch
                {
                    return 0.20f;
                }
            }
        }
        public static float  c_greydwarfshamanDubiousHealPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfshamanDubiousHealPassive"] - 1f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_greydwarfshamanDubiousHealPassiveEitr
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfshamanDubiousHealPassiveEitr"];
                }
                catch
                {
                    return 20f;
                }
            }
        }
        
        //Troll    
        public static float c_trollTrollTossCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_trollTrollTossCD"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        public static float c_trollTrollTossProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_trollTrollTossProjectile"]/100;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_trollTrollTossPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_trollTrollTossPassive"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        
        //Dvergr
        public static float c_dvergrRandomizeCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizeCD"];
                }
                catch
                {
                    return 2f;
                }
            }
        }
        public static float c_dvergrRandomizeCost
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizeCost"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        public static float c_dvergrRandomizeIceProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizeIceProjectile"]/100;
                }
                catch
                {
                    return 0.05f;
                }
            }
        }
        public static float c_dvergrRandomizeFireProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizeFireProjectile"]/100;
                }
                catch
                {
                    return 0.5f;
                }
            }
        }
        public static float  c_dvergrRandomizeHealPlayer
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizeHealPlayer"];
                }
                catch
                {
                    return 110f;
                }
            }
        }
        public static float  c_dvergrRandomizeHealCreature
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizeHealCreature"]/100f;
                }
                catch
                {
                    return 0.50f;
                }
            }
        }
        public static float c_dvergrRandomizePassiveEitr
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizePassiveEitr"];
                }
                catch
                {
                    return 80f;
                }
            }
        }
        public static float c_dvergrRandomizePassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_dvergrRandomizePassive"]/100;
                }
                catch
                {
                    return 0.6f;
                }
            }
        }
        //Neck
        public static float c_neckSplashCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_neckSplashCD"];
                }
                catch
                {
                    return 3f;
                }
            }
        }
        //Leech
        public static float c_leechBloodSiphonCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_leechBloodSiphonCD"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        public static float c_leechBloodSiphonStack
        {
            get
            {
                try
                {
                    return ConfigStrings["li_leechBloodSiphonStack"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        public static float c_leechBloodSiphonStackCap
        {
            get
            {
                try
                {
                    return ConfigStrings["li_leechBloodSiphonStackCap"];
                }
                catch
                {
                    return 500f;
                }
            }
        }
        
        //BoneMass
        public static float c_bonemassMassReleaseCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_bonemassMassReleaseCD"];
                }
                catch
                {
                    return 20f;
                }
            }
        }
        public static float c_bonemassMassReleaseSummonDuration
        {
            get
            {
                try
                {
                    return ConfigStrings["li_bonemassMassReleaseSummonDuration"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        public static float c_bonemassMassReleaseProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_bonemassMassReleaseProjectile"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        //GreydwarfBrute
        public static float c_greydwarfbruteBashCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfbruteBashCD"];
                }
                catch
                {
                    return 8f;
                }
            }
        }
        public static float c_greydwarfbruteBashMultiplier
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfbruteBashMultiplier"];
                }
                catch
                {
                    return 2f;
                }
            }
        }
        public static float c_greydwarfbruteRangedReductionPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfbruteRangedReductionPassive"]/100f;
                }
                catch
                {
                    return 0.9f;
                }
            }
        }
        
        //fuling
        public static float c_fulingLonginusCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingLonginusCD"];
                }
                catch
                {
                    return 8f;
                }
            }
        }
        public static float c_fulingLonginusMultiplier
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingLonginusMultiplier"];
                }
                catch
                {
                    return 3f;
                }
            }
        }
        public static float c_fulingLonginusPassiveBlockMultiplier
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingLonginusPassiveBlockMultiplier"];
                }
                catch
                {
                    return 2f;
                }
            }
        }
        public static float c_fulingLonginusPassiveMotivated
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingLonginusPassiveMotivated"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float c_fulingLonginusPassiveDemotivated
        {
            get
            {
                try
                {
                    return ConfigStrings["li_fulingLonginusPassiveDemotivated"]/100;
                }
                catch
                {
                    return 0.5f;
                }
            }
        }
        
        //Gjall
        public static float c_gjallGjallarhornCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_gjallGjallarhornCD"];
                }
                catch
                {
                    return 25f;
                }
            }
        }
        public static float c_gjallGjallarhornSummonDuration
        {
            get
            {
                try
                {
                    return ConfigStrings["li_gjallGjallarhornSummonDuration"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        public static float c_gjallGjallarhornProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_gjallGjallarhornProjectile"];
                }
                catch
                {
                    return 130f;
                }
            }
        }
        public static float c_gjallGjallarhornArmor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_gjallGjallarhornArmor"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        
        //Greydwarf
        public static float c_greydwarfPebbleCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfPebbleCD"];
                }
                catch
                {
                    return 5f;
                }
            }
        }
        public static float c_greydwarfPebbleProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfPebbleProjectile"];
                }
                catch
                {
                    return 20f;
                }
            }
        }
        public static float c_greydwarfPebblePassiveCarry
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfPebblePassiveCarry"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        public static float c_greydwarfPebbleForestAnger
        {
            get
            {
                try
                {
                    return ConfigStrings["li_greydwarfPebbleForestAnger"]/100;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        
        //Elder
        public static float c_elderAncientAweCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_elderAncientAweCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_elderAncientAwePassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_elderAncientAwePassive"]-1f;
                }
                catch
                {
                    return 2f;
                }
            }
        }
         
        //Blob
        public static float c_blobFumesCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_blobFumesCD"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        public static float c_blobFumes
        {
            get
            {
                try
                {
                    return ConfigStrings["li_blobFumes"]/100;
                }
                catch
                {
                    return 0.5f;
                }
            }
        }
        
        //Skeleton
        public static float c_skeletonVigilCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_skeletonVigilCD"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        public static float c_skeletonVigilSummons
        {
            get
            {
                try
                {
                    return ConfigStrings["li_skeletonVigilSummons"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        
        public static float c_skeletonVigilSummonDuration
        {
            get
            {
                try
                {
                    return ConfigStrings["li_skeletonVigilSummonDuration"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        public static float c_skeletonVigilSoulCap
        {
            get
            {
                try
                {
                    return ConfigStrings["li_skeletonVigilSoulCap"];
                }
                catch
                {
                    return 300f;
                }
            }
        }
        
        //Abomination
        public static float c_abominationBaneCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_abominationBaneCD"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float c_abominationBaneArmor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_abominationBaneArmor"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_abominationBaneHealth
        {
            get
            {
                try
                {
                    return ConfigStrings["li_abominationBaneHealth"]/100;
                }
                catch
                {
                    return 0.05f;
                }
            }
        }
        public static float c_abominationBaneAllySpeed
        {
            get
            {
                try
                {
                    return ConfigStrings["li_abominationBaneAllySpeed"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_abominationBaneAllyHealth
        {
            get
            {
                try
                {
                    return ConfigStrings["li_abominationBaneAllyHealth"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_abominationBaneAllyAttack
        {
            get
            {
                try
                {
                    return ConfigStrings["li_abominationBaneAllyAttack"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        
        //Wraith
        public static float c_wraithTwinSoulsCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wraithTwinSoulsCD"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        public static float c_wraithTwinSoulsArmor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wraithTwinSoulsArmor"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        public static float c_wraithTwinSoulsPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wraithTwinSoulsPassive"]/100;
                }
                catch
                {
                    return 0.15f;
                }
            }
        }
        public static float c_wraithTwinSoulsAllySpeed
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wraithTwinSoulsAllySpeed"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_wraithTwinSoulsAllyHealth
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wraithTwinSoulsAllyHealth"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_wraithTwinSoulsAllyAttack
        {
            get
            {
                try
                {
                    return ConfigStrings["li_wraithTwinSoulsAllyAttack"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        //Draugr
        public static float c_draugrForgottenCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugrForgottenCD"];
                }
                catch
                {
                    return 0f;
                }
            }
        }
        public static float c_draugrForgottenRot
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugrForgottenRot"]/100;
                }
                catch
                {
                    return 0.05f;
                }
            }
        }
        public static float c_draugrForgottenPassiveCarry
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugrForgottenPassiveCarry"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float c_draugrForgottenActive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugrForgottenActive"]/100;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }

        //DraugrElite
        public static float c_draugreliteFallenHeroCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugreliteFallenHeroCD"];
                }
                catch
                {
                    return 60f;
                }
            }
        }
        public static float c_draugreliteFallenHeroRot
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugreliteFallenHeroRot"]/100;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        public static float c_draugreliteFallenHeroPassiveCarry
        {
            get
            {
                try
                {
                    return ConfigStrings["li_draugreliteFallenHeroPassiveCarry"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        public static float c_draugreliteFallenHeroActive
        {
            get
            {
                try
                {
                    return ConfigStrings["lidraugreliteFallenHeroActive"]/100;
                }
                catch
                {
                    return 0.2f;
                }
            }
        }
        
        //Geirrhafa
        public static float c_geirrhafaIceAgeCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_geirrhafaIceAgeCD"];
                }
                catch
                {
                    return 45f;
                }
            }
        }
        public static float c_geirrhafaIceAgeAoe
        {
            get
            {
                try
                {
                    return ConfigStrings["li_geirrhafaIceAgeAoe"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        public static float c_geirrhafaIceAgePassiveEitr
        {
            get
            {
                try
                {
                    return ConfigStrings["li_geirrhafaIceAgePassiveEitr"];
                }
                catch
                {
                    return 70f;
                }
            }
        }
        
        public static float c_geirrhafaIceAgePassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_geirrhafaIceAgePassive"]/100;
                }
                catch
                {
                    return 0.2f;
                }
            }
        }
        
        //Cultist
        public static float c_cultistLoneSunCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_cultistLoneSunCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_cultistLoneSunAoe
        {
            get
            {
                try
                {
                    return ConfigStrings["li_cultistLoneSunAoe"];
                }
                catch
                {
                    return 50f;
                }
            }
        }
        public static float c_cultistLoneSunPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_cultistLoneSunPassive"]/100;
                }
                catch
                {
                    return 0.15f;
                }
            }
        }

        //Hare
        public static float c_hareLuckyFootCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_hareLuckyFootCD"];
                }
                catch
                {
                    return 40f;
                }
            }
        }
        public static float c_hareLuckyFoot
        {
            get
            {
                try
                {
                    return ConfigStrings["li_hareLuckyFoot"]/100f;
                }
                catch
                {
                    return 1f;
                }
            }
        }
        public static float c_hareLuckyFootArmor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_hareLuckyFootArmor"]/100;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        
        //Sea Serpent
        public static float c_seaserpentSeaKingCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_seaserpentSeaKingCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_seaserpentSeaKingProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_seaserpentSeaKingProjectile"]/100;
                }
                catch
                {
                    return 0.2f;
                }
            }
        }
        
        //Tick
        public static float c_tickBloodWellCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_tickBloodWellCD"];
                }
                catch
                {
                    return 10f;
                }
            }
        }
        public static float c_tickBloodWellLifeSteal
        {
            get
            {
                try
                {
                    return ConfigStrings["li_tickBloodWellLifeSteal"]/100;
                }
                catch
                {
                    return 0.1f;
                }
            }
        }
        public static float c_tickBloodWellArmor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_tickBloodWellArmor"];
                }
                catch
                {
                    return 25f;
                }
            }
        }
        public static float c_tickBloodWellStackCap
        {
            get
            {
                try
                {
                    return ConfigStrings["li_tickBloodWellStackCap"];
                }
                catch
                {
                    return 500f;
                }
            }
        }
        
        //Moder
        public static float c_moderDraconicFrostCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_moderDraconicFrostCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_moderDraconicFrostProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_moderDraconicFrostProjectile"]/100;
                }
                catch
                {
                    return 0.05f;
                }
            }
        }
        
        public static float c_moderDraconicFrostPassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_moderDraconicFrostPassive"]/100;
                }
                catch
                {
                    return 0.25f;
                }
            }
        }
        public static float c_moderDraconicFrostDragonBreath
        {
            get
            {
                try
                {
                    return ConfigStrings["li_moderDraconicFrostDragonBreath"];
                }
                catch
                {
                    return 200f;
                }
            }
        }
        
        //boar
        public static float c_boarRecklessChargeCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_boarRecklessChargeCD"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_boarRecklessCharge
        {
            get
            {
                try
                {
                    return ConfigStrings["li_boarRecklessCharge"];
                }
                catch
                {
                    return 30f;
                }
            }
        }
        public static float c_boarRecklessChargePassive
        {
            get
            {
                try
                {
                    return ConfigStrings["li_boarRecklessChargePassive"];
                }
                catch
                {
                    return 30f;
                }
            }
        }

        //Stone Golem
        public static float c_stonegolemCoreOverdriveCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_stonegolemCoreOverdriveCD"];
                }
                catch
                {
                    return 5f;
                }
            }
        }
        public static float c_stonegolemCoreOverdriveStacks
        {
            get
            {
                try
                {
                    return ConfigStrings["li_stonegolemCoreOverdriveStacks"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        public static float c_stonegolemCoreOverdriveArmor
        {
            get
            {
                try
                {
                    return ConfigStrings["li_stonegolemCoreOverdriveArmor"];
                }
                catch
                {
                    return 2f;
                }
            }
        }
        
        //Yagluth
        public static float c_yagluthCulminationCD
        {
            get
            {
                try
                {
                    return ConfigStrings["li_yagluthCulminationCD"];
                }
                catch
                {
                    return 20f;
                }
            }
        }
        public static float c_yagluthCulminationStaticCap
        {
            get
            {
                try
                {
                    return ConfigStrings["li_yagluthCulminationStaticCap"];
                }
                catch
                {
                    return 100f;
                }
            }
        }
        public static float c_yagluthCulmination
        {
            get
            {
                try
                {
                    return ConfigStrings["li_yagluthCulmination"];
                }
                catch
                {
                    return 1f;
                }
            }
        }
        
        //Ulv
        
        
        
        
        
        
        
        
        
    }
    
    
    
}