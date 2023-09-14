using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_BloodSiphon : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_BloodSiphon")]
        // public static float m_blood = int.Parse(xLeechEssencePassive.LeechStats[0]);
        public static float m_blood = 200f;
        
        
        public SE_BloodSiphon()
        {
            base.name = "SE_BloodSiphon";
            m_icon = AbilityIcon;
            m_tooltip = "Blood Siphon";
            m_name = "Blood Siphon";
            // m_startEffects = ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2;

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


