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

    // public void SpawnOnHit(GameObject go, Collider collider) in projectile
    // {
    public class xGjallEssence
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Gjallarhorn";
        
        private static GameObject GO_GjallGjallarhornProjectile;        
        private static Projectile P_GjallGjallarhornProjectile;
        private static GameObject GO_GjallGjallarhornSpawn;        
        private static Projectile P_GjallGjallarhornSpawn;
        
        private static float shotDelay = 2f;

        public static void Process_Input(Player player, int position) // gjall_egg_projectile  //gjall_spit_projectile
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                
                //projectile that explodes and spawns ally ticks

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xGjallCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_gjall_alerted"), player.transform.position, Quaternion.identity);
                
                ScheduleProjectiles(player);

            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        
        private static void ScheduleProjectiles(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleProjectilesCoroutine(player));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleProjectilesCoroutine(Player player)
        {
            yield return new WaitForSeconds(shotDelay);

             Vector3 vector1 = player.transform.position + player.transform.up * 2f + player.GetLookDir() * .5f;
                GameObject prefab1 = ZNetScene.instance.GetPrefab("gjall_spit_projectile");
                
                GO_GjallGjallarhornProjectile = UnityEngine.Object.Instantiate(prefab1, new Vector3(vector1.x, vector1.y, vector1.z), Quaternion.identity);
                P_GjallGjallarhornProjectile = GO_GjallGjallarhornProjectile.GetComponent<Projectile>();
                P_GjallGjallarhornProjectile.name = "GjallarhornMain";
                P_GjallGjallarhornProjectile.m_respawnItemOnHit = false;
                P_GjallGjallarhornProjectile.m_spawnOnHit = null;
                P_GjallGjallarhornProjectile.m_ttl = 60f;
                P_GjallGjallarhornProjectile.m_gravity = 2f;
                P_GjallGjallarhornProjectile.m_rayRadius = 0.5f;
                P_GjallGjallarhornProjectile.m_aoe = 4f;
                P_GjallGjallarhornProjectile.m_owner = player;
                P_GjallGjallarhornProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector1));
                P_GjallGjallarhornProjectile.transform.localScale = Vector3.one;
            
                RaycastHit hitInfo1 = default(RaycastHit);
                Vector3 player_position1 = player.transform.position;
                Vector3 target1 = (!Physics.Raycast(vector1, player.GetLookDir(), out hitInfo1, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo1.collider) ? (player_position1 + player.GetLookDir() * 1000f) : hitInfo1.point;
                HitData hitData1 = new HitData();
                hitData1.m_damage.m_blunt = UnityEngine.Random.Range(1f , 2f);
                hitData1.m_damage.m_fire = UnityEngine.Random.Range(1f , 2f);
                hitData1.ApplyModifier(LackingImaginationGlobal.c_gjallGjallarhornProjectile);// Change
                
                hitData1.m_pushForce = 1.5f;
                hitData1.SetAttacker(player);
                Vector3 a = Vector3.MoveTowards(GO_GjallGjallarhornProjectile.transform.position, target1, 1f);
                P_GjallGjallarhornProjectile.Setup(player, (a - GO_GjallGjallarhornProjectile.transform.position) * 25f, -1f, hitData1, null, null);
                
                GO_GjallGjallarhornProjectile = null;
                
                //spawn
                Vector3 vector2 = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * .5f;
                GameObject prefab2 = ZNetScene.instance.GetPrefab("gjall_egg_projectile");
            
                GO_GjallGjallarhornSpawn = UnityEngine.Object.Instantiate(prefab2, new Vector3(vector2.x, vector2.y, vector2.z), Quaternion.identity);
                P_GjallGjallarhornSpawn = GO_GjallGjallarhornSpawn.GetComponent<Projectile>();
                P_GjallGjallarhornSpawn.name = "GjallarhornSpawn";
                P_GjallGjallarhornSpawn.m_respawnItemOnHit = false;
                P_GjallGjallarhornSpawn.m_spawnOnHit.GetComponent<SpawnAbility>().m_minToSpawn = 3;
                P_GjallGjallarhornSpawn.m_spawnOnHit.GetComponent<SpawnAbility>().m_maxToSpawn = 3;
                P_GjallGjallarhornSpawn.m_spawnOnHit.GetComponent<SpawnAbility>().m_spawnRadius = 4;
                P_GjallGjallarhornSpawn.m_ttl = 60f;
                P_GjallGjallarhornSpawn.m_gravity = 2f;
                P_GjallGjallarhornSpawn.m_rayRadius = 0.2f;
                P_GjallGjallarhornSpawn.m_owner = player;
                P_GjallGjallarhornSpawn.m_aoe = 6f;
                P_GjallGjallarhornSpawn.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector2));
                P_GjallGjallarhornSpawn.transform.localScale = Vector3.one;
        
                RaycastHit hitInfo2 = default(RaycastHit);
                Vector3 player_position2 = player.transform.position;
                Vector3 target2 = (!Physics.Raycast(vector2, player.GetLookDir(), out hitInfo2, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo2.collider) ? (player_position2 + player.GetLookDir() * 1000f) : hitInfo2.point;
                HitData hitData2 = new HitData();
                hitData2.m_damage.m_blunt = UnityEngine.Random.Range(1f , 2f);
                hitData2.SetAttacker(player);
                Vector3 a2 = Vector3.MoveTowards(GO_GjallGjallarhornSpawn.transform.position, target2, 1f);
                P_GjallGjallarhornSpawn.Setup(player, (a2 - GO_GjallGjallarhornSpawn.transform.position) * 25f, -1f, hitData2, null, null);
            
                GO_GjallGjallarhornSpawn = null;
                
        }
    }
    
    
    

    [HarmonyPatch]
    public static class xGjallEssencePassive
    {
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.FixedUpdate))]
        public class Gjall_FixedUpdate_Patch
        {
            
            private static float xfrequency = 5f; // Adjust this value to control the frequency of the motion
            private static float xamplitude = 0.12f; // Adjust this value to control the amplitude of the motion
            private static float yfrequency = 5f; // Adjust this value to control the frequency of the motion
            private static float yamplitude = 0.12f; // Adjust this value to control the amplitude of the motion

            static void Prefix(Projectile __instance)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_gjall_essence") && __instance.name == "GjallarhornMain")
                {
                    if (__instance.m_didHit) return;
                    if (__instance.m_owner != null && __instance.m_owner.IsPlayer())
                    {
                        // LackingImaginationV2Plugin.Log($"xGjall{__instance.m_vel}");
                        
                        float originalY = __instance.transform.position.y;
                        float newY = Mathf.Cos(Time.time * yfrequency) * yamplitude;
                        
                        float originalX = __instance.transform.position.x;
                        float newX = Mathf.Sin(Time.time * xfrequency) * xamplitude;

                        Vector3 newPos = __instance.transform.position;
                        newPos.y = originalY + newY;
                        newPos.x = originalX + newX;
                        
                        __instance.transform.position = newPos;
                        
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.SpawnOnHit))]
        public static class Gjall_SpawnOnHit_Patch
        {
            public static void Postfix(Projectile __instance, ref GameObject go)
            {
                if (__instance.name == "GjallarhornSpawn")
                {
                     SE_TimedDeath se_timedeath = (SE_TimedDeath)ScriptableObject.CreateInstance(typeof(SE_TimedDeath));
                      se_timedeath.lifeDuration = LackingImaginationGlobal.c_gjallGjallarhornSummonDuration;
                      se_timedeath.m_ttl = LackingImaginationGlobal.c_gjallGjallarhornSummonDuration + 500f;
                     
                     Vector3 vector3 = __instance.transform.position + __instance.transform.TransformDirection(__instance.m_spawnOffset);

                     List<Character> allCharacters = new List<Character>();
                    allCharacters.Clear();
                    Character.GetCharactersInRange(vector3, 5f, allCharacters);
                    foreach (Character ch in allCharacters)
                    {
                        if (ch.GetBaseAI() != null && ch.name == "Tick(Clone)" && ch.GetHealth() == ch.GetMaxHealth())
                        {
                             // LackingImaginationV2Plugin.Log($"Bo{ch.name}");
                            ch.name += "Ally";
                            ch.m_faction = Character.Faction.Players;
                            ch.GetSEMan().AddStatusEffect(se_timedeath);
                            ch.SetMaxHealth(ch.GetMaxHealthBase() * 4f);
                            ch.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                            foreach (CharacterDrop.Drop drop in ch.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
                            MonsterAI ai = ch.GetBaseAI() as MonsterAI;
                            if (ai != null)
                            {
                                Traverse.Create(root: ai).Field("m_attackPlayerObjects").SetValue(false);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        public static class Gjall_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_gjall_essence"))
                {
                    __result += LackingImaginationGlobal.c_gjallGjallarhornArmor;
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Gjall_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_gjall_essence") && hit.GetAttacker() != null)
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
                    if (__instance != null && __instance.IsPlayer())
                    {
                        Player.m_localPlayer.m_damageModifiers.m_pierce = HitData.DamageModifier.VeryWeak;
                    }
                }
            }
        }
        
        

    }
}