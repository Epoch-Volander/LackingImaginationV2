using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2
{
    public class SE_Courage : SE_Stats
    {
        public static Sprite AbilityIcon;

        [Header("SE_Courage")]
        public static float m_baseTTL = 600f;
        // public static float m_baseTTL = 60f;

        public SE_Courage()
        {
            base.name = "SE_Courage";
            m_icon = AbilityIcon;
            m_tooltip = "Courage: You no longer fear fire.";
            m_name = "Courage";
            m_ttl = m_baseTTL;
        }
        
        public override void Setup(Character character) => base.Setup(character);

        
        
        
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


