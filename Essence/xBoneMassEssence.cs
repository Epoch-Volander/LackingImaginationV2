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
using UnityEditor;
using UnityEngine;

namespace LackingImaginationV2
{

    public class xBoneMassEssence  // the lob that summons rnadom allies
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Mass \nRelease";
        
        private static GameObject GO_BoneMassMassReleaseProjectile;        
        private static Projectile P_BoneMassMassReleaseProjectile;
        // private static GameObject GO_BoneMassMassReleaseSkeleton; 
        // private static GameObject GO_BoneMassMassReleaseBlob; 
        
        private static float shotDelay = 0.5f;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xBoneMassCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                    
                //Effects, animations, and sounds
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab(""), player.transform.position, Quaternion.identity);
                
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("throw_bomb"); 

                Vector3 vector = player.transform.position + player.transform.up  * 2f + player.GetLookDir() * .5f;
                GameObject prefab = ZNetScene.instance.GetPrefab("bonemass_throw_projectile");
                // ExpMethods.LogGameObjectInfo(prefab);
                player.transform.rotation = Quaternion.LookRotation(player.GetLookDir()); 
                
                ScheduleProjectiles(player, vector, prefab);
                
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        
        private static void ScheduleProjectiles(Player player, Vector3 vector, GameObject prefab)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleProjectilesCoroutine(player, vector, prefab));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleProjectilesCoroutine(Player player, Vector3 vector, GameObject prefab)
        {
            yield return new WaitForSeconds(shotDelay);
            
            GO_BoneMassMassReleaseProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
            
            // GO_BoneMassMassReleaseSkeleton = GO_BoneMassMassReleaseProjectile.GetComponent<Projectile>().m_spawnOnHit.GetComponent<SpawnAbility>().m_spawnPrefab[0];
            // GO_BoneMassMassReleaseSkeleton.name += "Ally" ;
            // GO_BoneMassMassReleaseProjectile.GetComponent<Projectile>().m_spawnOnHit.GetComponent<SpawnAbility>().m_spawnPrefab[0] = GO_BoneMassMassReleaseSkeleton;
            // GO_BoneMassMassReleaseBlob = GO_BoneMassMassReleaseProjectile.GetComponent<Projectile>().m_spawnOnHit.GetComponent<SpawnAbility>().m_spawnPrefab[1];
            // GO_BoneMassMassReleaseBlob.name += "Ally" ;
            // GO_BoneMassMassReleaseProjectile.GetComponent<Projectile>().m_spawnOnHit.GetComponent<SpawnAbility>().m_spawnPrefab[1] = GO_BoneMassMassReleaseBlob;
            
            //SkeletonAlly(Clone)   //BlobAlly(Clone)
            
            P_BoneMassMassReleaseProjectile = GO_BoneMassMassReleaseProjectile.GetComponent<Projectile>();
            P_BoneMassMassReleaseProjectile.name = "Mass Release";
            P_BoneMassMassReleaseProjectile.m_respawnItemOnHit = false;
            // P_BoneMassMassReleaseProjectile.m_spawnOnHit = null;
            P_BoneMassMassReleaseProjectile.m_ttl = 60f;
            P_BoneMassMassReleaseProjectile.m_gravity = 10f;
            P_BoneMassMassReleaseProjectile.m_rayRadius = 0.5f;
            P_BoneMassMassReleaseProjectile.m_aoe = 6f;
            P_BoneMassMassReleaseProjectile.m_canHitWater = true;
            P_BoneMassMassReleaseProjectile.m_owner = player;
            P_BoneMassMassReleaseProjectile.m_skill = Skills.SkillType.None;
            P_BoneMassMassReleaseProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
            P_BoneMassMassReleaseProjectile.transform.localScale = Vector3.one;
            
            RaycastHit hitInfo = default(RaycastHit);
            Vector3 player_position = player.transform.position;
            Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
            HitData hitData = new HitData();
            hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f , 2f);
            hitData.m_damage.m_poison = UnityEngine.Random.Range(1f, 2f);
            hitData.ApplyModifier(LackingImaginationGlobal.c_bonemassMassReleaseProjectile);
            hitData.m_pushForce = 3f;
            hitData.SetAttacker(player);
            Vector3 a = Vector3.MoveTowards(GO_BoneMassMassReleaseProjectile.transform.position, target, 1f);
            P_BoneMassMassReleaseProjectile.Setup(player, (a - GO_BoneMassMassReleaseProjectile.transform.position) * 25f, -1f, hitData, null, null);
            GO_BoneMassMassReleaseProjectile = null;
        }


    }

    [HarmonyPatch]
    public class xBoneMassEssencePassive
    {
        [HarmonyPatch(typeof(Projectile),  nameof(Projectile.SpawnOnHit))]
        public static class BoneMass_SpawnOnHit_Patch
        {
            public static void Postfix(Projectile __instance, ref GameObject go)
            {
                if (__instance.name == "Mass Release")
                {
                     SE_TimedDeath se_timedeath = (SE_TimedDeath)ScriptableObject.CreateInstance(typeof(SE_TimedDeath));
                      se_timedeath.lifeDuration = LackingImaginationGlobal.c_bonemassMassReleaseSummonDuration;
                      se_timedeath.m_ttl = LackingImaginationGlobal.c_bonemassMassReleaseSummonDuration + 500f;
                     
                     Vector3 vector3 = __instance.transform.position + __instance.transform.TransformDirection(__instance.m_spawnOffset);

                     List<Character> allCharacters = new List<Character>();
                    allCharacters.Clear();
                    Character.GetCharactersInRange(vector3, 5f, allCharacters);
                    foreach (Character ch in allCharacters)
                    {
                        if (ch.GetBaseAI() != null && ch.name == "Skeleton(Clone)" && ch.GetHealth() == ch.GetMaxHealth() 
                            || ch.GetBaseAI() != null && ch.name == "Blob(Clone)" && ch.GetHealth() == ch.GetMaxHealth())
                        {
                            // LackingImaginationV2Plugin.Log($"Bo{ch.name}");
                            ch.name += "Ally";
                            ch.m_faction = Character.Faction.Players;
                            ch.GetSEMan().AddStatusEffect(se_timedeath);
                            ch.SetMaxHealth(ch.GetMaxHealthBase() * 10f);
                            ch.gameObject.AddComponent<Tameable>();
                            ch.GetComponent<Tameable>().Tame();
                            ch.GetComponent<Tameable>().m_unsummonDistance = 100f;
                            ch.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
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

        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class BoneMass_RPC_Damage_Patch
        {
            public static void Postfix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_bonemass_essence") && hit.GetAttacker() != null)
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
                        int Cloud = UnityEngine.Random.Range(1, 6); // 1-5 inclusive
                        // LackingImaginationV2Plugin.Log($"Cloud{Cloud}");
                        if (Cloud == 3)
                        {
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_blobelite_attack"), __instance.transform.position, Quaternion.identity);
                            List<Character> allCharacters = new List<Character>();
                            allCharacters.Clear();
                            Character.GetCharactersInRange(__instance.transform.position, 4f, allCharacters);
                            foreach (Character ch in allCharacters)
                            {
                                if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer))
                                    && !ch.m_tamed || ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                                {
                                    HitData hitData = new HitData();
                                    hitData.m_damage.m_poison = ch.GetMaxHealth() * 0.1f;
                                    hitData.m_point = ch.transform.position;
                                    hitData.SetAttacker(__instance);
                                    ch.Damage(hitData);
                                    
                                }
                            }
                            
                        }
                    }
                }
            }
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_bonemass_essence") && hit.GetAttacker() != null)
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
                        Player.m_localPlayer.m_damageModifiers.m_blunt = HitData.DamageModifier.VeryWeak;
                        Player.m_localPlayer.m_damageModifiers.m_spirit = HitData.DamageModifier.VeryWeak;
                        Player.m_localPlayer.m_damageModifiers.m_pierce = HitData.DamageModifier.Resistant;
                    }
                }
            }
        }
        
        

    }
}