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
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");

        
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
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
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
                Vector3 playerPosition = player.transform.position ;
                Vector3 forwardDirection = player.GetLookDir() * 5f;
                Vector3 upDirection = player.transform.up;
                
                Vector3 spawnPosition = playerPosition + forwardDirection  + (upDirection * 1.4f);
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
            
             // Create a HashSet to keep track of detected objects.
            HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

            Vector3 capsuleCenter = player.transform.position;
            float capsuleRadius = 5f; // Radius of the capsule

            // Perform the capsule overlap check with the specified layer mask
            Collider[] colliders = Physics.OverlapSphere(capsuleCenter, capsuleRadius, Script_Breath_Layermask);

            foreach (Collider collider in colliders)
            {
                IDestructible destructibleComponent = collider.gameObject.GetComponent<IDestructible>();
                Character characterComponent = collider.gameObject.GetComponent<Character>();
                if (destructibleComponent != null || (characterComponent != null && !characterComponent.IsOwner()))
                {
                    // This is a valid target (creature) if it hasn't been detected before.
                    if (!detectedObjects.Contains(collider.gameObject))
                    {
                        detectedObjects.Add(collider.gameObject);
                        
                        HitData hitData = new HitData();
                        hitData.m_damage.m_fire = 65f;
                        hitData.m_damage.m_blunt = 65f;
                        hitData.m_dir = collider.transform.position - player.transform.position;
                        hitData.ApplyModifier(LackingImaginationGlobal.c_yagluthCulmination);
                        hitData.m_pushForce = 10f;
                        hitData.m_hitCollider = collider;
                        hitData.m_dodgeable = true;
                        hitData.m_blockable = true;
                        hitData.m_point = collider.gameObject.transform.position;
                        hitData.SetAttacker(player);
                        hitData.m_hitType = HitData.HitType.PlayerHit;
                        destructibleComponent.Damage(hitData);
                    }
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
                P_CulminationMeteorProjectile.m_aoe = 5f;
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
        public static GameObject[] Aura;
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
                        && !boolAura && !__instance.IsDead() && !__instance.InCutscene() && !__instance.IsTeleporting())
                    {
                        EffectList m_starteffect = new EffectList
                        {
                            m_effectPrefabs = new EffectList.EffectData[]
                            {
                                new()
                                {
                                    m_prefab = ZNetScene.instance.GetPrefab("fx_Lightning"),
                                    m_enabled = true,
                                    m_variant = 0,
                                    m_attach = false,
                                    m_follow = true,
                                    m_inheritParentScale = true,
                                    m_multiplyParentVisualScale = true,
                                    m_scale = true,
                                    m_inheritParentRotation = true,
                                }
                            }
                        };
                        
                        // Aura = UnityEngine.GameObject.Instantiate(ZNetScene.instance.GetPrefab("fx_Lightning"), __instance.GetCenterPoint(), Quaternion.identity);
                        // Aura.transform.parent = __instance.transform;
                        m_starteffect.m_effectPrefabs[0].m_prefab.transform.Find("sfx").GetComponent<ZSFX>().m_minVol = 0.2f;
                        m_starteffect.m_effectPrefabs[0].m_prefab.transform.Find("sfx").GetComponent<ZSFX>().m_maxVol = 0.2f;
                        m_starteffect.m_effectPrefabs[0].m_prefab.transform.Find("sfx").GetComponent<ZSFX>().m_vol = 0.2f;
                        Aura = m_starteffect.Create(__instance.GetCenterPoint(), __instance.transform.rotation, __instance.transform, __instance.GetRadius() * 2f, __instance.GetPlayerModel());
                        
                        boolAura = true;
                    }
                    if (int.Parse(YagluthStats[0]) < (int)LackingImaginationGlobal.c_yagluthCulminationStaticCap - 19 && boolAura)
                    {
                        foreach (GameObject startEffectInstance in Aura)
                        {
                            if ((bool)(UnityEngine.Object) startEffectInstance)
                            {
                                ZNetView component = startEffectInstance.GetComponent<ZNetView>();
                                if (component.IsValid())
                                {
                                    component.ClaimOwnership();
                                    component.Destroy();
                                }
                            }
                        }
                        boolAura = false;
                    }
                    if (int.Parse(YagluthStats[0]) > (int)LackingImaginationGlobal.c_yagluthCulminationStaticCap)
                    {
                        GameObject Lightning = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("lightningAOE"), __instance.transform.position, Quaternion.identity);
                        Aoe Aoe = Lightning.transform.Find("AOE_ROD").GetComponent<Aoe>();
                        Aoe.m_useTriggers = true;
                        Aoe.m_hitOwner = true;
                        Aoe.m_owner = __instance;

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