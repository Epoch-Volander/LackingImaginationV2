using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_LuckyFoot : SE_Stats
    {
        
        public static Sprite AbilityIcon;
       
        
        [Header("SE_LuckyFoot")]
        public static float m_baseTTL = LackingImaginationUtilities.xHareCooldownTime * 0.5f;
        private float speedAmount = 2f;

        public SE_LuckyFoot()
        {
            base.name = "SE_LuckyFoot";
            m_icon = AbilityIcon;
            m_tooltip = "Lucky Foot";
            m_name = "Lucky Foot";
            m_ttl = m_baseTTL;
            
        }
        
        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            speed *= speedAmount * LackingImaginationGlobal.c_hareLuckyFoot;
            
            base.ModifySpeed(baseSpeed, ref speed);
        }
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


