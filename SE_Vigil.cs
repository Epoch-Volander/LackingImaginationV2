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

        [Header("SE_BloodSiphon")]
       
        public static float m_blood = 200f;
        
        
        public SE_Vigil()
        {
            base.name = "SE_Vigil";
            m_icon = AbilityIcon;
            m_tooltip = "Vigil";
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


