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
using TMPro;
using UnityEngine.UI;
using YamlDotNet.Core;


namespace LackingImaginationV2
{

    public class xLeechEssence // make the drop red
    {
        public static string Ability_Name = "Blood \nSiphon";
        
        public static List<Character> Marked = new List<Character>();
        public static void Process_Input(Player player, int position)
        {

            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xLeechCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                // ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2.Create(player.transform.position, Quaternion.identity);
                 GameObject[] playerStones = ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2.Create(player.transform.position, Quaternion.identity);
                 foreach (GameObject effect in playerStones)
                 {
                     effect.transform.parent = player.transform;
                     // Access the ParticleSystem component of the child GameObject
                     foreach (Transform child in effect.transform)
                     {
                         ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
                         if (particleSystem != null)
                         {
                             // Create a ParticleSystem.MainModule object to access particle system properties
                             ParticleSystem.MainModule mainModule = particleSystem.main;
                             // mainModule.startSizeMultiplier = 0.1f;
                             if(child.name != "smoke")
                             {
                                 // Change the start color of the particles to black
                                 mainModule.startColor = Color.yellow + Color.red + Color.red;
                             }
                         }
                         Light lighting = child.GetComponent<Light>();
                         if(lighting != null)
                         {
                             lighting.color = Color.black + Color.red + Color.red;
                         }
                         Transform TransformSystem = child.GetComponent<Transform>();
                         if (TransformSystem != null)
                         {
                             TransformSystem.localScale *= 0.3f;
                         }
                     }
                 }
                 
                List<Character> allCharacters = new List<Character>();
                allCharacters.Clear();
                Character.GetCharactersInRange(player.transform.position, 25f, allCharacters);
                foreach (Character ch in allCharacters)
                {
                    if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer)) 
                        && !ch.m_tamed ||ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                    {
                        // SE_BloodSiphon se_bloodsiphon = (SE_BloodSiphon)ScriptableObject.CreateInstance(typeof(SE_BloodSiphon));
                        // ch.GetSEMan().AddStatusEffect(se_bloodsiphon);
                        if (!Marked.Contains(ch))
                        {
                            Marked.Add(ch);
                            GameObject effect = UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_BloodSiphon, ch.transform.position, Quaternion.identity);
                            effect.transform.parent = ch.transform;
                        }
                    }
                }
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        

    }

    [HarmonyPatch]
    public class xLeechEssencePassive
    {
        public static List<string> LeechStats = new List<string>() { "off", "0" };

        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Leech_GetTotalFoodValue_Patch
        {
            public static void Prefix(Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_leech_essence"))
                {
                    if (LeechStats[0] == "off")
                    {
                        __instance.m_eiterRegen *= 0.5f;
                        __instance.m_eitrRegenDelay *= 2f;
                        LeechStats[0] = "on";
                    }
                }
                else
                {
                    if (LeechStats[0] == "on")
                    {
                        __instance.m_eiterRegen *= 2f;
                        __instance.m_eitrRegenDelay *= 0.5f;
                        LeechStats[0] = "off";
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class Leech_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (xLeechEssence.Marked.Any() && __instance.IsPlayer() )
                {
                    List<int> dead = new List<int>();
                    // foreach (Character mark in xLeechEssence.Marked)
                    for (int i = 0; i < xLeechEssence.Marked.Count; i++)    
                    {
                        if (xLeechEssence.Marked[i].IsDead() || (double) xLeechEssence.Marked[i].GetHealth() <= 0.0)
                        {
                            // LackingImaginationV2Plugin.Log($"Leech{xLeechEssence.Marked[i].name}");
                            int count = int.Parse(LeechStats[1]);
                            count += (int)LackingImaginationGlobal.c_leechBloodSiphonStack;
                            if (count > (int)LackingImaginationGlobal.c_leechBloodSiphonStackCap) count = (int)LackingImaginationGlobal.c_leechBloodSiphonStackCap;
                            LeechStats[1] = count.ToString();
                            dead.Add(i);
                        }
                    }
                    for (int i = dead.Count - 1; i >= 0; i--)
                    {
                        xLeechEssence.Marked.RemoveAt(dead[i]);
                    }
                    dead.Clear();
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Leech_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                SE_BloodSiphon se_bloodsiphon = (SE_BloodSiphon)ScriptableObject.CreateInstance(typeof(SE_BloodSiphon));
                if (EssenceItemData.equipedEssence.Contains("$item_leech_essence"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_BloodSiphon"))
                    {
                        __instance.GetSEMan().AddStatusEffect(se_bloodsiphon);
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_BloodSiphon"))
                {
                    __instance.GetSEMan().RemoveStatusEffect(se_bloodsiphon);
                }
            }
        }

        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStatusEffects))]
        public static class Leech_UpdateStatusEffects_Patch
        {
            public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
            {
                string iconText = LeechStats[1];
                for (int index = 0; index < statusEffects.Count; ++index)
                {
                    StatusEffect statusEffect1 = statusEffects[index];
                    if (statusEffect1.name == "SE_BloodSiphon")
                    {
                        RectTransform statusEffect2 = __instance.m_statusEffects[index];
                        TMP_Text component2 = statusEffect2.Find("TimeText").GetComponent<TMP_Text>();
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
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Leech_RPC_Damage_Patch
        {
            public static void Postfix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_leech_essence") )
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        Game.instance.IncrementPlayerStat(__instance.IsPlayer() ? PlayerStatType.PlayerHits : PlayerStatType.EnemyHits);
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    if (__instance != null && __instance.IsPlayer() && (double) __instance.GetHealth() < __instance.GetMaxHealth())
                    {
                        float damage = __instance.GetMaxHealth() - __instance.GetHealth();
                        float pool = float.Parse(LeechStats[1]);
                        float maxHealing = Math.Min(damage, pool); // Cap healing at the pool value
                        if ((double)maxHealing > 0.0) ;//play hit effect
                        __instance.Heal(maxHealing); // Heal the object by the capped amount
                        pool -= maxHealing; // Subtract the amount healed 
                        int poolNew = (int)Math.Round(pool);
                        LeechStats[1] = poolNew.ToString();
                    }
                }
            }
        }
        
        
        
        
        
    }
}