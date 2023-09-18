using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_FallenHero : SE_Stats
    {
        
        public static Sprite AbilityIcon;
        public static float m_baseTTL = LackingImaginationUtilities.xDraugrEliteCooldownTime -15f;
        
        [Header("SE_FallenHero")]
        public static float bonusDmg = LackingImaginationGlobal.c_draugreliteFallenHeroActive + 1f;
        
        
        public SE_FallenHero()
        {
            base.name = "SE_FallenHero";
            m_icon = AbilityIcon;
            m_tooltip = "Fallen Hero";
            m_name = "Fallen Hero";
            m_ttl = m_baseTTL;

        }

        
        public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
        {
            if (skill == Skills.SkillType.Swords || skill == Skills.SkillType.Polearms)
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


