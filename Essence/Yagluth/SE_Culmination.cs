using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Culmination : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Culmination")]
        public static float m_blood = 200f;
        
        
        public SE_Culmination()
        {
            base.name = "SE_Culmination";
            m_icon = AbilityIcon;
            m_tooltip = "Culmination";
            m_name = "Culmination";
           

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


