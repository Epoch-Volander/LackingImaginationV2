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

    public class xGeirrhafaEssence
    {
        public static string Ability_Name = "Ice Age";
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"Geirrhafa Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                 se_cd.m_ttl = LackingImaginationUtilities.xGeirrhafaCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                //Effects, animations, and sounds
                
                
                // //caverock_ice_stalagtite_falling  have these droop on enemies and a few random spots or maybe fall in groups of three
                // Fenring_attack_IceNova
                // fx_fenring_icenova
                //     fx_fenring_frost_hand_aoestart
                // Frost AoE Spell Attack 3 Burst
                
                // UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_fenring_icenova"), player.transform.position, Quaternion.identity);

                EffectList m_startEffects = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_icenova"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = false,
                        }
                    }
                };
                EffectList m_HandEffectsRight = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_frost_hand_aoestart"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = false,
                            m_follow = true,
                            m_childTransform = "RightHand_Attach"
                        }
                    }
                };
                EffectList m_HandEffectsLeft = new EffectList
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("fx_fenring_frost_hand_aoestart"),
                            m_enabled = true,
                            m_variant = 0,
                            m_attach = false,
                            m_follow = true,
                            m_childTransform = "LeftHand_Attach"
                        }
                    }
                };
                
                // string controllerName = "Original";
                //        
                // // in case this is called before the first Player.Start
                // if (LackingImaginationV2Plugin.CustomRuntimeControllers.TryGetValue(controllerName, out RuntimeAnimatorController controller))
                // {
                //     LackingImaginationV2Plugin.FastReplaceRAC(player, controller);
                // }
                
                ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");

                // player.gameObject.transform.Find("Visual").GetComponent<>().
                // player.m_animator
                m_startEffects.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                m_HandEffectsRight.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());
                m_HandEffectsLeft.Create(player.GetCenterPoint(), player.transform.rotation, player.transform, player.GetRadius() * 2f, player.GetPlayerModel());

                
                
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

    }

    
    
    [HarmonyPatch]
    public static class xGeirrhafaEssencePassive
    {
        
        
        
        
        
    }
    
    
    
    
    
    
    
    
}