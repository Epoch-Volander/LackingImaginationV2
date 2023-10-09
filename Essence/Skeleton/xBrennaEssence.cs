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

        public static bool Awakened = Boolean.Parse(xBrennaEssencePassive.BrennaStats[0]);
        private static float equipDelay = 0.5f;
        private static float deleteDelay = 3600f;
        public static bool Throwable;

        public static GameObject Aura;
        // public static  GameObject FireSword = Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                 GameObject FireSword = Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                
                // LackingImaginationV2Plugin.Log($"Brenna Button was pressed");
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);

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
                    ScheduleEquip(player, ref sword);
                    ScheduleDelete(player, ref sword);
                }
                else if(!Throwable)
                {
                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime * 2f;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    Throwable = true;
                    //add aura, maybe brenna head flames
                    GameObject prefab =  ExpMethods.DeepCopy(ZNetScene.instance.GetPrefab("Skeleton_Hildir").transform.Find("Visual").transform.Find("_skeleton_base").transform.Find("Armature").transform.Find("Hips").transform.Find("Spine").transform.Find("Spine1").transform.Find("Spine2").transform.Find("Neck").transform.Find("Head").transform.Find("fx_Torch_Carried").gameObject);
                    Aura = UnityEngine.GameObject.Instantiate(prefab, player.GetHeadPoint(), Quaternion.identity);
                    Aura.transform.parent = player.transform;
                }
                else
                {
                    //Ability Cooldown
                    se_cd.m_ttl = LackingImaginationUtilities.xBrennaCooldownTime * 2f;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    Throwable = false;
                    
                    UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                }
                
                //Effects, animations, and sounds
                // LackingImaginationV2Plugin.Log($"Brenna {player.transform.position}");
                
             
              
                   
                //x brenna swords fall from the sky and do the Aoe, on kill spawn a brenna 

                //xsummon brenna sword, last for a set duration, you can use it like a sword or throw it with recast, spawns the aoe around hit spot and in random spots nearby and summons a brenna ally at the location, can only have one

                //destroy if dropped, destroy if essence removed, destroy on death// destroy on logout
                
                // broken base form, sacrifice fully upgraded krom, to unlock true version, just a stats list to say yes or no to the version of the sword summoned
                
                // synergy, add element ot the vigil spirts, give them the fire head
                
                


            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        private static void ScheduleEquip(Player player, ref ItemDrop.ItemData item)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleEquipCoroutine(player, item));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleEquipCoroutine(Player player, ItemDrop.ItemData item)
        {
            yield return new WaitForSeconds(equipDelay);
            
            player.EquipItem(item);
        }
        private static void ScheduleDelete(Player player, ref ItemDrop.ItemData item)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleDeleteCoroutine(player, item));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleDeleteCoroutine(Player player, ItemDrop.ItemData item)
        {
            yield return new WaitForSeconds(deleteDelay);
            
            if (player.m_inventory.ContainsItem(item))
            {
                if(player.IsItemEquiped(item)) player.UnequipItem(item);
                player.m_inventory.RemoveItem(item);
            }
        }    
        
    }

    
    
    [HarmonyPatch]
    public static class xBrennaEssencePassive
    {
        public static List<string> BrennaStats = new List<string>() { "false" };
        
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");
        
        
        [HarmonyPatch(typeof(Menu), "OnLogout", null)]
        public class Remove_OnLogout_Patch
        {
            private static List<string> weap = new List<string>()
            {
                LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name,
                LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name
            };
            public static bool Prefix()
            {
                foreach (string bound in weap)
                {
                    if (Player.m_localPlayer.m_inventory.ContainsItemByName(bound))
                    {
                        Player.m_localPlayer.m_inventory.RemoveItem(bound, 1);
                    }
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.DropItem))]
        public static class Brenna_DropItem_Patch
        {
            static bool Prefix(Player __instance, Inventory inventory, ItemDrop.ItemData item, int amount)
            {
                if (amount == 0 || item.m_shared.m_questItem)
                    return true; // Continue with the original method if amount is 0 or it's a quest item.
                
                if (item.m_shared.m_name == LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name || item.m_shared.m_name == LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    if(__instance.IsItemEquiped(item)) __instance.UnequipItem(item);
                    inventory.RemoveItem(item);
                    if(xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xBrennaEssence.Throwable = false;
                    InventoryGui.instance.SetupDragItem((ItemDrop.ItemData) null, (Inventory) null, 1);
                    inventory.Changed();
                    // Item name matches the condition, so we won't drop it.
                    return false;
                }
                // Continue with the original method to drop other items.
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveItemToThis), new Type[] { typeof(Inventory), typeof(ItemDrop.ItemData) })]
        public static class Brenna_MoveItemToThis1_Patch
        {
            static bool Prefix(Inventory __instance, ref Inventory fromInventory, ref ItemDrop.ItemData item)
            {
                if (__instance != fromInventory && (item.m_shared.m_name == LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name || item.m_shared.m_name == LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name))
                {
                    if(Player.m_localPlayer.IsItemEquiped(item)) Player.m_localPlayer.UnequipItem(item);
                    fromInventory.RemoveItem(item);
                    InventoryGui.instance.SetupDragItem((ItemDrop.ItemData) null, (Inventory) null, 1);
                    // __instacne.RemoveItem(item);
                    if(xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xBrennaEssence.Throwable = false;
                    
                    __instance.Changed();
                    fromInventory.Changed();
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveItemToThis), new Type[] { typeof(Inventory), typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int) })]
        public static class Brenna_MoveItemToThis2_Patch
        {
            static void Postfix(Inventory __instance, ref Inventory fromInventory, ref ItemDrop.ItemData item, ref int amount,  ref int x, ref int y)
            {
                if (__instance != fromInventory && (item.m_shared.m_name == LackingImaginationV2Plugin.GO_VulcanSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name || item.m_shared.m_name == LackingImaginationV2Plugin.GO_VulcanSwordBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_name))
                {
                    __instance.RemoveItem(__instance.GetItemAt(x, y));

                    if(xBrennaEssence.Aura != null) UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
                    xBrennaEssence.Throwable = false;
                    
                    __instance.Changed();
                }
               
            }
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.StartAttack))]
        public static class Brenna_Throwable_StartAttack_Patch
        {
            public static bool Prefix(Humanoid __instance, ref bool __result, bool secondaryAttack)
            {
                if (!secondaryAttack) return true;
            
                __instance.ClearActionQueue();
                if (__instance.InAttack() && !__instance.HaveQueuedChain() || __instance.InDodge() || !__instance.CanMove() || __instance.IsKnockedBack() || __instance.IsStaggering() || __instance.InMinorAction())
                {
                    return true;
                }
            
                var currentWeapon = __instance.GetCurrentWeapon();
                if (currentWeapon == null || currentWeapon.m_dropPrefab == null)
                {
                    return true;
                }
            
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (currentWeapon.m_shared.m_name != FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name || !xBrennaEssence.Throwable)
                {
                    return true;
                }
               
                var spearPrefab = ObjectDB.instance?.GetItemPrefab("SpearWolfFang");
                if (spearPrefab == null)
                {
                    return true;
                }
               
                if (__instance.m_currentAttack != null)
                {
                    __instance.m_currentAttack.Stop();
                    __instance.m_previousAttack = __instance.m_currentAttack;
                    __instance.m_currentAttack = null;
                }

                var attack = spearPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_secondaryAttack.Clone();
                if (!attack.Start(__instance, __instance.m_body, __instance.m_zanim, __instance.m_animEvent, __instance.m_visEquipment, currentWeapon, __instance.m_previousAttack, __instance.m_timeSinceLastAttack, __instance.GetAttackDrawPercentage()))
                {
                    return false;
                }
            
                __instance.m_currentAttack = attack;
                __instance.m_lastCombatTimer = 0.0f;
                __result = true;
                xBrennaEssence.Throwable = false;
                ScheduleDelete();
                
                return false;
            }
            private static void ScheduleDelete()
            {
                CoroutineRunner.Instance.StartCoroutine(ScheduleDeleteCoroutine());
            }
            // ReSharper disable Unity.PerformanceAnalysis
            private static IEnumerator ScheduleDeleteCoroutine()
            {
                yield return new WaitForSeconds(2f);
            
                UnityEngine.GameObject.Destroy(xBrennaEssence.Aura);
            }    
        }
        
        [HarmonyPatch(typeof(Attack), nameof(Attack.ProjectileAttackTriggered))]
        public static class Brenna_Throwable_ProjectileAttackTriggered_Patch
        {
            public static void Prefix(Attack __instance, ref EffectList __state)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_weapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    __state = __instance.m_weapon.m_shared.m_triggerEffect;
                    __instance.m_weapon.m_shared.m_triggerEffect = new EffectList();
                }
            }
		      
            public static void Postfix(Attack __instance, EffectList __state)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_weapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    if (__instance.m_weapon.m_lastProjectile.GetComponent<Projectile>() is Projectile projectile)
                    {
                        projectile.m_spawnOnHitEffects = new EffectList { m_effectPrefabs = projectile.m_spawnOnHitEffects.m_effectPrefabs.Concat(__state.m_effectPrefabs).ToArray() };
                        projectile.m_aoe = __instance.m_weapon.m_shared.m_attack.m_attackRayWidth;
                    }
                    __instance.m_weapon.m_shared.m_triggerEffect = __state;
                }
            }
        }        
        
        [HarmonyPatch(typeof(Attack), nameof(Attack.FireProjectileBurst))]
        public static class Brenna_Throwable_FireProjectileBurst_Patch
        {
            public static void Postfix(Attack __instance)
            {
                GameObject FireSword = xBrennaEssence.Awakened ? LackingImaginationV2Plugin.GO_VulcanSword : LackingImaginationV2Plugin.GO_VulcanSwordBroken;
                if (__instance.m_weapon.m_lastProjectile != null && __instance.m_weapon.m_shared.m_name == FireSword.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    var existingMesh = __instance.m_weapon.m_lastProjectile.transform.Find("fangspear");
                    if (existingMesh != null)
                    {
                        ZNetScene.instance.Destroy(existingMesh.gameObject);
                    }
                    var weaponMesh = __instance.m_weapon.m_dropPrefab.transform.Find("attach");
                    if (weaponMesh == null)
                    {
                        return;
                    }
                    var newMesh = UnityEngine.Object.Instantiate(weaponMesh.gameObject, __instance.m_weapon.m_lastProjectile.transform, false);
                    newMesh.AddComponent<ExpMethods.Flip>();
                }
            }
        }

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.SpawnOnHit))]
        public static class Brenna_SpawnOnHit_Patch
        {
            public static void Prefix(Projectile __instance)
            {
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
                    ScheduleNova(Player.m_localPlayer, vector3, 10f, 6);
                    if(xBrennaEssence.Awakened) ScheduleNova(Player.m_localPlayer, vector3, 20f, 12);
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
            
            
            private static void ScheduleNova(Player player, Vector3 vector3, float radius, int num)
            {
                CoroutineRunner.Instance.StartCoroutine(ScheduleNovaCoroutine(player, vector3, radius, num));
            }
            // ReSharper disable Unity.PerformanceAnalysis
            private static IEnumerator ScheduleNovaCoroutine(Player player, Vector3 vector3, float radius, int num)
            {
                yield return new WaitForSeconds(1.5f);
                
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

        
        
        
        
        
        
        
        

    }
}