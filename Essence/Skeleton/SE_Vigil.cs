using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Vigil : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Vigil")]
       
        public static float m_blood = 200f;
        
        
        public SE_Vigil()
        {
            base.name = "SE_Vigil";
            m_icon = AbilityIcon;
            m_tooltip = "Vigil: Summon ally ghosts to fight alongside you.(Gather Souls bt killing Skeletons)";
            m_name = "Vigil";
            
        }

        public override void ModifyFallDamage(float baseDamage, ref float damage)
        {
            damage *= 2f;
            base.ModifyFallDamage(baseDamage, ref damage);
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


