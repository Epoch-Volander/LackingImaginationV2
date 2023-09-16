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

    public class xTrollEssence //A way to throw a player if close enough? think about it
    {
        private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Troll Toss";
        
        private static GameObject GO_TrollTossProjectile;        
        private static Projectile P_TrollTossProjectile;
        
        private static float shotDelay = 0.5f;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"xTrollEssence Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xTrollCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                 UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_troll_idle"), player.transform.position, Quaternion.identity);
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("throw_bomb"); 
                //Throw_1
                // troll_throw_projectile

                Vector3 vector = player.transform.position + player.transform.up  * 2f + player.GetLookDir() * .5f;
                GameObject prefab = ZNetScene.instance.GetPrefab("troll_throw_projectile");
                player.transform.rotation = Quaternion.LookRotation(player.GetLookDir()); // test
                System.Threading.Timer timer = new System.Threading.Timer
                    (_ => { ScheduleProjectile(player, vector, prefab); }, null, (int)(shotDelay * 1000), System.Threading.Timeout.Infinite);
                
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        //passive heavy, throw rock that does dmg based on hp and/or armor

        private static void ScheduleProjectile(Player player, Vector3 vector, GameObject prefab)
        {
            GO_TrollTossProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
            P_TrollTossProjectile = GO_TrollTossProjectile.GetComponent<Projectile>();
            P_TrollTossProjectile.name = "Boulder Attack";
            P_TrollTossProjectile.m_respawnItemOnHit = false;
            P_TrollTossProjectile.m_spawnOnHit = null;
            P_TrollTossProjectile.m_ttl = 60f;
            P_TrollTossProjectile.m_gravity = 20f;
            P_TrollTossProjectile.m_rayRadius = 0.5f;
            P_TrollTossProjectile.m_aoe = 3.5f;
            P_TrollTossProjectile.m_canHitWater = true;
            P_TrollTossProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
            P_TrollTossProjectile.transform.localScale = Vector3.one;
            
            RaycastHit hitInfo = default(RaycastHit);
            Vector3 player_position = player.transform.position;
            Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
            HitData hitData = new HitData();
            hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f , 2f);
            hitData.m_damage.m_chop = UnityEngine.Random.Range(1f , 2f);
            hitData.ApplyModifier(player.GetMaxHealth() * LackingImaginationGlobal.c_trollTrollTossProjectile);
            hitData.m_pushForce = 4f;
            hitData.SetAttacker(player);
            Vector3 a = Vector3.MoveTowards(GO_TrollTossProjectile.transform.position, target, 1f);
            P_TrollTossProjectile.Setup(player, (a - GO_TrollTossProjectile.transform.position) * 25f, -1f, hitData, null, null);
            GO_TrollTossProjectile = null;
            
        }
    }
    
    [HarmonyPatch]
    public static class xTrollEssencePassive
    {
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        public static class Troll_GetTotalFoodValue_Patch
        {
            public static void Postfix( ref float hp)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_troll_essence"))
                {
                    hp += LackingImaginationGlobal.c_trollTrollTossPassive;
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Troll_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_troll_essence") && hit.GetAttacker() != null)
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
                        Player.m_localPlayer.m_damageModifiers.m_pierce = HitData.DamageModifier.VeryWeak;
                    }
                }
            }
        }
        
        
        
        
        
    }
}