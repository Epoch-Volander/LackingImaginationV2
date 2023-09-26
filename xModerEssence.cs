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

                if (player.IsBlocking())
                {
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldbreath_start"), player.transform.position, Quaternion.identity);
                    GameObject Breath = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_dragon_coldbreath"), player.GetCenterPoint(), player.transform.rotation, player.transform);
                    
                    
                    float rayLength = 30f; // Set the desired ray length.
                    float minDistance = 2f; // Set the minimum distance to truncate the cone.
                    float coneAngle = 10f; // Set the cone angle in degrees.
                    int numRays = 360; // Number of rays to cast in the cone.

                    
                    // Create a HashSet to keep track of detected objects.
                    HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
                    
                    // Loop through the cone of rays.
                    for (int i = 0; i < numRays; i++)
                    {
                        // Calculate the direction of the current ray based on the angle.
                        Vector3 rayDirection = Quaternion.Euler(0, i * (360f / numRays), 0) * Breath.transform.forward;

                        // // Create the ray.
                        // Ray ray = new Ray(Breath.transform.position, rayDirection);
                        //
                        // // Adjust the ray's starting point slightly to prevent self-intersections.
                        // ray.origin += rayDirection * 0.1f;
                        
                        // Create the ray.
                        Ray ray = new Ray(Breath.transform.position - rayDirection * 2f, rayDirection); // Subtract 2f from the starting position.

                        // Initialize the hitInfo.
                        RaycastHit hitInfo = default(RaycastHit);

                        // Perform the raycast.
                        if (Physics.Raycast(ray, out hitInfo, rayLength, Script_Projectile_Layermask))
                        {
                            // Calculate the distance from the ray's origin to the hit point.
                            float distanceToHit = hitInfo.distance;

                            // Check if the hit point is closer than the minimum distance.
                            if (distanceToHit > minDistance)
                            {
                                if (hitInfo.collider.gameObject != null)
                                {
                                    // Debug.Log("1Hit detected on layer: " + LayerMask.LayerToName(hitInfo.collider.gameObject.layer));
                                    // Debug.Log("1Hit name: " + hitInfo.collider.gameObject.name);
                                    
                                    // Check if the hit object has a component that implements the IDestructible interface.
                                    // IDestructible destructibleComponent = hitInfo.collider.gameObject.GetComponent<IDestructible>();
                                    //
                                    // if (destructibleComponent != null)
                                    // {
                                    if (detectedObjects.Any())
                                    {
                                        Debug.Log("why is it not empty");
                                    }
                                        // This is a valid target (creature) if it hasn't been detected before.
                                        if (!detectedObjects.Contains(hitInfo.collider.gameObject))
                                        {
                                            // Add the object to the detected set to prevent future detections.
                                            detectedObjects.Add(hitInfo.collider.gameObject);

                                            Debug.Log("1Hit detected on layer: " + LayerMask.LayerToName(hitInfo.collider.gameObject.layer));
                                            Debug.Log("1Hit name: " + hitInfo.collider.gameObject.name);

                                            // Deal damage or perform other actions on the creature.
                                            // Call a method on the destructibleComponent, e.g., destructibleComponent.TakeDamage(damageAmount);

                                            // Set didDamage to true if you want to track if damage was dealt.
                                            
                                            // didDamage = true;
                                        }
                                    // }
                                    
                                }
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
                    ScheduleProjectile(player, 6, 6, 45f);
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        private static void ScheduleProjectile(Player player, int inputRows, int inputProjectilesPerRow, float inputConeAngle)
        {
            GameObject prefab = ZNetScene.instance.GetPrefab("dragon_ice_projectile");
            int rows = inputRows;
            int projectilesPerRow = inputProjectilesPerRow;
            float initialConeAngle = inputConeAngle;
            
            if (rows > 0)
            { 
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldball_launch"), player.transform.position, Quaternion.identity);
                if (projectilesPerRow > 0)
                {
                    Vector3 playerPosition = player.transform.position;
                    Vector3 forwardDirection = player.GetLookDir();
                    Vector3 upDirection = player.transform.up;
                    float totalProjectiles = rows;
                    float initialRows = 6.0f;
                    float verticalStrength = 0.75f;
                    
                    
                    float currentConeAngle = initialConeAngle * ((float)rows / initialRows);
                    // Calculate the angle between each projectile
                    float angleBetweenProjectiles = currentConeAngle / totalProjectiles;
                    
                    // Calculate the current horizontal angle for the current projectile
                    float currentHorizontalAngle = -currentConeAngle / 2f + (projectilesPerRow) * angleBetweenProjectiles;
                    Quaternion horizontalRotation = Quaternion.AngleAxis(currentHorizontalAngle, upDirection);

                    Vector3 horizontalDirection = horizontalRotation * forwardDirection;
                    horizontalDirection.y -= ((initialRows - (float)rows) * 0.1f) * verticalStrength;
                    Vector3 direction = horizontalDirection;
                    
                    // LackingImaginationV2Plugin.Log($"direction {direction}");
                    
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
                    hitData.m_damage.m_frost = UnityEngine.Random.Range(2f, 4f);
                    hitData.m_damage.m_spirit = UnityEngine.Random.Range(1f, 2f);
                    hitData.ApplyModifier(player.GetCurrentWeapon().GetDamage().GetTotalDamage() * LackingImaginationGlobal.c_moderDraconicFrostProjectile);
                    hitData.m_pushForce = 0.5f;
                    hitData.m_statusEffectHash = Player.s_statusEffectFreezing;
                    hitData.SetAttacker(player);

                    Vector3 velocity = (target - spawnPosition).normalized * 25f;
                    P_DraconicFrostProjectile.Setup(player, velocity, -1f, hitData, null, null);
                    
                    projectilesPerRow--;
                    
                    // System.Threading.Timer timer = new System.Threading.Timer
                    // ((_) => { ScheduleProjectile(player, rows, projectilesPerRow, initialConeAngle); }, null, (int)(shotDelay * 1000), System.Threading.Timeout.Infinite);
                    ScheduleProjectile(player, rows, projectilesPerRow, initialConeAngle);
                }
                else
                {
                    rows--;
                    projectilesPerRow = rows;
                    
                    // System.Threading.Timer timer = new System.Threading.Timer
                    //     ((_) => { ScheduleProjectile(player, rows, projectilesPerRow, initialConeAngle); }, null, (int)(shotDelay * 1000), System.Threading.Timeout.Infinite);
                    ScheduleProjectile(player, rows, projectilesPerRow, initialConeAngle);
                }
            }
        }
        
     
    }



    [HarmonyPatch]
    public class xModerEssencePassive
    {
        
        
        
        
        
    }
}