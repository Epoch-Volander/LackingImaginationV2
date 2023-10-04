using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_TerritorialSlumber : StatusEffect
    {
        
        public static Sprite AbilityIcon;
        
        

        [Header("SE_TerritorialSlumber")]
        public static float m_blood = 200f;
        public static int Comfort;
        
        
        
        public SE_TerritorialSlumber()
        {
            base.name = "SE_TerritorialSlumber";
            m_icon = AbilityIcon;
            m_tooltip = "Territorial \nSlumber";
            m_name = "Territorial \nSlumber";
           

        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

        public override void UpdateStatusEffect(float dt)
        {
            if (Player.m_localPlayer != null  && Player.m_localPlayer.GetSEMan().HaveStatusEffect("Rested".GetStableHashCode()))
            {
                if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Resting".GetStableHashCode()))
                {
                    int newComfort = Player.m_localPlayer.GetComfortLevel();
                    Comfort = Math.Max(Comfort, newComfort);
                }
            }
            else
            {
                Comfort = 0;
            }


        }

        public override string GetIconText() => Comfort.ToString();
    }
}


