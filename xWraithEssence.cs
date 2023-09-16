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

    public class xWraithEssence
    {
        
        public static string Ability_Name = "Twin Souls";// resist physical, weak elemental
        
        public static List<Character> Wraith = new List<Character>();
        
        public static GameObject Aura;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //in the day you do bonus spirit dmg, at night a wraith companion spawns and follows you
                LackingImaginationV2Plugin.Log($"Wraith Button was pressed");
                    
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xWriathCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_wraith_alerted"), player.transform.position, Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_wraith_death"), player.transform.position, Quaternion.identity);
                
                Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_TwinSouls, player.GetCenterPoint(), Quaternion.identity);
                
                Aura.transform.parent = player.transform;
                
                //Lingering effects
                SE_TwinSouls se_twinsouls = (SE_TwinSouls)ScriptableObject.CreateInstance(typeof(SE_TwinSouls));
                se_twinsouls.m_ttl = SE_TwinSouls.m_baseTTL;
            
                player.GetSEMan().AddStatusEffect(se_twinsouls);
                
                if (EnvMan.instance.IsNight() && player.IsBlocking())
                {
                    foreach (Character ch in Wraith)
                    {
                        if(ch == null) break;
                        ch.GetComponent<Tameable>().UnSummon();
                    }
                    Wraith.Clear();
                    
                    Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * 3f;
                    Vector3 randomPosition = player.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
                    GameObject wraith = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Wraith"), randomPosition, Quaternion.identity);
                    wraith.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                    wraith.GetComponent<Humanoid>().m_name = "Second(Ally)";
                    wraith.GetComponent<Humanoid>().SetMaxHealth(wraith.GetComponent<Humanoid>().GetMaxHealthBase() * 5f);
                    wraith.GetComponent<Humanoid>().m_speed = 3f;
                    wraith.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                    wraith.AddComponent<Tameable>();
                    // baby.GetComponent<Tameable>().m_startsTamed = true;
                    wraith.GetComponent<Tameable>().Tame();
                    wraith.GetComponent<Tameable>().m_unsummonDistance = 150f;
                    wraith.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
                    wraith.GetComponent<Tameable>().m_commandable = true;
                    wraith.GetComponent<Tameable>().m_unSummonEffect = new EffectList()
                    {
                        m_effectPrefabs = new EffectList.EffectData[]
                        {
                            new()
                            {
                                m_prefab = ZNetScene.instance.GetPrefab("vfx_wraith_death"),
                                m_enabled = true,
                            }
                        }
                    };
                    wraith.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                    foreach (CharacterDrop.Drop drop in wraith.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
                    Wraith.Add(wraith.GetComponent<Character>());
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        
    }

    [HarmonyPatch]
    public static class  xWraithEssencePassive
    {
        [HarmonyPatch(typeof(Character), "CustomFixedUpdate")]
        public static class Wraith_CustomFixedUpdate_Patch  
        {
            public static void Postfix(Character __instance)
            {
                if (__instance.m_name == "Second(Ally)" && (!EssenceItemData.equipedEssence.Contains("$item_wraith_essence") ||  EnvMan.instance.IsDay()))
                {
                    foreach (Character ch in xWraithEssence.Wraith)
                    {
                        if(ch == null) break;
                        ch.GetComponent<Tameable>().UnSummon();
                    }
                    xWraithEssence.Wraith.Clear();
                }
                if (xWraithEssence.Aura != null && __instance.IsPlayer() && !__instance.GetSEMan().HaveStatusEffect("SE_TwinSouls"))
                {
                    UnityEngine.GameObject.Destroy(xWraithEssence.Aura);
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), "GetBodyArmor")]
        public static class Wraith_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_wraith_essence"))
                {
                    __result -= LackingImaginationGlobal.c_wraithTwinSoulsArmor;
                    if (__result < 0)
                    {
                        __result = 0;
                    }
                }
            }
        }
        
         [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Wraith_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_wraith_essence") && hit.GetAttacker() != null)
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
                    if (EnvMan.instance.IsDay() && (UnityEngine.Object) __instance.m_baseAI != (UnityEngine.Object) null && (bool) (UnityEngine.Object) attacker && attacker.IsPlayer())
                    {
                        hit.m_damage.m_spirit += (Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_wraithTwinSoulsPassive;;
                    }
                }
            }
        }
    }
}