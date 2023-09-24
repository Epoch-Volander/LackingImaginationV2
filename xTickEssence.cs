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
using UnityEngine.UI;


namespace LackingImaginationV2
{

    public class xTickEssence
    {
        public static string Ability_Name = "Blood Well";
        
        public static GameObject Aura;
        public static bool Activated;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                if(xTickEssencePassive.TickStats[0] != "0")
                {
                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xSeaSerpentCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    LackingImaginationV2Plugin.Log($"Tick Button was pressed");
                    
                    Activated = true;
                    Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Longinus, player.GetCenterPoint(), Quaternion.identity);
                    Aura.transform.parent = player.transform;
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Is Empty");
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        


    }

    [HarmonyPatch]
    public class xTickEssencePassive
    {
        public static List<string> TickStats = new List<string>() {"0", };
        
        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Tick_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_tick_essence") && hit.GetAttacker() != null)
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
                        if (xTickEssence.Activated)
                        {
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_crit"), hit.m_point, Quaternion.identity);

                            
                            
                            hit.m_damage.m_slash += float.Parse(TickStats[0]);
                            TickStats[0] = "0";

                            xTickEssence.Activated = false;
                            UnityEngine.GameObject.Destroy(xTickEssence.Aura);
                        }
                        attacker.Heal(hit.GetTotalDamage() * LackingImaginationGlobal.c_tickBloodWellLifeSteal);
                        float blood = float.Parse(TickStats[0]) + (hit.GetTotalDamage() * LackingImaginationGlobal.c_tickBloodWellLifeSteal);
                        
                        if (blood > LackingImaginationGlobal.c_tickBloodWellStackCap) blood = LackingImaginationGlobal.c_tickBloodWellStackCap;
                        TickStats[0] = blood.ToString();
                    }
                }
            }
        }
        [HarmonyPatch(typeof(Player), "GetBodyArmor")]
        public static class Tick_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_tick_essence"))
                {
                    __result -= LackingImaginationGlobal.c_tickBloodWellArmor;
                    if (__result < 0.0) __result = 0.0f;
;                }
            }
        }
        
          [HarmonyPatch(typeof(Player), "UpdateEnvStatusEffects")]
        public static class Tick_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                SE_BloodWell se_bloodwell = (SE_BloodWell)ScriptableObject.CreateInstance(typeof(SE_BloodWell));
                if (EssenceItemData.equipedEssence.Contains("$item_tick_essence"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_BloodWell"))
                    {
                        __instance.GetSEMan().AddStatusEffect(se_bloodwell);
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_BloodWell"))
                {
                    __instance.GetSEMan().RemoveStatusEffect(se_bloodwell);
                    if (xTickEssence.Activated)
                    {
                        xTickEssence.Activated = false;
                        UnityEngine.GameObject.Destroy(xTickEssence.Aura);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Hud), "UpdateStatusEffects")]
        public static class Leech_UpdateStatusEffectsn_Patch
        {
            public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
            {
                string iconText = TickStats[0];
                for (int index = 0; index < statusEffects.Count; ++index)
                {
                    StatusEffect statusEffect1 = statusEffects[index];
                    if (statusEffect1.name == "SE_BloodWell")
                    {
                        RectTransform statusEffect2 = __instance.m_statusEffects[index];
                        Text component2 = statusEffect2.Find("TimeText").GetComponent<Text>();
                        if (!string.IsNullOrEmpty(iconText))
                        {
                            component2.gameObject.SetActive(value: true);
                            component2.text = iconText;
                        }
                        else
                        {
                            component2.gameObject.SetActive(value: false);
                        }
                    }
                }
            }
        }
        
        
    }
}