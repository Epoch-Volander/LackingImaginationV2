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

    public class xBlobEssence // poison cloud, double jump , at half hp spawn 2 friendly blob and gain status effect split
    {
        public static string Ability_Name = "Fumes";
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                LackingImaginationV2Plugin.Log($"xBlobEssence Button was pressed");
            
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xBlobCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                 UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_blob_attack"), player.transform.position, Quaternion.identity);
                 UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_blob_attack"), player.transform.position, Quaternion.identity);
                 
                 List<Character> allCharacters = new List<Character>();
                 allCharacters.Clear();
                 Character.GetCharactersInRange(player.transform.position, 6f, allCharacters);
                 foreach (Character ch in allCharacters)
                 {
                     if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer)) 
                         && !ch.m_tamed ||ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                     {
                         HitData hitData = new HitData();
                         hitData.m_damage.m_poison = UnityEngine.Random.Range(1f, 1f);
                         hitData.m_dir = ch.transform.position - player.transform.position;
                         hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_blobFumes));
                         hitData.m_point = ch.transform.position;
                         hitData.SetAttacker(player);
                         ch.Damage(hitData);
                     }
                 }
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
    }

    [HarmonyPatch]
    public static class xBlobEssencePassive
    {
        public static int canDoubleJump;
        
        [HarmonyPatch(typeof(Player), "Update", null)]
        public class Blob_AbilityInput_Patch
        {
            public static void Postfix(Player __instance)
            {
                Player localPlayer = Player.m_localPlayer;
                if (localPlayer != null && LackingImaginationV2Plugin.playerEnabled && EssenceItemData.equipedEssence.Contains("$item_blob_essence"))
                {
                    canDoubleJump = 1;
                }
                else
                {
                    canDoubleJump = 0;
                }
            }
        }

        [HarmonyPatch(typeof(Character), "RPC_Damage")]
        public static class Blob_RPC_Damage_Patch
        {
            public static void Postfix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_blob_essence") && hit.GetAttacker() != null)
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
                        if (__instance.GetHealth() <= (__instance.GetMaxHealth() * 0.5) && !__instance.GetSEMan().HaveStatusEffect("SE_FumesSplit"))
                        {
                            SE_FumesSplit se_fumessplit = (SE_FumesSplit)ScriptableObject.CreateInstance(typeof(SE_FumesSplit));
                            __instance.GetSEMan().AddStatusEffect(se_fumessplit);
                            
                            Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * 2f;
                            Vector3 randomPosition = __instance.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
                            GameObject Blob1 = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Blob"), randomPosition, Quaternion.identity);    
                            GameObject Blob2 = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Blob"), randomPosition, Quaternion.identity);

                            Blob1.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                            Blob1.GetComponent<Humanoid>().m_name = "AllyBlob1";
                            Blob1.GetComponent<Humanoid>().SetMaxHealth(Blob1.GetComponent<Humanoid>().GetMaxHealthBase() * 10f);
                            Blob1.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                            Blob1.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                            
                            Blob2.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                            Blob2.GetComponent<Humanoid>().m_name = "AllyBlob2";
                            Blob2.GetComponent<Humanoid>().SetMaxHealth(Blob2.GetComponent<Humanoid>().GetMaxHealthBase() * 10f);
                            Blob2.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                            Blob2.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                        }
                    }
                    if (__instance != null && __instance.IsPlayer())
                    {
                        __instance.m_damageModifiers.m_lightning = HitData.DamageModifier.Weak;
                        __instance.m_damageModifiers.m_blunt = HitData.DamageModifier.Weak;
                    }
                }
            }
        }













    }
}