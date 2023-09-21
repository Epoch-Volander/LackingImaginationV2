using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Harbinger : StatusEffect
    {
        
        public static Sprite AbilityIcon;
       
        
        [Header("SE_Harbinger")]
        public static float m_baseTTL = LackingImaginationUtilities.xSurtlingCooldownTime - 5f;

        public SE_Harbinger()
        {
            base.name = "SE_Harbinger";
            m_icon = AbilityIcon;
            m_tooltip = "Harbinger";
            m_name = "Harbinger";
            m_ttl = m_baseTTL;
            
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


