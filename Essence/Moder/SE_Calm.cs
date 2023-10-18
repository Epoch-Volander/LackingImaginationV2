using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Calm : SE_Stats
    {
        public static Sprite AbilityIcon;

        [Header("SE_Calm")]
        public static float m_baseTTL = 1800f;
        // public static float m_baseTTL = 60f;

        public SE_Calm()
        {
            base.name = "SE_Calm";
            m_icon = AbilityIcon;
            m_tooltip = "Calm: The chill is suppressed.";
            m_name = "Calm";
            m_ttl = m_baseTTL;
        }
        
        public override void Setup(Character character) => base.Setup(character);

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


