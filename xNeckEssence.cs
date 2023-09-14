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

    public class xNeckEssence
    {
        private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", /*"piece",*/ "viewblock");

        
        public static string Ability_Name = "Splash";
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                if (player.IsSwimming())
                {
                    LackingImaginationV2Plugin.Log($"Neck Button was pressed");

                    //Ability Cooldown
                    StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                    se_cd.m_ttl = LackingImaginationUtilities.xNeckCooldownTime;
                    player.GetSEMan().AddStatusEffect(se_cd);

                    //Effects, animations, and sounds

                    WaterDash(player);
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Can only be cast while Swimming");
                }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
        
         public static void WaterDash(Player player)
        {
            // sound and visual effects
            

            //RaycastHit hitInfo = default(RaycastHit);
            Vector3 lookVec = player.GetLookDir();
            lookVec.y = 0f;
            player.transform.rotation = Quaternion.LookRotation(lookVec);

            Vector3 hitVec = default(Vector3);            
            Vector3 fwdVec = player.transform.forward;
            Vector3 moveVec= player.transform.position;
            Vector3 yVec = player.transform.position;
            yVec.y += 0.1f;
            List<int> list = new List<int>();
            int i = 0;
            for (; i <= 10; i++)
            {
                RaycastHit hitInfo = default(RaycastHit);
                bool flag = false;
                for (int j = 0; j <= 10; j++)
                {
                    Vector3 vdirect = Vector3.MoveTowards(player.transform.position, player.transform.position + fwdVec * 100f, (float)((float)i + (float)j * 0.1f));
                    vdirect.y = yVec.y;
                    if (vdirect.y < ZoneSystem.instance.m_waterLevel) 
                    {
                        yVec.y = ZoneSystem.instance.m_waterLevel + 1f;
                        vdirect.y = yVec.y;
                    }
                    flag = Physics.SphereCast(vdirect, 0.05f, fwdVec, out hitInfo, float.PositiveInfinity, Script_Layermask);
                    if (flag && (bool)hitInfo.collider)
                    {
                        hitVec = hitInfo.point;
                        break;
                    }
                }
                moveVec= Vector3.MoveTowards(player.transform.position, player.transform.position + fwdVec * 100f, (float)i);

                if (flag && Vector3.Distance(new Vector3(moveVec.x, yVec.y, moveVec.z), hitVec) <= 1f)
                {
                    yVec = Vector3.MoveTowards(hitVec, yVec, 1f);
                    break;
                }
                yVec = new Vector3(moveVec.x, yVec.y, moveVec.z);
                foreach (Character ch in Character.GetAllCharacters())
                {
                    HitData hitData = new HitData();                    
                    hitData.m_damage = player.GetCurrentWeapon().GetDamage();
                    hitData.ApplyModifier(player.GetCurrentWeapon().m_shared.m_attack.m_speedFactor); 
                    hitData.m_point = ch.GetCenterPoint();
                    hitData.m_dir = (ch.transform.position - moveVec);
                    float num = Vector3.Distance(ch.transform.position, moveVec);
                    if (BaseAI.IsEnemy(ch, player) && num <= 3f && !list.Contains(ch.GetInstanceID()))
                    {
                        ch.Damage(hitData);
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("fx_crit"), ch.GetCenterPoint(), Quaternion.identity);
                        list.Add(ch.GetInstanceID());
                    }
                }
            }
            list.Clear();
            yVec.y = player.transform.position.y;
            player.transform.position = yVec;
            player.transform.rotation = Quaternion.LookRotation(fwdVec);
        }

    }

    
    [HarmonyPatch]
    public class xNeckEssencePassive
    {
        public static List<string> NeckStats = new List<string>(){"off"};
        
        [HarmonyPatch(typeof(Character), "UpdateSwimming")]
        public static class Neck_UpdateSwimming_Patch
        {
            public static void Prefix(Character __instance)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_neck_essence") && __instance != null)
                {
                    if (NeckStats[0] == "off")
                    {
                        __instance.m_swimSpeed *= 2f;
                        NeckStats[0] = "on";
                    }
                }
                else if (NeckStats[0] == "on" && !EssenceItemData.equipedEssence.Contains("$item_neck_essence") && __instance != null)
                {
                    __instance.m_swimSpeed *= 0.5f;
                    NeckStats[0] = "off";
                }
            }
        }
        
        //negative
        
        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Neck_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_neck_essence") && hit.GetAttacker() != null)
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
                    if (__instance != null && __instance.IsPlayer() && !__instance.GetSEMan().HaveStatusEffect("Wet"))
                    {
                        Player.m_localPlayer.m_damageModifiers.m_fire = HitData.DamageModifier.Weak;
                    }
                    if (__instance != null && __instance.IsPlayer() && __instance.GetSEMan().HaveStatusEffect("Wet"))
                    {
                        Player.m_localPlayer.m_damageModifiers.m_poison = HitData.DamageModifier.Resistant;
                    }
                }
            }
        }
        
        
        
        
        



    }
}