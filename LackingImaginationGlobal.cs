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
        
        //Drake
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
        //Leech
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
        
        //Wraith
        
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
        
        //Draugr
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
        public static float c_seaSerpentSeaKingeProjectile
        {
            get
            {
                try
                {
                    return ConfigStrings["li_seaSerpentSeaKingeProjectile"]/100;
                }
                catch
                {
                    return 0.2f;
                }
            }
        }
        
        //Tick
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
                    return ConfigStrings["li_moderDraconicFrostProjectile"]/100;
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
        
        
        
        
        
        
        
        
        
        
    }
    
    
    
}