using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using LocalizationManager;
using LocationManager;
using PieceManager;
using ServerSync;
using SkillManager;
using StatusEffectManager;
using PrefabManager = ItemManager.PrefabManager;
using System.Collections.Generic;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using ItemDataManager;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.Animations;
using System.Collections;
using UnityEngine;



namespace LackingImaginationV2

{
    public static class RPC_LI
    {
        
        [HarmonyPatch(typeof(Game), nameof(Game.Start))]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                // Register the UpdateBooleanOnClients method
                // ZRoutedRpc.instance.Register("UpdateBooleanOnClients", new Action<long, string>(UpdateBooleanOnClients));
                // ZRoutedRpc.instance.Register<Essence, bool>("AnimationCaller", new Action<long, Essence, bool>(RPC_LI.RPC_AnimationCaller));
                
                
               // ZRoutedRpc.instance.Register<string, bool>("AnimationCaller", new Action<long, string, bool>(RPC_LI.RPC_AnimationCaller));
                
                
            }
        }
        
        
        // [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
        // public static class GameStartPatch
        // {
        //     private static void Postfix(Character __instance)
        //     {
        //         
        //         if (__instance.m_nview.GetZDO() != null)
        //         {
        //             if (!__instance.IsPlayer())
        //             {
        //                 __instance.m_tamed = __instance.m_nview.GetZDO().GetBool(ZDOVars.s_tamed, __instance.m_tamed);
        //                 // __instance.m_level = __instance.m_nview.GetZDO().GetInt(ZDOVars.s_level, 1);
        //                 // if (__instance.m_nview.IsOwner() && (double) __instance.GetHealth() == (double) __instance.GetMaxHealth())
        //                 //     __instance.SetupMaxHealth();
        //             }
        //             // __instance.m_nview.Register<HitData>("Damage", new Action<long, HitData>(__instance.RPC_Damage));
        //             // __instance.m_nview.Register<float, bool>("Heal", new Action<long, float, bool>(__instance.RPC_Heal));
        //             // __instance.m_nview.Register<float>("AddNoise", new Action<long, float>(__instance.RPC_AddNoise));
        //             // __instance.m_nview.Register<Vector3>("Stagger", new Action<long, Vector3>(__instance.RPC_Stagger));
        //             // __instance.m_nview.Register("ResetCloth", new Action<long>(__instance.RPC_ResetCloth));
        //             __instance.m_nview.Register<bool>("SetTamed", new Action<long, bool>(__instance.RPC_SetTamed));
        //             // __instance.m_nview.Register<float>("FreezeFrame", new Action<long, float>(__instance.RPC_FreezeFrame));
        //             // __instance.m_nview.Register<Vector3, Quaternion, bool>("RPC_TeleportTo", new Action<long, Vector3, Quaternion, bool>(__instance.RPC_TeleportTo));
        //         }
        //         
        //         
        //     }
        // }
        
        
        
        // public void SetTamed(bool tamed)
        // {
        //     if (!this.m_nview.IsValid() || this.m_tamed == tamed)
        //         return;
        //     this.m_nview.InvokeRPC(nameof (SetTamed), (object) tamed);
        // }
        //
        // public void RPC_SetTamed(long sender, bool tamed)
        // {
        //     if (!this.m_nview.IsOwner() || this.m_tamed == tamed)
        //         return;
        //     this.m_tamed = tamed;
        //     this.m_nview.GetZDO().Set(ZDOVars.s_tamed, this.m_tamed);
        // }
        
        
        
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        public static class GameStartPatch2
        {
            private static void Postfix(Player __instance)
            {
                if (__instance.m_nview.GetZDO() == null)
                    return;
                if (!__instance.m_nview.IsValid())
                    return;
                
                
                if (__instance.m_nview.IsOwner())
                {
                    __instance.m_nview.Register<float, long>("SizeCaller", new Action<long, float, long>(RPC_LI.RPC_SizeCaller));
                    
                    
                     
                    __instance.m_nview.Register<string, bool>("AnimationCaller", new Action<long, string, bool>(RPC_LI.RPC_AnimationCaller));
                    
                    // __instance.m_nview.Register<int, string, int>("Message", new Action<long, int, string, int>(__instance.RPC_Message));
                    // __instance.m_nview.Register<bool, bool>("OnTargeted", new Action<long, bool, bool>(__instance.RPC_OnTargeted));
                    
                    // __instance.m_nview.Register<float>("UseStamina", new Action<long, float>(__instance.RPC_UseStamina));
                    
                    // if ((bool) (UnityEngine.Object) MusicMan.instance)
                    //     MusicMan.instance.TriggerMusic("Wakeup");
                    // Debug.Log("GameStartPatch2 Postfix executed");
                    // Rest of the code
                    
                    
                    // __instance.m_nview.Register("AnimationCaller", new Action<long, Essence, bool>(RPC_LI.RPC_AnimationCaller));
                   
                }
            }
        }

        
        

      
        
        
        

        // public static void AnimationCaller( RPC_LI.Essence essence, bool state)
        public static void AnimationCaller( string essence, bool state)
        {
            if (!Player.m_localPlayer.m_nview.IsValid())
                return;
            if (Player.m_localPlayer.m_nview == null || !Player.m_localPlayer.m_nview.IsValid())
                return;
            
            
            // Player.m_localPlayer.m_nview.InvokeRPC( nameof(AnimationCaller), (object)essence, (object)state);
            
            // ZRoutedRpc.instance.InvokeRoutedRPC(ZNetView.Everybody, nameof(AnimationCaller), (object)essence, (object)state);
            
            
            // if (Player.m_localPlayer.m_nview.IsOwner())
            //     RPC_AnimationCaller(0L, essence, state);
            // else
                Player.m_localPlayer.m_nview.InvokeRPC(ZNetView.Everybody, nameof(AnimationCaller), (object)essence, (object)state);

            
            
            
            
            // if (ZNet.instance.IsServer() && !Player.m_localPlayer)
            // {
            //     Debug.Log("GSev11111111111");
            //     
            //     Player.m_localPlayer.m_nview.InvokeRPC( nameof(AnimationCaller), (object)essence, (object)state);
            //     // RPC_AnimationCaller(0, essence, state);
            // }
            // else 
            // {
            //     if (Player.m_localPlayer.m_nview == null || !Player.m_localPlayer.m_nview.IsValid())
            //         return;
            //     
            //     RPC_AnimationCaller(0, essence, state);
            // }
        }
        
        // public static void RPC_AnimationCaller(long sender, RPC_LI.Essence essence, bool state)
        public static void RPC_AnimationCaller(long sender, string essence, bool state)
        {
            switch (essence)
            {
                case "Geirrhafa":
                    // Update the local boolean
                    xGeirrhafaEssence.GeirrhafaController = state;
                    break;
            
                case "Brenna":
                    xSkeletonSynergy.SkeletonSynergyBrennaController = state;
                    break;
                
                case "RancidRemains":
                    xSkeletonSynergy.SkeletonSynergyRancidController = state;
                    break;
              
                case "Elder":
                    xElderEssence.ElderController = state;
                    break;
                
                case "Moder":
                    xModerEssence.ModerController = state;
                    break;
                
                case "Greydwarf":
                    xGreydwarfShamanEssence.GreydwarfShamanController = state;
                    break;
                
                case "Yagluth2":
                    xYagluthEssence.YagluthController2 = state;
                    break;
              
                case "Yagluth1":
                    xYagluthEssence.YagluthController1 = state;
                    break;
                
                case "Boar":
                    xBoarEssence.BoarController = state;
                    break;
                
                case "Eikthyr":
                    xEikthyrEssence.EikthyrController = state;
                    break;
                
                case "Cultist":
                    xCultistEssence.CultistController = state;
                    break;
                
                case "Surtling":
                    xSurtlingEssence.SurtlingController = state;
                    break;
                
                case "Fenring":
                    xFenringEssence.FenringController = state;
                    break;
            }
            
        }

        public static void SizeCaller(float size, long player)
        {
            if (!Player.m_localPlayer.m_nview.IsValid())
                return;
            if (Player.m_localPlayer.m_nview == null || !Player.m_localPlayer.m_nview.IsValid())
                return;
            
            // if (Player.m_localPlayer.m_nview.IsOwner())
            //     RPC_SizeCaller(0L, size, player);
            // else
                Player.m_localPlayer.m_nview.InvokeRPC(ZNetView.Everybody, nameof(SizeCaller), (object)size, (object)player);
            
            // ZRoutedRpc.instance.InvokeRoutedRPC(ZNetView.Everybody, nameof(SizeCaller), (object)size); 
            
            
        }
        
        public static void RPC_SizeCaller(long sender, float size, long player)
        {
            Player.GetPlayer(player).GetComponent<ZSyncTransform>().m_syncScale = true;
            Player.GetPlayer(player).transform.localScale = size * Vector3.one;
          
        }
        
        
        
        // public override void UseStamina(float v, bool isBaseUsage = false)
        // {
        //     if ((double) v == 0.0)
        //         return;
        //     v *= Game.m_staminaRate;
        //     if (isBaseUsage)
        //         v *= 1f + this.m_equipmentBaseItemModifier;
        //     if (!this.m_nview.IsValid())
        //         return;
        //     if (this.m_nview.IsOwner())
        //         this.RPC_UseStamina(0L, v);
        //     else
        //         this.m_nview.InvokeRPC(nameof (UseStamina), (object) v);
        // }
        //
        // public void RPC_UseStamina(long sender, float v)
        // {
        //     if ((double) v == 0.0)
        //         return;
        //     this.m_stamina -= v;
        //     if ((double) this.m_stamina < 0.0)
        //         this.m_stamina = 0.0f;
        //     this.m_staminaRegenTimer = this.m_staminaRegenDelay;
        // // }
        
        
        
        
        
    }
}