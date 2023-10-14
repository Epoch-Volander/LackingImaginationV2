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

    public class xGreydwarfEssence
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
        private static bool Stone_Pay;
        
        public static string Ability_Name = "Pebble";
        
        private static GameObject GO_GreydwarfPebbleProjectile;        
        private static Projectile P_GreydwarfPebbleProjectile;
        
        private static float shotDelay = 0.5f;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                Stone_Pay = StonePay();

                if (Stone_Pay)
                {
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xGreydwarfCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                    
                    //Effects, animations, and sounds
                     UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_greydwarf_death"), player.transform.position, Quaternion.identity);
                    ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("throw_bomb");
                    //Throw_1
                    // troll_throw_projectile

                    Vector3 vector = player.transform.position + player.transform.up * 1.8f + player.GetLookDir() * .5f;
                    GameObject prefab = ZNetScene.instance.GetPrefab("Greydwarf_throw_projectile");
                    player.transform.rotation = Quaternion.LookRotation(player.GetLookDir()); // test
                    
                    ScheduleProjectiles(player, vector, prefab);
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} is missing");
                }
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
            
            GO_GreydwarfPebbleProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
            P_GreydwarfPebbleProjectile = GO_GreydwarfPebbleProjectile.GetComponent<Projectile>();
            P_GreydwarfPebbleProjectile.name = "Boulder Attack";
            P_GreydwarfPebbleProjectile.m_respawnItemOnHit = false;
            P_GreydwarfPebbleProjectile.m_spawnOnHit = null;
            P_GreydwarfPebbleProjectile.m_ttl = 60f;
            P_GreydwarfPebbleProjectile.m_gravity = 10f;
            P_GreydwarfPebbleProjectile.m_rayRadius = 0.5f;
            P_GreydwarfPebbleProjectile.m_aoe = 0f;
            P_GreydwarfPebbleProjectile.m_owner = player;
            P_GreydwarfPebbleProjectile.m_canHitWater = true;
            P_GreydwarfPebbleProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
            P_GreydwarfPebbleProjectile.transform.localScale = Vector3.one;
            
            RaycastHit hitInfo = default(RaycastHit);
            Vector3 player_position = player.transform.position;
            Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
            HitData hitData = new HitData();
            hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f , 2f);
            hitData.ApplyModifier(LackingImaginationGlobal.c_greydwarfPebbleProjectile);
            hitData.SetAttacker(player);
            Vector3 a = Vector3.MoveTowards(GO_GreydwarfPebbleProjectile.transform.position, target, 1f);
            P_GreydwarfPebbleProjectile.Setup(player, (a - GO_GreydwarfPebbleProjectile.transform.position) * 25f, -1f, hitData, null, null);
            GO_GreydwarfPebbleProjectile = null;
            
        }
        
        static bool StonePay()
        {
            if (Player.m_localPlayer.m_inventory.CountItems("$item_stone") >= 1)
            {
                Player.m_localPlayer.m_inventory.RemoveItem("$item_stone", 1);
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch]
    public static class xGreydwarfEssencePassive
    {
        public static List<string> GreydwarfStats = new List<string>(){"off"};
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        class Greydwarf_GetTotalFoodValue_Patch
        {
            public static void Postfix(Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_greydwarf_essence"))
                {
                    if (GreydwarfStats[0] == "off")
                    {
                        __instance.m_maxCarryWeight += LackingImaginationGlobal.c_greydwarfPebblePassiveCarry;
                        GreydwarfStats[0] = "on";
                    }
                }
                else if (GreydwarfStats[0] == "on" && !EssenceItemData.equipedEssence.Contains("$item_greydwarf_essence"))
                {
                    __instance.m_maxCarryWeight -= LackingImaginationGlobal.c_greydwarfPebblePassiveCarry;
                    GreydwarfStats[0] = "off";
                }
            }
        }
    }
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateMovementModifier))]
    public static class Greydwarf_UpdateMovementModifier_Patch
    {
        public static void Postfix(Player __instance)
        {
            if (EssenceItemData.equipedEssence.Contains("$item_greydwarf_essence"))
            {
                __instance.m_equipmentMovementModifier += 0.05f;
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    public static class Greydwarf_RPC_Damage_Patch
    {
        public static void Prefix(Character __instance, ref HitData hit)
        {
            if (EssenceItemData.equipedEssence.Contains("$item_greydwarf_essence") && hit.GetAttacker() != null)
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
                if (__instance != null && __instance.IsPlayer() && attacker.m_faction == Character.Faction.ForestMonsters)
                {
                    hit.ApplyModifier(LackingImaginationGlobal.c_greydwarfPebbleForestAnger);
                }
            }
        }
    }
    
    
}