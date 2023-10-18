using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LackingImaginationV2

{
    public class SE_Bash : StatusEffect
    {
        
        public static Sprite AbilityIcon;

        [Header("SE_Bash")]
        public static float m_blood = 200f;
        
        
        public SE_Bash()
        {
            base.name = "SE_Bash";
            m_icon = AbilityIcon;
            m_tooltip = "Bash:  Your next melee hit is empowered.";
            m_name = "Bash";
            // m_startEffects = ZNetScene.instance.GetPrefab("BossStone_Eikthyr").GetComponent<BossStone>().m_activateStep2;

            // m_startEffects = new EffectList
            // {
            //     m_effectPrefabs = new EffectList.EffectData[]
            //     {
            //         new()
            //         {
            //             m_prefab = LackingImaginationV2Plugin.fx_Bash,
            //             m_enabled = true,
            //             m_variant = -1,
            //             m_attach = true,
            //             // m_follow = true,
            //         }
            //     }
            // };
            //
            // m_stopEffects = new EffectList();
        }

        public override bool CanAdd(Character character)
        {
            return character.IsPlayer();
        }
        
    }
}


