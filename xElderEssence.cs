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


namespace LackingImaginationV2
{
    public class xElderEssence
    {
        private static float minDistanceBetweenCharacters = 2f; // Set the minimum distance between characters

        public static string Ability_Name = "Ancient Awe";
        
        public static bool ElderController = false;
        public static List<Character> Roots = new List<Character>();
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Elder Button was pressed");
                
                
                List<Character> allch = new List<Character>();
                allch.Clear();
                Character.GetCharactersInRange(player.GetCenterPoint(), 15f, allch);
                Func<Character, bool> condition = ch => ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI;
                if (allch.Any(condition))
                {
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xElderCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                    
                    //Effects, animations, and sounds
                    // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_ImpDeath"), player.transform.position, Quaternion.identity);
                
                    LackingImaginationV2Plugin.UseGuardianPower = false;
                
                    ElderController = true;
                    ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                    ElderController = false;
                    
                    SE_AncientAwe se_ancientawe = (SE_AncientAwe)ScriptableObject.CreateInstance(typeof(SE_AncientAwe));
                    se_ancientawe.m_ttl = SE_AncientAwe.m_baseTTL;
                
                    player.GetSEMan().AddStatusEffect(se_ancientawe);
                    
                    foreach (Character ch in allch)
                    {
                        if (ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI )
                        {
                            ch.GetSEMan().AddStatusEffect(se_ancientawe);
                        }
                    }

                    //Get suitable targets
                    List<Character> allCharacters = new List<Character>();
                    allCharacters.Clear();
                    Character.GetCharactersInRange(Player.m_localPlayer.GetCenterPoint(), 15f, allCharacters);
                    List<Character> affectedCharacters = new List<Character>();
                    affectedCharacters.Clear();
                    foreach (Character currentCharacter in allCharacters)
                    {
                        if ((currentCharacter.GetBaseAI() is MonsterAI && currentCharacter.GetBaseAI().IsEnemy(Player.m_localPlayer)) && !currentCharacter.m_tamed ||
                            currentCharacter.GetBaseAI() is AnimalAI)
                        {
                            bool tooClose = false;
                            // Check if the current character is too close to any affected character
                            foreach (Character affectedCharacter in affectedCharacters)
                            {
                                float distance = Vector3.Distance(currentCharacter.transform.position, affectedCharacter.transform.position);
                                if (distance < minDistanceBetweenCharacters)
                                {
                                    tooClose = true;
                                    break; // No need to check other affected characters if one is too close
                                }
                            }
                            if (!tooClose)
                            {
                                // Add the current character to the list of affected characters
                                affectedCharacters.Add(currentCharacter);
                                SummonRoots(currentCharacter, 2f);
                                SummonRoots(currentCharacter, 8f);
                                
                            }
                        }
                    }
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Has No Targets");
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        static void SummonRoots(Character currentCharacter, float range)
        {
            Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * range;
            Vector3 randomPosition = currentCharacter.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
            GameObject tenta = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("TentaRoot"), randomPosition, Quaternion.identity);   
            tenta.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
            tenta.GetComponent<Humanoid>().m_name = "TentaRoot(Ally)";
            tenta.GetComponent<Humanoid>().SetMaxHealth(tenta.GetComponent<Humanoid>().GetMaxHealthBase() * 8f);
            tenta.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
            CharacterDrop characterDrop = tenta.GetComponent<CharacterDrop>();
            if (characterDrop != null)  characterDrop.m_dropsEnabled = false;
            Roots.Add(tenta.GetComponent<Character>());
        }
    }

    [HarmonyPatch]
    public static class xElderEssencePassive
    {
        
        [HarmonyPatch(typeof(Character), "CustomFixedUpdate")]
        public static class Elder_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (xElderEssence.Roots.Any() && __instance.IsPlayer() && !__instance.GetSEMan().HaveStatusEffect("SE_AncientAwe"))
                {
                    foreach (Character root in xElderEssence.Roots)
                    {
                        DestroyNow(root);
                    }

                    xElderEssence.Roots.Clear();
                }
            }

            static void DestroyNow(Character root)
            {
                if (!root.m_nview.IsValid() || !root.m_nview.IsOwner())
                    return;
                root.GetComponent<Character>().ApplyDamage(new HitData()
                {
                    m_damage =
                    {
                        m_damage = 999999f
                    },
                    m_point = root.transform.position
                }, false, true);
            }
        }
        
        [HarmonyPatch(typeof(Player), "UpdateFood")]
        public static class Elder_UpdateFood_Patch
        {
            public static void Postfix(Player __instance)// regen
            {
                if (EssenceItemData.equipedEssence.Contains("$item_elder_essence"))
                {
                    float num1 = 0.0f;
                    foreach (Player.Food food in __instance.m_foods)
                        num1 += food.m_item.m_shared.m_foodRegen;
                    if ((double) num1 <= 0.0)
                        return;
                    float regenMultiplier = 1f;
                    __instance.m_seman.ModifyHealthRegen(ref regenMultiplier);
                    __instance.Heal(num1 * regenMultiplier * LackingImaginationGlobal.c_elderAncientAwePassive);
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), "UpdateEnvStatusEffects")]
        public static class Elder_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance,ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_elder_essence")) 
                {
                    List<HitData.DamageModPair> ElderRes = new List<HitData.DamageModPair>() {new HitData.DamageModPair() {m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.VeryWeak} };
                    __instance.m_damageModifiers.Apply(ElderRes);
                }
            }
        }
        
    }

}