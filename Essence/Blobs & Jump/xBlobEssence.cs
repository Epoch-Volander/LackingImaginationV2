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
        
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");

        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xBlobCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_blob_attack"), player.transform.position, Quaternion.identity);
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_blob_attack"), player.transform.position, Quaternion.identity);
                 
                HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

                Vector3 capsuleCenter = player.transform.position;
                float capsuleRadius = 6f; // Radius of the capsule

                // Perform the capsule overlap check with the specified layer mask
                Collider[] colliders = Physics.OverlapSphere(capsuleCenter, capsuleRadius, Script_Breath_Layermask);

                foreach (Collider collider in colliders)
                {
                    IDestructible destructibleComponent = collider.gameObject.GetComponent<IDestructible>();
                    Character characterComponent = collider.gameObject.GetComponent<Character>();
                    if (destructibleComponent != null || (characterComponent != null && !characterComponent.IsOwner()))
                    {
                        // This is a valid target (creature) if it hasn't been detected before.
                        if (!detectedObjects.Contains(collider.gameObject))
                        {
                            detectedObjects.Add(collider.gameObject);
                        
                            HitData hitData = new HitData();
                            hitData.m_damage.m_poison = UnityEngine.Random.Range(5f, 10f);
                            hitData.m_dodgeable = true;
                            hitData.m_blockable = true;
                            hitData.m_hitCollider = collider;
                            hitData.m_dir = collider.transform.position - player.transform.position;
                            hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_blobFumes));
                            hitData.m_point = collider.transform.position;
                            hitData.SetAttacker(player);
                            destructibleComponent.Damage(hitData);
                            
                        }
                    }
                }
                 
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
    }

    [HarmonyPatch]
    public static class xBlobEssencePassive
    {
        public static int canDoubleJump;
        
        [HarmonyPatch(typeof(Player), nameof(Player.Update), null)]
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

        [HarmonyPatch(typeof(Character),  nameof(Character.RPC_Damage))]
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
                            Blob1.AddComponent<Tameable>();
                            Blob1.GetComponent<Tameable>().Tame();
                            Blob1.GetComponent<Tameable>().m_unsummonDistance = 100f;
                            Blob1.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
                            foreach (CharacterDrop.Drop drop in Blob1.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
                            
                            Blob2.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                            Blob2.GetComponent<Humanoid>().m_name = "AllyBlob2";
                            Blob2.GetComponent<Humanoid>().SetMaxHealth(Blob2.GetComponent<Humanoid>().GetMaxHealthBase() * 10f);
                            Blob2.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                            Blob2.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                            Blob2.AddComponent<Tameable>();
                            Blob2.GetComponent<Tameable>().Tame();
                            Blob2.GetComponent<Tameable>().m_unsummonDistance = 100f;
                            Blob2.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
                            foreach (CharacterDrop.Drop drop in Blob2.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
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