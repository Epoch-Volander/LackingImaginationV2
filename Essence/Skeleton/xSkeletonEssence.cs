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


namespace LackingImaginationV2
{

    public class xSkeletonEssence
    {
        public static string Ability_Name = "Vigil"; // kill skeletons as currency to cast, summon ghosts//summon the shattered fragments of the soul
        //negative, fall damage doubled, exposed bones
        public static int Charges;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                Charges = int.Parse(xSkeletonEssencePassive.SkeletonStats[0]);

                if (Charges > 0)
                {
                    Charges--;
                    xSkeletonEssencePassive.SkeletonStats[0] = Charges.ToString();

                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xSkeletonCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_ghost_alert"), player.transform.position, Quaternion.identity);
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
                                    mainModule.startColor = Color.black + Color.blue;
                                }
                            }
                            Light lighting = child.GetComponent<Light>();
                            if(lighting != null)
                            {
                                lighting.color = Color.black;
                            }
                            Transform TransformSystem = child.GetComponent<Transform>();
                            if (TransformSystem != null)
                            {
                                TransformSystem.localScale *= 0.3f;
                            }
                        }
                    }
                    
                    for (int i = 0; i < LackingImaginationGlobal.c_skeletonVigilSummons; i++)
                    {
                        SummonGhost(player, 15f);
                    }
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Has no one to Honour");
                }
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        static void SummonGhost(Player player, float range)
        {
            Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * range;
            Vector3 randomPosition = player.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
            GameObject ghost = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Ghost"), randomPosition, Quaternion.identity);
            ghost.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
            ghost.GetComponent<Humanoid>().m_name = "Ghost(Ally)";
            ghost.GetComponent<Humanoid>().SetMaxHealth(ghost.GetComponent<Humanoid>().GetMaxHealthBase() * 5f);
            ghost.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
            CharacterDrop characterDrop = ghost.GetComponent<CharacterDrop>();
            if (characterDrop != null)  characterDrop.m_dropsEnabled = false;
            foreach (CharacterDrop.Drop drop in characterDrop.m_drops) drop.m_chance = 0f;
            
            SE_TimedDeath se_timedeath = (SE_TimedDeath)ScriptableObject.CreateInstance(typeof(SE_TimedDeath));
            se_timedeath.lifeDuration = LackingImaginationGlobal.c_skeletonVigilSummonDuration;
            se_timedeath.m_ttl = LackingImaginationGlobal.c_skeletonVigilSummonDuration + 500f;
            
            ghost.GetComponent<Character>().GetSEMan().AddStatusEffect(se_timedeath);
        }
    }

    [HarmonyPatch]
    public static class xSkeletonEssencePassive
    {
        public static List<string> SkeletonStats = new List<string>() { "0" };
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Skeleton_RPC_Damage_Patch
        {
            public static void Postfix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_skeleton_essence") )
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        Game.instance.IncrementPlayerStat(__instance.IsPlayer() ? PlayerStatType.PlayerHits : PlayerStatType.EnemyHits);
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if ((UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
                    {
                        if ((__instance.IsDead() || (double) __instance.GetHealth() <= 0.0 ) && (__instance.name == "Skeleton(Clone)" || __instance.name == "Skeleton_Hildir(Clone)" || __instance.name == "Skeleton_Poison(Clone)"))
                        {
                            xSkeletonEssence.Charges = int.Parse(xSkeletonEssencePassive.SkeletonStats[0]);
                            xSkeletonEssence.Charges++;
                            if (xSkeletonEssence.Charges > (int)LackingImaginationGlobal.c_skeletonVigilSoulCap) xSkeletonEssence.Charges = (int)LackingImaginationGlobal.c_skeletonVigilSoulCap;
                            SkeletonStats[0] = xSkeletonEssence.Charges.ToString();
                        }
                    }
                }
            }
            
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_skeleton_essence") && hit.GetAttacker() != null)
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
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if ((UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
                    {
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_ghost_attack"), hit.m_point, Quaternion.identity, __instance.transform);
                        hit.m_damage.m_spirit += float.Parse(xSkeletonEssencePassive.SkeletonStats[0]) * 0.1f;
                    }
                }
            }
        
            
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Skeleton_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                SE_Vigil se_vigil = (SE_Vigil)ScriptableObject.CreateInstance(typeof(SE_Vigil));
                if (EssenceItemData.equipedEssence.Contains("$item_skeleton_essence"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_Vigil"))
                    {
                        __instance.GetSEMan().AddStatusEffect(se_vigil);
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_Vigil"))
                {
                    __instance.GetSEMan().RemoveStatusEffect(se_vigil);
                }
            }
        }

        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStatusEffects))]
        public static class Skeleton_UpdateStatusEffects_Patch
        {
            public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
            {
                string iconText = SkeletonStats[0];
                for (int index = 0; index < statusEffects.Count; ++index)
                {
                    StatusEffect statusEffect1 = statusEffects[index];
                    if (statusEffect1.name == "SE_Vigil")
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
        
        
        
    }

}