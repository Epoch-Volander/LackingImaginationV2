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
        // private static int m_LOSMask = LayerMask.GetMask("piece", "piece_nonsolid", "Default", "static_solid", "Default_small", "terrain", "vehicle");
        // private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", /*"piece",*/ "viewblock");

        public static int collisionMask = LayerMask.GetMask("piece", "piece_nonsolid", "Default", "static_solid", "Default_small", "vehicle");
        
        public static string Ability_Name = "Twin Souls";// resist physical, weak elemental
        
        public static List<Character> Wraith = new List<Character>();
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

                
                
                
                // Collider[] colliders = Physics.OverlapSphere(player.transform.position, 0.2f, collisionMask);
                //
                // foreach (Collider collider in colliders)
                // {
                //     // You can implement custom logic here, such as triggering events or interactions.
                //     // For now, let's just log the collided object's name.
                //     Debug.Log("Collided with: " + collider.gameObject.name);
                // }
                
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
                else
                {
                    
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
        private static bool hasCollided;
        public static GameObject objectToIgnoreCollisionWith;
        public static Collider IgnoreCollision;
        
        [HarmonyPatch(typeof(Character), "CustomFixedUpdate")]
        public static class Wraith_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {

                // if (Player.m_localPlayer.GetEyePoint() != null)
                // {
                //     Collider[] colliders = Physics.OverlapSphere(Player.m_localPlayer.GetEyePoint(), 0.25f, xWraithEssence.collisionMask);
                //
                //     foreach (Collider collider in colliders)
                //     {
                //         // You can implement custom logic here, such as triggering events or interactions.
                //         // For now, let's just log the collided object's name.
                //         // Debug.Log("1Collided with: " + collider.gameObject.name);
                //         
                //         int playerLayer = Player.m_localPlayer.gameObject.layer;
                //         int collidedObjectLayer = collider.gameObject.layer;
                //         Physics.IgnoreLayerCollision(playerLayer, collidedObjectLayer, true);
                //         
                //     }
                // }
                
                if (Player.m_localPlayer.GetEyePoint() != null)
                {
                    Collider[] colliders = Physics.OverlapSphere((Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 0.5f), 0.2f, xWraithEssence.collisionMask);

                    // foreach (Collider collider in colliders)
                    // {
                    //     // You can implement custom logic here, such as triggering events or interactions.
                    //     // For now, let's just log the collided object's name.
                    //     // Debug.Log("2Collided with: " + collider.gameObject.name);
                    //     
                    //     int playerLayer = Player.m_localPlayer.gameObject.layer;
                    //     int collidedObjectLayer = collider.gameObject.layer;
                    //     Physics.IgnoreLayerCollision(playerLayer, collidedObjectLayer, true);
                    //     
                    // }
                    
                    if (colliders.Length > 0)
                    {
                        // A collision has occurred
                        hasCollided = true;
                        
                        foreach (Collider collider in colliders)
                        {
                            // You can implement custom logic here, such as triggering events or interactions.
                            // For now, let's just log the collided object's name.
                            // Debug.Log("2Collided with: " + collider.gameObject.name);
                            
                            // int playerLayer = Player.m_localPlayer.gameObject.layer;
                            // int collidedObjectLayer = collider.gameObject.layer;
                            // Physics.IgnoreLayerCollision(playerLayer, collidedObjectLayer, true);
                            //
                            
                            IgnoreCollision = collider;
                            // Disable collision between the player and the specific object
                            Physics.IgnoreCollision(Player.m_localPlayer.m_collider, collider, true);
                            
                        }
                        
                        
                        
                    }
                    else if (hasCollided)
                    {
                        // No collision found, and collision was detected in the previous frame.
                        // Re-enable collision between the player and objects on the LayerMask.

                        // int playerLayer = Player.m_localPlayer.gameObject.layer;
                        //
                        // // Iterate through all layers to enable collisions with them.
                        // for (int layer = 0; layer < 32; layer++)
                        // {
                        //     if (Physics.GetIgnoreLayerCollision(playerLayer, layer))
                        //     {
                        //         Physics.IgnoreLayerCollision(playerLayer, layer, false);
                        //     }
                        // }
                        if (IgnoreCollision != null)
                        {

                            Physics.IgnoreCollision(Player.m_localPlayer.m_collider, IgnoreCollision, false);
                            
                        }
                        

                        // Reset the flag to false since there's no collision.
                        hasCollided = false;
                    }
                    
                }
                
                
                
                
                if (__instance.m_name == "Second(Ally)" && (!EssenceItemData.equipedEssence.Contains("$item_abomination_essence") ||  EnvMan.instance.IsDay()))
                {
                    foreach (Character ch in xWraithEssence.Wraith)
                    {
                        if(ch == null) break;
                        ch.GetComponent<Tameable>().UnSummon();
                    }
                    xWraithEssence.Wraith.Clear();
                }
            }
        }
        
        
        
        
        
    }

}