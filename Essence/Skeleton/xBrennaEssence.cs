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


namespace LackingImaginationV2
{

    public class xBrennaEssence
    {
        public static string Ability_Name = "Vulcan";

        public static bool Awakened;
        private static float equipDelay = 0.5f;
        private static float deleteDelay = 3600f;
        public static bool Throwable;

        public static GameObject Aura;
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                
                Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
                GameObject FireSword = Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                
                // LackingImaginationV2Plugin.Log($"Brenna Button was pressed");
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                
                
                // GameObject prefab =  ExpMethods.DeepCopy(ZNetScene.instance.GetPrefab("Skeleton_Hildir").transform.Find("Visual").transform.Find("_skeleton_base").transform.Find("Armature").transform.Find("Hips").transform.Find("Spine").transform.Find("Spine1").transform.Find("Spine2").transform.Find("Neck").transform.Find("Head").transform.Find("fx_Torch_Carried").gameObject);
                //
                //  Aura = new EffectList
                // {
                //     m_effectPrefabs = new EffectList.EffectData[]
                //     {
                //         new()
                //         {
                //             m_prefab = prefab,
                //             m_enabled = true,
                //             m_variant = 0,
                //             m_attach = false,
                //             m_follow = true,
                //             m_childTransform = "Head",
                //             // m_inheritParentScale = true,
                //             // m_multiplyParentVisualScale = true,
                //             // m_scale = true,
                //             m_inheritParentRotation = true,
                //         }
                //     }
                // };

                if (!player.m_inventory.ContainsItemByName(FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name))
                {
                    if (!player.m_inventory.CanAddItem(FireSword))
                    {
                        player.Message(MessageHud.MessageType.TopLeft, $"Inventory Full");
                        return;
                    }
                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                    
                    player.m_inventory.AddItem(FireSword, 1);
                    ItemDrop.ItemData sword = player.m_inventory.GetItem(FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
                    UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Vulcan, player.transform.position + player.transform.up * 0.2f, Quaternion.identity);
                    xSkeletonSynergy.ScheduleEquip(player, ref sword, equipDelay);
                    xSkeletonSynergy.ScheduleDelete(player, ref sword, deleteDelay);
                }
                else if(!Throwable)
                {
                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime * 2f;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    Throwable = true;
                    //add aura, maybe brenna head flames
                    ItemDrop.ItemData sword = player.m_inventory.GetItem(FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
                    xSkeletonSynergy.ScheduleEquip(player, ref sword, equipDelay);
                    
                    BrennaThrow se_brennathrow = (BrennaThrow)ScriptableObject.CreateInstance(typeof(BrennaThrow));
                    // Aura =  ExpMethods.DeepCopy(ZNetScene.instance.GetPrefab("Skeleton_Hildir").transform.Find("Visual").transform.Find("_skeleton_base").transform.Find("Armature").transform.Find("Hips").transform.Find("Spine").transform.Find("Spine1").transform.Find("Spine2").transform.Find("Neck").transform.Find("Head").transform.Find("fx_Torch_Carried").gameObject);
                    // se_brennathrow.m_startEffects.m_effectPrefabs[0].m_prefab = Aura;
                    
                    player.GetSEMan().AddStatusEffect(se_brennathrow);
                    
                    // GameObject prefab =  ExpMethods.DeepCopy(ZNetScene.instance.GetPrefab("Skeleton_Hildir").transform.Find("Visual").transform.Find("_skeleton_base").transform.Find("Armature").transform.Find("Hips").transform.Find("Spine").transform.Find("Spine1").transform.Find("Spine2").transform.Find("Neck").transform.Find("Head").transform.Find("fx_Torch_Carried").gameObject);
                    // Aura = UnityEngine.GameObject.Instantiate(prefab, player.GetHeadPoint(), Quaternion.identity);
                    // Aura.transform.parent = player.transform;
                    UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Vulcan, player.transform.position + player.transform.up * 0.2f, Quaternion.identity);
                }
                else
                {
                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime * 2f;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    Throwable = false;
                    // if (xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    if(player.GetSEMan().HaveStatusEffect("BrennaThrow".GetStableHashCode()))player.GetSEMan().RemoveStatusEffect("BrennaThrow".GetStableHashCode());
                }
                
                //Effects, animations, and sounds
                // LackingImaginationV2Plugin.Log($"Brenna {player.transform.position}");
                 
