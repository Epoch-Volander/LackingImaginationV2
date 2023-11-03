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
        public static float m_baseTTL = Math.Min(LackingImaginationUtilities.xSurtlingCooldownTime * LackingImaginationGlobal.c_surtlingHarbingerSED, LackingImaginationUtilities.xSurtlingCooldownTime);

        public SE_Harbinger()
        {
            base.name = "SE_Harbinger";
            m_icon = AbilityIcon;
            m_tooltip = "Harbinger: Summon ally surtlings.(Sacrifice a surtling core to gain charges)";
            m_name = "Harbinger";
            m_ttl = m_baseTTL;
            
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


