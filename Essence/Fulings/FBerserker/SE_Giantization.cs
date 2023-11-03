using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace LackingImaginationV2

{
    public class SE_Giantization : SE_Stats
    {
        
        public static Sprite AbilityIcon;
        
        
        private float giantDuration = Math.Min(LackingImaginationUtilities.xFulingBerserkerCooldownTime * LackingImaginationGlobal.c_fulingberserkerGiantizationSED, LackingImaginationUtilities.xFulingBerserkerCooldownTime);
        private float m_timer = 1f;
        
        [Header("SE_Giantization")]
        public static float m_baseTTL = LackingImaginationUtilities.xFulingBerserkerCooldownTime * LackingImaginationGlobal.c_fulingberserkerGiantizationSED;

        public SE_Giantization()
        {
            base.name = "SE_Giantization";
            m_icon = AbilityIcon;
            m_tooltip = "Giantization: Double your size, gaining double health but halving stamina.";
            m_name = "Giantization";
            m_ttl = m_baseTTL;
            
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = LackingImaginationV2Plugin.fx_Giantization,
                        m_enabled = true,
                        m_variant = -1,
                        m_attach = true,
                        // m_follow = true,
                    }
                }
            };
            
        }

        public override void ModifyJump(Vector3 baseJump, ref Vector3 jump)
        {
            if(giantDuration > 0)
            {
                jump *= 1.2f;
            }
            base.ModifyJump(baseJump, ref jump);
        }
        
        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            if(giantDuration > 0)
            {
                speed *= 2f;
            }
            base.ModifySpeed(baseSpeed, ref speed);
        }
        
        public override void ModifyFallDamage(float baseDamage, ref float damage)
        {
            if(giantDuration > 0)
            {
                damage *= 0.5f;
            }
            base.ModifyFallDamage(baseDamage, ref damage);
        }
        
        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            m_timer -= dt;
            if (m_timer <= 0f)
            {
                m_timer = 1f;
                giantDuration--;
            }
        
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


