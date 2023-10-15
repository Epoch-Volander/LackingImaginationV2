using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using System.IO;
using BepInEx.Configuration;
using TMPro;
using UnityEngine.UI;

namespace LackingImaginationV2
{
    public static class CombinationGenerator
    {
        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> elements, int k)
        {
            if (k == 0)
            {
                yield return Enumerable.Empty<T>();
            }
            else if (elements.Any())
            {
                var first = elements.First();
                var remaining = elements.Skip(1);
                foreach (var combination in GetCombinations(remaining, k - 1))
                {
                    yield return new[] { first }.Concat(combination);
                }
                foreach (var combination in GetCombinations(remaining, k))
                {
                    yield return combination;
                }
            }
        }
    }
    
    public static class KeyboardShortcutHandler
    {
        
         private static KeyCode[] movements = new KeyCode[]
         {
             KeyCode.W,
             KeyCode.A,
             KeyCode.S,
             KeyCode.D,
             KeyCode.LeftShift,
         };


         public static bool InputWithCombo(KeyCode hotkey, KeyCode combokey)
         {
             bool atLeastOneCombinationIsTrue = false;
             
             for (int numAdditionalKeys = 0; numAdditionalKeys <= 3; numAdditionalKeys++)
             {
                 // Generate combinations of additional keys
                 IEnumerable<IEnumerable<KeyCode>> combinations = CombinationGenerator.GetCombinations(movements, numAdditionalKeys);

                 // Iterate through the generated combinations
                 foreach (IEnumerable<KeyCode> combination in combinations)
                 {
                     List<KeyCode> combinationKeys = new List<KeyCode>(combination);
                     // Create a KeyboardShortcut instance with the constant and additional keys in the combination

                     KeyboardShortcut test = new KeyboardShortcut(hotkey, combokey);
                     if(combinationKeys.Count == 1)
                     {
                         test = new KeyboardShortcut(hotkey, combokey, combinationKeys[0]);
                     }
                     if(combinationKeys.Count == 2)
                     {
                         test = new KeyboardShortcut(hotkey, combokey, combinationKeys[0], combinationKeys[1]);
                     }
                     if(combinationKeys.Count == 3)
                     {
                         test = new KeyboardShortcut(hotkey, combokey, combinationKeys[0], combinationKeys[1],combinationKeys[2]);
                     }

                     // Check if the combination is pressed and update the flag
                     if (test.IsPressed())
                     {
                         atLeastOneCombinationIsTrue = true;
                         return atLeastOneCombinationIsTrue;
                     }
                 }
             }
             return atLeastOneCombinationIsTrue;
         }

         public static bool InputWithoutCombo(KeyCode hotkey)
         {
             bool atLeastOneCombinationIsTrue = false;
             
             for (int numAdditionalKeys = 0; numAdditionalKeys <= 3; numAdditionalKeys++)
             {
                 // Generate combinations of additional keys
                 IEnumerable<IEnumerable<KeyCode>> combinations = CombinationGenerator.GetCombinations(movements, numAdditionalKeys);

                 // Iterate through the generated combinations
                 foreach (IEnumerable<KeyCode> combination in combinations)
                 {
                     List<KeyCode> combinationKeys = new List<KeyCode>(combination);
                     // Create a KeyboardShortcut instance with the constant and additional keys in the combination

                     KeyboardShortcut test = new KeyboardShortcut(hotkey);
                     if(combinationKeys.Count == 1)
                     {
                         test = new KeyboardShortcut(hotkey, combinationKeys[0]);
                     }
                     if(combinationKeys.Count == 2)
                     {
                         test = new KeyboardShortcut(hotkey, combinationKeys[0], combinationKeys[1]);
                     }
                     if(combinationKeys.Count == 3)
                     {
                         test = new KeyboardShortcut(hotkey, combinationKeys[0], combinationKeys[1], combinationKeys[2]);
                     }

                     // Check if the combination is pressed and update the flag
                     if (test.IsPressed())
                     {
                         atLeastOneCombinationIsTrue = true;
                         return atLeastOneCombinationIsTrue;
                     }
                 }
             }
             return atLeastOneCombinationIsTrue;
         }


         
    }
}