                //x brenna swords fall from the sky and do the Aoe, on kill spawn a brenna 

                //x summon brenna sword, last for a set duration, you can use it like a sword or throw it with recast, spawns the aoe around hit spot and in random spots nearby and summons a brenna ally at the location, can only have one

                //x destroy if dropped, destroy if essence removed, destroy on death// destroy on logout
                
                //x broken base form, sacrifice fully upgraded krom, to unlock true version, just a stats list to say yes or no to the version of the sword summoned
                
                //X synergy, add element ot the vigil spirts, give them the fire head
                
            }
           
        }
    }

    
    
    [HarmonyPatch]
    public static class xBrennaEssencePassive
    {
        public static List<string> BrennaStats = new List<string>() { "false" };
        
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");
        
        

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.SpawnOnHit))]
        public static class Brenna_SpawnOnHit_Patch
        {
            public static void Prefix(Projectile __instance)
            {
                xBrennaEssence.Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_owner == Player.m_localPlayer && __instance.m_spawnItem?.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    Vector3 vector3 = __instance.transform.position + (__instance.transform.up * 0.6f) + __instance.transform.TransformDirection(__instance.m_spawnOffset);
                    Quaternion rotation = __instance.transform.rotation;
                    rotation = Quaternion.Euler(0.0f, __instance.transform.rotation.eulerAngles.y, 0.0f);
                    
                    //sphere radius 9.5
                    // fx_fireskeleton_nova
                    GameObject Aoe = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_fireskeleton_nova"), vector3, rotation);
                    DelayRemoval(Aoe);
                    StartAoe(Player.m_localPlayer, vector3);
                    ScheduleNova(Player.m_localPlayer, vector3, 10f, 6, 1f);
                    xBrennaEssence.Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
                    if(xBrennaEssence.Awakened) ScheduleNova(Player.m_localPlayer, vector3, 20f, 12, 2f);
                    __instance.m_spawnItem = null;
                }
            }
            static void DelayRemoval(GameObject Gobject)
            {
                ParticleSystem[] particleSystems = Gobject.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    var main = ps.main;
                    main.startDelay = 0f;
                }
                ZSFX[] zsfx = Gobject.GetComponentsInChildren<ZSFX>();
                foreach (ZSFX sound in zsfx)
                {
                    sound.m_delay = 0f;
                    sound.m_maxDelay = 0f;
                    sound.m_minDelay = 0f;
                }
            }

            static void StartAoe(Player player, Vector3 vector3)
            {
                HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

                Vector3 capsuleCenter = vector3;
                float capsuleRadius = 4.75f; // Radius of the capsule

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
                        
                            xBrennaEssence.Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
                            HitData hitData = new HitData();
                            hitData.m_damage.m_fire = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_fire : LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_fire;
                            hitData.m_damage.m_slash = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_slash : LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_slash;
                            hitData.m_dir = collider.transform.position - capsuleCenter;
                            hitData.ApplyModifier(2f);
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
            }
            
            
            private static void ScheduleNova(Player player, Vector3 vector3, float radius, int num, float delay)
            {
                CoroutineRunner.Instance.StartCoroutine(ScheduleNovaCoroutine(player, vector3, radius, num, delay));
            }
            // ReSharper disable Unity.PerformanceAnalysis
            private static IEnumerator ScheduleNovaCoroutine(Player player, Vector3 vector3, float radius, int num, float delay)
            {
                yield return new WaitForSeconds(delay);
                
                float circleRadius = radius; // Radius of the circle
                int numberOfAoEToSpawn = num; // Number of AoE objects to spawn

                for (int i = 0; i < numberOfAoEToSpawn; i++)
                {
                    float angle = i * 360f / numberOfAoEToSpawn; // Calculate the angle for each AoE

                    // Calculate the position around the circle using trigonometry
                    float x = vector3.x + circleRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
                    float z = vector3.z + circleRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

                    // Create a new instance of the AoE prefab at the calculated position
                    GameObject Aoe = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_fireskeleton_nova"), new Vector3(x, vector3.y, z), Quaternion.identity);
                    DelayRemoval(Aoe);
                    StartAoe(player, new Vector3(x, vector3.y, z));
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        public static class Brenna_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_brenna_essence"))
                {
                    __result -= LackingImaginationGlobal.c_brennaVulcanArmor;
                    if (__result < 0)
                    {
                        __result = 0;
                    }
                }
            }
        }
        
    }
}