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

    public class xUlvEssence
    {
        public static string Ability_Name = "Territorial \nSlumber";
        
        public static List<Character> Marked = new List<Character>();
        
        public static HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
        
        private static readonly int collisionMask = LayerMask.GetMask( "character", "character_noenv", "character_net", "character_ghost");
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xUlvCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                LackingImaginationV2Plugin.fx_TerritorialSlumber.GetComponent<TimedDestruction>().m_timeout  = (LackingImaginationUtilities.xUlvCooldownTime * LackingImaginationGlobal.c_ulvTerritorialSlumberSED) + ((float)SE_TerritorialSlumber.Comfort * 0.5f);
                GameObject Ring = UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_TerritorialSlumber, player.transform.position + player.transform.up * 0.5f, Quaternion.identity);
                // make circles higher, coroutine while / summon ulv pal on kill in circle/ stats scale with Comfort

                ScheduleRing(player, Ring);
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        private static void ScheduleRing(Player player, GameObject ring)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleRingCoroutine(player,ring));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleRingCoroutine(Player player, GameObject ring)
        {
            while (ring != null && player != null && player.m_nview.IsOwner())
            {
                float size = 9f;
                Collider[] colliders = Physics.OverlapSphere(ring.transform.position, size, collisionMask);
                HashSet<GameObject> hashSet = new HashSet<GameObject>();
                
                foreach (Collider collider in colliders)
                {
                    Character characterComponent = collider.gameObject.GetComponent<Character>();
                    if (characterComponent != null)
                    {
                        hashSet.Add(collider.gameObject);
                        detectedObjects.Add(collider.gameObject);
                    }
                }
                detectedObjects.IntersectWith(hashSet);
                
                yield return null;
            }
            detectedObjects.Clear();
        }
    }

    [HarmonyPatch]
    public static class xUlvEssencePassive
    {
        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class Ulv_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (xUlvEssence.detectedObjects.Any() && __instance.IsPlayer() && __instance == Player.m_localPlayer)
                {
                     List<GameObject> dead = new List<GameObject>();
                     foreach (GameObject gameObject in xUlvEssence.detectedObjects)
                     {
                         Character ch = gameObject.GetComponent<Character>();
                         if ((ch.IsDead() || (double)ch.GetHealth() <= 0.0) && ch.m_name != "Ulv(Ally)")
                         {
                             // LackingImaginationV2Plugin.Log($"{ch.m_name} died");
                             SummonUlv(ch.transform.position);
                             dead.Add(gameObject);
                         }
                     }
                     foreach (GameObject itemToRemove in dead)
                     {
                         if(xUlvEssence.detectedObjects.Contains(itemToRemove))
                         {
                             xUlvEssence.detectedObjects.Remove(itemToRemove);
                         }
                     }
                     dead.Clear();
                }
            }
            static void SummonUlv(Vector3 position)
            {
                GameObject Ulv = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Ulv"), position, Quaternion.identity);
                Ulv.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                Ulv.GetComponent<Humanoid>().m_name = "Ulv(Ally)";
                Ulv.GetComponent<ZSyncTransform>().m_syncScale = true;
                Ulv.GetComponent<Transform>().localScale = 0.8f * Vector3.one;
                Ulv.GetComponent<Humanoid>().SetMaxHealth(Ulv.GetComponent<Humanoid>().GetMaxHealthBase() * LackingImaginationGlobal.c_ulvTerritorialSlumberSummonHealth);
                Ulv.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                Ulv.AddComponent<Tameable>();
                Ulv.GetComponent<Tameable>().Tame();
                Ulv.GetComponent<Tameable>().m_unsummonDistance = 100f;
                Ulv.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
                CharacterDrop characterDrop = Ulv.GetComponent<CharacterDrop>();
                if (characterDrop != null)  characterDrop.m_dropsEnabled = false;
                foreach (CharacterDrop.Drop drop in characterDrop.m_drops) drop.m_chance = 0f;
            
                SE_TimedDeath se_timedeath = (SE_TimedDeath)ScriptableObject.CreateInstance(typeof(SE_TimedDeath));
                se_timedeath.lifeDuration = LackingImaginationGlobal.c_ulvTerritorialSlumberSummonDuration;
                se_timedeath.m_ttl = LackingImaginationGlobal.c_ulvTerritorialSlumberSummonDuration + 500f;
            
                Ulv.GetComponent<Character>().GetSEMan().AddStatusEffect(se_timedeath);
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Ulv_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ulv_essence"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_TerritorialSlumber"))
                    {
                        __instance.GetSEMan().AddStatusEffect("SE_TerritorialSlumber".GetStableHashCode());
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_TerritorialSlumber"))
                {
                    SE_TerritorialSlumber.Comfort = 0;
                    __instance.GetSEMan().RemoveStatusEffect("SE_TerritorialSlumber".GetStableHashCode());
                }
            }
        }
        [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.CalculateComfortLevel), new Type[] {typeof(Player)})]
        public static class Ulv_CalculateComfortLevel_Patch
        {
            public static void Postfix(SE_Rested __instance, ref int __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ulv_essence"))
                {
                     __result += (int)LackingImaginationGlobal.c_ulvTerritorialSlumberComfort;
                }
            }
        }
        [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.UpdateTTL))]
        public static class Ulv_UpdateTTL_Patch
        {
            public static void Prefix(SE_Rested __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ulv_essence"))
                {
                    __instance.m_TTLPerComfortLevel = 30f;
                }
                else if (__instance.m_TTLPerComfortLevel == 30f)
                {
                    __instance.m_TTLPerComfortLevel = 60f;
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Ulv_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix(ref float stamina, Player __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_ULV_essence"))
                {
                    stamina *= (1f + ((float)SE_TerritorialSlumber.Comfort * LackingImaginationGlobal.c_ulvTerritorialSlumberStamina));
                }
            }
        }
        
        
        
        
    }
}