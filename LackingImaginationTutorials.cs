using HarmonyLib;
using System.Collections.Generic;


namespace LackingImaginationV2
{
    public class LackingImaginationTutorials
    {
  [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        public static class Player_Tutorials_Patch
        {
            public static void Postfix(Player __instance)
            {
                Tutorial.TutorialText LI = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label ="Lacking Imagination",
                    m_name = "Lacking_Imagination",
                    m_text = "This World is vast and dangerous. \nExplore, Kill and Conquer to grow your power!" +
                             "\nGain more power at levels 50, 160, 330 & 550.",
                    
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
                    m_text = "The fallen land, struggling to regrow despite the darkness coiled around its neck",
                     
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
                    m_text = "Essence Power: Blitz \n\n" +
                             "Active: Shoot a lightning cone forward(Weapon scaling). \n\n" +
                             "Positive Passive: All attacks do bonus lightning damage. \n\n" +
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
                    m_text = "Essence Power: Mass Release \n\n" +
                             "Active: Lob a projectile that summons ally skeletons and blobs on impact. \n\n" +
                             "Positive Passive: When hit you have a 20% chance to release a poison cloud. \n\n" +
                             "Positive Passive: You become resistant to pierce. \n\n" +
                             "Negative Passive: You become very weak to blunt. \n\n" +
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
                    m_text = "Essence Power: Draconic Frost \n\n" +
                             "Active: You fire a cone of ice shards that spawn ice on the ground(Weapon scaling). \n\n" +
                             "Active(Block): You shoot an ice dragon breath. \n\n" +
                             "Positive Passive: All attacks do added frost damage. \n\n" +
                             "Negative Passive: The draconic essence makes you always feel cold. \n\n" +
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
                    m_text = "Essence Power: Ancient Awe \n\n" +
                             "Active: Briefly root targets in place & grow the roots from under them. \n\n" +
                             "Positive Passive: Multiplied regen. \n\n" +
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
                    m_text = "Essence Power: Culmination \n\n" +
                             "Active: Shoot a line of lightningfire.(Builds Static) \n\n" +
                             "Active(Block): Call a rain of meteors.(Builds Static) \n\n" +
                             "Active(Crouch): At the cost of some health, create a nova(Reduces Static) \n\n" +
                             "Positive Passive: Static decreases over time. \n\n" +
                             "Positive Passive: Immune to burning.(Builds Static) \n\n" +
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
                    m_text = "Essence Power: Bane \n\n" +
                             "Active: Summons an ally Abomination.(Eats Wood) \n\n" +
                             "Positive Passive: Increased armor. \n\n" +
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
                    m_text = "Essence Power: Troll Toss \n\n" +
                             "Active: Lob a boulder(Max Health scaling). \n\n" +
                             "Positive Passive: Bonus health. \n\n" +
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
                    m_text = "Essence Power: Fumes \n\n" +
                             "Active: Release a cloud of poison. \n\n" +
                             "Positive Passive: 1 extra jump.\n\n" +
                             "Positive Passive: At 50% hp, spawn 2 ally blobs to aid you.\n\n" +
                             "Negative Passive: You become weak to blunt. \n\n" +
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
                    m_text = "Essence Power: Relentless \n\n" +
                             "Active: For a duration of time your projectiles gain homing.\n\n" +
                             "Positive Passive: Gain added pierce damage to projectiles.\n\n" +
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
                    m_text = "Essence Power: Three Freeze \n\n" +
                             "Active: Fires a burst of 3 ice shards that freeze enemies for 3 seconds and \n" +
                             "applies freezing.\n\n" +
                             "Positive Passive: You become very resistant to frost.\n\n" +
                             "Negative Passive: You become weak to blunt.",
                     
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
                    m_text = "Essence Power: Fallen Hero \n\n" +
                             "Active: For a duration, increase the damage of Swords and Polearms(builds 5% Rot).\n\n" +
                             "Positive Passive: Increase carry weight.\n\n" +
                             "Positive Passive: Increase movement speed by 10%.\n\n" +
                             "Positive Passive: A portion of damage received is instead gained as Rot.\n\n" +
                             "Positive Passive: Entrails can be eaten to reduce Rot build up.\n\n" +
                             "Negative Passive: If Rot build up reaches 100% the ability is locked, \n" +
                             "damage reduction is removed and movement speed is reduced by 50%.\n\n" +
                             "Synergy  Passive: If the Draugr Essence is also equipped, \n" +
                             "a bonus potion of damage received is instead gained as Rot.",
                    
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
                    m_text = "Essence Power: Forgotten \n\n" +
                             "Active: For a duration, increase the damage of Bows and Axes(builds 3% Rot).\n\n" +
                             "Positive Passive: Increase carry weight.\n\n" +
                             "Positive Passive: A portion of damage received is instead gained as Rot.\n\n" +
                             "Positive Passive: Entrails can be eaten to reduce Rot build up.\n\n" +
                             "Negative Passive: If Rot build up reaches 100% the ability is locked, \n" +
                             "damage reduction is removed and movement speed is reduced by 50%.\n\n" +
                             "Synergy  Passive: If the Draugr Elite Essence is also equipped, \n" +
                             "a bonus potion of damage received is instead gained as Rot.",
                     
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
                    m_text = "Essence Power: Longinus \n\n" +
                             "Active: Empower your next spear throw hit.\n\n" +
                             "Positive Passive: Spears return when thrown.\n\n" +
                             "Positive Passive: Gain increased block power.\n\n" +
                             "Positive Passive: Gain bonus stamina when you carry coins.\n\n" +
                             "Negative Passive: Lose a % of stamina when not carrying coins.\n\n",
                     
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
                    m_text = "Essence Power: Gjallarhorn \n\n" +
                             "Active: A pair of entangled projectiles are fired, one explosive and the other summons ally ticks.\n\n" +
                             "Positive Passive: Gain bonus armor.\n\n" +
                             "Negative Passive: You become very weak to pierce.\n\n" +
                             "Negative Passive: Your head becomes a weakpoint(double damage)(disabled).\n\n",
                     
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
                    m_text = "Essence Power: Bash \n\n" +
                             "Active: Empower your next melee hit.\n\n" +
                             "Positive Passive: Gain bonus health.\n\n" +
                             "Negative Passive: Reduce the damage of ranged attacks.\n\n",
                     
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
                    m_text = "Essence Power: Dubious Heal \n\n" +
                             "Active: Cast an Aoe heal.\n\n" +
                             "Positive Passive: Gain increased player regen.\n\n" +
                             "Positive Passive: Gain a little bonus eitr.\n\n" +
                             "Negative Passive: When hit you have a 1 in 20 chance to take 10% bonus poison damage.\n\n",
                     
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
                    m_text = "Essence Power: Pebble \n\n" +
                             "Active: Throw a rock from your inventory.\n\n" +
                             "Positive Passive: Gain increases carry weight.\n\n" +
                             "Positive Passive: Gain 5% bonus movement speed.\n\n" +
                             "Negative Passive: Forest Monsters do bonus damage to you.\n\n",
                     
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
                    m_text = "Essence Power: Blood Siphon \n\n" +
                             "Active: Mark nearby enemies and when you kill them, gain Siphon stacks.\n\n" +
                             "Positive Passive: When hit, regain health if you have stacks.\n\n" +
                             "Negative Passive: Eitr regen is halved and regen delay is doubled.\n\n",
                     
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
                    m_text = "Essence Power: Vigil \n\n" +
                             "Active: Summon ally ghosts to fight alongside you.(Cost 1 Soul to cast)\n\n" +
                             "Positive Passive: Gain bonus spirit damage equal to 10% of Souls.\n\n" +
                             "Negative Passive: Kill skeletons to gain souls.\n\n" +
                             "Synergy  Passive: If the Brenna Essence is also equipped, ally ghosts do bonus fire damage. \n\n" +
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
                    m_text = "Essence Power: Harbinger \n\n" +
                             "Active: Summon ally surtlings from the Ash Lands.(Sacrifice a surtling core to gain charges)\n\n" +
                             "Positive Passive: The essence is your own personal campfire.\n\n" +
                             "Negative Passive: You take damage when wet.\n\n",
                     
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
                    m_text = "Essence Power: Blood Well \n\n" +
                             "Active: Empower next hit to do bonus slash damage equal to Well stacks.\n\n" +
                             "Positive Passive: Gain % LifeSteal.\n\n" +
                             "Positive Passive: Gain Well stacks equal to life stolen.\n\n" +
                             "Negative Passive: Armor is reduced.",
                     
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
                    m_text = "Essence Power: Lone Sun\n\n" +
                             "Active: Cast an Aoe fire field.\n\n" +
                             "Positive Passive: You are immune to smoke.\n\n" +
                             "Positive Passive: All attacks do bonus fire damage.\n\n" +
                             "Negative Passive: When hit you have a 1 in 20 chance to take 10% bonus fire damage.",
                     
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
                    m_text = "Essence Power: Giantization \n\n" +
                             "Active: Double your size, gaining double health but halving stamina.\n\n" +
                             "Positive Passive: Gain bonus health.\n\n" +
                             "Negative Passive: Eitr reduced by 75%.",
                     
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
                    m_text = "Essence Power: Ritual\n\n" +
                             "Active: Create a shield that protects you.(Cost 5 coins)\n\n" +
                             "Active(Shielded): Shoot a fireball.(Cost 1 coin)\n\n" +
                             "Positive Passive: Gain bonus eitr.\n\n" +
                             "Negative Passive: Reduce carry weight.",
                     
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
                    m_text = "Essence Power: Ancient Tar\n\n" +
                             "Active: Shoot a burst of 4 tar balls.(Weapon scaling)\n\n" +
                             "Positive Passive: Attacks tar enemies.\n\n" +
                             "Positive Passive: 2 extra jumps.\n\n" +
                             "Negative Passive: You become very weak to fire.\n\n",
                     
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
                    m_text = "Essence Power: Rancorous\n\n" +
                             "Active: Summon the bound mace Rancorous.\n\n" +
                             "Active(Recast): Rancorous seconary attack chnages to a throw.\n\n" +
                             "Active(Re-Recast): Rancorous seconary attack returns to normal.\n\n" +
                             "Positive Passive: Sacrifice a fully upgraded Iron mace to Awaken Rancorous permanently.\n\n" +
                             "Negative Passive: Armor is reduced.",
                     
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
                    m_text = "Essence Power: Sea King\n\n" +
                             "Active: Shoots a Whirlpool that sucks enemies in.(Weapon scaling)\n\n" +
                             "Passive: The essence wil crave a random known fish and eating it will,\n" +
                             "increase the range and duration of Sea King.\n\n" +
                             "A higher quality fish can be eaten after to get a bigger buff.\n" +
                             "Perch>Pike>Trollfish>Tetra>Tuna>Coral Cod>Giant Herring>Grouper>\n" +
                             "Pufferfish>Anglerfish>Magmafish>Northern Salmon",
                     
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
                    m_text = "Essence Power: Territorial Slumber\n\n" +
                             "Active: Create a zone with a high chance to summon an ally ulv when an enemy dies inside.\n\n" +
                             "Positive Passive: Gain bonus Comfort.\n\n" +
                             "Positive Passive: Gain bonus stamina based on max comfort.\n\n" +
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
                    m_text = "Essence Power: Twin Souls\n\n" +
                             "Active: Become intangible for a duration, moving through structures and creatures.\n" +
                             "Block and Crouch to move down.\n\n" +
                             "Active(Block): If cast at night, summon an ally wraith(Dies at dawn).\n\n" +
                             "Positive Passive: All attacks do bonus spirit damage during the day.\n\n" +
                             "Negative Passive: Armor reduced.",
                     
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
                    m_text = "Essence Power: Randomize\n\n" +
                             "Active: Cast one of three spells.(Cost 50 Eitr)\n\n" +
                             "Active(Fire): Cast 1 of 2 fire ball types.(Weapon scaling)\n\n" +
                             "Active(Ice): Cast a burst of ice shards.(Weapon scaling)\n\n" +
                             "Active(Heal): Cast an Aoe Heal.\n\n" +
                             "Positive Passive: Gain bonus eitr.\n\n" +
                             "Positive Passive: Gain bonus pierce damage with crossbows.\n\n" +
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
                    m_text = "Essence Power: Reckless Charge\n\n" +
                             "Active: For a duration, after running for 3 seconds you collide with the next enemy.\n\n" +
                             "Positive Passive: When you gather your courage you gain bonus stamina.\n\n" +
                             "Negative Passive: When near a fire you will cower in fear.\n\n",
                     
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
                    m_text = "Essence Power: Horizon Haste\n\n" +
                             "Active: For a duration, movement speed is increased.\n\n" +
                             "Positive Passive: Stamina increased.\n\n" +
                             "Positive Passive: Movement speed increased by 5%.\n\n" +
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
                    m_text = "Essence Power: Lucky Foot\n\n" +
                             "Active: For a duration, movement speed is increased & jumps are doubled.\n\n" +
                             "Positive Passive: Gain 1 bonus jump.\n\n" +
                             "Positive Passive: Movement speed increased by 10%.\n\n" +
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
                    m_text = "Essence Power: Wild Tremor\n\n" +
                             "Active: Stomp the ground causing an Aoe.\n\n" +
                             "Positive Passive: Food eaten will gave bonus health.\n\n" +
                             "Negative Passive: Duration of food reduced by 25%.",
                     
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
                    m_text = "Essence Power: Splash(Effect in progress)\n\n" + 
                             "Active: Dash forward while swimming.(Weapon scaling)\n\n" +
                             "Positive Passive: Swim speed doubled.\n\n" +
                             "Positive Passive: You become weak to fire while wet.\n\n" +
                             "Negative Passive: You become resistant to poison while wet.",
                     
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
                    m_text = "Essence Power: Ravenous Hunger\n\n" +
                             "Active: For a duration, every 5th hit will deal a % of max health in slash damage.\n\n" +
                             "Passive: Different effects based on number of foods eaten.\n\n" +
                             "3 foods: damage reduced by 25%.\n\n" +
                             "2 foods: damage increased by 25% & x bonus stamina.\n\n" +
                             "1 foods: damage increased by 50% & 2x bonus stamina.\n\n" +
                             "0 foods: damage increased by 100% & 3x bonus stamina.\n\n" +
                             "Below 3 foods run, sneak, dodge & jump stamina drain reduced by half." ,
                     
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
                    m_text = "Essence Power: Vulcan\n\n" +
                             "Active: Summon the bound mace Vulcan.\n\n" +
                             "Active(Recast): Vulcan secondary attack changes to a throw.\n\n" +
                             "Active(Re-Recast): Vulcan secondary attack returns to normal.\n\n" +
                             "Positive Passive: Sacrifice a fully upgraded Krom to Awaken Vulcan permanently.\n\n" +
                             "Negative Passive: Armor is reduced.",
                     
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
                    m_text = "Essence Power: Ice Age\n\n" +
                             "Active: Case 3 ice waves & then summon icicles above enemies in range.\n\n" +
                             "Positive Passive: Gain bonus eitr.\n\n" +
                             "Positive Passive: All attacks do bonus frost damage.\n\n" +
                             "Negative Passive:  When hit you have a 1 in 20 chance to take 10% bonus frost damage.",
                     
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
                //Dvergr 
                Tutorial.TutorialText _dvergrTowerExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xDvergr Tower",
                    m_name = "DvergrTower_Exp",
                    m_text = "A flickering beacon of life and civilization.",
                     
                    m_topic = "Dvergr Tower"
                };
                if (!Tutorial.instance.m_texts.Contains(_dvergrTowerExp))
                {
                    Tutorial.instance.m_texts.Add(_dvergrTowerExp);
                }
                Tutorial.TutorialText _dvergrExcavationExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xDvergr Excavation",
                    m_name = "DvergrExcavation_Exp",
                    m_text = "The remains of mythic warriors lay beneath.",
                     
                    m_topic = "Dvergr Excavation"
                };
                if (!Tutorial.instance.m_texts.Contains(_dvergrExcavationExp))
                {
                    Tutorial.instance.m_texts.Add(_dvergrExcavationExp);
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
                    m_text = "The Queen of Drakes is nearby.",
                    
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
                //Vendors
                Tutorial.TutorialText _haldorExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xHaldor",
                    m_name = "Haldor_Exp",
                    m_text = "The roaming Dvergr trader of the Black Forest, his wares may interest you.",
                    
                    m_topic = "Haldor"
                };
                if (!Tutorial.instance.m_texts.Contains(_haldorExp))
                {
                    Tutorial.instance.m_texts.Add(_haldorExp);
                }
                Tutorial.TutorialText _hildirExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xHildir",
                    m_name = "Hildir_Exp",
                    m_text = "The roaming Dvergr trader of the Meadows, she has a task for you.",
                    
                    m_topic = "Hildir"
                };
                if (!Tutorial.instance.m_texts.Contains(_hildirExp))
                {
                    Tutorial.instance.m_texts.Add(_hildirExp);
                }
                // Special
                Tutorial.TutorialText _infestedTreeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xInfested Tree",
                    m_name = "InfestedTree_Exp",
                    m_text = "This tree has been completely consumed by the lands poison.",
                    
