using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Forgotten : SE_Stats
    {
        
        public static Sprite AbilityIcon;
        public static float m_baseTTL = Math.Min(LackingImaginationUtilities.xDraugrCooldownTime * LackingImaginationGlobal.c_draugrForgottenSED, LackingImaginationUtilities.xDraugrCooldownTime);
        
        [Header("SE_Forgotten")]
        public static float bonusDmg = LackingImaginationGlobal.c_draugrForgottenActive + 1f;
        
        
        public SE_Forgotten()
        {
            base.name = "SE_Forgotten";
            m_icon = AbilityIcon;
            m_tooltip = "Forgotten: Increase the damage of Bows and Axes by " +((bonusDmg - 1f) * 100f).ToString("0") + "%.";
            m_name = "Forgotten";
            m_ttl = m_baseTTL;

        }

        
        public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
        {
            if (skill == Skills.SkillType.Axes || skill == Skills.SkillType.Bows)
            {
                hitData.m_damage.Modify(bonusDmg);
            }
            base.ModifyAttack(skill, ref hitData);
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
}


