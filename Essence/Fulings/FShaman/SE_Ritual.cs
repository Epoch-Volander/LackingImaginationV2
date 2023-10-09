using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Ritual : SE_Shield 
    {
        
        
        public static Sprite AbilityIcon;

        [Header("SE_Ritual")]
        public static float m_baseTTL = 240f;

        public SE_Ritual()
        {
            base.name = "SE_Ritual";
            m_icon = AbilityIcon;
            m_tooltip = "Ritual Shield";
            m_name = "Ritual Shield";
            m_ttl = m_baseTTL;
            m_absorbDamage = 1f;
            m_absorbDamageWorldLevel = 1f;
            
             m_startEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = ZNetScene.instance.GetPrefab("vfx_GoblinShield"),
                        m_enabled = true,
                        m_variant = -1,
                        m_attach = true,
                        // m_follow = true,
                    }
                }
            };
             
             m_hitEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = ZNetScene.instance.GetPrefab("fx_GoblinShieldHit"),
                        m_enabled = true,
                        m_variant = -1,
                    }
                }
            };
             
             m_breakEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = ZNetScene.instance.GetPrefab("fx_GoblinShieldBreak"),
                        m_enabled = true,
                        m_variant = -1,
                    }
                }
            };
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


