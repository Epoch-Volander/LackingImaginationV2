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

    public class xFulingEssence
    {
        public static string Ability_Name = "Longinus";

        public static GameObject Aura;

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                // spear boy
                // return spear passive 
                // 

                LackingImaginationV2Plugin.Log($"Fuling Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xFulingCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                SE_Longinus se_longinus = (SE_Longinus)ScriptableObject.CreateInstance(typeof(SE_Longinus));
                player.GetSEMan().AddStatusEffect(se_longinus);


                Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Longinus, player.GetCenterPoint(), Quaternion.identity);
                Aura.transform.parent = player.transform;
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

    }

    [HarmonyPatch]
    public class xFulingEssencePassive// retreval effect
    {
        private static float equipDelay = 0.5f;

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.SpawnOnHit))]
        public static class Fuling_SpawnOnHit_Patch
        {
            public static void Prefix(Projectile __instance)
            {
                if (__instance.m_owner == Player.m_localPlayer && EssenceItemData.equipedEssence.Contains("$item_goblin_essence") && __instance.m_spawnItem?.m_shared.m_skillType == Skills.SkillType.Spears)
                {
                    if (__instance.m_spawnItem != null)
                    {
                        if (Player.m_localPlayer.m_inventory.CanAddItem(__instance.m_spawnItem)) 
                        {
                            Player.m_localPlayer.m_inventory.AddItem(__instance.m_spawnItem);
                            ScheduleEquip(ref __instance.m_spawnItem);
                        }
                        else
                        {
                            ItemDrop.DropItem(__instance.m_spawnItem, 0, Player.m_localPlayer.transform.position, Quaternion.identity);
                        }
                        __instance.m_spawnItem = null;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.FixedUpdate))]
        public static class Fuling_FixedUpdate_Patch
        {
            public static void Prefix(Projectile __instance)
            {
                if (__instance.m_owner == Player.m_localPlayer && EssenceItemData.equipedEssence.Contains("$item_goblin_essence") && __instance.m_spawnItem?.m_shared.m_skillType == Skills.SkillType.Spears)
                {
                    if (__instance.m_spawnItem != null)
                    {
                        float ttl = __instance.m_ttl;
                        
                        ttl -= Time.fixedDeltaTime;
                        if ((double) ttl > 0.0)
                            return;
                        
                        if (Player.m_localPlayer.m_inventory.CanAddItem(__instance.m_spawnItem)) 
                        {
                            Player.m_localPlayer.m_inventory.AddItem(__instance.m_spawnItem);
                            ScheduleEquip(ref __instance.m_spawnItem);
                        }
                        else
                        {
                            ItemDrop.DropItem(__instance.m_spawnItem, 0, Player.m_localPlayer.transform.position, Quaternion.identity);
                        }
                        __instance.m_spawnItem = null;
                    }
                }
            }
        }
        private static void ScheduleEquip(ref ItemDrop.ItemData spear)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleEquipCoroutine(spear));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleEquipCoroutine(ItemDrop.ItemData spear)
        {
            yield return new WaitForSeconds(equipDelay);

            Player.m_localPlayer.EquipItem(spear);
        }
        

        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        class Fuling_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_goblin_essence") && hit.GetAttacker() != null)
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object)hit.GetAttacker() == (UnityEngine.Object)Player.m_localPlayer)
                    {
                        Game.instance.IncrementPlayerStat(__instance.IsPlayer() ? PlayerStatType.PlayerHits : PlayerStatType.EnemyHits);
                        __instance.m_localPlayerHasHit = true;
                    }

                    if (!__instance.m_nview.IsOwner() || (double)__instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if ((UnityEngine.Object)__instance.m_baseAI != (UnityEngine.Object)null && (bool)(UnityEngine.Object)attacker && attacker.IsPlayer())
                    {
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Longinus") && hit.m_ranged && hit.m_skill == Skills.SkillType.Spears)
                        {
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_crit"), hit.m_point, Quaternion.identity);
                            GameObject hitEffect = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_skeleton_mace_hit"), hit.m_point, Quaternion.identity);
                            ParticleSystem.MainModule mainModule = hitEffect.transform.Find("wetsplsh").GetComponent<ParticleSystem>().main;
                            mainModule.startColor = Color.red + Color.blue;
                            ParticleSystem.MainModule mainModule2 = hitEffect.transform.Find("Chunks").GetComponent<ParticleSystem>().main;
                            mainModule2.startColor = Color.yellow;
                            ParticleSystem.MainModule mainModule3 = hitEffect.transform.Find("Chunks").Find("blob_splat").GetComponent<ParticleSystem>().main;
                            mainModule3.startColor = Color.red + Color.blue;

                            hit.ApplyModifier(LackingImaginationGlobal.c_fulingLonginusMultiplier);
                            
                            Player.m_localPlayer.GetSEMan().RemoveStatusEffect("SE_Longinus".GetStableHashCode());

                            UnityEngine.GameObject.Destroy(xFulingEssence.Aura);
                        }
                    }

                    if (__instance.IsPlayer() && (UnityEngine.Object)attacker.m_baseAI != (UnityEngine.Object)null && hit.m_blockable && __instance.IsBlocking())
                    {
                        LackingImaginationV2Plugin.Log($"You blocked");
                        Player.m_localPlayer.GetCurrentBlocker().m_shared.m_blockPower *= LackingImaginationGlobal.c_fulingLonginusPassiveBlockMultiplier;
                        Player.m_localPlayer.GetCurrentBlocker().m_shared.m_blockPowerPerLevel *= LackingImaginationGlobal.c_fulingLonginusPassiveBlockMultiplier;
                    }
                }
            }
            
             public static void Postfix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_goblin_essence") && hit.GetAttacker() != null)
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
                    if (__instance.IsPlayer() && (UnityEngine.Object)attacker.m_baseAI != (UnityEngine.Object)null && hit.m_blockable && __instance.IsBlocking())
                    {
                        LackingImaginationV2Plugin.Log($"You blocked2");
                        Player.m_localPlayer.GetCurrentBlocker().m_shared.m_blockPower /= LackingImaginationGlobal.c_fulingLonginusPassiveBlockMultiplier;
                        Player.m_localPlayer.GetCurrentBlocker().m_shared.m_blockPowerPerLevel /= LackingImaginationGlobal.c_fulingLonginusPassiveBlockMultiplier;
                    }
                }
            }
            
        }


        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Fuling_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix(Player __instance, ref float stamina)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_goblin_essence"))
                {
                    if (Player.m_localPlayer.m_inventory.CountItems("$item_coins") >= 1)
                    {
                        stamina += LackingImaginationGlobal.c_fulingLonginusPassiveMotivated;
                    }
                    else
                    {
                        stamina -= (stamina * LackingImaginationGlobal.c_fulingLonginusPassiveDemotivated);
                    }
                }
                if (!EssenceItemData.equipedEssence.Contains("$item_goblin_essence") && __instance.GetSEMan().HaveStatusEffect("SE_Longinus"))
                {
                    Player.m_localPlayer.GetSEMan().RemoveStatusEffect("SE_Longinus".GetStableHashCode());

                    UnityEngine.GameObject.Destroy(xFulingEssence.Aura);
                }
            }
        }


    }

}

