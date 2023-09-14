using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;



namespace LackingImaginationV2

{
    [HarmonyPatch(typeof(GuiBar), "Awake")]
    public static class GuiBar_Awake_Patch
    {
        private static bool Prefix(GuiBar __instance)
        {
            // I have no idea why this bar is set to zero initially
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (__instance.name == "durability" && __instance.m_bar.sizeDelta.x != 54)
            {
                __instance.m_bar.sizeDelta = new Vector2(54, 0);
            }
    
            return true;
        }
    }
    
    [HarmonyPatch(typeof(InventoryGrid), "GetElement", typeof(int), typeof(int), typeof(int))]
    public static class InventoryGrid_GetElement_Patch
    {
        private static bool Prefix(InventoryGrid __instance, ref InventoryGrid.Element __result, int x, int y, int width)
        {
            var index = y * width + x;
            if (index < 0 || index >= __instance.m_elements.Count)
            {
                LackingImaginationV2Plugin.LogError($"Tried to get element for item ({x}, {y}) in inventory ({__instance.m_inventory.m_name}) but that element is outside the bounds!");
                __result = null;
            }
            else
            {
                __result = __instance.m_elements[index];
            }

            return false;
        }
    }


    [HarmonyPatch(typeof(InventoryGrid), "UpdateGui", typeof(Player), typeof(ItemDrop.ItemData))]
    public static class EssenceInventoryGridLI
    {
        private static void Postfix(InventoryGrid __instance)
        {
            if (__instance.name == "EssenceSlotGrid") //PlayerGrid
            {
                // LackingImaginationV2Plugin.Log($"OnSelected: start of GridLI");
                var horizontalSpacing = __instance.m_elementSpace + 10;
                string[] equipNames = { "1", "2", "3", "4", "5" };
                Vector2[] equipPositions = {
                    new Vector2(), // 1
                    new Vector2(horizontalSpacing, 0), // 2
                    new Vector2(2 * horizontalSpacing , 0), // 3
                    new Vector2(3 * horizontalSpacing, 0), // 4
                    new Vector2(4 * horizontalSpacing, 0), // 5
                    
                };
    
                for (var i = 0; i < LackingImaginationV2Plugin.EquipSlotCount; ++i)
                {
                    var element = __instance.m_elements[i];
    
                    var bindingText = element.m_go.transform.Find("binding").GetComponent<Text>();
                    bindingText.enabled = true;
                    
                    //bindingText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    bindingText.text = equipNames[i];
                    bindingText.rectTransform.anchoredPosition = new Vector2(32, 5);
                    
                    var offset = new Vector2(-140, -5);
                    
                      element.m_go.RectTransform().anchoredPosition = offset + equipPositions[i];
                      // LackingImaginationV2Plugin.Log($"OnSelected: end of GridLI");
                    
                }
            }
        }
    }
    
    public static class GameObjectExtensions
    {
        public static RectTransform RectTransform(this GameObject go)
        {
            return go.transform as RectTransform;
        }

        public static T RequireComponent<T>(this GameObject go) where T:Component
        {
            var c = go.GetComponent<T>();
            return c == null ? go.AddComponent<T>() : c;
        }
    }
}
