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
            m_tooltip = "Blood Well";
            m_name = "Blood Well";
            // m_startEffects = ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2;

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


