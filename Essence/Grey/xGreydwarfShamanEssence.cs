using System;
using System.CodeDom;
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

    public class xGreydwarfShamanEssence
    {
        public static string Ability_Name = "Dubious \nHeal";
        
        public static bool GreydwarfShamanController = false;
        private static float animDelay = 0.3f;
        private static float healDelay = 0.3f;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xGreydwarfShamanCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                player.SetCrouch(true);
                
                ScheduleHeal(player);
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        private static void ScheduleHeal(Player player)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleHealCoroutine(player));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleHealCoroutine(Player player)
        {
            yield return new WaitForSeconds(animDelay);
            
            LackingImaginationV2Plugin.UseGuardianPower = false;
            // GreydwarfShamanController = true;
            RPC_LI.AnimationCaller("Greydwarf", true);
            ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
            // GreydwarfShamanController = false;
            RPC_LI.AnimationCaller("Greydwarf", false);
            
            yield return new WaitForSeconds(healDelay);
            
            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_greydwarf_shaman_heal"), player.transform.position, Quaternion.identity);
            // simple heal (faction targeted heal)
            List<Character> allCharacters = new List<Character>();
            allCharacters.Clear();
            Character.GetCharactersInRange(player.GetCenterPoint(), 30f, allCharacters);
            foreach (Character ch in allCharacters)
            {
                if (ch != null && ch.m_faction == Character.Faction.Players && ch.IsPlayer())
                {
                    // LackingImaginationV2Plugin.Log($"{ch.name}");
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("shaman_heal_aoe"), ch.transform.position, Quaternion.identity);
                    ch.Heal(LackingImaginationGlobal.c_greydwarfshamanDubiousHealPlayer);
                }
                if (ch.GetBaseAI() != null && ch.m_faction == Character.Faction.Players || ch.GetBaseAI() != null && ch.m_tamed)
                {
                    // LackingImaginationV2Plugin.Log($"{ch.name}");
                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("shaman_heal_aoe"), ch.transform.position, Quaternion.identity);
                    ch.Heal(ch.GetMaxHealth() * LackingImaginationGlobal.c_greydwarfshamanDubiousHealCreature);
                }
            }
            player.SetCrouch(false);
        }
    }

    [HarmonyPatch]
    public static class xGreydwarfShamanEssencePassive
    {
        // regen?
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateFood))]
        public static class GreydwarfShaman_UpdateFood_Patch
        {
            public static void Postfix(Player __instance, ref float dt)//doubles regen
            {
                if (EssenceItemData.equipedEssence.Contains("$item_greydwarfshaman_essence"))
                {
                    if ((double) __instance.m_foodRegenTimer < (10.0 + dt))
                        return;
                    float num1 = 0.0f;
                    foreach (Player.Food food in __instance.m_foods)
                        num1 += food.m_item.m_shared.m_foodRegen;
                    if ((double) num1 <= 0.0)
                        return;
                    float regenMultiplier = 1f;
                    __instance.m_seman.ModifyHealthRegen(ref regenMultiplier);
                    __instance.Heal(num1 * regenMultiplier * LackingImaginationGlobal.c_greydwarfshamanDubiousHealPassive);
                }
            }
        }
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class GreydwarfShaman_GetTotalFoodValue_Patch
        {
            public static void Postfix( ref float eitr)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_greydwarfshaman_essence"))
                {
                    eitr += LackingImaginationGlobal.c_greydwarfshamanDubiousHealPassiveEitr;
                }
            }
        }
        //negative
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class GreydwarfShaman_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_greydwarfshaman_essence") && hit.GetAttacker() != null)
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if (__instance != null && __instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        int Dubious = UnityEngine.Random.Range(1, 21); // 1-20 inclusive
                        if (Dubious == 1)
                        {
                            hit.m_damage.m_poison += hit.GetTotalDamage() * 0.1f;
                        }
                    }
                }
            }
        }
    }
}