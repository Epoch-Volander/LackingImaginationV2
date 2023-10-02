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

    public class xYagluthEssence
    {
        private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
        private static int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");
        
        public static string Ability_Name = "Culmination";
        
        public static bool YagluthController1 = false;
        public static bool YagluthController2 = false;
        private static float breathDelay = 0.1f;
        private static float meteorDelay = 1f;
        
        public static void Process_Input(Player player, int position)
        {
            //Ability Cooldown
            StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
            se_cd.m_ttl = LackingImaginationUtilities.xYagluthCooldownTime;
            player.GetSEMan().AddStatusEffect(se_cd);
            
            LackingImaginationV2Plugin.Log($"Yag Button was pressed");
            
            
            //projectile_meteor
            //projectile_beam


            if (player.IsBlocking())
            {
                LackingImaginationV2Plugin.UseGuardianPower = false;
                YagluthController1 = true;
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                YagluthController1 = false;

                ScheduleMeteor(player);
            }
            if (player.IsCrouching())
            {
                LackingImaginationV2Plugin.UseGuardianPower = false;
                YagluthController2 = true;
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                YagluthController2 = false;

                ScheduleNova(player);
            }
            else
            {
                ScheduleBeam(player);
            }


        }
        
         private static void ScheduleBeam(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleBeamCoroutine(player));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleBeamCoroutine(Player player)
        {
            yield return new WaitForSeconds(breathDelay);
            
            // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("projectile_beam"), player.GetCenterPoint() +player.transform.forward * 0.5f, player.transform.rotation, player.transform);
           
            GameObject prefab = ZNetScene.instance.GetPrefab("projectile_beam");
            
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_goblinking_beam"), player.transform.position, Quaternion.identity);
            
            int maxUsages = 20;
            float count = 0;

            while (count <= maxUsages)
            {
                Vector3 playerPosition = player.transform.position;
                Vector3 forwardDirection = player.GetLookDir();
                Vector3 upDirection = player.transform.up;
                
                Vector3 spawnPosition = playerPosition + upDirection * 1.4f + forwardDirection * 0.5f;
                GameObject GO_CulminationBeamProjectile = UnityEngine.Object.Instantiate(prefab, spawnPosition, Quaternion.identity);

                Projectile P_CulminationBeamProjectile = GO_CulminationBeamProjectile.GetComponent<Projectile>();
                
                P_CulminationBeamProjectile.name = "CulminationBeamProjectile";
                P_CulminationBeamProjectile.m_respawnItemOnHit = false;
                P_CulminationBeamProjectile.m_spawnOnHit = null;
                P_CulminationBeamProjectile.m_ttl = 60f;
                P_CulminationBeamProjectile.m_gravity = 0.0f;
                P_CulminationBeamProjectile.m_rayRadius = 0.5f;
                P_CulminationBeamProjectile.m_aoe = 0.2f;
                P_CulminationBeamProjectile.m_hitNoise = 100f;
                P_CulminationBeamProjectile.m_owner = player;
                    
                P_CulminationBeamProjectile.transform.localRotation = Quaternion.LookRotation(forwardDirection);
                // P_CulminationBeamProjectile.transform.localScale = Vector3.one;
                
                RaycastHit hitInfo;
                Vector3 target = (!Physics.Raycast(spawnPosition, forwardDirection, out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (spawnPosition + forwardDirection * 1000f) : hitInfo.point;
                    
                HitData hitData = new HitData();
                hitData.m_damage.m_fire = 4f;
                hitData.m_damage.m_lightning = 2f;
                hitData.m_damage.m_chop = 5f;
                hitData.m_damage.m_pickaxe = 5f;
                // hitData.ApplyModifier(player.GetCurrentWeapon().GetDamage().GetTotalDamage() * LackingImaginationGlobal.c_moderDraconicFrostProjectile);
                hitData.m_pushForce = 0.5f;
                hitData.SetAttacker(player);
                hitData.m_dodgeable = true;
                hitData.m_blockable = true;
                    
                Vector3 velocity = (target - spawnPosition).normalized * 25f;
                P_CulminationBeamProjectile.Setup(player, velocity, -1f, hitData, null, null);
                
                
                yield return new WaitForSeconds(breathDelay);
                
                count ++;
                
                yield return null;
            }
        }

        private static void ScheduleNova(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleNovaCoroutine(player));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleNovaCoroutine(Player player)
        {
            yield return new WaitForSeconds(0.1f);
            
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("aoe_nova"), player.transform.position, Quaternion.identity);
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_goblinking_nova"), player.transform.position, Quaternion.identity);

            List<Character> allCharacters = new List<Character>();
            allCharacters.Clear();
            Character.GetCharactersInRange(player.transform.position, 5f, allCharacters);
            foreach (Character ch in allCharacters)
            {
                if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer)) 
                    && !ch.m_tamed ||ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                {
                    HitData hitData = new HitData();
                    hitData.m_damage.m_fire = 65f;
                    hitData.m_damage.m_blunt = 65f;
                    hitData.m_dir = ch.transform.position - player.transform.position;
                    // hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_loxWildTremor));
                    hitData.m_pushForce = 10f;
                    hitData.m_point = ch.transform.position;
                    hitData.SetAttacker(player);
                    ch.Damage(hitData);
                }
            }
            
        }

        private static void ScheduleMeteor(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleMeteorCoroutine(player));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleMeteorCoroutine(Player player)
        {
            yield return new WaitForSeconds(meteorDelay);
            
            // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("projectile_beam"), player.GetCenterPoint() +player.transform.forward * 0.5f, player.transform.rotation, player.transform);
           
            GameObject prefab = ZNetScene.instance.GetPrefab("projectile_meteor");
            
            int maxUsages = 12;
            float count = 0;

            Vector3 playerPosition = player.transform.position;
            Vector3 Direction = player.GetLookDir();
            Vector3 upDirection = player.transform.up;
            
            while (count <= maxUsages)
            {
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("GoblinKing").GetComponent<Humanoid>().m_defaultItems[1].GetComponent<ItemDrop>().m_itemData.m_shared.m_trailStartEffect.m_effectPrefabs[0], player.transform.position, Quaternion.identity);
                
                Vector3 spawnPosition = playerPosition + upDirection * 50f + Direction * 0.5f;
                GameObject GO_CulminationMeteorProjectile = UnityEngine.Object.Instantiate(prefab, spawnPosition, Quaternion.identity);

                Projectile P_CulminationMeteorProjectile = GO_CulminationMeteorProjectile.GetComponent<Projectile>();
                
                P_CulminationMeteorProjectile.name = "CulminationMeteorProjectile";
                P_CulminationMeteorProjectile.m_respawnItemOnHit = false;
                P_CulminationMeteorProjectile.m_spawnOnHit = null;
                P_CulminationMeteorProjectile.m_ttl = 60f;
                P_CulminationMeteorProjectile.m_gravity = 0.0f;
                P_CulminationMeteorProjectile.m_rayRadius = 1f;
                P_CulminationMeteorProjectile.m_aoe = 2f;
                P_CulminationMeteorProjectile.m_hitNoise = 100f;
                P_CulminationMeteorProjectile.m_owner = player;
                    
                P_CulminationMeteorProjectile.transform.localRotation = Quaternion.LookRotation(Direction);
                // P_CulminationBeamProjectile.transform.localScale = Vector3.one;
                
                RaycastHit hitInfo;
                Vector3 target = (!Physics.Raycast(player.GetEyePoint() , Direction, out hitInfo, 1000f, Script_Layermask) || !(bool)hitInfo.collider) ? (player.GetEyePoint() + Direction * 1000f) : hitInfo.point;
                target += UnityEngine.Random.insideUnitSphere * 15f;
                
                HitData hitData = new HitData();
                hitData.m_damage.m_fire = 12f;
                hitData.m_damage.m_blunt = 4f;
                hitData.m_damage.m_chop = 5f;
                hitData.m_damage.m_pickaxe = 5f;
                // hitData.ApplyModifier(player.GetCurrentWeapon().GetDamage().GetTotalDamage() * LackingImaginationGlobal.c_moderDraconicFrostProjectile);
                hitData.m_pushForce = 0.5f;
                hitData.SetAttacker(player);
                hitData.m_dodgeable = true;
                hitData.m_blockable = true;
                    
                Vector3 velocity = (target - spawnPosition).normalized * 25f;
                
                P_CulminationMeteorProjectile.Setup(player, velocity, -1f, hitData, null, null);
                
                
                yield return new WaitForSeconds(meteorDelay * 0.5f);
                
                count ++;
                
                yield return null;
            }
        }
        
        
        
    }

    [HarmonyPatch]
    public class xYagluthEssencePassive
    {
        
        
        
        
    }
}