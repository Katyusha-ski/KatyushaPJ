using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Inventory/LootTable")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> lootEntries = new List<LootEntry>();

    public List<(ItemData item, int amount)> GetRandomLoot()
    {
        List<(ItemData, int)> droppedItems = new List<(ItemData, int)>();

        foreach (var entry in lootEntries)
        {
            if (Random.Range(0f, 100f) <= entry.dropChance)
            {
                int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
                droppedItems.Add((entry.item, amount));
            }
        }

        return droppedItems;
    }
}
