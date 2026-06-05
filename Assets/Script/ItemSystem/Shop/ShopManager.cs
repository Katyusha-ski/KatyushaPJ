using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopEntrySO> entries;

    public bool CanAfford(ShopEntrySO entry)
    {
        foreach (var cost in entry.costs)
        {
            if (Inventory.Instance.GetItemCount(cost.item) < cost.amount)
            {
                return false;
            }
        }
        return true;
    }

    public bool Purchase(ShopEntrySO entry)
    {
        if (!CanAfford(entry))
            return false;

        foreach (var cost in entry.costs)
        {
            Inventory.Instance.RemoveItem(cost.item, cost.amount);
        }

        Inventory.Instance.AddItem(entry.item, entry.amount);

        if (entry.stock > 0)
        {
            entry.stock -= 1;
        }

        return true;
    }

    public void UnlockByChapter(int currentChapter)
    {
        {
            foreach (var entry in entries)
            {
                entry.isUnlocked = entry.unlockChapter <= currentChapter;
            }
        }
    }
}