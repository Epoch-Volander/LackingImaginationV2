using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace LackingImaginationV2

{
    public class SE_TwinSouls : StatusEffect
    {
        public static Sprite AbilityIcon;
        
        [Header("SE_TwinSouls")]
        
        public static float m_baseTTL = LackingImaginationUtilities.xWriathCooldownTime - 10f;

        public bool hasCollided;
        public Collider IgnoreCollision;
        public List<Collider> CollisionList = new List<Collider>();
        private float delayTimer = 0.4f; // Delay timer set to 1 second.
        private bool isDelaying; // Flag to track whether we are in the delay period.

        public int collisionMask = LayerMask.GetMask("piece", "piece_nonsolid", "Default", "static_solid", "Default_small", "vehicle", "character");
        public int collisionMaskDungeon = LayerMask.GetMask("piece", "piece_nonsolid", "static_solid", "Default_small", "vehicle", "character");

        public float Duration = m_baseTTL - 2f;
        private float m_timer = 1f;

        public SE_TwinSouls()
        {
            base.name = "SE_TwinSouls";
            m_icon = AbilityIcon;
            m_tooltip = "Twin Souls";
            m_name = "Twin Souls";
            m_ttl = m_baseTTL;

        }
        
        public override void Setup(Character character) => base.Setup(character);

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            
            m_timer -= dt;
            if (m_timer <= 0f)
            {
                m_timer = 1f;
                Duration--;
            }

            if (Duration > 2)
            {
                if (Player.m_localPlayer != null && Player.m_localPlayer.GetEyePoint() != null)
                {
                    // Collider[] colliders = Physics.OverlapSphere((Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 0.5f), 0.7f, collisionMask);
                    Collider[] colliders = Physics.OverlapSphere((Player.m_localPlayer.GetEyePoint()), 1.2f, collisionMask);

                    if (Player.m_localPlayer.transform.position.y > 3500f)
                        colliders = Physics.OverlapSphere((Player.m_localPlayer.GetEyePoint()), 1.2f, collisionMaskDungeon);
                    
                    // if (Player.m_localPlayer.IsBlocking())
                    // {
                    //     LackingImaginationV2Plugin.Log($"Block");
                    // }
                    // if (ZInput.GetButtonDown("Crouch") && !Player.m_localPlayer.IsDead() && !Player.m_localPlayer.InAttack()
                    //     && !Player.m_localPlayer.InDodge() && !Player.m_localPlayer.IsKnockedBack())
                    // {
                    //     LackingImaginationV2Plugin.Log($"Crouch");
                    // }
                    
                    
                    if (Player.m_localPlayer.IsBlocking() && (ZInput.GetButton("Crouch") && !Player.m_localPlayer.IsDead() && !Player.m_localPlayer.InAttack()
                                                              && !Player.m_localPlayer.InDodge() && !Player.m_localPlayer.IsKnockedBack()))
                    {
                        // LackingImaginationV2Plugin.Log($"Combo");
                         colliders = Physics.OverlapSphere((Player.m_localPlayer.transform.position), 0.85f, collisionMask);
                         
                         if (Player.m_localPlayer.transform.position.y > 3500f)
                             colliders = Physics.OverlapSphere((Player.m_localPlayer.transform.position), 0.85f, collisionMaskDungeon);
                    }
                    
                    if (colliders.Length > 0)
                    {
                        // A collision has occurred
                        hasCollided = true;
                   
                        foreach (Collider collider in colliders)
                        {
                            // IgnoreCollision = collider;
                            if(CollisionList != null && !CollisionList.Contains(collider)) CollisionList.Add(collider);
                            // Disable collision between the player and the specific object
                            Physics.IgnoreCollision(Player.m_localPlayer.m_collider, collider, true);
                        }
                    }
                    else if (hasCollided)
                    {
                        if(CollisionList != null && CollisionList.Any())
                        {
                            if (isDelaying)
                            {
                                delayTimer -= dt;
                                if (delayTimer <= 0f)
                                {
                                    isDelaying = false;
                                    delayTimer = 0.4f; // Reset the timer for future use
                                }
                            }
                            else 
                            {
                                foreach (Collider col in CollisionList)
                                {
                                    Physics.IgnoreCollision(Player.m_localPlayer.m_collider, col, false);
                                }
                                if(CollisionList.Count > 20) CollisionList.Clear();
                            }   
                        }
                        else
                        {
                            // Start the delay when there are no collisions
                            isDelaying = true;
                        }
                        // Reset the flag to false since there's no collision.
                        hasCollided = false;
                    }
                }
            }
            else
            {
                foreach (Collider col in CollisionList)
                {
                    Physics.IgnoreCollision(Player.m_localPlayer.m_collider, col, false);
                }
            }

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

        
       
        
        
    }
}


