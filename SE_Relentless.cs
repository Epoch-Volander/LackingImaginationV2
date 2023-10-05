using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Relentless : SE_Stats
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Relentless")]
        public static float m_baseTTL = LackingImaginationUtilities.xDeathsquitoCooldownTime * LackingImaginationGlobal.c_deathsquitoRelentlessSED;
        

        public SE_Relentless()
        {
            base.name = "SE_Relentless";
            m_icon = AbilityIcon;
            m_tooltip = "Relentless";
            m_name = "Relentless";
            m_ttl = m_baseTTL;
            m_stealthModifier = 0.1f;
            m_noiseModifier = 3f;

        }
        
        public override void ModifyNoise(float baseNoise, ref float noise) => noise += baseNoise * this.m_noiseModifier;
        
        public override void ModifyStealth(float baseStealth, ref float stealth) => stealth += baseStealth * this.m_stealthModifier;
        
        
        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }

    }
}


