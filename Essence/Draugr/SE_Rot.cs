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
        public static float rot = 1f;
        
        
        public SE_Rot()
        {
            if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence")) rot -= xDraugrEssencePassive.DraugrRot;
            if (EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence")) rot -= xDraugrEliteEssencePassive.DraugrEliteRot;
            if (EssenceItemData.equipedEssence.Contains("$item_draugr_essence") && EssenceItemData.equipedEssence.Contains("$item_draugrelite_essence"))
            {
                rot -= LackingImaginationGlobal.c_draugrSynergyRot;
            }
            
            base.name = "SE_Rot";
            m_icon = AbilityIcon;
            m_tooltip = "Rot: A portion of damage (" +((rot) * 100f).ToString("0") + "%) is reduced & is instead gained as Rot.";
            m_name = "Rot";
           
            

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


