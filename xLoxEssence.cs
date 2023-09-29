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

    public class xLoxEssence
    {
        public static string Ability_Name = "Wild Tremor ";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Lox Button was pressed");
                
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xLoxCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_lox_death"), player.transform.position, Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_lox_groundslam"), player.transform.position,Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_lox_attack_stomp"), player.transform.position,Quaternion.identity);
                
                List<Character> allCharacters = new List<Character>();
                allCharacters.Clear();
                Character.GetCharactersInRange(player.transform.position, 10f, allCharacters);
                foreach (Character ch in allCharacters)
                {
                    if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer)) 
                        && !ch.m_tamed ||ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                    {
                        HitData hitData = new HitData();
                        hitData.m_damage.m_blunt = UnityEngine.Random.Range(10f, 15f);
                        hitData.m_dir = ch.transform.position - player.transform.position;
                        hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_loxWildTremor));
                        hitData.m_pushForce = 100f;
                        hitData.m_point = ch.transform.position;
                        hitData.SetAttacker(player);
                        ch.Damage(hitData);
                    }
                }
                
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
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