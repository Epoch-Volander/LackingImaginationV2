using System;
using System.Collections;
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

    public class xRancidRemainsEssence
    {
        public static string Ability_Name = "Rancorous";
        
        public static bool Awakened = Boolean.Parse(xRancidRemainsEssencePassive.RancidRemainsStats[0]);
        private static float equipDelay = 0.5f;
        private static float deleteDelay = 3600f;
        public static bool Throwable;
        
        public static GameObject Aura;
        public static void Process_Input(Player player, int position)
        {
              if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
              {
                  GameObject PoisonMace = Awakened ? LackingImaginationV2Plugin.GO_RancorousMace : LackingImaginationV2Plugin.GO_RancorousMaceBroken;
                  
                  //Ability Cooldown
                  StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);

                  if (!player.m_inventory.ContainsItemByName(PoisonMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_name))
                  {
                      if (!player.m_inventory.CanAddItem(PoisonMace))
                      {
                          player.Message(MessageHud.MessageType.TopLeft, $"Inventory Full");
                          return;
                      }
                      //Ability Cooldown
                      se_cd.m_ttl = LackingImaginationUtilities.xRancidRemainsCooldownTime;
                      player.GetSEMan().AddStatusEffect(se_cd);
                    
                      player.m_inventory.AddItem(PoisonMace, 1);
                      ItemDrop.ItemData mace = player.m_inventory.GetItem(PoisonMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_name);
                      ScheduleEquip(player, ref mace);
                      ScheduleDelete(player, ref mace);
                  }
                  else if(!Throwable)
                  {
                      //Ability Cooldown
                      se_cd.m_ttl = LackingImaginationUtilities.xRancidRemainsCooldownTime * 2f;
                      player.GetSEMan().AddStatusEffect(se_cd);

                      Throwable = true;
                      //add aura, maybe brenna head flames
                      GameObject prefab =  ExpMethods.DeepCopy(ZNetScene.instance.GetPrefab("Skeleton_Poison").transform.Find("Visual").transform.Find("vfx_drippingwater").gameObject);
                      Aura = UnityEngine.GameObject.Instantiate(prefab, player.GetHeadPoint(), Quaternion.identity);
                      Aura.transform.parent = player.transform;
                     //fix this
                     
                  }
                  else
                  {
                      //Ability Cooldown
                      se_cd.m_ttl = LackingImaginationUtilities.xRancidRemainsCooldownTime * 2f;
                      player.GetSEMan().AddStatusEffect(se_cd);

                      Throwable = false;
                    
                      if(xRancidRemainsEssence.Aura != null) UnityEngine.GameObject.Destroy(xRancidRemainsEssence.Aura);
                  }
                
                  //Effects, animations, and sounds
                  // LackingImaginationV2Plugin.Log($"Brenna {player.transform.position}");
              }
        }
        private static void ScheduleEquip(Player player, ref ItemDrop.ItemData item)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleEquipCoroutine(player, item));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleEquipCoroutine(Player player, ItemDrop.ItemData item)
        {
            yield return new WaitForSeconds(equipDelay);
            
            player.EquipItem(item);
        }
        private static void ScheduleDelete(Player player, ref ItemDrop.ItemData item)
        {
            CoroutineRunner.Instance.StartCoroutine(ScheduleDeleteCoroutine(player, item));
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator ScheduleDeleteCoroutine(Player player, ItemDrop.ItemData item)
        {
            yield return new WaitForSeconds(deleteDelay);
            
            if (player.m_inventory.ContainsItem(item))
            {
                if(player.IsItemEquiped(item)) player.UnequipItem(item);
                player.m_inventory.RemoveItem(item);
            }
        }
    }

     [HarmonyPatch]
    public static class xRancidRemainsEssencePassive
    {
        public static List<string> RancidRemainsStats = new List<string>() { "false" };
        
        private static readonly int Script_Breath_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid", "terrain", "character", "character_net", "character_ghost", "hitbox", "character_noenv", "vehicle", "viewblock");
        

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.SpawnOnHit))]
        public static class Brenna_SpawnOnHit_Patch
        {
            public static void Prefix(Projectile __instance)
            {
                GameObject PoisonMace = xRancidRemainsEssence.Awakened ? LackingImaginationV2Plugin.GO_RancorousMace : LackingImaginationV2Plugin.GO_RancorousMaceBroken;
                if (__instance.m_owner == Player.m_localPlayer && __instance.m_spawnItem?.m_shared.m_name == PoisonMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                {
                    Vector3 vector3 = __instance.transform.position + (__instance.transform.up * 0.6f) + __instance.transform.TransformDirection(__instance.m_spawnOffset);
                    Quaternion rotation = __instance.transform.rotation;
                    rotation = Quaternion.Euler(0.0f, __instance.transform.rotation.eulerAngles.y, 0.0f);
                    
                    //sphere radius 9.5
                    // fx_fireskeleton_nova

                    if (xRancidRemainsEssence.Awakened)
                    {
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_skeleton_mace_hit"), vector3, rotation);
                        GameObject Aoe = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_skeleton_mace_hit"), vector3, rotation);
                        Aoe.GetComponent<Transform>().localScale *= 2f;
                        StartAoe(Player.m_localPlayer, vector3, 9.5f);
                    }
                    else
                    {
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_skeleton_mace_hit"), vector3, rotation);
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_skeleton_mace_hit"), vector3, rotation);
                        StartAoe(Player.m_localPlayer, vector3, 4.75f);
                    }
                    
                    __instance.m_spawnItem = null;
                }
            }
            
            static void StartAoe(Player player, Vector3 vector3, float radius)
            {
                HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

                Vector3 capsuleCenter = vector3;
                float capsuleRadius = radius; // Radius of the capsule

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
                            hitData.m_damage.m_poison =  xRancidRemainsEssence.Awakened ? LackingImaginationV2Plugin.GO_RancorousMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_poison : LackingImaginationV2Plugin.GO_RancorousMaceBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_poison;
                            hitData.m_damage.m_blunt = xRancidRemainsEssence.Awakened ? LackingImaginationV2Plugin.GO_RancorousMace.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_blunt : LackingImaginationV2Plugin.GO_RancorousMaceBroken.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_blunt;
                            hitData.m_dir = collider.transform.position - capsuleCenter;
                            hitData.ApplyModifier(2f);
                            hitData.m_pushForce = 0f;
                            hitData.m_hitCollider = collider;
                            hitData.m_dodgeable = true;
                            hitData.m_blockable = true;
                            hitData.m_point = collider.gameObject.transform.position;
                            hitData.SetAttacker(player);
                            hitData.m_hitType = HitData.HitType.PlayerHit;
                            destructibleComponent.Damage(hitData);
                        }
                    }
                }
            }
        }

        
        
        
        
        
        
        
        

    }
    
    

}