using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Longinus :  SE_Stats //StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Longinus")]
        public static float m_blood = 200f;
        
        
        public SE_Longinus()
        {
            base.name = "SE_Longinus";
            m_icon = AbilityIcon;
            m_tooltip = "Longinus: Your next spear throw hit is empowered.";
            m_name = "Longinus";
            
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = LackingImaginationV2Plugin.fx_Longinus,
                        m_enabled = true,
                        m_variant = -1,
                        m_attach = false,
                        m_follow = true,
                        m_inheritParentScale = true,
                        m_multiplyParentVisualScale = true,
                        m_scale = true,
                        m_inheritParentRotation = true,
                        m_childTransform = "Hips",
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


