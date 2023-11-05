using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core;

namespace LackingImaginationV2

{
    public class SE_Disdain : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Disdain")]
        public static float m_baseTTL = Math.Min(LackingImaginationUtilities.xThungrCooldownTime * LackingImaginationGlobal.c_thungrTyrantSED, LackingImaginationUtilities.xThungrCooldownTime); 
        
        
        public SE_Disdain()
        {
            base.name = "SE_Disdain";
            m_icon = AbilityIcon;
            m_tooltip = "Disdain: Stolen Power.\n" +
                        "Blunt:"+xThungrEssencePassive.ThungrStats[0]+"\n" +
                        "Pierce:"+xThungrEssencePassive.ThungrStats[1]+"\n" +
                        "Slash:"+xThungrEssencePassive.ThungrStats[2]+"\n" +
                        "Fire:"+xThungrEssencePassive.ThungrStats[3]+"\n" +
                        "Frost:"+xThungrEssencePassive.ThungrStats[4]+"\n" +
                        "Lightning:"+xThungrEssencePassive.ThungrStats[5]+"\n" +
                        "Poison:"+xThungrEssencePassive.ThungrStats[6]+"\n" +
                        "Spirit:"+xThungrEssencePassive.ThungrStats[7]+"\n";
            m_name = "Disdain";
            m_ttl = m_baseTTL;

        }
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
        public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
        {
            hitData.m_damage.m_blunt += float.Parse(xThungrEssencePassive.ThungrStats[0]);
            hitData.m_damage.m_pierce += float.Parse(xThungrEssencePassive.ThungrStats[1]);
            hitData.m_damage.m_slash += float.Parse(xThungrEssencePassive.ThungrStats[2]);
            hitData.m_damage.m_fire += float.Parse(xThungrEssencePassive.ThungrStats[3]);
            hitData.m_damage.m_frost += float.Parse(xThungrEssencePassive.ThungrStats[4]);
            hitData.m_damage.m_lightning += float.Parse(xThungrEssencePassive.ThungrStats[5]);
            hitData.m_damage.m_poison += float.Parse(xThungrEssencePassive.ThungrStats[6]);
            hitData.m_damage.m_spirit += float.Parse(xThungrEssencePassive.ThungrStats[7]);

        }
        
        
        
    }
}


