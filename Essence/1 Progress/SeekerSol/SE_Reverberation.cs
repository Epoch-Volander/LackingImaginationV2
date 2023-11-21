using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Reverberation : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Reverberation")]
        public static float m_baseTTL = Math.Min(LackingImaginationUtilities.xSeekerSoldierCooldownTime * LackingImaginationGlobal.c_seekersoldierReverberationSED, LackingImaginationUtilities.xSeekerSoldierCooldownTime);
        

        public SE_Reverberation()
        {
            base.name = "SE_Reverberation";
            m_icon = AbilityIcon;
            m_tooltip = "Reverberation: ";
            m_name = "Reverberation";
            m_ttl = m_baseTTL;
            

            
        }
        
       
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


