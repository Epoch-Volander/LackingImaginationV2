using System;
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

    public class xBoarEssence
    {
        public static string Ability_Name = "PH";
        public static void Process_Input(Player player)
        {
            System.Random rnd = new System.Random();
            Vector3 pVec = default(Vector3);
            
                LackingImaginationV2Plugin.Log($"Wolf Button was pressed");
            
                
                
           
            
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