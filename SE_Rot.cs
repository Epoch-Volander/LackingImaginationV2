using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Rot : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Rot")]
        // public static float m_blood = int.Parse(xLeechEssencePassive.LeechStats[0]);
        public static float m_blood = 200f;
        
        
        public SE_Rot()
        {
            base.name = "SE_Rot";
            m_icon = AbilityIcon;
            m_tooltip = "Rot";
            m_name = "Rot";
            // m_startEffects = ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2;

        }

        public override void Setup(Character character) => base.Setup(character);
        
        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            
            if(xDraugrRot.RotStats[0] == "100")
            {
                speed *= 0.5f;
            }
            if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence"))
            {
                speed *= 1.1f;
            }
            base.ModifySpeed(baseSpeed, ref speed);
        }
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


