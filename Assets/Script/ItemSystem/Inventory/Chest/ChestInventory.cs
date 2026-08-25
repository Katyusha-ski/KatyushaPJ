using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ChestInventory : IItemContainer
{
    public const int Capacity = 40;

    public string chestId;
    public List<ItemStack> itemSlots = new List<ItemStack>(Capacity);

    public string ContainerId => chestId;
    public int MaxSlots => Capacity;
    public int OccupiedSlots => itemSlots.Count;

    public event Action OnInventoryChanged;

    public ChestInventory(string chestId)
    {
        if (string.IsNullOrWhiteSpace(chestId))
            throw new ArgumentException("Chest id cannot be empty.", nameof(chestId));

        this.chestId = chestId;
    }

    public ItemStack GetItemAt(int index)
    {
        if (index < 0 || index >= itemSlots.Count)
            return null;

        return itemSlots[index];
    }

    public bool TryAddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        int maxAmountPerSlot = item.isStackable && item.maxStackSize > 0 ? item.maxStackSize : 1;
        int availableAmount = 0;

        foreach (ItemStack stack in itemSlots)
        {
            if (stack == null || stack.item != item)
                continue;

            availableAmount += item.isStackable
                ? Math.Max(0, maxAmountPerSlot - stack.amount)
                : 0;
        }

        int emptySlots = Capacity - itemSlots.Count;
        availableAmount += emptySlots * maxAmountPerSlot;
        if (availableAmount < amount)
            return false;

        int remaining = amount;

        if (item.isStackable)
        {
            foreach (ItemStack stack in itemSlots)
            {
                if (stack == null || stack.item != item || stack.amount >= maxAmountPerSlot)
                    continue;

                int added = Math.Min(remaining, maxAmountPerSlot - stack.amount);
                stack.amount += added;
                remaining -= added;
                if (remaining == 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        while (remaining > 0 && itemSlots.Count < Capacity)
        {
            int amountForSlot = Math.Min(remaining, maxAmountPerSlot);
            itemSlots.Add(new ItemStack(item, amountForSlot));
            remaining -= amountForSlot;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool TryRemoveItemAt(int index, int amount)
    {
        ItemStack stack = GetItemAt(index);
        if (stack == null || stack.item == null || amount <= 0 || amount > stack.amount)
            return false;

        stack.amount -= amount;
        if (stack.amount == 0)
            itemSlots.RemoveAt(index);

        OnInventoryChanged?.Invoke();
        return true;
    }

    public List<SerializableItemStack> GetSerializableItems()
    {
        List<SerializableItemStack> serializableItems = new List<SerializableItemStack>();

        foreach (ItemStack stack in itemSlots)
        {
            if (stack == null || stack.item == null || stack.amount <= 0)
                continue;

            serializableItems.Add(new SerializableItemStack(
                stack.item.itemName,
                stack.amount
            ));
        }

        return serializableItems;
    }

    public void LoadSerializableItems(List<SerializableItemStack> serializedItems)
    {
        itemSlots.Clear();

        if (serializedItems != null)
        {
            foreach (SerializableItemStack serializedItem in serializedItems)
            {
                if (serializedItem == null)
                    continue;

                ItemStack stack = serializedItem.ToItemStack();
                if (stack == null || stack.item == null || stack.amount <= 0)
                    continue;

            if (!TryAddItem(stack.item, stack.amount))
            {
                Debug.LogWarning(
                    $"[ChestInventory] Could not load item '{serializedItem.itemName}' " +
                    $"with amount {serializedItem.amount} into chest '{chestId}'. " +
                    "The item was skipped because the chest has no available capacity."
                );
                break;
            }
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public void Clear()
    {
        if (itemSlots.Count == 0)
            return;

        itemSlots.Clear();
        OnInventoryChanged?.Invoke();
    }
}
