using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine;

namespace LackingImaginationV2
{
    public static class EssenceInventoryGuiLI
    {
        public static InventoryGrid EssenceSlotGrid;

        public static GameObject EssencePanel;

        [HarmonyPatch(typeof(InventoryGui), "Awake")]
        
        public static class InventoryGui_Awake_Patch
        {
            public static void Postfix(InventoryGui __instance)
            {
                BuildEquipmentSlotGrid(__instance);
            }
            private static void BuildEquipmentSlotGrid(InventoryGui inventoryGui)
            {
                // var pos = new Vector2(350, -880);
                // BuildInventoryGrid(ref EssenceSlotGrid, "EssenceSlotGrid", pos, new Vector2(450, 100), inventoryGui);
                var pos = new Vector2(100, -250);
                BuildInventoryGrid(ref EssenceSlotGrid, "EssenceSlotGrid", pos, new Vector2(450, 100), inventoryGui);
            }

            private static void BuildInventoryGrid(ref InventoryGrid grid, string name, Vector2 position, Vector2 size, InventoryGui inventoryGui)
            {
                if (grid != null)
                {
                    UnityEngine.Object.Destroy(grid);
                    grid = null;
                }

                if (EssencePanel == null)
                {
                    EssencePanel = new GameObject("Essences", typeof(RectTransform));
                    EssencePanel.transform.SetParent(inventoryGui.m_player);
                    var rt = (RectTransform)EssencePanel.transform;
                    rt.anchorMin = new Vector2(0, 1);   
                    rt.anchorMax = new Vector2(0, 1);
                    // rt.anchoredPosition = new Vector2(752, -166);
                    float xMod = (float)(Screen.width / 1920f);
                    float yMod = (float)(Screen.height / 1080f);
                    float xStep = 200f * xMod;                
                    float yStep = -780f;
                    float yOffset = (106f * yMod) + LackingImaginationV2Plugin.icon_Y_Offset.Value;
                    float xOffset = (209f * xMod) + LackingImaginationV2Plugin.icon_X_Offset.Value;
                    
                    rt.anchoredPosition = new Vector2(xOffset + xStep, yOffset + yStep);
                    // rt.anchoredPosition = new Vector2(752, -850);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 255);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 352);
                    
                }

                var go = new GameObject(name, typeof(RectTransform));
                go.transform.SetParent(EssencePanel.transform, false);

                grid = go.AddComponent<InventoryGrid>();
                var root = new GameObject("Root", typeof(RectTransform));
                root.transform.SetParent(go.transform, false);

                var rect = (RectTransform)go.transform;
                rect.anchoredPosition = position;

                grid.m_elementPrefab = inventoryGui.m_playerGrid.m_elementPrefab;
                grid.m_gridRoot = root.transform as RectTransform;
                grid.m_elementSpace = inventoryGui.m_playerGrid.m_elementSpace;
                grid.ResetView();

                if (name == "EssenceSlotGrid")
                {
                    grid.m_onSelected += OnSelected(inventoryGui);
                    // grid.m_onRightClick += OnRightClicked(inventoryGui);
                }

                grid.m_uiGroup = grid.gameObject.AddComponent<UIGroupHandler>();
                grid.m_uiGroup.m_groupPriority = 1;
                grid.m_uiGroup.m_active = true;
                
                var highlight = new GameObject("SelectedFrame", typeof(RectTransform));
                highlight.transform.SetParent(go.transform, false);
                highlight.AddComponent<Image>().color = Color.yellow;
                var highlightRT = (RectTransform)highlight.transform;
                highlightRT.SetAsFirstSibling();
                highlightRT.anchoredPosition = new Vector2(0, 0);
                highlightRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + 2);
                highlightRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + 2);
                highlightRT.localScale = new Vector3(1, 1, 1);
                grid.m_uiGroup.m_enableWhenActiveAndGamepad = highlight;
                

                var bkg = inventoryGui.m_player.Find("Bkg").gameObject;
                var background = UnityEngine.Object.Instantiate(bkg, go.transform);
                background.name = name + "Bkg";
                var backgroundRT = (RectTransform)background.transform;
                backgroundRT.SetSiblingIndex(1);
                backgroundRT.anchoredPosition = new Vector2(0, 0);
                backgroundRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                backgroundRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
                backgroundRT.localScale = new Vector3(1, 1, 1);

                var list = inventoryGui.m_uiGroups.ToList();
                list.Insert(2, grid.m_uiGroup);
                inventoryGui.m_uiGroups = list.ToArray();
            }
            

