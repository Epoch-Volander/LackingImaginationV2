using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace LackingImaginationV2

{
    public class SE_TimedDeath : StatusEffect
    {
        public static Sprite AbilityIcon;
        
        [Header("SE_TimedDeath")]
        
        // public static float m_baseTTL = LackingImaginationGlobal.c_bonemassMassReleaseSummonDuration;
        public static float m_baseTTL = 200f;
        public float lifeDuration;
        private float m_timer = 1f;

        public SE_TimedDeath()
        {
            base.name = "SE_TimedDeath";
            m_icon = AbilityIcon;
            m_tooltip = "Timed \nDeath";
            m_name = "Timed \nDeath";
            m_ttl = m_baseTTL;

        }
        
        public override void Setup(Character character) => base.Setup(character);

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            m_timer -= dt;
            if (m_timer <= 0f)
            {
                m_timer = 1f;
                lifeDuration--;
                
            }
            if (lifeDuration <= 0)
            {
                this.m_character.ApplyDamage(new HitData() { m_damage = { m_damage = 99999f }, m_point = this.m_character.transform.position},false, true);
            }
            
        }

        // public override bool CanAdd(Character character)
        // {
        //     return character.IsPlayer();
        // }

    }
}


