using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Forgotten : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Forgotten")]
        // public static float m_blood = int.Parse(xLeechEssencePassive.LeechStats[0]);
        public static float m_blood = 200f;
        
        
        public SE_Forgotten()
        {
            base.name = "SE_Forgotten";
            m_icon = AbilityIcon;
            m_tooltip = "Forgotten";
            m_name = "Forgotten";
            // m_startEffects = ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2;

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
}