                    m_topic = "Infested Tree"
                };
                if (!Tutorial.instance.m_texts.Contains(_infestedTreeExp))
                {
                    Tutorial.instance.m_texts.Add(_infestedTreeExp);
                }
                Tutorial.TutorialText _drakeNestExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xDrake Nest",
                    m_name = "DrakeNest_Exp",
                    m_text = "Here her descendants roost.",
                    
                    m_topic = "Drake Nest"
                };
                if (!Tutorial.instance.m_texts.Contains(_drakeNestExp))
                {
                    Tutorial.instance.m_texts.Add(_drakeNestExp);
                }
                Tutorial.TutorialText _tarPitExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xTar Pit",
                    m_name = "TarPit_Exp",
                    m_text = "Try not to fall in.",
                    
                    m_topic = "Tar Pit"
                };
                if (!Tutorial.instance.m_texts.Contains(_tarPitExp))
                {
                    Tutorial.instance.m_texts.Add(_tarPitExp);
                }
                Tutorial.TutorialText _dvergrHarbourExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xDvergr Harbour",
                    m_name = "DvergrHarbour_Exp",
                    m_text = "The first signs of civilization.",
                    
                    m_topic = "Dvergr Harbour"
                };
                if (!Tutorial.instance.m_texts.Contains(_dvergrHarbourExp))
                {
                    Tutorial.instance.m_texts.Add(_dvergrHarbourExp);
                }
                //RuneStones
                Tutorial.TutorialText _runestoneMeadowsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Meadows",
                    m_name = "MeadowRune_Exp",
                    m_text = "An inscription about the meadows.",
                    
                    m_topic = "Meadows Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneMeadowsExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneMeadowsExp);
                }
                Tutorial.TutorialText _runestoneBoarExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Boar",
                    m_name = "BoarRune_Exp",
                    m_text = "An inscription about boars.",
                    
                    m_topic = "Boar Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneBoarExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneBoarExp);
                }
                Tutorial.TutorialText _runestoneBlackForestsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Black Forests",
                    m_name = "BlackForestRune_Exp",
                    m_text = "An inscription about the black forests",
                    
                    m_topic = "Black Forests Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneBlackForestsExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneBlackForestsExp);
                }
                Tutorial.TutorialText _runestoneGreydwarfExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Greydwarf",
                    m_name = "GreydwarfRune_Exp",
                    m_text = "An inscription about greydwarfs.",
                    
                    m_topic = "Greydwarf Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneGreydwarfExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneGreydwarfExp);
                }
                Tutorial.TutorialText _runestoneSwampsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Swamps",
                    m_name = "SwampRune_Exp",
                    m_text = "An inscription about the swamps.",
                    
                    m_topic = "Swamps Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneSwampsExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneSwampsExp);
                }
                Tutorial.TutorialText _runestoneDraugrExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Draugr",
                    m_name = "DraugrRune_Exp",
                    m_text = "An inscription about draugrs.",
                    
                    m_topic = "Draugr Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneDraugrExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneDraugrExp);
                }
                Tutorial.TutorialText _runestoneMountainsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Mountains",
                    m_name = "MountainRune_Exp",
                    m_text = "An inscription about the mountains.",
                    
                    m_topic = "Mountains Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneMountainsExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneMountainsExp);
                }
                Tutorial.TutorialText _runestoneDrakeExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Drake",
                    m_name = "DrakeRune_Exp",
                    m_text = "An inscription about hatchlings.",
                    
                    m_topic = "Drake Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneDrakeExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneDrakeExp);
                }
                Tutorial.TutorialText _runestonePlainsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Plains",
                    m_name = "PlainsRune_Exp",
                    m_text = "An inscription about the plains.",
                    
                    m_topic = "Plains Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestonePlainsExp))
                {
                    Tutorial.instance.m_texts.Add(_runestonePlainsExp);
                }
                Tutorial.TutorialText _runestoneMistlandsExp = new Tutorial.TutorialText
                {
                    m_isMunin = true,
                    m_label = "xRunestone Mistlands",
                    m_name = "MistRune_Exp",
                    m_text = "An inscription about the mistlands.",
                    
                    m_topic = "Mistlands Runestone"
                };
                if (!Tutorial.instance.m_texts.Contains(_runestoneMistlandsExp))
                {
                    Tutorial.instance.m_texts.Add(_runestoneMistlandsExp);
                }
                
                
            }
        }
    }
}