using UnityEngine;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    public CharacterStats characterStats;
    private ItemStats[] cachedEquipmentStats = new ItemStats[4];
    private List<SlotModEntry>[] slotModifiers = new List<SlotModEntry>[4];

    private class SlotModEntry
    {
        public StatType statType;
        public StatsModifier modifier;
    }

    private void Start()
    {
        Inventory.Instance.OnEquipmentChanged += HandleEquipmentChanged;
        ApplyAllCurrentEquipment();
    }
    private void OnDestroy()
    {
        Inventory.Instance.OnEquipmentChanged -= HandleEquipmentChanged;
    }

    private void HandleEquipmentChanged()
    {
        for (int i = 0; i < Inventory.Instance.equipment.Length; i++)
        {
            ItemStack currentStack = Inventory.Instance.equipment[i];

            if (slotModifiers[i] != null)
                RemoveEquipmentStats(i);
            if (currentStack != null)
                ApplyEquipmentStats(i);
        }
    }

    private void ApplyEquipmentStats(int slotIndex)
    {
        ItemStack stack = Inventory.Instance.equipment[slotIndex];
        if (stack == null || stack.item == null) return;

        ItemStats itemStats = stack.item.GetStats();
        if (!itemStats.HasStats()) return;

        cachedEquipmentStats[slotIndex] = itemStats.Clone();
        var entries = new List<SlotModEntry>(itemStats.statConfigs.Count);

        foreach (var cfg in itemStats.statConfigs)
        {
            var mod = new StatsModifier(cfg.value, cfg.modifierType, stack.item.itemName);
            characterStats.AddStatModifier(cfg.statType, mod);
            entries.Add(new SlotModEntry { statType = cfg.statType, modifier = mod });
        }

        slotModifiers[slotIndex] = entries;
    }

    private void RemoveEquipmentStats(int slotIndex)
    {
        var entries = slotModifiers[slotIndex];
        if (entries == null) return;

        foreach (var entry in entries)
        {
            characterStats.RemoveStatModifier(entry.statType, entry.modifier);
        }

        slotModifiers[slotIndex] = null;
        cachedEquipmentStats[slotIndex] = null;
    }

    private void ApplyAllCurrentEquipment()
    {
        for (int i = 0; i < cachedEquipmentStats.Length; i++)
        {
            cachedEquipmentStats[i] = null;
            slotModifiers[i] = null;
        }

        for (int i = 0; i < Inventory.Instance.equipment.Length; i++)
        {
            if (Inventory.Instance.equipment[i] != null)
            {
                ApplyEquipmentStats(i);
            }
        }
    }
}
