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
using PieceManager;


namespace LackingImaginationV2
{

    public class xAbominationEssence
    {
        public static string Ability_Name = "Bane";
        
        public static Character Abom;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
            
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xAbominationCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                if(Abom != null)
                {
                    if(Abom.GetComponent<Tameable>() != null) Abom.GetComponent<Tameable>().UnSummon();
                }
                Abom = null;
                
                GameObject effect = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_firetreecut_dead_abomination"), player.transform.position, Quaternion.identity);
                foreach (Transform child in effect.transform)
                {
                    Transform TransformSystem = child.GetComponent<Transform>();
                    if (TransformSystem != null)
                    {
                        TransformSystem.localScale *= 0.3f;
                    }
                }
            
                Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * 3f;
                Vector3 randomPosition = player.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
                GameObject baby = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Abomination"), randomPosition, Quaternion.identity);
                baby.GetComponent<Humanoid>().m_name = "Bane";
                baby.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                baby.GetComponent<ZSyncTransform>().m_syncScale = true;
                baby.GetComponent<Transform>().localScale = 0.5f * Vector3.one;
                baby.GetComponent<FootStep>().m_effects.Clear();
                baby.GetComponent<Humanoid>().SetMaxHealth(baby.GetComponent<Humanoid>().GetMaxHealthBase() * LackingImaginationGlobal.c_abominationBaneAllyHealth);
                baby.GetComponent<Humanoid>().m_speed = LackingImaginationGlobal.c_abominationBaneAllySpeed;
                baby.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                baby.GetComponent<MonsterAI>().m_consumeItems = new List<ItemDrop>() {ZNetScene.instance.GetPrefab("Wood").GetComponent<ItemDrop>()};
                baby.GetComponent<MonsterAI>().m_consumeRange = 2f;
                baby.GetComponent<MonsterAI>().m_consumeSearchRange = 10f;
                baby.GetComponent<MonsterAI>().m_consumeSearchInterval = 10f;
                baby.AddComponent<Tameable>();
                // baby.GetComponent<Tameable>().m_startsTamed = true;
                baby.GetComponent<Tameable>().Tame();
                baby.GetComponent<Tameable>().m_commandable = true;
                baby.GetComponent<Tameable>().Command(player);
                baby.GetComponent<Tameable>().m_unSummonEffect = new EffectList()
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("vfx_firetreecut_dead_abomination"),
                            m_enabled = true,
                        }
                    }
                };
                baby.GetComponent<Tameable>().m_unsummonDistance = 100f;
                baby.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
                baby.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                foreach (CharacterDrop.Drop drop in baby.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
                // baby.GetComponent<Character>().SetLevel(3);
                Abom = (baby.GetComponent<Character>());

               
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        
    }

    [HarmonyPatch]
    public static class xAbominationEssencePassive
    {
        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        public static class Abomination_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_abomination_essence"))
                {
                    __result += LackingImaginationGlobal.c_abominationBaneArmor;
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Abomination_GetTotalFoodValue_Patch
        {
            public static void Postfix( ref float hp)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_abomination_essence") && EnvMan.instance.IsDay())
                {
                    hp -= hp * LackingImaginationGlobal.c_abominationBaneHealth;
                }
            }
        }
        [HarmonyPatch(typeof(Character), (nameof(Character.RPC_Damage)))]
        class Abomination_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_abomination_essence") && hit.GetAttacker() != null)
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
                    if ((UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null && attacker.m_name == "Bane")
                    {
                        hit.ApplyModifier(LackingImaginationGlobal.c_abominationBaneAllyAttack);
                    }
                }
            }
        }
        
    }
}