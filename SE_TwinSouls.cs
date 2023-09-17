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
        // public Collider IgnoreCollision;
        public List<Collider> CollisionList = new List<Collider>();
        // private float delayTimer = 0.4f; // Delay timer set to 1 second.
        // private bool isDelaying; // Flag to track whether we are in the delay period.

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
                    // Collider[] colliders = Physics.OverlapSphere((Player.m_localPlayer.GetEyePoint()), 1.2f, collisionMask);
                    float radius = 0.5f; // Set the desired radius.
                    Vector3 size = new Vector3(radius, 0.7f, radius ); // Keep the height (2.4f) constant.
                    Collider[] colliders = Physics.OverlapBox(Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 1.4f, size , Quaternion.identity, collisionMask);
                    
                    if (Player.m_localPlayer.transform.position.y > 3500f)
                        colliders = Physics.OverlapBox(Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 1.4f, size , Quaternion.identity, collisionMaskDungeon);
                    
                    if (Player.m_localPlayer.IsBlocking() && (ZInput.GetButton("Crouch") && !Player.m_localPlayer.IsDead() && !Player.m_localPlayer.InAttack()
                                                              && !Player.m_localPlayer.InDodge() && !Player.m_localPlayer.IsKnockedBack()))
                    {
                        // LackingImaginationV2Plugin.Log($"Combo");
                         // colliders = Physics.OverlapSphere((Player.m_localPlayer.transform.position), 0.85f, collisionMask);
                         colliders = Physics.OverlapBox(Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 0.35f, size , Quaternion.identity, collisionMask);
                         
                         if (Player.m_localPlayer.transform.position.y > 3500f)
                             // colliders = Physics.OverlapSphere((Player.m_localPlayer.transform.position), 0.85f, collisionMaskDungeon);
                            colliders = Physics.OverlapBox(Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 0.35f, size , Quaternion.identity, collisionMaskDungeon);
                    }
                    
                    if (colliders.Length > 1)
                    {
                        // LackingImaginationV2Plugin.Log($"length {colliders.Length}");
                        // A collision has occurred
                        hasCollided = true;
                   
                        foreach (Collider collider in colliders)
                        {
                            // IgnoreCollision = collider;
                            if(CollisionList != null && collider != null && !CollisionList.Contains(collider) && collider!= Player.m_localPlayer.m_collider) CollisionList.Add(collider);
                            // Disable collision between the player and the specific object
                            if(collider != null && collider != Player.m_localPlayer.m_collider) Physics.IgnoreCollision(Player.m_localPlayer.m_collider, collider, true);
                        }
                    }
                    else if (hasCollided)
                    {
                        foreach (Collider col in CollisionList)
                        {
                            if(col != null && col != Player.m_localPlayer.m_collider)  Physics.IgnoreCollision(Player.m_localPlayer.m_collider, col, false);
                        }
                        if(CollisionList.Count > 20) CollisionList.Clear();
                      
                        hasCollided = false;
                    }
                }
            }
            else
            {
                foreach (Collider col in CollisionList)
                {
                    if(col != null && col != Player.m_localPlayer.m_collider) Physics.IgnoreCollision(Player.m_localPlayer.m_collider, col, false);
                }
            }

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

        
       
        
        
    }
}


