using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Craving : StatusEffect
    {
        public static Sprite AbilityIcon;

        [Header("SE_Craving")]
        public static float m_blood = 200f;
        
        public SE_Craving()
        {
            base.name = "SE_Craving";
            m_icon = AbilityIcon;
            m_tooltip = "Craving";
            m_name = "Craving";
        }

        public override void Setup(Character character) => base.Setup(character);

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


