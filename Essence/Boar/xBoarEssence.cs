using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Threading;


namespace LackingImaginationV2
{

    public class xBoarEssence//charging effect, shield
    {
        public static string Ability_Name = "Reckless \nCharge";

        public static bool BoarController = false;
       
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
               
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xBoarCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
               
                //Effects, animations, and sounds
                UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_boar_death"), player.transform.position, Quaternion.identity);
                GameObject odin = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_odin_despawn"), player.transform.position, Quaternion.identity);
                ParticleSystem.MainModule mainModule = odin.transform.Find("smoke_expl").GetComponent<ParticleSystem>().main;
                mainModule.startColor = new Color(0.8f,0.7f, 0.1f,0.7f);
                
                //Apply effects
                SE_RecklessCharge se_recklesscharge = (SE_RecklessCharge)ScriptableObject.CreateInstance(typeof(SE_RecklessCharge));
                player.GetSEMan().AddStatusEffect(se_recklesscharge);
                
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
    }

    [HarmonyPatch]
    public class xBoarEssencePassive
    {
        private static float nearFire;
        private static EffectArea FireArea;

        [HarmonyPatch(typeof(Player), (nameof(Player.UpdateEnvStatusEffects)))]
        public static class Boar_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_boar_essence") && !__instance.GetSEMan().HaveStatusEffect("SE_Courage"))
                {
                    EffectArea effectArea = EffectArea.IsPointInsideArea(__instance.transform.position, EffectArea.Type.Fire, 3f);
                    if ((bool)(UnityEngine.Object)effectArea)
                    {
                        nearFire = Time.time;
                        FireArea = effectArea;
                    }
                    if ((double) Time.time - (double) nearFire < 6.0 && (bool) (UnityEngine.Object) FireArea)
                    {
                        LackingImaginationV2Plugin.UseGuardianPower = false;
                        xBoarEssence.BoarController = true;
                        ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Player.m_localPlayer)).SetTrigger("gpower");
                        xBoarEssence.BoarController = false;
                        nearFire = 0f;
                        Player.m_localPlayer.GetSEMan().AddStatusEffect("SE_Courage".GetStableHashCode());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        public static class Boar_GetTotalFoodValue_Patch
        {
            public static void Postfix(Player __instance, ref float stamina)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_boar_essence"))
                {
                    stamina += LackingImaginationGlobal.c_boarRecklessChargePassive;
                    if(__instance.GetSEMan().HaveStatusEffect("SE_Courage")) stamina += LackingImaginationGlobal.c_boarRecklessChargePassive;
                }
            }
        }

        
        
        
        


    }
}




// Cower (animation clip)
//Running(animation clip)

// public float moveSpeed = 5.0f; // Adjust the speed as needed ///GPT
// private Vector3 moveDirection;
//
// void Update()
// {
//     // Get input from the player (e.g., arrow keys or WASD)
//     float horizontalInput = Input.GetAxis("Horizontal");
//     float verticalInput = Input.GetAxis("Vertical");
//
//     // Calculate the movement direction based on player input
//     moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;
//
//     // If the player has input, move the player
//     if (moveDirection != Vector3.zero)
//     {
//         // Translate the player's position based on the direction and speed
//         transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
//     }
// }





// public bool AvoidFire(float dt, Character moveToTarget, bool superAfraid)//assembly
// {
//     if (this.m_character.IsTamed())
//         return false;
//     if (superAfraid)
//     {
//         EffectArea effectArea = EffectArea.IsPointInsideArea(this.transform.position, EffectArea.Type.Fire, 3f);
//         if ((bool) (UnityEngine.Object) effectArea)
//         {
//             this.m_nearFireTime = Time.time;
//             this.m_nearFireArea = effectArea;
//         }
//         if ((double) Time.time - (double) this.m_nearFireTime < 6.0 && (bool) (UnityEngine.Object) this.m_nearFireArea)
//         {
//             this.SetAlerted(true);
//             this.Flee(dt, this.m_nearFireArea.transform.position);
//             return true;
//         }
//     }
//     else
//     {
//         EffectArea effectArea = EffectArea.IsPointInsideArea(this.transform.position, EffectArea.Type.Fire, 3f);
//         if ((bool) (UnityEngine.Object) effectArea)
//         {
//             if ((UnityEngine.Object) moveToTarget != (UnityEngine.Object) null && (bool) (UnityEngine.Object) EffectArea.IsPointInsideArea(moveToTarget.transform.position, EffectArea.Type.Fire))
//             {
//                 this.RandomMovementArroundPoint(dt, effectArea.transform.position, (float) ((double) effectArea.GetRadius() + 3.0 + 1.0), this.IsAlerted());
//                 return true;
//             }
//             this.RandomMovementArroundPoint(dt, effectArea.transform.position, (float) (((double) effectArea.GetRadius() + 3.0) * 1.5), this.IsAlerted());
//             return true;
//         }
//     }
//     return false;
// }