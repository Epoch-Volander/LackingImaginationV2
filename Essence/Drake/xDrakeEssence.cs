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
using System.Collections;
using System.Threading;

namespace LackingImaginationV2
{

    public static class xDrakeEssence
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
        
        public static string Ability_Name = "Three \nFreeze";
        
        private static float shotDelay = 0.2f;

        public static void Process_Input(Player player, int position)
        {

            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xDrakeCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_hatchling_alerted"), player.transform.position, Quaternion.identity);
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("hatchling_spit_cold"), player.transform.position, Quaternion.identity);

                // Vector3 vector = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * .5f;
                GameObject prefab = ZNetScene.instance.GetPrefab("hatchling_cold_projectile");
                // Schedule the first projectile
                ScheduleProjectiles(player, prefab);
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }

        private static void ScheduleProjectiles(Player player, GameObject prefab)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleProjectilesCoroutine(player, prefab));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleProjectilesCoroutine(Player player, GameObject prefab)
        {
            int shotsFired = 0;

            while (shotsFired < 3)
            {
                Vector3 vector = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * 0.5f;
                // Create the projectile
                GameObject GO_ThreeFreezeProjectile = UnityEngine.Object.Instantiate(prefab, vector, Quaternion.identity);
                Projectile P_ThreeFreezeProjectile = GO_ThreeFreezeProjectile.GetComponent<Projectile>();
                P_ThreeFreezeProjectile.name = "Three Freeze Shots";
                P_ThreeFreezeProjectile.m_respawnItemOnHit = false;
                P_ThreeFreezeProjectile.m_spawnOnHit = null;
                P_ThreeFreezeProjectile.m_ttl = 60f;
                P_ThreeFreezeProjectile.m_gravity = 2.5f;
                P_ThreeFreezeProjectile.m_rayRadius = 0.5f;
                P_ThreeFreezeProjectile.m_aoe = 1f;
                P_ThreeFreezeProjectile.m_owner = player;
                P_ThreeFreezeProjectile.m_statusEffect = "SE_ThreeFreeze";
                P_ThreeFreezeProjectile.m_hitNoise = 100f;
                P_ThreeFreezeProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
                P_ThreeFreezeProjectile.transform.localScale = Vector3.one;

                RaycastHit hitInfo = default(RaycastHit);
                Vector3 playerPosition = player.transform.position;
                Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !hitInfo.collider) ? (playerPosition + player.GetLookDir() * 1000f) : hitInfo.point;
                target += UnityEngine.Random.insideUnitSphere * 2f;
                HitData hitData = new HitData();
                hitData.m_damage.m_frost = UnityEngine.Random.Range(2f, 4f);
                hitData.m_damage.m_spirit = UnityEngine.Random.Range(1f, 2f);
                hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_drakeThreeFreezeProjectile));
                hitData.m_pushForce = 0.5f;
                hitData.m_statusEffectHash = Player.s_statusEffectFreezing;
                hitData.SetAttacker(player);
                Vector3 destination = Vector3.MoveTowards(GO_ThreeFreezeProjectile.transform.position, target, 1f);
                P_ThreeFreezeProjectile.Setup(player, (destination - GO_ThreeFreezeProjectile.transform.position) * 25f, -1f, hitData, null, null);

                // Increment the shots fired counter
                shotsFired++;

                // Delay before the next projectile
                yield return new WaitForSeconds(shotDelay);
            }
        }
    }
    
    [HarmonyPatch]
    public class xDrakeEssencePassive
    {
        public static List<string> DrakeStats = new List<string>(){"off"};
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Drake_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_hatchling_essence") && hit.GetAttacker() != null)
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if (__instance != null && __instance.IsPlayer())
                    {
                        Player.m_localPlayer.m_damageModifiers.m_blunt = HitData.DamageModifier.Weak;
                    }
                }
            }
        }
        
        
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Drake_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance,ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_hatchling_essence")) 
                {
                    if (DrakeStats[0] == "off")
                    {
                        // __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectFreezing);
                        List<HitData.DamageModPair> DrakeRes = new List<HitData.DamageModPair>() { new HitData.DamageModPair() { m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.VeryResistant } };
                        __instance.m_damageModifiers.Apply(DrakeRes);
                        DrakeStats[0] = "on";
                    }
                }
                else if (DrakeStats[0] == "on" && !EssenceItemData.equipedEssence.Contains("$item_hatchling_essence"))
                {
                    List<HitData.DamageModPair> DrakeWeak = new List<HitData.DamageModPair>() {new HitData.DamageModPair() {m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.Normal} };
                    __instance.m_damageModifiers.Apply(DrakeWeak);
                    DrakeStats[0] = "off";
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
    }
}