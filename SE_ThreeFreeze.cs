using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_ThreeFreeze : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_ThreeFreeze")]
        public static float m_baseTTL = 3f;
        private float speedAmount =0f;
        private float m_timer = 1f;

        public SE_ThreeFreeze()
        {
            base.name = "SE_ThreeFreeze";
            m_icon = AbilityIcon;
            m_tooltip = "Three Freeze";
            m_name = "Three Freeze";
            m_ttl = m_baseTTL;
        }

        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            speed *= speedAmount;
            base.ModifySpeed(baseSpeed, ref speed);
        }

        public override bool CanAdd(Character character)
        {
            return true;
        }

    }
}


