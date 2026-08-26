using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class AddItemAction : SequenceAction
{
    public List<ItemStack> items;

    public override IEnumerator Execute()
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("[AddItemAction] Item is not assigned. Action skipped.");
            yield break;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("[AddItemAction] Inventory instance is null. Cannot add item.");
            yield break;
        }
        foreach (ItemStack item in items) {
            if (item != null) {
                Inventory.Instance.AddItem(item.item, item.amount);
            }
            else
            {
                Debug.LogWarning("[AddItemAction] One of the items in the list is null. Skipping this item.");
            }
           
        }
        yield return null;
    }
}
