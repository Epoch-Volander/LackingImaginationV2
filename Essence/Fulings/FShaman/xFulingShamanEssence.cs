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

    public class xFulingShamanEssence
    {
        private static readonly int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Ritual";
        private static bool Shield_Pay;
        private static bool Attack_Pay;
        
        private static GameObject GO_RitualProjectile;        
        private static Projectile P_RitualProjectile;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                
                Shield_Pay = RitualPay(5);
                if (Shield_Pay)
                {
                    
                    int GatheredWealth = int.Parse(xFulingShamanEssencePassive.FulingShamanStats[1]);
                    if (GatheredWealth < LackingImaginationGlobal.c_fulingshamanRitualShieldGrowthCap)
                    {
                        GatheredWealth += 5;
                        GatheredWealth = (int)Math.Min(GatheredWealth, LackingImaginationGlobal.c_fulingshamanRitualShieldGrowthCap);
                        xFulingShamanEssencePassive.FulingShamanStats[1] = GatheredWealth.ToString();
                    }
                    
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xFulingShamanCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);
                
                    //Effects, animations, and sounds
                
                    //Lingering effects
                    SE_Ritual se_ritualshield =(SE_Ritual)ScriptableObject.CreateInstance(typeof(SE_Ritual));
                    se_ritualshield.m_absorbDamage = LackingImaginationGlobal.c_fulingshamanRitualShield + GatheredWealth;
                    se_ritualshield.m_absorbDamageWorldLevel = LackingImaginationGlobal.c_fulingshamanRitualShield + GatheredWealth;
                    se_ritualshield.m_ttl = SE_Ritual.m_baseTTL;
                
                    //Apply effects
                    player.GetSEMan().AddStatusEffect(se_ritualshield);
                    Shield_Pay = false;
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Requires Coin Sacrifice");
                }
            }
            else if(player.GetSEMan().HaveStatusEffect("SE_Ritual"))
            {
                if (!player.GetSEMan().HaveStatusEffect("RitualProjectileCooldown"))
                {
                    Attack_Pay = RitualPay(1);
                    if (Attack_Pay)
                    { 
                        int GatheredWealth = int.Parse(xFulingShamanEssencePassive.FulingShamanStats[1]);
                        if (GatheredWealth < LackingImaginationGlobal.c_fulingshamanRitualShieldGrowthCap)
                        {
                            GatheredWealth++;
                            GatheredWealth = (int)Math.Min(GatheredWealth, LackingImaginationGlobal.c_fulingshamanRitualShieldGrowthCap);
                            xFulingShamanEssencePassive.FulingShamanStats[1] = GatheredWealth.ToString();
                        } 
                        
                        Vector3 vector = player.transform.position + player.transform.up  * 1.5f + player.GetLookDir() * .5f;// player.GetLookDir() * 2f;
                        GameObject prefab = ZNetScene.instance.GetPrefab("GoblinShaman_projectile_fireball");
                        GO_RitualProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
                        P_RitualProjectile = GO_RitualProjectile.GetComponent<Projectile>();
                        P_RitualProjectile.name = "Ritual Attack";
                        P_RitualProjectile.m_respawnItemOnHit = false;
                        P_RitualProjectile.m_spawnOnHit = null;
                        P_RitualProjectile.m_ttl = 60f;
                        P_RitualProjectile.m_gravity = 5f;
                        P_RitualProjectile.m_rayRadius = .2f;
                        P_RitualProjectile.m_aoe = 3f;
                        P_RitualProjectile.m_owner = player;
                        P_RitualProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
                        P_RitualProjectile.transform.localScale = Vector3.one;
                    
                        RaycastHit hitInfo = default(RaycastHit);
                        Vector3 player_position = player.transform.position;
                        Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
                        HitData hitData = new HitData();
                        hitData.m_damage.m_fire = UnityEngine.Random.Range(1f , 2f);
                        hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f , 2f);
                        hitData.m_damage.m_spirit = UnityEngine.Random.Range(1f , 2f);
                        hitData.ApplyModifier(LackingImaginationGlobal.c_fulingshamanRitualProjectile);
                        hitData.m_pushForce = 2f;
                        hitData.SetAttacker(player);
                        Vector3 a = Vector3.MoveTowards(GO_RitualProjectile.transform.position, target, 1f);
                        P_RitualProjectile.Setup(player, (a - GO_RitualProjectile.transform.position) * 25f, -1f, hitData, null, null);
                        GO_RitualProjectile = null;
                   
                        RitualProjectileCooldown se_ritualprojectileCD =(RitualProjectileCooldown)ScriptableObject.CreateInstance(typeof(RitualProjectileCooldown));
                        se_ritualprojectileCD.m_ttl = RitualProjectileCooldown.m_baseTTL;
                        //Apply CD
                        player.GetSEMan().AddStatusEffect(se_ritualprojectileCD);
                       
                        Attack_Pay = false;
                    }
                    else
                    {
                        player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Requires Coin Sacrifice");
                    }
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Strike Gathering Power");
                }
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }

        public static bool RitualPay(int cost)
        {
            if (Player.m_localPlayer.m_inventory.CountItems("$item_coins") >= cost)
            {
                Player.m_localPlayer.m_inventory.RemoveItem("$item_coins", cost);
                return true;
            }
            return false;
        }
    }


    [HarmonyPatch]
    public static class xFulingShamanEssencePassive
    {
        public static List<string> FulingShamanStats = new List<string>(){"off", "0"};
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        class FulingShaman_GetTotalFoodValue_Patch
        {
            public static void Postfix(Player __instance, ref float eitr)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_goblinshaman_essence"))
                {
                    eitr += LackingImaginationGlobal.c_fulingshamanRitualPassiveEitr;
                    
                    if (FulingShamanStats[0] == "off")
                    {
                        __instance.m_maxCarryWeight -= LackingImaginationGlobal.c_fulingshamanRitualPassiveCarry;
                        FulingShamanStats[0] = "on";
                    }
                }
                else if (FulingShamanStats[0] == "on" && !EssenceItemData.equipedEssence.Contains("$item_goblinshaman_essence"))
                {
                    __instance.m_maxCarryWeight += LackingImaginationGlobal.c_fulingshamanRitualPassiveCarry;
                    FulingShamanStats[0] = "off";
                }
            }
        }
    }
}