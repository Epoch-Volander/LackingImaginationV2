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

    public class xModerEssence  //got to do dragon breath
    {
        private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
        private static int Script_Projectile_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");

        
        public static string Ability_Name = "Draconic Frost"; //freezing aura passive
        
        // private static GameObject GO_DraconicFrostProjectile;
        // private static Projectile P_DraconicFrostProjectile;
        

        private static float shotDelay = 0.05f;
        // private static float triangleSize = 2.0f;
        // private static int shotsFired = 0;
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Moder Button was pressed");
                LackingImaginationV2Plugin.Log($"Moder {player.transform.position}");
                
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xModerCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_dragon_death"), player.transform.position, Quaternion.identity);

                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("dragon_coldbreath"), player.transform.position, player.transform.rotation, player.transform);
                
                //dragon_spit_shotgun //accuracy 13, burst 16, interval 0.05
                //dragon_ice_projectile
                //sfx_dragon_coldball_start
                //vfx_ColdBall_launch
                //sfx_dragon_coldball_launch
                
                
                
                //dragon_coldbreath
                // vfx_dragon_coldbreath
                // sfx_dragon_coldbreath_start
                //sfx_dragon_coldbreath_trailon

                if (player.IsBlocking())//just frost
                {
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldbreath_start"), player.transform.position, Quaternion.identity);
                    GameObject Breath = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_dragon_coldbreath"), player.GetCenterPoint(), player.transform.rotation, player.transform);
                    
                    // Create a HashSet to keep track of detected objects.
                    HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
                    Debug.Log("start");
                    
                    Vector3 capsuleCenter = player.GetCenterPoint() + player.transform.forward * 0.5f;
                    float capsuleRadius = 0.65f; // Radius of the capsule
                    float capsuleHeight = 37f; // Height of the capsule (equals the ray length)

                    // Perform the capsule overlap check with the specified layer mask
                    Collider[] colliders = Physics.OverlapCapsule(capsuleCenter, capsuleCenter + player.transform.forward * capsuleHeight, capsuleRadius, Script_Projectile_Layermask);

                    foreach (Collider collider in colliders)
                    {
                        // IDestructible destructibleComponent = hitInfo.collider.gameObject.GetComponent<IDestructible>();
                        IDestructible destructibleComponent = collider.gameObject.GetComponent<IDestructible>();
                        if (destructibleComponent != null)
                        {
                            // This is a valid target (creature) if it hasn't been detected before.
                            if (!detectedObjects.Contains(collider.gameObject))
                            {
                                // Add the object to the detected set to prevent future detections.
                                detectedObjects.Add(collider.gameObject);
                        
                                Debug.Log("1Hit detected on layer: " + LayerMask.LayerToName(collider.gameObject.layer));
                                Debug.Log("1Hit name: " + collider.gameObject.name);
                        
                                // Deal damage or perform other actions on the creature.
                                // Call a method on the destructibleComponent, e.g., destructibleComponent.TakeDamage(damageAmount);
                        
                                // Set didDamage to true if you want to track if damage was dealt.
                                
                                // didDamage = true;
                            }
                        }
                        Character characterComponent = collider.gameObject.GetComponent<Character>();
                        if (characterComponent != null)
                        {
                            // This is a valid target (creature) if it hasn't been detected before.
                            if (!detectedObjects.Contains(collider.gameObject))
                            {
                                // Add the object to the detected set to prevent future detections.
                                detectedObjects.Add(collider.gameObject);
                        
                                Debug.Log("2Hit detected on layer: " + LayerMask.LayerToName(collider.gameObject.layer));
                                Debug.Log("2Hit name: " + collider.gameObject.name);
                        
                                // Deal damage or perform other actions on the creature.
                                // Call a method on the destructibleComponent, e.g., destructibleComponent.TakeDamage(damageAmount);
                        
                                // Set didDamage to true if you want to track if damage was dealt.
                                
                                // didDamage = true;
                            }
                        }
                        
                    }
                }
                else
                {
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_ColdBall_launch"), player.transform.position, player.transform.rotation, player.transform);
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldball_start"), player.transform.position, Quaternion.identity);
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldball_launch"), player.transform.position, Quaternion.identity);

                    // GameObject prefab = ZNetScene.instance.GetPrefab("dragon_ice_projectile");
                    ScheduleProjectiles(player, 6, 6, 45f);
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        
        private static void ScheduleProjectiles(Player player, int inputRows, int inputProjectilesPerRow, float inputConeAngle)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleProjectilesCoroutine(player, inputRows, inputProjectilesPerRow, inputConeAngle));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleProjectilesCoroutine(Player player, int rows, int projectilesPerRow, float initialConeAngle)
        {
            GameObject prefab = ZNetScene.instance.GetPrefab("dragon_ice_projectile");

            while (rows > 0)
            {
                if (projectilesPerRow > 0)
                {
                    Vector3 playerPosition = player.transform.position;
                    Vector3 forwardDirection = player.GetLookDir();
                    Vector3 upDirection = player.transform.up;
                    float totalProjectiles = rows;
                    float initialRows = 6.0f;
                    float verticalStrength = 0.75f;

                    float currentConeAngle = initialConeAngle * ((float)rows / initialRows);
                    float angleBetweenProjectiles = currentConeAngle / totalProjectiles;
                    float currentHorizontalAngle = -currentConeAngle / 2f + (projectilesPerRow) * angleBetweenProjectiles;
                    Quaternion horizontalRotation = Quaternion.AngleAxis(currentHorizontalAngle, upDirection);

                    Vector3 horizontalDirection = horizontalRotation * forwardDirection;
                    horizontalDirection.y -= ((initialRows - (float)rows) * 0.1f) * verticalStrength;
                    Vector3 direction = horizontalDirection;

                    Vector3 spawnPosition = playerPosition + upDirection * 1.6f + direction * 0.5f;
                    GameObject GO_DraconicFrostProjectile = UnityEngine.Object.Instantiate(prefab, spawnPosition, Quaternion.identity);

                    Projectile P_DraconicFrostProjectile = GO_DraconicFrostProjectile.GetComponent<Projectile>();
                    
                    P_DraconicFrostProjectile.name = "DraconicFrostProjectile";
                    P_DraconicFrostProjectile.m_respawnItemOnHit = false;
                    P_DraconicFrostProjectile.m_ttl = 60f;
                    P_DraconicFrostProjectile.m_gravity = 2.5f;
                    P_DraconicFrostProjectile.m_rayRadius = 0.5f;
                    P_DraconicFrostProjectile.m_aoe = 0f;
                    P_DraconicFrostProjectile.m_hitNoise = 100f;
                    P_DraconicFrostProjectile.m_owner = player;
                    
                    P_DraconicFrostProjectile.transform.localRotation = Quaternion.LookRotation(direction);
                    P_DraconicFrostProjectile.transform.localScale = Vector3.one;
                    
                    RaycastHit hitInfo;
                    Vector3 target = (!Physics.Raycast(spawnPosition, direction, out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (spawnPosition + direction * 1000f) : hitInfo.point;
                    
                    HitData hitData = new HitData();
                    hitData.m_damage.m_frost = UnityEngine.Random.Range(10f, 20f);
                    hitData.m_damage.m_pierce = UnityEngine.Random.Range(1f, 3f);
                    hitData.ApplyModifier(player.GetCurrentWeapon().GetDamage().GetTotalDamage() * LackingImaginationGlobal.c_moderDraconicFrostProjectile);
                    hitData.m_pushForce = 0.5f;
                    hitData.m_statusEffectHash = Player.s_statusEffectFreezing;
                    hitData.SetAttacker(player);
                    
                    Vector3 velocity = (target - spawnPosition).normalized * 25f;
                    P_DraconicFrostProjectile.Setup(player, velocity, -1f, hitData, null, null);

                    // Delay the next projectile creation
                    yield return new WaitForSeconds(shotDelay);

                    projectilesPerRow--;
                    
                    // ScheduleProjectiles(player, rows, projectilesPerRow, initialConeAngle);
                }
                else
                {
                    rows--;
                    projectilesPerRow = rows;
                    
                    // ScheduleProjectiles(player, rows, projectilesPerRow, initialConeAngle);
                }
            }
        }
    }



    [HarmonyPatch]
    public class xModerEssencePassive
    {
        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Moder_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_dragonqueen_essence") && hit.GetAttacker() != null)
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
                        hit.m_damage.m_frost += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_moderDraconicFrostPassive;
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(EnvMan), "IsCold")]
        public static class Moder_IsCold_Patch
        {
            public static void Postfix(ref bool __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_dragonqueen_essence") && !Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Calm"))
                {
                    __result = true;
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), "GetDamageModifiers")]
        public static class Moder_GetDamageModifiers_Patch
        {
            public static void Postfix(ref HitData.DamageModifiers __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_dragonqueen_essence") && !Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Calm"))
                {
                    __result.m_frost = HitData.DamageModifier.Normal;
                }
            }
        }
        
        [HarmonyPatch(typeof(Humanoid), "UseItem")]
        public static class Moder_UseItem_Patch
        {
            public static void Prefix(Humanoid __instance, ref Inventory inventory, ref ItemDrop.ItemData item, ref bool fromInventoryGui)
            {
                if (inventory == null)
                    inventory = __instance.m_inventory;
                if (!inventory.ContainsItem(item))
                    return;
                GameObject hoverObject = __instance.GetHoverObject();
                Hoverable componentInParent1 = (bool) (UnityEngine.Object) hoverObject ? hoverObject.GetComponentInParent<Hoverable>() : (Hoverable) null;
                if (componentInParent1 != null && !fromInventoryGui)
                {
                    Interactable componentInParent2 = hoverObject.GetComponentInParent<Interactable>();
                    if (componentInParent2 != null && componentInParent2.UseItem(__instance, item))
                    {
                        __instance.DoInteractAnimation(hoverObject.transform.position);
                        return;
                    }
                }
                if (!__instance.m_seman.HaveStatusEffect("SE_Calm"))
                {
                    if (item.m_shared.m_name == "$item_freezegland")
                    {
                        __instance.m_consumeItemEffects.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                        __instance.m_zanim.SetTrigger("eat");
                        inventory.RemoveItem(item.m_shared.m_name, 1);
                        __instance.m_seman.AddStatusEffect("SE_Calm".GetHashCode());
                        return;
                    }
                }
               
            }
        }
        
        
        
    }
    
   
}