using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace LackingImaginationV2

{
    public class SE_RecklessCharge : StatusEffect
    {
        public static Sprite AbilityIcon;
        
        [Header("SE_RecklessCharge")]
        
        public static float m_baseTTL = LackingImaginationUtilities.xBoarCooldownTime * LackingImaginationGlobal.c_boarRecklessChargeSED;//

        private  HashSet<GameObject> detectedObjects = new HashSet<GameObject>();

        private bool hasHit;
        private float isRunning;
        private readonly int collisionMask = LayerMask.GetMask( "character");
        
        private float Duration = m_baseTTL - 1f;
        private float m_timer = 1f;
        private GameObject Aura;

        public SE_RecklessCharge()
        {
            base.name = "SE_RecklessCharge";
            m_icon = AbilityIcon;
            m_tooltip = "Reckless \nCharge";
            m_name = "Reckless \nCharge";
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
                if (Player.m_localPlayer.IsRunning() && Player.m_localPlayer != null)
                {
                    isRunning++;
                    // LackingImaginationV2Plugin.Log($"counter {isRunning}");
                }
            }
            if (Duration > 1 && Player.m_localPlayer != null)
            {
                if (!Player.m_localPlayer.IsRunning())
                {
                    isRunning = 0f;
                    if(Aura != null) UnityEngine.GameObject.Destroy(Aura);
                    detectedObjects.Clear();
                }
                    
                if (isRunning > 3f)
                {
                    if(Aura == null)
                    {
                        Aura = UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_RecklessCharge, Player.m_localPlayer.GetCenterPoint() + Player.m_localPlayer.transform.forward * 0.5f, Quaternion.identity);
                        Aura.transform.parent = Player.m_localPlayer.transform;
                        Aura.transform.rotation = Player.m_localPlayer.transform.rotation;
                    }
                    Vector3 sizeScan = Vector3.one * 0.5f;
                    Vector3 size = Vector3.one * 1.2f; 
                    Collider[] colliderScan = Physics.OverlapBox(Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 1.4f, sizeScan , Quaternion.identity, collisionMask);
                    Collider[] colliders = Physics.OverlapBox(Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.up  * 1.4f, size , Quaternion.identity, collisionMask);
                    
                    if (colliderScan.Length > 1)
                    {
                        hasHit = true;

                        foreach (Collider collider in colliders)
                        {
                            Character characterComponent = collider.gameObject.GetComponent<Character>();
                            if (characterComponent != null && collider != Player.m_localPlayer.m_collider)
                            {
                                // This is a valid target (creature) if it hasn't been detected before.
                                if (!detectedObjects.Contains(collider.gameObject))
                                {
                                    detectedObjects.Add(collider.gameObject);
                                    // LackingImaginationV2Plugin.Log($"should hit");
                                    HitData hitData = new HitData();
                                    hitData.m_hitCollider = collider;
                                    hitData.m_dodgeable = true;
                                    hitData.m_blockable = true;
                                    hitData.m_ranged = false;
                                    hitData.m_damage.m_blunt = LackingImaginationGlobal.c_boarRecklessCharge;
                                    hitData.m_dir = collider.gameObject.transform.position - Player.m_localPlayer.transform.position;
                                    // hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage() ) * LackingImaginationGlobal.c_loxWildTremor));
                                    if(Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_Courage")) hitData.m_pushForce = 300f;
                                    else hitData.m_pushForce = 150f;
                                    hitData.m_backstabBonus = 1f;
                                    // hitData.m_staggerMultiplier = 2f;
                                    hitData.m_point = collider.gameObject.transform.position;
                                    hitData.SetAttacker(Player.m_localPlayer);
                                    hitData.m_hitType = HitData.HitType.PlayerHit;
                                    characterComponent.Damage(hitData);
                                    UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_boar_hit"), hitData.m_point, Quaternion.identity);
                                    try
                                    {
                                        UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_RecklessChargeHit, characterComponent.GetCenterPoint(), Quaternion.identity);
                                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_boar_hit"), characterComponent.GetCenterPoint(), Quaternion.identity);
                                    }
                                    catch 
                                    {
                                        UnityEngine.GameObject.Instantiate(LackingImaginationV2Plugin.fx_RecklessChargeHit, characterComponent.GetCenterPoint(), Quaternion.identity);
                                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_boar_hit"), hitData.m_point, Quaternion.identity);
                                    }
                                }
                            }
                        }
                    }
                    else if(hasHit)
                    {
                        hasHit = false;
                        isRunning = 0f;
                        if(Aura != null) UnityEngine.GameObject.Destroy(Aura);
                        detectedObjects.Clear();
                    }
                }
            }
            else
            {
                if(Aura != null) UnityEngine.GameObject.Destroy(Aura);
            }
            if(Player.m_localPlayer.IsDead()) if(Aura != null) UnityEngine.GameObject.Destroy(Aura);
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

        
       
        
        
    }
}


