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
using UnityEngine;
using PrefabManager = ItemManager.PrefabManager;
using Range = LocationManager.Range;
using System.Collections.Generic;
using System.CodeDom;
using System.Linq;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using ItemDataManager;
using System.Threading;




namespace LackingImaginationV2
{
    public class ExpMethods
    {
        
        
        
        public static void CreateEssenceBase(List<string> list)
        {
            Item essenceBase = new("essence_bundle_2", list[0]);
            
            essenceBase.Name.English(list[1]);
            essenceBase.DropsFrom.Add(list[2], float.Parse(list[3]));
            // Debug.Log($"Current Drop target: {list[2]}");
            essenceBase.Description.English(list[4]);
            essenceBase.Prefab.AddComponent<EssenceMotion>();

            // GameObject essencePiece = new();
            // essencePiece = PrefabManager.RegisterPrefab("essence_bundle_2", list[5]);

            // essenceBase.Prefab.GetComponent(PiecePrefabManager);
             // MaterialReplacer.RegisterGameObjectForShaderSwap(essencePiece, MaterialReplacer.ShaderType.CustomCreature);
        }

        public static void ImaginationExpIncrease(int level)
        {
            for (int i = 0; i < level; i++)
            {
                Player.m_localPlayer.RaiseSkill("Imagination", 1000f);
            }
        }
        

        public static void BiomeExpMethod(string biomeKey, Heightmap.Biome mapBiome)
        {
            if (mapBiome == Player.m_localPlayer.m_currentBiome && !Player.m_localPlayer.HaveSeenTutorial(biomeKey))
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                    $"{Player.m_localPlayer.m_currentBiome.ToString().ToLower()} exp gained");
                Debug.Log($"Current Biome: {Player.m_localPlayer.m_currentBiome.ToString().ToLower()}");

                Tutorial.TutorialText tutorialText =
                    Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == biomeKey));
                if (tutorialText != null)
                {
                    // Player.m_localPlayer.ShowTutorial(biomeKey);
                    Tutorial.instance.ShowText(biomeKey, true);
                    Player.m_localPlayer.SetSeenTutorial(biomeKey);
                    ImaginationExpIncrease(2);
                    Player.m_localPlayer.AddKnownText(tutorialText.m_label, tutorialText.m_text);
                }
            }
        }

        
         public static void TrophyExpMethod(List<string> tutorial, string item, string allItem)
         {
             if (allItem == item)
             {
                 if (!Player.m_localPlayer.HaveSeenTutorial(tutorial[0]))
                 {
                     Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>) (x => x.m_name == tutorial[0]));
                     if (tutorialText != null)
                     {
                         Player.m_localPlayer.AddKnownText(tutorialText.m_label, tutorialText.m_text);
                         ImaginationExpIncrease(int.Parse(tutorial[1]));
                         Player.m_localPlayer.SetSeenTutorial(tutorial[0]);
                     }
                 }
             }
         }
         
         public static void LogGameObjectInfo(GameObject go, string indent = "")
         {
             // Log information about the current GameObject
             Debug.Log(indent + "GameObject: " + go.name);

             // Log information about each component on the current GameObject
             foreach (Component component in go.GetComponents<Component>())
             {
                 Debug.Log(indent + " - Component: " + component.GetType().Name);
             }

             // Recursively inspect the children of the current GameObject
             foreach (Transform child in go.transform)
             {
                 LogGameObjectInfo(child.gameObject, indent + "   ");
             }
         }

         // GameObject bossStonePrefab = ZNetScene.instance.GetPrefab("BossStone_Eikthyr");
         //     if (bossStonePrefab != null)
         // {
         //     // Try to find the BossStone component on the prefab
         //     BossStone bossStoneComponent = bossStonePrefab.GetComponent<BossStone>();
         //
         //     if (bossStoneComponent != null)
         //     {
         //         // Now you can access m_activateStep2
         //         EffectList activateStep2 = bossStoneComponent.m_activateStep2;
         //
         //         // Use activateStep2 as needed
         //         Debug.Log("m_activateStep2 contains: " + activateStep2);
         //     }
         //     else
         //     {
         //         Debug.LogError("BossStone component not found on BossStone_Eikthyr prefab.");
         //     }
         // }
         
         
         public static GameObject GetLocationFromZoneSystem(ZoneSystem __instance, string LocationName)
         {
             foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
             {
                 if (gameObject.name == "_Locations" && gameObject.transform.Find("Misc") is { } locationMisc)
                 {
                     // LackingImaginationV2Plugin.Log($"Location Misc: {locationMisc.childCount}");
                     foreach (Transform locationTransform in locationMisc)
                     {
                         if (locationTransform.name == LocationName)
                         {
                             return locationTransform.gameObject;
                         }
                     }
                 }
             }
             return null; // Return null if the location is not found.
         }

         // public static void DestroyNow(Character surt)
         // {
         //     if (!surt.m_nview.IsValid() || !surt.m_nview.IsOwner())
         //         return;
         //     surt.GetComponent<Character>().ApplyDamage(new HitData()
         //     {
         //         m_damage = {
         //             m_damage = 99999f
         //         },
         //         m_point = surt.transform.position
         //     }, false, true);
         // }
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
    }

}