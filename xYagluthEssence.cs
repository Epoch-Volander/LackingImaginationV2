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
using TMPro;
using UnityEngine.UI;


namespace LackingImaginationV2
{

    public class xYagluthEssence
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
        
        public static string Ability_Name = "Culmination";
        
        public static bool YagluthController1 = false;
        public static bool YagluthController2 = false;
        private static float breathDelay = 0.1f;
        private static float meteorDelay = 1f;
        private static float novaDelay = 1f;
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xYagluthCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
            
                LackingImaginationV2Plugin.Log($"Yag Button was pressed");
                
                //projectile_meteor
                //projectile_beam
                int staticCharge;

                if (player.IsBlocking())
                {
                    LackingImaginationV2Plugin.UseGuardianPower = false;
                    YagluthController1 = true;
                    ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                    YagluthController1 = false;

                    ScheduleMeteor(player);
                    staticCharge = int.Parse(xYagluthEssencePassive.YagluthStats[0]);
                    staticCharge += 20;
                    xYagluthEssencePassive.YagluthStats[0] = staticCharge.ToString();
                }
                else if (player.IsCrouching())
                {
                    LackingImaginationV2Plugin.UseGuardianPower = false;
                    YagluthController2 = true;
                    ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                    YagluthController2 = false;

                    ScheduleNova(player);
                    staticCharge = int.Parse(xYagluthEssencePassive.YagluthStats[0]);
                    staticCharge -= 10;
                    if (staticCharge < 0) staticCharge = 0;
                    xYagluthEssencePassive.YagluthStats[0] = staticCharge.ToString();
                
                    HitData hitData = new HitData();
                    hitData.m_damage.m_lightning = player.m_health * 0.2f;
                    hitData.m_hitType = HitData.HitType.EnemyHit;
                    player.Damage(hitData);
                }
                else
                {
                    ScheduleBeam(player);
                    staticCharge = int.Parse(xYagluthEssencePassive.YagluthStats[0]);
                    staticCharge += 20;
                    xYagluthEssencePassive.YagluthStats[0] = staticCharge.ToString();
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
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
                hitData.m_damage.m_fire = 40f;
                hitData.m_damage.m_lightning = 20f;
                hitData.m_damage.m_chop = 50f;
                hitData.m_damage.m_pickaxe = 50f;
                hitData.ApplyModifier(LackingImaginationGlobal.c_yagluthCulmination);
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
            yield return new WaitForSeconds(novaDelay);

            GameObject Aoe = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("aoe_nova"), player.transform.position, Quaternion.identity);
            Aoe.GetComponent<Aoe>().m_owner = player;
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
                    hitData.ApplyModifier(LackingImaginationGlobal.c_yagluthCulmination);
                    hitData.m_pushForce = 10f;
                    hitData.m_point = ch.transform.position;
                    hitData.SetAttacker(player);
                    ch.Damage(hitData);
                }
            }
            
            ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("Crouch");
            // player.SetCrouch(false);
        }

        private static void ScheduleMeteor(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleMeteorCoroutine(player));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleMeteorCoroutine(Player player)
        {
            yield return new WaitForSeconds(meteorDelay);
            
            GameObject prefab = ZNetScene.instance.GetPrefab("projectile_meteor");
            
            int maxUsages = 12;
            float count = 0;

            Vector3 playerPosition = player.transform.position;
            Vector3 Direction = player.GetLookDir();
            Vector3 upDirection = player.transform.up;
            
            while (count <= maxUsages)
            {
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
                hitData.m_damage.m_fire = 120f;
                hitData.m_damage.m_blunt = 40f;
                hitData.m_damage.m_chop = 50f;
                hitData.m_damage.m_pickaxe = 50f;
                hitData.ApplyModifier(LackingImaginationGlobal.c_yagluthCulmination);
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
        public static List<string> YagluthStats = new List<string>() { "0" };

        private static float timer = 30f;
        public static GameObject Aura;
        public static bool boolAura;
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Yagluth_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_yagluth_essence"))
                {
                    if(YagluthStats[0] != "0")
                    {
                        timer -= dt;
                        if (timer <= 0f)
                        {
                            timer = 30f;
                            YagluthStats[0] = (int.Parse(YagluthStats[0]) - 5).ToString();
                        }
                    }
                    if (int.Parse(YagluthStats[0]) >= (int)LackingImaginationGlobal.c_yagluthCulminationStaticCap - 19
                        && int.Parse(YagluthStats[0]) <= (int)LackingImaginationGlobal.c_yagluthCulminationStaticCap
                        && !boolAura)
                    {
                        Aura = UnityEngine.GameObject.Instantiate(ZNetScene.instance.GetPrefab("fx_Lightning"), __instance.GetCenterPoint(), Quaternion.identity);
                        Aura.transform.parent = __instance.transform;
                        Aura.transform.Find("sfx").GetComponent<ZSFX>().m_minVol = 0.2f;
                        Aura.transform.Find("sfx").GetComponent<ZSFX>().m_maxVol = 0.2f;
                        Aura.transform.Find("sfx").GetComponent<ZSFX>().m_vol = 0.2f;
                        boolAura = true;
                    }
                    if (int.Parse(YagluthStats[0]) < (int)LackingImaginationGlobal.c_yagluthCulminationStaticCap - 19 && boolAura)
                    {
                        UnityEngine.GameObject.Destroy(Aura);
                        boolAura = false;
                    }
                    if (int.Parse(YagluthStats[0]) > (int)LackingImaginationGlobal.c_yagluthCulminationStaticCap)
                    {
                        GameObject Lightning = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("lightningAOE"), __instance.transform.position, Quaternion.identity);
                        Aoe Aoe = Lightning.transform.Find("AOE_ROD").GetComponent<Aoe>();
                        Aoe.m_useTriggers = true;

                        YagluthStats[0] = "0";
                    }
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_Culmination"))
                    {
                        __instance.GetSEMan().AddStatusEffect("SE_Culmination".GetStableHashCode());
                    }
                    if (__instance.GetSEMan().HaveStatusEffect("Burning"))
                    {
                        __instance.GetSEMan().RemoveStatusEffect("Burning".GetStableHashCode());
                        
                        YagluthStats[0] = (int.Parse(YagluthStats[0]) + 5).ToString();
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_Culmination"))
                {
                    __instance.GetSEMan().RemoveStatusEffect("SE_Culmination".GetStableHashCode());
                }
                
            }
        }
        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStatusEffects))]
        public static class Yagluth_UpdateStatusEffects_Patch
        {
            public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
            {
                string iconText = YagluthStats[0];
                for (int index = 0; index < statusEffects.Count; ++index)
                {
                    StatusEffect statusEffect1 = statusEffects[index];
                    if (statusEffect1.name == "SE_Culmination")
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