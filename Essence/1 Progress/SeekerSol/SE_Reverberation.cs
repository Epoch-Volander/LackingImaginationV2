using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Reverberation : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Reverberation")]
        public static float m_baseTTL = Math.Min(LackingImaginationUtilities.xSeekerSoldierCooldownTime * LackingImaginationGlobal.c_seekersoldierReverberationSED, LackingImaginationUtilities.xSeekerSoldierCooldownTime);
        

        public SE_Reverberation()
        {
            base.name = "SE_Reverberation";
            m_icon = AbilityIcon;
            m_tooltip = "Reverberation: ";
            m_name = "Reverberation";
            m_ttl = m_baseTTL;
            
            m_startEffects = new EffectList
            {
                m_effectPrefabs = new EffectList.EffectData[]
                {
                    new()
                    {
                         m_prefab = LackingImaginationV2Plugin.fx_Reverberation,
                        m_enabled = true,
                        m_variant = -1,
                        m_attach = true,
                        m_follow = true,
                        m_inheritParentScale = true,
                        m_multiplyParentVisualScale = true,
                        m_scale = true,
                        m_inheritParentRotation = true,
                        m_childTransform = "Spine2",
                    }
                }
            };
            
        }

        
        public override void ModifyDamageMods(ref HitData.DamageModifiers modifiers)
        {
            modifiers.m_fire = HitData.DamageModifier.Weak;
            modifiers.m_poison = HitData.DamageModifier.Weak;
            modifiers.m_frost = HitData.DamageModifier.Weak;
            modifiers.m_lightning = HitData.DamageModifier.Weak;
            modifiers.m_spirit = HitData.DamageModifier.Weak;
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


