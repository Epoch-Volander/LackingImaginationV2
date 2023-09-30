using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_HorizonHaste : SE_Stats
    {
        
        
        public static Sprite AbilityIcon;

        [Header("SE_HorizonHaste")]
        public static float m_baseTTL = LackingImaginationUtilities.xDeerCooldownTime -5f;
        private float speedDuration = LackingImaginationUtilities.xDeerCooldownTime -5f;
        private float speedAmount = 1.5f;
        private float m_timer = 1f;
        
        public SE_HorizonHaste()
        {
            base.name = "SE_HorizonHaste";
            m_icon = AbilityIcon;
            m_tooltip = "Horizon \nHaste";
            m_name = "Horizon \nHaste";
            m_ttl = m_baseTTL;
            
        }
        
        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            // if(m_character.IsRunning() || m_character.IsWalking())
            // {
            //     speed *= 1.5f  * LackingImaginationGlobal.c_deerHorizonHaste;
            // }
            // else 
            if(speedDuration > 0)
            {
                speed *= speedAmount * LackingImaginationGlobal.c_deerHorizonHaste;
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
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


