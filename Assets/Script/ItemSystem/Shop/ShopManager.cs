using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopEntrySO> entries;
    public Dictionary<ShopEntrySO, RuntimeState> runtimeData = new Dictionary<ShopEntrySO, RuntimeState>();

    private void Start()
    {
        if (entries == null)
        {
            Debug.LogError("[ShopManager] Start: entries list is null! Ensure it is assigned in the inspector.");
            return;
        }
        foreach (var entry in entries)
        {
            if (entry != null && !runtimeData.ContainsKey(entry))
            {
                runtimeData[entry] = new RuntimeState(entry);
            }
        }
    }

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
        if (!CanAfford(entry) || entry.item == null)
            return false;

        if (entry.costs != null)
        {
            foreach (var cost in entry.costs)
            {
                if (cost?.item != null)
                    Inventory.Instance.RemoveItem(cost.item, cost.amount);
            }
        }
        Inventory.Instance.AddItem(entry.item, entry.amount);

        if (runtimeData[entry].currentStock > 0)
        {
            runtimeData[entry].ReduceStock();
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

    public int GetCurrentStock(ShopEntrySO entry)
    {
        if (runtimeData.TryGetValue(entry, out var state))
        {
            return state.currentStock;
        }
        return 0; // Default to 0 if entry not found, though ideally this should not happen if entries are properly initialized
    }
}