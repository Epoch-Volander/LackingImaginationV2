using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_MoonlitLeap : SE_Stats
    {
        
        
        
        public static Sprite AbilityIcon;

        [Header("SE_MoonlitLeap")]
        public static float m_baseTTL = LackingImaginationUtilities.xFenringCooldownTime * LackingImaginationGlobal.c_fenringMoonlitLeapSED;
        private float JumpDuration = LackingImaginationUtilities.xFenringCooldownTime * LackingImaginationGlobal.c_fenringMoonlitLeapSED;
        private float m_timer = 1f;
        private float FallDuration = LackingImaginationUtilities.xFenringCooldownTime * LackingImaginationGlobal.c_fenringMoonlitLeapSED;
        
        
        public SE_MoonlitLeap()
        {
            base.name = "SE_MoonlitLeap";
            m_icon = AbilityIcon;
            m_tooltip = "Moonlit \nLeap";
            m_name = "Moonlit \nLeap";
            m_ttl = m_baseTTL;
            
        }
        
        public override void ModifyJump(Vector3 baseJump, ref Vector3 jump)
        {
            if(JumpDuration > 0)
            {
                jump *= LackingImaginationGlobal.c_fenringMoonlitLeap;
            }
            base.ModifyJump(baseJump, ref jump);
        }
        public override void ModifyFallDamage(float baseDamage, ref float damage)
        {
            if(FallDuration > 0)
            {
                damage *= 0f;
            }
            base.ModifyFallDamage(baseDamage, ref damage);
        }
        
        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            m_timer -= dt;
            if (m_timer <= 0f)
            {
                m_timer = 1f;
                JumpDuration--;
                FallDuration--;
            }
        
        }
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


