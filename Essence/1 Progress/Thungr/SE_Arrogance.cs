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
            m_tooltip = "Arrogance: Player damage reduced to "
                        + LackingImaginationGlobal.c_thungrTyrantArrogancePlayerDebuff * 100f 
                        +"% and Marked enemy damage increased by "+LackingImaginationGlobal.c_thungrTyrantArroganceEnemyBuff *100f +"%.";
            m_name = "Arrogance";
            m_ttl = m_baseTTL;
            
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = LackingImaginationV2Plugin.fx_ArroBuff,
                        m_enabled = true,
                        m_variant = 0,
                        m_attach = false,
                        m_follow = true,
                        m_inheritParentScale = true,
                        m_multiplyParentVisualScale = true,
                        m_scale = true,
                        m_inheritParentRotation = true,
                    }
                }
            };
        }
        
        public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
        {
            if (this.m_character.IsPlayer())
            {
                hitData.m_damage.Modify(LackingImaginationGlobal.c_thungrTyrantArrogancePlayerDebuff);
            }
            else
            {
                hitData.m_damage.Modify(1.0f + LackingImaginationGlobal.c_thungrTyrantArroganceEnemyBuff);
            }
            base.ModifyAttack(skill, ref hitData);
        }
       

    }
}


