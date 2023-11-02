using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;



namespace LackingImaginationV2
{
    //  public static class EquipmentSlotHelper
    // {
    //     public static bool DoingEquip = false;
    //     public static bool AllowMove = true;
    //
    //     public class SwapData
    //     {
    //         public Inventory InventoryA;
    //         public ItemDrop.ItemData Item;
    //         public Inventory InventoryB;
    //         public Vector2i SlotB;
    //
    //         public SwapData(Inventory inventoryA, ItemDrop.ItemData item, Inventory inventoryB, Vector2i slotB)
    //         {
    //             InventoryA = inventoryA;
    //             Item = item;
    //             InventoryB = inventoryB;
    //             SlotB = slotB;
    //         }
    //     }
    //
    //     public static void Swap(Inventory inventoryA, ItemDrop.ItemData item, Inventory inventoryB, Vector2i slotB)
    //     {
    //         var slotA = item.m_gridPos;
    //         if (inventoryA == inventoryB && item.m_gridPos == slotB)
    //         {
    //             LackingImaginationV2Plugin.Log("Item already in correct slot");
    //             return;
    //         }
    //
    //         var otherItemInSlot = inventoryB.GetItemAt(slotB.x, slotB.y);
    //         if (otherItemInSlot != null)
    //         {
    //             LackingImaginationV2Plugin.Log($"Item exists in other slot ({otherItemInSlot.m_shared.m_name})");
    //             inventoryB.m_inventory.Remove(otherItemInSlot);
    //         }
    //
    //         inventoryA.m_inventory.Remove(item);
    //         inventoryB.m_inventory.Add(item);
    //         item.m_gridPos = slotB;
    //
    //         if (otherItemInSlot != null)
    //         {
    //             otherItemInSlot.m_gridPos = slotA;
    //             inventoryA.m_inventory.Add(otherItemInSlot);
    //         }
    //
    //         inventoryA.Changed();
    //         inventoryB.Changed();
    //     }
    // }
}