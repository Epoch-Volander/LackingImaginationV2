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
    
    [HarmonyPatch]
    public class xDraugrRot
    {
        public static List<string> RotStats = new List<string>() { "0" };

        public static float startHP;
        
        
        [HarmonyPatch(typeof(Humanoid), "UseItem")]
        public static class Rot_UseItem_Patch
        {
            public static void Prefix(Humanoid __instance, ref Inventory inventory, ref ItemDrop.ItemData item, ref bool fromInventoryGui)
            {
                if (inventory == null)
                    inventory = __instance.m_inventory;
                if (!inventory.ContainsItem(item))
                    return;
                GameObject hoverObject = __instance.GetHoverObject();
                Hoverable componentInParent1 = (bool) (UnityEngine.Object) hoverObject ? hoverObject.GetComponentInParent<Hoverable>() : (Hoverable) null;
                if (componentInParent1 != null && !fromInventoryGui)
                {
                    Interactable componentInParent2 = hoverObject.GetComponentInParent<Interactable>();
                    if (componentInParent2 != null && componentInParent2.UseItem(__instance, item))
                    {
                        __instance.DoInteractAnimation(hoverObject.transform.position);
                        return;
                    }
                }
                if (item.m_shared.m_name == "$item_entrails" && __instance.m_seman.HaveStatusEffect("SE_Rot") && xDraugrRot.RotStats[0] != "0")
                {
                    __instance.m_consumeItemEffects.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                    __instance.m_zanim.SetTrigger("eat");
                    Player.m_localPlayer.m_inventory.RemoveItem("$item_entrails", 1);
                    RotStats[0] = (float.Parse(RotStats[0]) - 10f).ToString();
                    if (float.Parse(RotStats[0]) < 0f) RotStats[0] = "0";
                    return;
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), "UpdateEnvStatusEffects")]
        public static class Rot_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                SE_Rot se_rot = (SE_Rot)ScriptableObject.CreateInstance(typeof(SE_Rot));
                if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence") || EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_Rot"))
                    {
                        __instance.GetSEMan().AddStatusEffect(se_rot);
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_Rot"))
                {
                    __instance.GetSEMan().RemoveStatusEffect(se_rot);
                }
                
            }
            
            [HarmonyPatch(typeof(Hud), "UpdateStatusEffects")]
            public static class Rot_UpdateStatusEffectsn_Patch
            {
                public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
                {
                    int text = (int)float.Parse(RotStats[0]);
                    string iconText = text.ToString();
                    for (int index = 0; index < statusEffects.Count; ++index)
                    {
                        StatusEffect statusEffect1 = statusEffects[index];
                        if (statusEffect1.name == "SE_Rot")
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

        
        
        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Rot_RPC_Damage_Patch
        {

            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (xDraugrRot.RotStats[0] != "100" && (EssenceItemData.equipedEssence.Contains("$item_draugr_essence") || EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence")) && hit.GetAttacker() != null)
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
                    if (__instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        startHP = __instance.GetHealth();
                        float multi = 1f;
                        if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence")) multi -= xDraugrEssencePassive.DraugrRot;
                        if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence")) multi -= xDraugrEliteEssencePassive.DraugrEliteRot;
                        
                        hit.ApplyModifier(multi);
                        
                    }
                }
            }
            
             public static void Postfix(Character __instance, ref HitData hit)
            {
                if (xDraugrRot.RotStats[0] != "100" && (EssenceItemData.equipedEssence.Contains("$item_draugr_essence") || EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence")) && hit.GetAttacker() != null)
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
                    if (__instance != null && __instance.IsPlayer() && (UnityEngine.Object) attacker.m_baseAI != (UnityEngine.Object) null)
                    {
                        float endHP = __instance.GetHealth();
                        float multi = 1f;
                        if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence")) multi -= xDraugrEssencePassive.DraugrRot;
                        if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence")) multi -= xDraugrEliteEssencePassive.DraugrEliteRot;
                        float reduce = (startHP - endHP);
                        reduce = (reduce /multi) - reduce;
                        float rot = (reduce / __instance.GetMaxHealth()) * 100;
                        // int rotInt = (int)rot;

                        RotStats[0] = (float.Parse(RotStats[0]) + rot).ToString();
                        if (float.Parse(RotStats[0]) > 100f) RotStats[0] = "100";

                    }
                }
            }
            
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        

    }
}