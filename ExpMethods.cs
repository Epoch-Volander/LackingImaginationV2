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
using TMPro;


namespace LackingImaginationV2
{
    public class ExpMethods
    {
        public class Flip : MonoBehaviour //flip thrown weapons
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
        
        public static void CreateEssenceBase(List<string> list)  // item registration
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

        public static void FixMaterials(string key, string value)
        {
            // Find the key and value prefabs by name
            GameObject valuePrefab = ObjectDB.instance?.GetItemPrefab(value);
            GameObject keyPrefab = ObjectDB.instance?.GetItemPrefab(key).transform.Find("attach").transform.Find("Essence"+value).gameObject;

            // GameObject keyPrefab = ObjectDB.instance?.GetItemPrefab(key);
            // if (keyPrefab == null)
            // {
            //     Debug.LogError($"keyPrefab {key} is null");
            // }
            // else
            // {
            //     Transform attachTransform = keyPrefab.transform.Find("attach");
            //     if (attachTransform == null)
            //     {
            //         Debug.LogError($"attachTransform {key} is null");
            //     }
            //     else
            //     {
            //         Transform essenceTransform = attachTransform.transform.Find("Essence" + value);
            //         if (essenceTransform == null)
            //         {
            //             Debug.LogError($"essenceTransform {key} is null");
            //         }
            //         else
            //         {
            //             keyPrefab = essenceTransform.gameObject;
            //         }
            //     }
            // }
             
            if (keyPrefab != null && valuePrefab != null)
            {
                CopyMaterialsRecursively(keyPrefab.transform, valuePrefab.transform);
            }
            else
            {
                Debug.LogError($"key {key} or value {value} prefab not found by name.");
            }
        }
        
