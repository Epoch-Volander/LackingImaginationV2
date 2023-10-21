using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_GolemCore : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_GolemCore")]
        public static float m_blood = 200f;
        
        
        public SE_GolemCore()
        {
            base.name = "SE_GolemCore";
            m_icon = AbilityIcon;
            m_tooltip = "Golem Core";
            m_name = "Golem Core";
            
        }

       
        
    }
}


