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
                    GameObject Breath = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_dragon_coldbreath"), player.transform.position, Quaternion.identity);
                    
                    
                    
                }
                else
                {
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldball_start"), player.transform.position, player.transform.rotation, player.transform);
                    GameObject prefab = ZNetScene.instance.GetPrefab("dragon_ice_projectile");
                    ScheduleProjectile(player, prefab, 6, 6, 45f);
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        private static void ScheduleProjectile(Player player, GameObject prefab, int rows, int projectilesPerRow, float initialConeAngle)
        {
            if (rows > 0)
            {
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_ColdBall_launch"), player.transform.position, player.transform.rotation, player.transform);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_dragon_coldball_launch"), player.transform.position, player.transform.rotation, player.transform);
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
                    
                    System.Threading.Timer timer = new System.Threading.Timer
                    ((_) => { ScheduleProjectile(player, prefab, rows, projectilesPerRow, initialConeAngle); }, null, (int)(shotDelay * 1000), System.Threading.Timeout.Infinite);
                }
                else
                {
                    rows--;
                    projectilesPerRow = rows;
                    
                    System.Threading.Timer timer = new System.Threading.Timer
                        ((_) => { ScheduleProjectile(player, prefab, rows, projectilesPerRow, initialConeAngle); }, null, (int)(shotDelay * 1000), System.Threading.Timeout.Infinite);
                }
            }
        }
        
     
    }



    [HarmonyPatch]
    public class xModerEssencePassive
    {
        
        
        
        
        
    }
}