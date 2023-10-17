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

    public class xLoxEssence
    {
        public static string Ability_Name = "Wild \nTremor";
        
        private static float Delay = 0.3f;
        
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xLoxCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_lox_death"), player.transform.position, Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_lox_attack_stomp"), player.transform.position,Quaternion.identity);
                
                ScheduleStomp(player);
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        private static void ScheduleStomp(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleStompCoroutine(player));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleStompCoroutine(Player player)
        {
            yield return new WaitForSeconds(Delay);
            
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_lox_groundslam"), player.transform.position,Quaternion.identity);
                
            HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

            Vector3 capsuleCenter = player.transform.position;
            float capsuleRadius = 10f; // Radius of the capsule

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
                        hitData.m_damage.m_blunt = UnityEngine.Random.Range(10f, 15f);
                        hitData.m_dir = collider.transform.position - player.transform.position;
                        hitData.m_dodgeable = true;
                        hitData.m_blockable = true;
                        hitData.m_hitCollider = collider;
                        hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_loxWildTremor));
                        hitData.m_pushForce = 100f;
                        hitData.m_point = collider.transform.position;
                        hitData.SetAttacker(player);
                        
                        destructibleComponent.Damage(hitData);
                    }
                }
            }
        }
    }


    [HarmonyPatch]
    public static class xLoxEssencePassive
    {

        public static List<string> LoxEaten = new List<string>();

        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        class Lox_GetTotalFoodValue_Patch
        {
            public static void Prefix(Player __instance, ref float hp)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_lox_essence"))
                {
                    foreach (Player.Food food in __instance.m_foods)
                    {
                        // int position = __instance.m_foods.IndexOf(food);
                        if (LoxEaten.Contains(food.m_item.m_shared.m_name) )
                        {
                            food.m_health += LackingImaginationGlobal.c_loxWildTremorPassive;
                        }
                    }
                }
                else
                {
                    foreach (Player.Food food in __instance.m_foods)
                    {
                        if (food.m_health == food.m_item.m_shared.m_food + LackingImaginationGlobal.c_loxWildTremorPassive)
                        {
                             hp -= LackingImaginationGlobal.c_loxWildTremorPassive;
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.EatFood))] 
        class Lox_EatFood_Patch
        {
            static void Postfix(Player __instance, ref ItemDrop.ItemData item)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_lox_essence"))
                {
                    foreach (Player.Food food in __instance.m_foods)
                    {
                        if (food.m_item.m_shared.m_name == item.m_shared.m_name)
                        {
                            LoxEaten.Add(food.m_item.m_shared.m_name);
                            food.m_time *= 0.75f; // Multiply the time by 0.75
                            __instance.UpdateFood(0.0f, true);
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateFood))]
        class Lox_UpdateFood_Patch
        {
            static void Prefix(Player __instance)
            {
                foreach (Player.Food food in __instance.m_foods)
                {
                    if ((double) food.m_time <= 0.0)
                    {
                        LoxEaten.Remove(food.m_item.m_shared.m_name);
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.ClearFood))]
        class Lox_ClearFood_Patch
        {
            static void Postfix(Player __instance)
            {
                LoxEaten.Clear();
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.RemoveOneFood))]
        class Lox_RemoveOneFood_Patch
        {
            static void Prefix(Player __instance)
            {
                if (__instance.m_foods.Count > 0)
                {
                    int indexToRemove = UnityEngine.Random.Range(0, __instance.m_foods.Count);
                    string removedFood = __instance.m_foods[indexToRemove].m_item.m_shared.m_name;
                    LoxEaten.RemoveAt(indexToRemove);
                }
            }
        }
        
        
    }

    
    
    

}