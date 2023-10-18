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
            m_tooltip = "Ravenous Hunger: Every 5th hit will deal " +((LackingImaginationGlobal.c_wolfRavenousHunger) * 100f).ToString("0") + "% of max health in slash damage.";
            m_name = "Ravenous \nHunger";
            m_ttl = m_baseTTL;
            
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


