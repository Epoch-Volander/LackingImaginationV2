using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace LackingImaginationV2 
{

    public class CooldownStatusEffects_A1 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.AbilitySprites[0];

        public CooldownStatusEffects_A1()
        {
            base.name = "Ability1_CoolDown";
            m_icon = LackingImaginationV2Plugin.AbilitySprites[0];
            m_tooltip = LackingImaginationV2Plugin.AbilityNames[0] + " Cooldown";
            m_name = LackingImaginationV2Plugin.AbilityNames[0] + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }

    public class CooldownStatusEffects_A2 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.AbilitySprites[1];

        public CooldownStatusEffects_A2()
        {
            base.name = "Ability2_CoolDown";
            m_icon = LackingImaginationV2Plugin.AbilitySprites[1];
            m_tooltip = LackingImaginationV2Plugin.AbilityNames[1] + " Cooldown";
            m_name = LackingImaginationV2Plugin.AbilityNames[1] + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class CooldownStatusEffects_A3 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.AbilitySprites[2];

        public CooldownStatusEffects_A3()
        {
            base.name = "Ability3_CoolDown";
            m_icon = LackingImaginationV2Plugin.AbilitySprites[2];
            m_tooltip = LackingImaginationV2Plugin.AbilityNames[2] + " Cooldown";
            m_name = LackingImaginationV2Plugin.AbilityNames[2] + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class CooldownStatusEffects_A4 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.AbilitySprites[3];

        public CooldownStatusEffects_A4()
        {
            base.name = "Ability4_CoolDown";
            m_icon = LackingImaginationV2Plugin.AbilitySprites[3];
            m_tooltip = LackingImaginationV2Plugin.AbilityNames[3] + " Cooldown";
            m_name = LackingImaginationV2Plugin.AbilityNames[3] + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class CooldownStatusEffects_A5 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.AbilitySprites[4];

        public CooldownStatusEffects_A5()
        {
            base.name = "Ability5_CoolDown";
            m_icon = LackingImaginationV2Plugin.AbilitySprites[4];
            m_tooltip = LackingImaginationV2Plugin.AbilityNames[4] + " Cooldown";
            m_name = LackingImaginationV2Plugin.AbilityNames[4] + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class RitualProjectileCooldown : StatusEffect
    {
        [Header("SE_RitualProjectileCooldown")]
        public static float m_baseTTL = 2f;
        
        public RitualProjectileCooldown()
        {
            base.name = "RitualProjectileCooldown";
            m_ttl = m_baseTTL;
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    
    
    
}