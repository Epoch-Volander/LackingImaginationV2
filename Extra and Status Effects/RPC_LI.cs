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
        


        private static readonly List<string> ControllerCalls = new List<string>()
        {
            "GeirrhafaControllerTrue",
            "GeirrhafaControllerFalse",
            "SkeletonSynergyRancidControllerTrue",
            "SkeletonSynergyRancidControllerFalse",
            "SkeletonSynergyBrennaControllerTrue",
            "SkeletonSynergyBrennaControllerFalse",
        };

        public static void SendStringToServer(string message)
        {
            long senderID = ZRoutedRpc.instance.GetServerPeerID();
            ZRoutedRpc.instance.InvokeRoutedRPC(0L, "HandleStringFromClient", senderID, message);
        }
        
        // Method to check the validity of the string on the server
        public static void HandleStringFromClient(long senderID, string message)
        {
            // Check if the message is valid (add your own validation logic here)
            bool isValid = ControllerCalls.Contains(message);
        
            // Broadcast the string to all clients
            ZRoutedRpc.instance.InvokeRoutedRPC("BroadcastStringToClients", senderID, message, isValid);
        }
        
        // Method to broadcast the string to all clients
        public static void BroadcastStringToClients(long senderID, string message, bool isValid)
        {
            // Update the boolean on each client based on the received string
            if (isValid)
            {
                UpdateBooleanBasedOnString(senderID, message);
            }
        }
        
        // Method to update a boolean on clients based on the received string
        private static void UpdateBooleanBasedOnString(long senderID, string message)
        {
            
                // Broadcast the trigger string to all clients
                ZRoutedRpc.instance.InvokeRoutedRPC("UpdateBooleanOnClients", senderID, message);
        }
        
        public static void UpdateBooleanOnClients(long senderID, string message)
        {
            switch (message)
            {
                case "GeirrhafaControllerTrue":
                    // Update the local boolean
                    xGeirrhafaEssence.GeirrhafaController = true;
                    break;
                case "GeirrhafaControllerFalse":
                    xGeirrhafaEssence.GeirrhafaController = false;
                    break;
                case "SkeletonSynergyRancidControllerTrue":
                    // Update the local boolean
                    xSkeletonSynergy.SkeletonSynergyRancidController = true;
                    break;
                case "SkeletonSynergyRancidControllerFalse":
                    xSkeletonSynergy.SkeletonSynergyRancidController = false;
                    break;
                case "SkeletonSynergyBrennaControllerTrue":
                    // Update the local boolean
                    xSkeletonSynergy.SkeletonSynergyBrennaController = true;
                    break;
                case "SkeletonSynergyBrennaControllerFalse":
                    xSkeletonSynergy.SkeletonSynergyBrennaController = false;
                    break;
                
            }
        }

        
        [HarmonyPatch(typeof(Game), nameof(Game.Start))]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                // Register the UpdateBooleanOnClients method
                ZRoutedRpc.instance.Register("UpdateBooleanOnClients", new Action<long, string>(UpdateBooleanOnClients));
            }
        }
        
        
        
    }
}