        private static void CopyMaterialsRecursively(Transform keyTransform, Transform valueTransform)
        {
            // Copy materials from the value prefab to the key prefab
            MeshRenderer keyRenderer = keyTransform.GetComponent<MeshRenderer>();
            MeshRenderer valueRenderer = valueTransform.GetComponent<MeshRenderer>();

            if (keyRenderer != null && valueRenderer != null)
            {
                Material[] valueMaterials = valueRenderer.sharedMaterials;

                Material[] keyMaterials = new Material[valueMaterials.Length];
                for (int i = 0; i < keyMaterials.Length; i++)
                {
                    if (valueMaterials[i] != null)
                    {
                        keyMaterials[i] = new Material(valueMaterials[i]);
                    }
                }

                keyRenderer.materials = keyMaterials;
            }

            // Recursively process childr
            for (int i = 0; i < keyTransform.childCount; i++)
            {
                Transform keyChild = keyTransform.GetChild(i);
                Transform valueChild = valueTransform.GetChild(i);
                CopyMaterialsRecursively(keyChild, valueChild);
            }
            
            // Copy materials from the value prefab to the key prefab for ParticleSystemRenderer components
            ParticleSystemRenderer keyParticleRenderer = keyTransform.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer valueParticleRenderer = valueTransform.GetComponent<ParticleSystemRenderer>();

            if (keyParticleRenderer != null && valueParticleRenderer != null)
            {
                Material valueMaterial = valueParticleRenderer.material;
                if (valueMaterial != null)
                {
                    keyParticleRenderer.material = new Material(valueMaterial);
                }
            }

            // Recursively process childr
            for (int i = 0; i < keyTransform.childCount; i++)
            {
                Transform keyChild = keyTransform.GetChild(i);
                Transform valueChild = valueTransform.GetChild(i);
                CopyMaterialsRecursively(keyChild, valueChild);
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
        

        public static void ImaginationExpIncrease(int level) // level increase, doesn't overflow
        {
            for (int i = 0; i < level; i++)
            {
                Player.m_localPlayer.RaiseSkill("Imagination", 500000f);
            }
        }


        public static void BiomeExpMethod(Heightmap.Biome mapBiome, List<string> biomeKey)  // biome progress
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
        
        
        public static void dungeonExpMethod(List<string> tutorialValue)  // Location progress
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
        
        public static void TrophyExpMethod(List<string> tutorial, string item, string allItem) // trophy progress
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

        public static int SkillLevelCalculator() // checks your progress
        {
            // total     exp 615
            //                0     0         
            // meadows  x exp 15    15
            // black    x exp 66    81  
            // swamp      exp 69    150 
            // mountain x exp 116   266
            // ocean      exp 12    278
            // plains   x exp 160   438
            // mist     x exp 162   600
            // ash        exp 7     607
            // north      exp 8     615
            int total = (int)Player.m_localPlayer.GetSkillLevel(Skill.fromName("Imagination"));

            int slots;
            if (total >= 550) slots = 5;                      //220
            else if (total >= 330 && total < 550) slots = 4;  //170
            else if (total >= 160 && total < 330) slots = 3; //110
            else if (total >= 50 && total < 160) slots = 2;  //50
            else slots = 1;                                  //0
                                                             
            return slots;
        }

        
        
      

        [HarmonyPatch(typeof(Skills.Skill), nameof(Skills.Skill.Raise))]
        public class Skill_Raise_Patch
        {
            public static bool Prefix(Skills.Skill __instance,ref float factor, ref bool __result)
            {
                if (__instance.m_info.m_skill == Skill.fromName("Imagination"))
                {
                    __instance.m_accumulator += __instance.m_info.m_increseStep * factor * Game.m_skillGainRate;
                    if ((double) __instance.m_accumulator < (double) __instance.GetNextLevelRequirement())  __result = false;
                    
                    ++__instance.m_level;
                    __instance.m_level = Mathf.Clamp(__instance.m_level, 0.0f, 1000f);
                    __instance.m_accumulator = 0.0f;
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        
         // private static bool Patch_Skills_CheatRaiseskill(Skills __instance, string name, float value, Player ___m_player)
         // {
         //     foreach (Skills.SkillType id in skills.Keys)
         //     {
         //         Skill skillDetails = skills[id];
         //
         //         if (string.Equals(skillDetails.internalSkillName, name, StringComparison.CurrentCultureIgnoreCase))
         //         {
         //             Skills.Skill skill = __instance.GetSkill(id);
         //             skill.m_level += value;
         //             if (skill.m_info.m_skill != Skill.fromName("Imagination")) skill.m_level = Mathf.Clamp(skill.m_level, 0f, 100f);
         //             else skill.m_level = Mathf.Clamp(skill.m_level, 0f, 1000f);
         //             ___m_player.Message(MessageHud.MessageType.TopLeft,
         //                 "Skill increased " + Localization.instance.Localize("$skill_" + id) + ": " + (int)skill.m_level, 0,
         //                 skill.m_info.m_icon);
         //             Console.instance.Print("Skill " + skillDetails.internalSkillName + " = " + skill.m_level);
         //             return false;
         //         }
         //     }
         //
         //     return true;
         // }
         
        [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))] // [HarmonyPatch(typeof(SkillManager.Skill), nameof(SkillManager.Skill.Patch_Skills_CheatRaiseskill))] //Made this changes to this 
        public class SkillsDialog_Setup_Patch
        {
            public static void Postfix(SkillsDialog __instance,ref Player player)
            {
                List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
                for (int index = 0; index < skillList.Count; ++index)
                {
                    Skills.Skill skill = skillList[index];
                    if(skill.m_info.m_skill == Skill.fromName("Imagination"))
                    {
                        GameObject element = __instance.m_elements[index];
                        
                        float skillLevel = player.GetSkills().GetSkillLevel(skill.m_info.m_skill);
                        Utils.FindChild(element.transform, "leveltext").GetComponent<TMP_Text>().text = ((int) skill.m_level).ToString();
                        TMP_Text component = Utils.FindChild(element.transform, "bonustext").GetComponent<TMP_Text>();
                        if ((double) skillLevel != (double) Mathf.Floor(skill.m_level))
                        {
                            float num2 = skillLevel - skill.m_level;
                            component.text = num2.ToString("+0");
                        }
                        else component.gameObject.SetActive(false);
                        
                        // Here, we modify the levelbar setting.
                        Utils.FindChild(element.transform, "levelbar_total").GetComponent<GuiBar>().SetValue(skillLevel / 1000f);
                        Utils.FindChild(element.transform, "levelbar").GetComponent<GuiBar>().SetValue(skill.m_level / 1000f); // Change 100 to 1000 for the desired effect.
                    }
                }
            }
        }
        
        
        
        
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
        
         
         public static GameObject GetLocationFromZoneSystem(ZoneSystem __instance, string LocationName) //used to find firehole for harbinger
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
         
         public static GameObject DeepCopy(GameObject original) // create a deep copy of a gameObject
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