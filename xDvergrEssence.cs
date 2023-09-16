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
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace LackingImaginationV2
{

    public class xDvergrEssence
    {
        private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Randomize";
        
        private static GameObject GO_RandomizeFireProjectile;        
        private static Projectile P_RandomizeFireProjectile;
        private static GameObject GO_RandomizeIceProjectile;        
        private static Projectile P_RandomizeIceProjectile;
       
        public static bool Eitr_Pay;
        private static float shotDelay = 0.3f;
        private static int shotsFiredIce = 0;

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Dvergr Button was pressed");

                Eitr_Pay = RandomizePay();
                if (Eitr_Pay)
                {
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xDvergrCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    // randomly cast 1 of 3 abilities, fireball, ice shards, heal (faction targeted heal)
                
                    int RNDM = Random.Range(1, 9); // 1-8 inclusive
                   
                    if (RNDM == 1 || RNDM == 5 || RNDM == 8)//iceshards
                    {
                        //Effects, animations, and sounds
                        //DvergerStaffIce_projectile
                
                        // Vector3 vector = player.transform.position + player.transform.up  * 1.5f + player.GetLookDir() * .5f;
                        GameObject prefab = ZNetScene.instance.GetPrefab("DvergerStaffIce_projectile");
                    
                        int RNDMice = Random.Range(5, 11); // 1-10 inclusive
                        // Schedule the first projectile
                        ScheduleIceProjectile(player, prefab, RNDMice);
                    
                    }
                    if (RNDM == 2 || RNDM == 6 || RNDM == 7)//fireball
                    {
                        //Effects, animations, and sounds
                        //   DvergerStaffFire_fireball_projectile  big boy         ???  DvergerStaffFire_clusterbomb_projectile  ???

                        int RNDMfire = Random.Range(1, 7); // 1-6 inclusive

                        if (RNDMfire == 1 ||RNDMfire == 3 || RNDMfire == 5 || RNDMfire == 6)
                        {
                            Vector3 vector = player.transform.position + player.transform.up  * 1.5f + player.GetLookDir() * .5f;
                            GameObject prefab = ZNetScene.instance.GetPrefab("DvergerStaffFire_clusterbomb_projectile");
                            ScheduleFireProjectile(player, vector, prefab);
                        }
                        if (RNDMfire == 2 ||RNDMfire == 4 )
                        {
                            Vector3 vector = player.transform.position + player.transform.up  * 1.5f + player.GetLookDir() * .5f;
                            GameObject prefab = ZNetScene.instance.GetPrefab("DvergerStaffFire_fireball_projectile");
                            ScheduleFireProjectile(player, vector, prefab);
                        }
                    }
                    if (RNDM == 3 || RNDM == 4)// healAoe
                    {
                        //Effects, animations, and sounds
                        //DvergerStaffHeal_aoe
                        List<Character> allCharacters = new List<Character>();
                        allCharacters.Clear();
                        Character.GetCharactersInRange(player.GetCenterPoint(), 40f, allCharacters);
                        foreach (Character ch in allCharacters)
                        {
                            if (ch != null && ch.m_faction == Character.Faction.Players && ch.IsPlayer() )
                            {
                                LackingImaginationV2Plugin.Log($"{ch.name}");
                                Object.Instantiate(ZNetScene.instance.GetPrefab("DvergerStaffHeal_aoe"), ch.transform.position, Quaternion.identity);
                                ch.Heal(LackingImaginationGlobal.c_dvergrRandomizeHealPlayer);
                            }
                            if (ch.GetBaseAI() != null && ch.m_faction == Character.Faction.Players ||ch.GetBaseAI() != null && ch.m_tamed)
                            {
                                LackingImaginationV2Plugin.Log($"{ch.name}");
                                Object.Instantiate(ZNetScene.instance.GetPrefab("DvergerStaffHeal_aoe"), ch.transform.position, Quaternion.identity);
                                ch.Heal(ch.GetMaxHealth() * LackingImaginationGlobal.c_dvergrRandomizeHealCreature);
                            }
                        }
                    }
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Requires More Eitr");
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        static bool RandomizePay()
        {
            if (Player.m_localPlayer.GetEitr() >= LackingImaginationGlobal.c_dvergrRandomizeCost)
            {
                Player.m_localPlayer.UseEitr(LackingImaginationGlobal.c_dvergrRandomizeCost);
                return true;
            }
            return false;
        }
         private static void ScheduleIceProjectile(Player player, GameObject prefab, int RNDMice)
        {
            if (shotsFiredIce < RNDMice)
            {
                Vector3 vector = player.transform.position + player.transform.up  * 1.5f + player.GetLookDir() * .5f;
                // Create the projectile
                GO_RandomizeIceProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
                P_RandomizeIceProjectile = GO_RandomizeIceProjectile.GetComponent<Projectile>();
                P_RandomizeIceProjectile.name = "Dvergr Ice";
                P_RandomizeIceProjectile.m_respawnItemOnHit = false;
                P_RandomizeIceProjectile.m_spawnOnHit = null;
                P_RandomizeIceProjectile.m_ttl = 60f;
                P_RandomizeIceProjectile.m_gravity = 5f;
                P_RandomizeIceProjectile.m_rayRadius = .2f;
                P_RandomizeIceProjectile.m_aoe = 3f;
                P_RandomizeIceProjectile.m_hitNoise = 100f;
                P_RandomizeIceProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
                P_RandomizeIceProjectile.transform.localScale = Vector3.one;

                RaycastHit hitInfo = default(RaycastHit);
                Vector3 player_position = player.transform.position;
                Vector3 target =
                    (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
                target += UnityEngine.Random.insideUnitSphere * 3f;
                HitData hitData = new HitData();
                hitData.m_damage.m_frost = UnityEngine.Random.Range(2f, 4f);
                hitData.m_damage.m_pierce = UnityEngine.Random.Range(1f, 2f);
                hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_dvergrRandomizeIceProjectile));
                hitData.m_pushForce = 0.3f;
                hitData.SetAttacker(player);
                Vector3 a = Vector3.MoveTowards(GO_RandomizeIceProjectile.transform.position, target, 1f);
                P_RandomizeIceProjectile.Setup(player, (a - GO_RandomizeIceProjectile.transform.position) * 25f, -1f, hitData, null, null);
                
                // Increment the shots fired counter
                shotsFiredIce++;

                // Schedule the next projectile with a delay
                System.Threading.Timer timer = new System.Threading.Timer
                (_ => { ScheduleIceProjectile(player, prefab, RNDMice); }, null, (int)(shotDelay * 1000), System.Threading.Timeout.Infinite);
                
            }
            else
            {
                GO_RandomizeIceProjectile = null;
                shotsFiredIce = 0;
            }
        }
         private static void ScheduleFireProjectile(Player player, Vector3 vector, GameObject prefab)
        {
            // Create the projectile
            GO_RandomizeFireProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
            P_RandomizeFireProjectile = GO_RandomizeFireProjectile.GetComponent<Projectile>();
            P_RandomizeFireProjectile.name = "Dvergr Fire";
            P_RandomizeFireProjectile.m_respawnItemOnHit = false;
            // P_RandomizeFireProjectile.m_spawnOnHit = null;
            P_RandomizeFireProjectile.m_groundHitOnly = false;
            P_RandomizeFireProjectile.m_ttl = 60f;
            P_RandomizeFireProjectile.m_gravity = 5f;
            P_RandomizeFireProjectile.m_rayRadius = .3f;
            P_RandomizeFireProjectile.m_aoe = 3f;
            P_RandomizeFireProjectile.m_hitNoise = 100f;
            P_RandomizeFireProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
            P_RandomizeFireProjectile.transform.localScale = Vector3.one;

            RaycastHit hitInfo = default(RaycastHit);
            Vector3 player_position = player.transform.position;
            Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
            HitData hitData = new HitData();
            hitData.m_damage.m_fire = UnityEngine.Random.Range(2f, 4f);
            hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f, 2f);
            hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_dvergrRandomizeFireProjectile));
            hitData.m_pushForce = 3f;
            hitData.SetAttacker(player);
            Vector3 a = Vector3.MoveTowards(GO_RandomizeFireProjectile.transform.position, target, 1f);
            P_RandomizeFireProjectile.Setup(player, (a - GO_RandomizeFireProjectile.transform.position) * 25f, -1f, hitData, null, null);
            GO_RandomizeFireProjectile = null;
        }
    }

    
    
    [HarmonyPatch]
    public static class xDvergrEssencePassive
    {
        
        // bonus eitr, bonus crossbow dmg
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        public static class Dvergr_GetTotalFoodValue_Patch
        {
            public static void Postfix( ref float eitr)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_dvergr_essence"))
                {
                    eitr += LackingImaginationGlobal.c_dvergrRandomizePassiveEitr;
                }
            }
        }
        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        class Dvergr_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_dvergr_essence") && hit.GetAttacker() != null)
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
                        if (Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Crossbows)
                        {
                            hit.m_damage.m_pierce += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_dvergrRandomizePassive;
                        }
                    }
                }
            }
        }
        
        
        
        
        
        
        
        
    }
    
    
}