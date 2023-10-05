using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_RavenousHunger : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_RavenousHunger")]
        public static float m_baseTTL = LackingImaginationUtilities.xWolfCooldownTime * LackingImaginationGlobal.c_wolfRavenousHungerSED;

        public SE_RavenousHunger()
        {
            base.name = "SE_RavenousHunger";
            m_icon = AbilityIcon;
            m_tooltip = "Ravenous \nHunger";
            m_name = "Ravenous \nHunger";
            m_ttl = m_baseTTL;
            
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


