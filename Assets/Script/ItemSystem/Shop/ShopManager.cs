using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopEntrySO> entries;

    public bool CanAfford(ShopEntrySO entry)
    {
        if (entry == null || entry.item == null)
        {
            Debug.LogError("[ShopManager] CanAfford: entry or item is null!");
            return false;
        }

        if (entry.costs == null || entry.costs.Count == 0)
        {
            return true; // No costs = can afford
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("[ShopManager] CanAfford: Inventory.Instance is null! Ensure Inventory is initialized before ShopUI opens.");
            return false;
        }

        foreach (var cost in entry.costs)
        {
            if (cost == null || cost.item == null)
            {
                Debug.LogError("[ShopManager] CanAfford: cost or cost.item is null!");
                return false;
            }

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

        if (entry.costs != null)
        {
            foreach (var cost in entry.costs)
            {
                if (cost?.item != null)
                    Inventory.Instance.RemoveItem(cost.item, cost.amount);
            }
        }

        if (entry.item == null) return false;
        Inventory.Instance.AddItem(entry.item, entry.amount);

        if (entry.stock > 0)
        {
            entry.stock -= 1;
        }

        return true;
    }

    public void UnlockByChapter(int currentChapter)
    {
        if (entries == null) return;
        foreach (var entry in entries)
        {
            if (entry != null)
                entry.isUnlocked = entry.unlockChapter <= currentChapter;
        }
    }
}