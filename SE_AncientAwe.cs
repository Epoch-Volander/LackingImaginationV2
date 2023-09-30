using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_AncientAwe : StatusEffect
    {
        
        public static Sprite AbilityIcon;
       
        
        [Header("SE_AncientAwe")]
        public static float m_baseTTL = LackingImaginationUtilities.xElderCooldownTime - 5f;
        private float speedAmount = 0f;
        private float speedDuration = 5f;
        private float m_timer = 1f;
        
        
        public SE_AncientAwe()
        {
            base.name = "SE_AncientAwe";
            m_icon = AbilityIcon;
            m_tooltip = "Ancient \nAwe";
            m_name = "Ancient \nAwe";
            m_ttl = m_baseTTL;
            
        }
        
        public override void Setup(Character character) => base.Setup(character);
        
        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            if(speedDuration > 0 &&  !m_character.IsPlayer())
            {
                speed *= speedAmount;
            }
            base.ModifySpeed(baseSpeed, ref speed);
        }
        
        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            m_timer -= dt;
            if (m_timer <= 0f)
            {
                m_timer = 1f;
                speedDuration--;
            }
        
        }
        
    }
}


