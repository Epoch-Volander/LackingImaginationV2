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
        public class Flip : MonoBehaviour
        {
            private const float RotationSpeed = 720;
            public void Awake()
            {
                transform.Rotate(-90, 270, 0);
            }
            public void Update()
            {
                transform.Rotate(0, -RotationSpeed * Time.deltaTime, 0);
            }
        }
        
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


        public static void BiomeExpMethod(Heightmap.Biome mapBiome, List<string> biomeKey)
        {
            Tutorial.TutorialText tutorialText =
                Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == biomeKey[0]));
            if (tutorialText != null)
            {
                if (mapBiome == Player.m_localPlayer.m_currentBiome && !Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label))
                {

                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft,
                        $"{Player.m_localPlayer.m_currentBiome.ToString().ToLower()} exp gained");
                    // Debug.Log($"Current Biome: {Player.m_localPlayer.m_currentBiome.ToString().ToLower()}");
                    
                    Player.m_localPlayer.ShowTutorial(biomeKey[0]);
                    // Tutorial.instance.ShowText(biomeKey, true);
                    // Player.m_localPlayer.SetSeenTutorial(biomeKey);
                    ImaginationExpIncrease(int.Parse(biomeKey[1]));
                    Player.m_localPlayer.AddKnownText(tutorialText.m_label, tutorialText.m_text);
                    
                }
            }
        }
        
        
        public static void dungeonExpMethod(List<string> tutorialValue)
        {
            Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == tutorialValue[0]));
            if (tutorialText != null)
            {
                if (!Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label))
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"{tutorialText.m_topic} exp gained");
                    
                    Player.m_localPlayer.ShowTutorial(tutorialValue[0]);
                    // Tutorial.instance.ShowText("InfectedMine_Exp", true);
                    // Player.m_localPlayer.SetSeenTutorial("InfectedMine_Exp");
                    ImaginationExpIncrease(int.Parse(tutorialValue[1]));
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
            int total = (int)Player.m_localPlayer.GetSkillLevel(Skill.fromName("Imagination"));
            
            int slots;
            if (total >= 80) slots = 5;
            else if (total >= 60 && total < 80) slots = 4;
            else if (total >= 40 && total < 60) slots = 3;
            else if (total >= 20 && total < 40) slots = 2;
            else slots = 1;
            
            return slots;
        }

        // [HarmonyPostfix]
        // public static void Postfix(SkillsDialog __instance, Player player)
        // {
        //     // Check if this is the skill you want to modify
        //     if (__instance.m_elements.Count > 0)
        //     {
        //         // Find the skill you want to modify within the skillList
        //         Skills.Skill skillToModify = player.GetSkills().GetSkillList().Find(skill => skill.m_info.m_skill == SkillType.Swords);
        //
        //         if (skillToModify != null)
        //         {
        //             // Increase the skill level cap here
        //             skillToModify.m_info.m_maxLevel = NewSkillLevelCap;
        //         }
        //     }
        // }
        
        
        
        
        // public static int SkillLevelCalculator()
        // {
        //     int biome = 0;
        //     int location = 0;
        //     int trophy = 0;
        //     
        //     if (Player.m_localPlayer != null && Tutorial.instance != null)
        //     {
        //         foreach (KeyValuePair< Heightmap.Biome, List<string>> kvp in LackingImaginationV2Plugin.biomeDictionary)
        //         {
        //             Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Value[0]));
        //             if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label)) biome += int.Parse(kvp.Value[1]);
        //         }
        //         
        //         HashSet<string> uniqueValues = new HashSet<string>();
        //         foreach (KeyValuePair<string, List<string>> kvp in LackingImaginationV2Plugin.locationDictionary)
        //         {
        //             Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Value[0]));
        //             if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label))
        //             {
        //                 string valueToAdd = kvp.Value[0];
        //                 if (!uniqueValues.Contains(valueToAdd))
        //                 {
        //                     uniqueValues.Add(valueToAdd);
        //                     location += int.Parse(kvp.Value[1]);
        //                 }
        //             }
        //         }
        //
        //         foreach (KeyValuePair<string, List<string>> kvp in LackingImaginationV2Plugin.trophyDictionary)
        //         {
        //             Tutorial.TutorialText tutorialText = Tutorial.instance.m_texts.Find((Predicate<Tutorial.TutorialText>)(x => x.m_name == kvp.Value[0]));
        //             if (Player.m_localPlayer.m_knownTexts.ContainsKey(tutorialText.m_label)) trophy += int.Parse(kvp.Value[1]);
        //         }
        //     }
        //     // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={biome}");
        //     // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={dungeon}");
        //     // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={trophy}");
        //     int total =  biome + location + trophy;
        //     int slots;
        //     if (total >= 80) slots = 5;
        //     else if (total >= 60 && total < 80) slots = 4;
        //     else if (total >= 40 && total < 60) slots = 3;
        //     else if (total >= 20 && total < 40) slots = 2;
        //     else slots = 1;
        //
        //     return slots;
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