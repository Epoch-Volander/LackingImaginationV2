using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_FumesSplit : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_FumesSplit")]
        public static float m_baseTTL = 600f;
        

        public SE_FumesSplit()
        {
            base.name = "SE_FumesSplit";
            m_icon = AbilityIcon;
            m_tooltip = "Fumes Split: At 50% hp, spawn 2 ally blobs to aid you";
            m_name = "Fumes \nSplit";
            m_ttl = m_baseTTL;
           

        }
        
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


