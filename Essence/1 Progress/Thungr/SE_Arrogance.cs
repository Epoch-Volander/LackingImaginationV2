using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Arrogance : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Arrogance")]
        public static float m_baseTTL = 1800f;
        

        public SE_Arrogance()
        {
            base.name = "SE_Arrogance";
            m_icon = AbilityIcon;
            m_tooltip = "Arrogance: .";
            m_name = "Arrogance";
            m_ttl = m_baseTTL;
            
        }
        
        public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
        {
            if (this.m_character.IsPlayer())
            {
                hitData.m_damage.Modify(0.25f);
            }
            else
            {
                hitData.m_damage.Modify(1.75f);
            }
            base.ModifyAttack(skill, ref hitData);
        }
       

    }
}


