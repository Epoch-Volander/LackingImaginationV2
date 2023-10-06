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
            // GameObject essencePiece = ItemManager.PrefabManager.RegisterPrefab("essence_bundle_2", list[5]);
            
            MaterialReplacer.RegisterGameObjectForShaderSwap(essenceBase.Prefab.transform.Find("attach").transform.Find(list[5]).gameObject, MaterialReplacer.ShaderType.CustomCreature);
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
        

        public static void ImaginationExpIncrease(int level)
        {
            for (int i = 0; i < level; i++)
            {
                Player.m_localPlayer.RaiseSkill("Imagination", 1000f);
            }
        }


        public static void BiomeExpMethod(string biomeKey, Heightmap.Biome mapBiome)
        {
            Tutorial.TutorialText tutorialText =
                Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == biomeKey));
            if (tutorialText != null)
            {
                if (mapBiome == Player.m_localPlayer.m_currentBiome && !Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label))
                {

                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                        $"{Player.m_localPlayer.m_currentBiome.ToString().ToLower()} exp gained");
                    Debug.Log($"Current Biome: {Player.m_localPlayer.m_currentBiome.ToString().ToLower()}");

                    
                    Player.m_localPlayer.ShowTutorial(biomeKey);
                    // Tutorial.instance.ShowText(biomeKey, true);
                    // Player.m_localPlayer.SetSeenTutorial(biomeKey);
                    ImaginationExpIncrease(2);
                    Player.m_localPlayer.AddKnownText(tutorialText.m_label, tutorialText.m_text);
                    
                }
            }
        }
        
        
        public static void dungeonExpMethod(string tutorialValue)
        {
            Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == tutorialValue));
            if (tutorialText != null)
            {
                // ZLog.Log((object) ("Detected environment change"+env));
                if (!Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label))
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"{tutorialText.m_topic} exp gained");
                    
                    Player.m_localPlayer.ShowTutorial(tutorialValue);
                    // Tutorial.instance.ShowText("InfectedMine_Exp", true);
                    // Player.m_localPlayer.SetSeenTutorial("InfectedMine_Exp");
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




        public static int SkillLevelCalculator()
        {
            int biome = 0;
            int dungeon = 0;
            int trophy = 0;
            
            if (Player.m_localPlayer != null && Tutorial.instance != null)
            {
                foreach (KeyValuePair<string, Heightmap.Biome> kvp in LackingImaginationV2Plugin.biomeDictionary)
                {
                    Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Key));
                    if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label)) biome += 2;
                }

                foreach (KeyValuePair<string, string> kvp in LackingImaginationV2Plugin.dungeonDictionary)
                {
                    Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Value));
                    if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label)) dungeon += 2;
                }

                foreach (KeyValuePair<string, string> kvp in LackingImaginationV2Plugin.dungeonMusicDictionary)
                {
                    Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Value));
                    if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label)) dungeon += 2;
                }

                foreach (KeyValuePair<string, List<string>> kvp in LackingImaginationV2Plugin.trophyDictionary)
                {
                    Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Value[0]));
                    if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label)) trophy += int.Parse(kvp.Value[1]);
                }
            }
            // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={biome}");
            // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={dungeon}");
            // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={trophy}");
            int total =  biome + dungeon + trophy;
            int slots;
            if (total >= 80) slots = 5;
            else if (total >= 60 && total < 80) slots = 4;
            else if (total >= 40 && total < 60) slots = 3;
            else if (total >= 20 && total < 40) slots = 2;
            else slots = 1;

            return slots;
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
         
         public static GameObject DeepCopy(GameObject original)
         {
             GameObject copy = GameObject.Instantiate(original);
             copy.name = original.name;
             Component[] components = original.GetComponents<Component>();
             foreach (Component component in components)
             {
                 Type type = component.GetType();
                 Component copyComponent = copy.GetComponent(type);
                 if (copyComponent == null)
                 {
                     copyComponent = copy.AddComponent(type);
                 }
                 FieldInfo[] fields = type.GetFields();
                 foreach (FieldInfo field in fields)
                 {
                     field.SetValue(copyComponent, field.GetValue(component));
                 }
             }
             foreach (Transform child in original.transform)
             {
                 Transform childCopy = copy.transform.Find(child.name);
                 if (childCopy == null)
                 {
                     childCopy = DeepCopy(child.gameObject).transform;
                     childCopy.name = child.name;
                     childCopy.parent = copy.transform;
                 }
             }
             return copy;
         }
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
    }

}