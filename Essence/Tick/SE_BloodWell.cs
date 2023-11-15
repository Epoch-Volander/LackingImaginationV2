using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_BloodWell : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_BloodWell")]
        // public static float m_blood = int.Parse(xLeechEssencePassive.LeechStats[0]);
        public static float m_blood = 200f;
        
        
        public SE_BloodWell()
        {
            base.name = "SE_BloodWell";
            m_icon = AbilityIcon;
            m_tooltip = "Blood Well: Empower next hit to do bonus slash damage equal to Well stacks.";
            m_name = "Blood Well";
            
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                        m_prefab = LackingImaginationV2Plugin.fx_BloodWell,
                        m_enabled = true,
                        m_variant = -1,
                        m_attach = true,
                        m_inheritParentRotation = true,
                        // m_follow = true,
                    }
                }
            };
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