            private static Action<InventoryGrid, ItemDrop.ItemData, Vector2i, InventoryGrid.Modifier> OnSelected(InventoryGui inventoryGui)
            {
                return (InventoryGrid inventoryGrid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod) =>
                {
                    EssenceItemData.equipedEssence = EssenceItemData.GetEquippedEssence();
                    LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={ExpMethods.SkillLevelCalculator()}");
                    // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={inventoryGrid}, item={InventoryGui.m_instance.m_dragItem?.m_shared.m_name}, pos={pos}, mod={mod}");
                     // LackingImaginationV2Plugin.Log($"OnSelected: inventoryGrid={inventoryGrid}, item={item?.m_shared.m_name}, pos={pos.x}, mod={mod}");
                    if (InventoryGui.m_instance.m_dragItem?.m_shared.m_name != null 
                        && LackingImaginationV2Plugin.ItemBundleUnwrapDict.ContainsKey(InventoryGui.m_instance.m_dragItem?.m_shared.m_name) 
                        && !EssenceItemData.equipedEssence.Contains(InventoryGui.m_instance.m_dragItem?.m_shared.m_name) 
                        && !Player.m_localPlayer.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(pos.x))
                        && pos.x < ExpMethods.SkillLevelCalculator())
                    {
                        inventoryGui.OnSelectedItem(inventoryGrid, item, pos, mod);
                        LackingImaginationV2Plugin.Log($"OnSelected: pos={pos.x}");
                        if (Player.m_localPlayer.m_inventory.CanAddItem(item) && item != null)
                        {
                            EssencePlayerData.EssenceSlotInventory.m_inventory.Remove(item);
                            if(LackingImaginationV2Plugin.abilitiesStatus == null)
                            {
                                LackingImaginationV2Plugin.Log("== null 1");
                                LackingImaginationV2Plugin.abilitiesStatus = new List<RectTransform>();
                                LackingImaginationV2Plugin.abilitiesStatus.Clear();    
                                for (int i = 0; i < 5; i++) 
                                {
                                    LackingImaginationV2Plugin.abilitiesStatus.Add(null);
                                }
                            }
                            if (LackingImaginationV2Plugin.abilitiesStatus[pos.x] != null)
                            {
                                // LackingImaginationV2Plugin.Log("== des 1");
                                UnityEngine.Object.Destroy(LackingImaginationV2Plugin.abilitiesStatus[pos.x].gameObject);
                                LackingImaginationV2Plugin.abilitiesStatus[pos.x] = null;
                            }
                        }
                    }
                    else if (InventoryGui.m_instance.m_dragItem?.m_shared.m_name == null 
                             && !Player.m_localPlayer.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(pos.x)))
                    {
                        if (Player.m_localPlayer.m_inventory.CanAddItem(item) && item != null)
                        {
                            EssencePlayerData.EssenceSlotInventory.m_inventory.Remove(item);
                            Player.m_localPlayer.m_inventory.AddItem(item);
                            if(LackingImaginationV2Plugin.abilitiesStatus == null)
                            {
                                LackingImaginationV2Plugin.Log("== null 2");
                                LackingImaginationV2Plugin.abilitiesStatus = new List<RectTransform>();
                                LackingImaginationV2Plugin.abilitiesStatus.Clear();  
                                for (int i = 0; i < 5; i++) 
                                {
                                    LackingImaginationV2Plugin.abilitiesStatus.Add(null);
                                }
                            }
                            if (LackingImaginationV2Plugin.abilitiesStatus[pos.x] != null)
                            {
                                // LackingImaginationV2Plugin.Log("== des 2");
                                UnityEngine.Object.Destroy(LackingImaginationV2Plugin.abilitiesStatus[pos.x].gameObject);
                                LackingImaginationV2Plugin.abilitiesStatus[pos.x] = null;
                            }
                        }
                    }
                    EssenceItemData.equipedEssence = EssenceItemData.GetEquippedEssence();
                    EssenceItemData.equipedEssenceData = EssenceItemData.GetEquippedEssenceData();
                    LackingImaginationUtilities.NameEssence();
                    LackingImaginationUtilities.InitiateEssenceStatus(Hud.m_instance);
                    EssencePlayerData.OnInventoryChanged();
                };
            }
        }
        
        
        [HarmonyPatch(typeof(InventoryGui), "UpdateInventory")]
        public static class InventoryGui_UpdateInventory_Patch
        {
            public static bool Prefix(InventoryGui __instance, Player player)
            {
                // LackingImaginationV2Plugin.Log($"OnSelected:GridLI  "+__instance.m_playerGrid.name);
                __instance.m_playerGrid.UpdateInventory(player.m_inventory, player, __instance.m_dragItem);
                if (EssenceSlotGrid != null)
                {
                    var essenceSlotInventory = player.GetEssenceSlotInventory();
                    if (essenceSlotInventory != null)
                    {
                        EssenceSlotGrid.UpdateInventory(EssencePlayerData.EssenceSlotInventory, player, __instance.m_dragItem);
                    }
                }
                return false;
            }
        }
        
        
        
        //    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateGamepad))]
        // public static class InventoryGui_UpdateGamepad_Patch
        // {
        //     [UsedImplicitly]
        //     public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        //     {
        //         var activeGroupField = AccessTools.DeclaredField(typeof(InventoryGui), "m_activeGroup" );
        //         
        //         var instrs = instructions.ToList();
        //
        //         var counter = 0;
        //         var skipLines = 0;
        //
        //         for (int i = 0; i < instrs.Count; ++i)
        //         {
        //             if (i > 5 && instrs[i].opcode == OpCodes.Ldfld 
        //                 && instrs[i].operand.Equals(activeGroupField) && instrs[i + 1].opcode == OpCodes.Ldc_I4_3 
        //                 && instrs[i + 2].opcode == OpCodes.Bne_Un)
        //             {
        //                 //Replace Field with Call
        //                 yield return LackingImaginationV2Plugin.LogMessage(new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(InventoryGui_UpdateGamepad_Patch), nameof(GetMaxUiGroups))),counter);
        //                 counter++;
        //                 
        //                 //Remove ldc_i4.3
        //                 skipLines++;
        //                 
        //                 //Create new BrTrue
        //                 yield return LackingImaginationV2Plugin.LogMessage(new CodeInstruction(OpCodes.Brtrue, instrs[i +2].operand), counter);
        //                 
        //                 //Remove Bne
        //                 skipLines++;
        //
        //                 i += skipLines;
        //             }
        //             else
        //             {
        //                 yield return LackingImaginationV2Plugin.LogMessage(instrs[i], counter);
        //                 counter++;
        //             }
        //         }
        //     }
        //
        //     public static bool GetMaxUiGroups(InventoryGui instance)
        //     {
        //         return instance.m_activeGroup != instance.m_uiGroups.Length - 1;
        //     }
        // }
    }
}