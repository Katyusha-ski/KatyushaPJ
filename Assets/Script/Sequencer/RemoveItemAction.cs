using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RemoveItemAction : SequenceAction
{
    [Tooltip("Các item và số lượng cần xoá khỏi Inventory khi cutscene chạy.")]
    public List<ItemStack> items;

    public override IEnumerator Execute()
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("[RemoveItemAction] Item list is empty. Action skipped.");
            yield break;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("[RemoveItemAction] Inventory instance is null. Cannot remove item.");
            yield break;
        }

        foreach (ItemStack item in items)
        {
            if (item == null || item.item == null)
            {
                Debug.LogWarning("[RemoveItemAction] Item entry is null. Skipping this item.");
                continue;
            }

            if (item.amount <= 0)
            {
                Debug.LogWarning($"[RemoveItemAction] Invalid amount for '{item.item.itemName}'. Skipping this item.");
                continue;
            }

            if (item.item.itemType == ItemType.Quest)
            {
                for (int i = 0; i < item.amount; i++)
                    Inventory.Instance.RemoveQuestItem(item.item);
            }
            else
            {
                Inventory.Instance.RemoveItem(item.item, item.amount);
            }
        }

        yield return null;
    }
}
