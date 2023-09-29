using System;
using System.Collections;
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

    public class xCultistEssence
    {
        public static string Ability_Name = "Lone Sun";
        
        private static GameObject GO_CultistLoneSunAoe;        
        private static Aoe A_CultistLoneSunAoe;     
        
        private static float aoeDelay = 1.1f;
        
        public static bool CultistController = false;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Cultist Button was pressed");
                
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xCultistCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);//fx_fenring_burning_hand

                EffectList m_HandEffectsRight = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_burning_hand"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = false,
                            m_follow = true,
                            m_childTransform = "RightHandMiddle3_end"
                        }
                    }
                };
                
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_fenring_alerted"), player.transform.position, Quaternion.identity);
                
                m_HandEffectsRight.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());

                LackingImaginationV2Plugin.UseGuardianPower = false;
                CultistController = true;
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                CultistController = false;

                GameObject prefab = ZNetScene.instance.GetPrefab("DvergerStaffFire_fire_aoe");

                ScheduleAoes(player, prefab);
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

        private static void ScheduleAoes(Player player, GameObject prefab)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleAoe(player, prefab));
        }
        
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleAoe(Player player, GameObject prefab)
        {
            yield return new WaitForSeconds(aoeDelay);
            
            GO_CultistLoneSunAoe = UnityEngine.Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
            A_CultistLoneSunAoe = GO_CultistLoneSunAoe.GetComponent<Aoe>();
            A_CultistLoneSunAoe.m_owner = player;
            A_CultistLoneSunAoe.m_damage.m_fire = 2f;
            A_CultistLoneSunAoe.m_damage.m_blunt = 2f;
            A_CultistLoneSunAoe.m_damage.Modify(LackingImaginationGlobal.c_cultistLoneSunAoe);

        }
    }

    [HarmonyPatch]
    public static class xCultistEssencePassive
    {

        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class Cultist_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_cultist_essence"))
                {
                    __instance.m_tolerateSmoke = true;
                }
                else
                {
                    __instance.m_tolerateSmoke = false;
                }
            }
        }

        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Cultist_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_cultist_essence") && hit.GetAttacker() != null)
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
                    if (__instance != null && __instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        int Dubious = UnityEngine.Random.Range(1, 21); // 1-20 inclusive
                        if (Dubious == 1)
                        {
                            hit.m_damage.m_fire = hit.GetTotalDamage() * 0.05f;
                        }
                    }
                    if ((UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
                    {
                        hit.m_damage.m_fire += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_cultistLoneSunPassive;
                    }
                }
            }
        }


    }
}