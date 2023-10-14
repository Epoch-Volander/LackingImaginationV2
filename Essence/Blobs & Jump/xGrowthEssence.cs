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

    public class xGrowthEssence 
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Ancient \nTar";

        private static GameObject GO_TarProjectile;
        private static Projectile P_TarProjectile;

        private static float shotDelay = 0.3f;
      
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xGrowthCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_land_tar").GetComponent<GameObject>(), player.transform.position, Quaternion.identity);
                
                // Vector3 vector = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * .5f;
                GameObject prefab = ZNetScene.instance.GetPrefab("blobtar_projectile_tarball");
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
            
            while (shotsFired < 4)
            {
                Vector3 vector = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * .5f;
                // Create the projectile
                GO_TarProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
                P_TarProjectile = GO_TarProjectile.GetComponent<Projectile>();
                P_TarProjectile.name = "Ancient Tar Shot";
                P_TarProjectile.m_respawnItemOnHit = false;
                P_TarProjectile.m_spawnOnHit = null;
                P_TarProjectile.m_ttl = 60f;
                P_TarProjectile.m_gravity = 15f;
                P_TarProjectile.m_rayRadius = .3f;
                P_TarProjectile.m_aoe = 2f;
                P_TarProjectile.m_hitNoise = 100f;
                P_TarProjectile.m_owner = player;
                P_TarProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
                P_TarProjectile.transform.localScale = Vector3.one;

                RaycastHit hitInfo = default(RaycastHit);
                Vector3 player_position = player.transform.position;
                Vector3 target =
                    (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
                target += UnityEngine.Random.insideUnitSphere * 2f;
                HitData hitData = new HitData();
                hitData.m_damage.m_poison = UnityEngine.Random.Range(4f, 5f);
                hitData.m_damage.m_blunt = UnityEngine.Random.Range(4f, 5f);
                hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_growthAncientTarProjectile));
                hitData.m_pushForce = 0.5f;
                hitData.m_statusEffectHash = Player.s_statusEffectTared;
                hitData.SetAttacker(player);
                Vector3 a = Vector3.MoveTowards(GO_TarProjectile.transform.position, target, 1f);
                P_TarProjectile.Setup(player, (a - GO_TarProjectile.transform.position) * 25f, -1f,
                    hitData, null, null);
                
                // Increment the shots fired counter
                shotsFired++;

                // Delay before the next projectile
                yield return new WaitForSeconds(shotDelay);
            }
        }
    }

    [HarmonyPatch]
    public class xGrowthEssencePassive
    {
        public static int canDoubleJump;
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Growth_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance,ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_growth_essence")) 
                {
                    List<HitData.DamageModPair> GrowthRes = new List<HitData.DamageModPair>() {new HitData.DamageModPair() {m_type = HitData.DamageType.Fire, m_modifier = HitData.DamageModifier.VeryWeak} };
                    __instance.m_damageModifiers.Apply(GrowthRes); 
                    if(__instance.m_seman.HaveStatusEffect(Player.s_statusEffectTared))
                    {
                        __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectTared);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        class Growth_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_growth_essence") && hit.GetAttacker() != null)
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
                        hit.m_damage.m_poison += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_growthAncientTarPassive;
                        HitData hitData = new HitData();
                        hitData.m_statusEffectHash = Player.s_statusEffectTared;
                        hitData.m_point = hit.m_point;
                        __instance.Damage(hitData);
                        
                    }
                    
                    if (__instance != null && __instance.IsPlayer())
                    {
                        __instance.m_damageModifiers.m_fire = HitData.DamageModifier.VeryWeak;
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.Update), null)]
        public class Growth_Update_Patch
        {
            public static void Postfix(Player __instance)
            {
                Player localPlayer = Player.m_localPlayer;
                if (localPlayer != null && LackingImaginationV2Plugin.playerEnabled && EssenceItemData.equipedEssence.Contains("$item_growth_essence"))
                {
                    canDoubleJump = 2;
                }
                else
                {
                    canDoubleJump = 0;
                }
            }
        }
        
        
        
    }






}