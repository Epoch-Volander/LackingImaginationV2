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
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.Ability1_Sprite;

        public CooldownStatusEffects_A1()
        {
            base.name = "Ability1_CoolDown";
            m_icon = LackingImaginationV2Plugin.Ability1_Sprite;
            m_tooltip = LackingImaginationV2Plugin.Ability1_Name + " Cooldown";
            m_name = LackingImaginationV2Plugin.Ability1_Name + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }

    public class CooldownStatusEffects_A2 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.Ability2_Sprite;

        public CooldownStatusEffects_A2()
        {
            base.name = "Ability2_CoolDown";
            m_icon = LackingImaginationV2Plugin.Ability2_Sprite;
            m_tooltip = LackingImaginationV2Plugin.Ability2_Name + " Cooldown";
            m_name = LackingImaginationV2Plugin.Ability2_Name + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class CooldownStatusEffects_A3 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.Ability3_Sprite;

        public CooldownStatusEffects_A3()
        {
            base.name = "Ability3_CoolDown";
            m_icon = LackingImaginationV2Plugin.Ability3_Sprite;
            m_tooltip = LackingImaginationV2Plugin.Ability3_Name + " Cooldown";
            m_name = LackingImaginationV2Plugin.Ability3_Name + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class CooldownStatusEffects_A4 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.Ability4_Sprite;

        public CooldownStatusEffects_A4()
        {
            base.name = "Ability4_CoolDown";
            m_icon = LackingImaginationV2Plugin.Ability4_Sprite;
            m_tooltip = LackingImaginationV2Plugin.Ability4_Name + " Cooldown";
            m_name = LackingImaginationV2Plugin.Ability4_Name + " Cooldown";           
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
    
    public class CooldownStatusEffects_A5 : StatusEffect
    {
        public static Sprite AbilityIcon = LackingImaginationV2Plugin.Ability5_Sprite;

        public CooldownStatusEffects_A5()
        {
            base.name = "Ability5_CoolDown";
            m_icon = LackingImaginationV2Plugin.Ability5_Sprite;
            m_tooltip = LackingImaginationV2Plugin.Ability5_Name + " Cooldown";
            m_name = LackingImaginationV2Plugin.Ability5_Name + " Cooldown";           
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