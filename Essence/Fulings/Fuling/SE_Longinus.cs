using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Longinus : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Longinus")]
        public static float m_blood = 200f;
        
        
        public SE_Longinus()
        {
            base.name = "SE_Longinus";
            m_icon = AbilityIcon;
            m_tooltip = "Longinus";
            m_name = "Longinus";
            
            // m_startEffects = new EffectList
            // {
            //     m_effectPrefabs = new EffectList.EffectData[]
            //     {
            //         new()
            //         {
            //             m_prefab = LackingImaginationV2Plugin.fx_Longinus,
            //             m_enabled = true,
            //             m_variant = -1,
            //             m_attach = true,
            //             // m_follow = true,
            //         }
            //     }
            // };

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


