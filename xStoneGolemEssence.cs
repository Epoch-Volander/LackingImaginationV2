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
using TMPro;
using UnityEngine.UI;


namespace LackingImaginationV2
{

    public class xStoneGolemEssence
    {
        public static string Ability_Name = "Core \nOverdrive"; // heavy armor
        
        public static GameObject Aura;
        
        public static Animator prefabAnimator;

        private static bool able = false;
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xStoneGolemCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
            
                LackingImaginationV2Plugin.Log($"xStoneGolemEssence Button was pressed");
            
                // Player.m_localPlayer.m_inventory.AddItem(ZNetScene.instance.GetPrefab("TrophyBoar").GetComponent<ItemDrop>().m_itemData);
                // GameObject clubs = ZNetScene.instance.GetPrefab("StoneGolem_clubs");
                // clubs.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                // ItemDrop.ItemData spikes = clubs.GetComponent<ItemDrop>().m_itemData;
                // spikes.m_shared.m_name = "$item_spike";
                // spikes.m_shared.m_description = "$item_spike_description";
                // spikes.m_shared.m_icons = ZNetScene.instance.GetPrefab("Torch").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons;
                // spikes.m_shared.m_itemType = ItemDrop.ItemData.ItemType.TwoHandedWeapon;
                // spikes.m_shared.m_toolTier = 1;
                // spikes.m_shared.m_damages.m_blunt = 20f;
                // spikes.m_shared.m_dodgeable = true;
                // spikes.m_shared.m_blockable = true;
                // spikes.m_shared.m_attack.m_attackAnimation = "swing_longsword";
                // spikes.m_shared.m_secondaryAttack.m_attackAnimation = "sword_secondary";
                // spikes.m_shared.m_useDurability = true;
                // spikes.m_shared.m_destroyBroken = false;
                // clubs.GetComponent<ItemDrop>().m_itemData = spikes;
                //
                // Player.m_localPlayer.m_inventory.AddItem(clubs.GetComponent<ItemDrop>().m_itemData);



                if(able)
                {
                    Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.StoneGolem_Player, player.GetCenterPoint() - player.transform.up * 2f, Quaternion.identity);
                    Aura.transform.parent = player.transform;

                    prefabAnimator = Aura.transform.Find("Visual").GetComponent<Animator>();
                    //
                    // LackingImaginationV2Plugin.Log($"xbefore");
                    //
                    prefabAnimator.SetBool("sleeping", false);
                    //
                    prefabAnimator.SetTrigger("Movement");
                    //
                    // prefabAnimator.SetTrigger("attack3");
                    //
                    // LackingImaginationV2Plugin.Log($"xafterd");
                }
            
            
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        //vfx_stonegolem_hurt
        //sfx_stonegolem_hurt

    }

    [HarmonyPatch]
    public class xStoneGolemEssencePassive
    {
        public static List<string> StoneGolemStats = new List<string>() { "0", };
        
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class StoneGolem_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (__instance.GetSEMan().HaveStatusEffect("SE_GolemCore") && !EssenceItemData.equipedEssence.Contains("$item_stonegolem_essence"))
                {
                    __instance.GetSEMan().RemoveStatusEffect("SE_GolemCore".GetStableHashCode());
                }
            }
        }
        
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UseItem))]
        public static class StoneGolem_UseItem_Patch
        {
            public static bool Prefix(Humanoid __instance, ref Inventory inventory, ref ItemDrop.ItemData item, ref bool fromInventoryGui)
            {
                if (inventory == null)
                    inventory = __instance.m_inventory;
                if (!inventory.ContainsItem(item))
                    return true;
                if (!__instance.m_seman.HaveStatusEffect("SE_GolemCore") && EssenceItemData.equipedEssence.Contains("$item_stonegolem_essence"))
                {
                    if (item.m_shared.m_name == "$item_crystal")
                    {
                        __instance.m_consumeItemEffects.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                        __instance.m_zanim.SetTrigger("eat");
                        inventory.RemoveItem(item.m_shared.m_name, 1);
                        __instance.m_seman.AddStatusEffect("SE_GolemCore".GetHashCode());
                        return false;
                    }
                }
                if (__instance.m_seman.HaveStatusEffect("SE_GolemCore") && EssenceItemData.equipedEssence.Contains("$item_stonegolem_essence"))
                {
                    if (item.m_shared.m_name == "$item_stone" && int.Parse(StoneGolemStats[0]) != (int)LackingImaginationGlobal.c_stonegolemCoreOverdriveStacks)
                    {
                        int stone = inventory.CountItems("$item_stone");
                        int stoneRemain = (int)LackingImaginationGlobal.c_stonegolemCoreOverdriveStacks - int.Parse(StoneGolemStats[0]);

                        int use = (stone <= stoneRemain)? stone : stoneRemain;
                        if (stoneRemain >= 10 && stone >= 10) use = 10;
                        
                        __instance.m_equipEffects.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                        // __instance.m_consumeItemEffects.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                        __instance.m_zanim.SetTrigger("equip_hip");
                        inventory.RemoveItem(item.m_shared.m_name, use);
                        StoneGolemStats[0] = (int.Parse(StoneGolemStats[0]) + use).ToString();
                        
                        return false;
                    }
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateStatusEffects))]
        public static class StoneGolem_UpdateStatusEffects_Patch
        {
            public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
            {
                string iconText = StoneGolemStats[0];
                for (int index = 0; index < statusEffects.Count; ++index)
                {
                    StatusEffect statusEffect1 = statusEffects[index];
                    if (statusEffect1.name == "SE_GolemCore")
                    {
                        RectTransform statusEffect2 = __instance.m_statusEffects[index];
                        TMP_Text component2 = statusEffect2.Find("TimeText").GetComponent<TMP_Text>();
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
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
        public static class StoneGolem_GetBodyArmor_Patch
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix(Player __instance, ref float __result)
            {
                if (__instance.m_seman.HaveStatusEffect("SE_GolemCore"))
                {
                    __result += float.Parse(StoneGolemStats[0]);
                }
                if (EssenceItemData.equipedEssence.Contains("$item_stonegolem_essence"))
                {
                    __result *= LackingImaginationGlobal.c_stonegolemCoreOverdriveArmor;
                }
            }
        }
    }
    
    
    
}