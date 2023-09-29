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

    public class xGreydwarfBruteEssence
    {
        public static string Ability_Name = "Bash";

        public static GameObject Aura;

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {

                LackingImaginationV2Plugin.Log($"GreydwarfBrute Button was pressed");

                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xGreydwarfBruteCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                SE_Bash se_bash = (SE_Bash)ScriptableObject.CreateInstance(typeof(SE_Bash));
                player.GetSEMan().AddStatusEffect(se_bash);

                Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_Bash, player.GetCenterPoint(), Quaternion.identity);

                Aura.transform.parent = player.transform;
                // GameObject mace = ZNetScene.instance.GetPrefab("skeleton_mace").transform.Find("attach").Find("mace").gameObject;
                // GameObject[] effects = new[]
                // {
                //     mace.transform.Find("Particle System").gameObject,
                //     mace.transform.Find("vfx_drippingwater").gameObject,
                //     mace.transform.Find("Point light").gameObject,
                //     mace.transform.Find("Particle System (1)").gameObject
                // };
                // foreach (GameObject obj in effects)
                // {
                //     UnityEngine.Object.Instantiate(obj, player.GetHeadPoint(), Quaternion.identity);
                //     // obj.transform.parent = player.transform;
                //     
                // //     if (obj != null)
                // //     {
                // //         // LackingImaginationV2Plugin.Log($"{obj.name}");
                // //         // // UnityEngine.Object.Instantiate(obj, player.transform.position, Quaternion.identity);
                // //         // // Set the parent of the GameObject to the specified Transform
                // //         // obj.transform.parent = player.m_visEquipment.m_rightHand;
                // //         // UnityEngine.Object.Instantiate(obj,player.m_visEquipment.m_rightHand.transform);
                // //         if (player != null && player.m_visEquipment != null && player.m_visEquipment.m_rightHand != null)
                // //         {
                // //             // Set the parent of the GameObject to the specified Transform
                // //             obj.transform.SetParent(player.m_visEquipment.m_rightHand);
                // //             obj.transform.localPosition = Vector3.zero; // Optionally reset local position
                // //         }
                // //     }
                // }

                // status effect no timer, empowered attack, double effect for clubs, racid remains on hit cloud effect, grey and crit effect

            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
    }



    [HarmonyPatch]
    public class xGreydwarfBruteEssencePassive
    {
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        class GreydwarfBrute_RPC_Damage_Patch
        {
            static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_greydwarfbrute_essence") && hit.GetAttacker() != null)
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
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Bash") && Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType != Skills.SkillType.None 
                                                                                        && Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType != Skills.SkillType.ElementalMagic 
                                                                                        && Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType != Skills.SkillType.BloodMagic
                                                                                        && Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType != Skills.SkillType.Bows
                                                                                        && Player.m_localPlayer.GetCurrentWeapon().m_shared.m_skillType != Skills.SkillType.Crossbows
                                                                                        && !hit.m_ranged)
                        {
                            UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_crit"), hit.m_point, Quaternion.identity);
                            GameObject hitEffect = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_skeleton_mace_hit"), hit.m_point, Quaternion.identity);
                            ParticleSystem.MainModule mainModule = hitEffect.transform.Find("wetsplsh").GetComponent<ParticleSystem>().main;
                            mainModule.startColor = Color.grey;
                            ParticleSystem.MainModule mainModule2 = hitEffect.transform.Find("Chunks").GetComponent<ParticleSystem>().main;
                            mainModule2.startColor = Color.red;
                            ParticleSystem.MainModule mainModule3 = hitEffect.transform.Find("Chunks").Find("blob_splat").GetComponent<ParticleSystem>().main;
                            mainModule3.startColor = Color.grey;
                            
                            hit.ApplyModifier(LackingImaginationGlobal.c_greydwarfbruteBashMultiplier);
                            
                            Player.m_localPlayer.GetSEMan().RemoveStatusEffect("SE_Bash".GetStableHashCode());
                            
                            UnityEngine.GameObject.Destroy(xGreydwarfBruteEssence.Aura);
                            
                        }
                        if (hit.m_ranged)
                        {
                            hit.ApplyModifier(LackingImaginationGlobal.c_greydwarfbruteRangedReductionPassive);
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class GreydwarfBrute_GetTotalFoodValue_Patch
        {
            [HarmonyPriority(Priority.High)]
            public static void Postfix(Player __instance, ref float hp)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_greydwarfbrute_essence"))
                {
                    hp += 25f;
                }
                if (!EssenceItemData.equipedEssence.Contains("$item_greydwarfbrute_essence") && __instance.GetSEMan().HaveStatusEffect("SE_Bash"))
                {
                    Player.m_localPlayer.GetSEMan().RemoveStatusEffect("SE_Bash".GetStableHashCode());

                    UnityEngine.GameObject.Destroy(xGreydwarfBruteEssence.Aura);
                }
            }
        }
        
        
        
        
    }
}
    
