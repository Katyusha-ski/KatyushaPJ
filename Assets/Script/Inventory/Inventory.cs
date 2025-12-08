using System;
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public event Action OnInventoryChanged;
    public int maxSlots = 30;
    public List<ItemStack> itemSlots = new List<ItemStack>(30);

    public int equipmentSlots = 4;
    public ItemStack[] equipment = new ItemStack[4];
    public int maxStack = 99;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < maxSlots; i++) itemSlots.Add(null);
        for (int i = 0; i < equipmentSlots; i++) equipment[i] = null;
    }

    public void TriggerInventoryChanged() => OnInventoryChanged?.Invoke();

    public bool CanAddItem(ItemData item, int amount = 1)
    {
        if(amount <= 0) return false; // Invalid amount
        int freeSpace = 0;
        // Check for non-stackable items
        if (!item.isStackable)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i] == null)
                {
                    freeSpace++;
                }
            }
            return freeSpace >= amount;
        }

        // Check for non-null slots with same item
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i] != null && itemSlots[i].item == item && itemSlots[i].amount < maxStack)
            {
                freeSpace += (maxStack - itemSlots[i].amount);
            }
        }
        // Check for null slots
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i] == null)
            {
                freeSpace += maxStack;
            }
        }
        return freeSpace >= amount;
    }

    public bool CanRemoveItem(ItemData item, int amount = 1)
    {
        if (amount <= 0) return false;

        int remainingToCheck = amount;

        for (int i = 0; i < itemSlots.Count && remainingToCheck > 0; i++)
        {
            if (itemSlots[i] != null && itemSlots[i].item == item)
            {
                int availableInSlot = itemSlots[i].amount;
                remainingToCheck -= Mathf.Min(remainingToCheck, availableInSlot);
            }
        }

        return remainingToCheck == 0;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        // Check if we can add the item
        if (!CanAddItem(item, amount)) 
            return;

        if (!item.isStackable)
        {
            for (int i = 0; i < itemSlots.Count && amount > 0; i++)
            {
                if (itemSlots[i] == null)
                {
                    itemSlots[i] = new ItemStack(item, 1);
                    amount--;
                }
            }
        }
        else
        {
            // Add to existing stacks first
            for (int i = 0; i < itemSlots.Count && amount > 0; i++)
            {
                if (itemSlots[i] != null && itemSlots[i].item == item && itemSlots[i].amount < maxStack)
                {
                    int canAdd = Mathf.Min(amount, maxStack - itemSlots[i].amount);
                    itemSlots[i].amount += canAdd;
                    amount -= canAdd;
                }
            }

            // Add to empty slots
            for (int i = 0; i < itemSlots.Count && amount > 0; i++)
            {
                if (itemSlots[i] == null)
                {
                    int stackAmount = Mathf.Min(amount, maxStack);
                    itemSlots[i] = new ItemStack(item, stackAmount);
                    amount -= stackAmount;
                }
            }
        }
        
        OnInventoryChanged?.Invoke();
    }
    
    public void RemoveItem(ItemData item, int amount = 1)
    {
        // Check if we can remove the item
        if (!CanRemoveItem(item, amount))
            return;

        int remaining = amount;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i] != null && itemSlots[i].item == item)
            {
                itemSlots[i].amount -= remaining;
                if (itemSlots[i].amount <= 0)
                {
                    remaining = -itemSlots[i].amount;
                    itemSlots[i] = null;
                }
                if (remaining <= 0)
                    break;
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public bool EquipItem(int inventorySlotIndex, int equipSlotIndex)
    {
        var stack = itemSlots[inventorySlotIndex];
        if (stack == null || stack.item.itemType != ItemType.Equipment) return false;

        if ((int)stack.item.equipmentType != equipSlotIndex + 1) return false;

        if (equipment[equipSlotIndex] != null)
        {
            AddItem(equipment[equipSlotIndex].item, equipment[equipSlotIndex].amount);
        }
        equipment[equipSlotIndex] = stack;
        itemSlots[inventorySlotIndex] = null;
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    public bool UnequipItem(int slotIndex)
    {
        var stack = equipment[slotIndex];
        if (stack == null) return false;

        if (!CanAddItem(stack.item, stack.amount)) 
            return false;

        AddItem(stack.item, stack.amount);

        equipment[slotIndex] = null;
        OnInventoryChanged?.Invoke();
        return true;
    }


    public bool SwapItem(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= itemSlots.Count || indexB < 0 || indexB >= itemSlots.Count) return false;
        var temp = itemSlots[indexA];
        itemSlots[indexA] = itemSlots[indexB];
        itemSlots[indexB] = temp;
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Converts the current Inventory to a Serializable format
    /// </summary>
    public List<SerializableItemStack> GetSerializableInventory()
    {
        List<SerializableItemStack> serializableList = new List<SerializableItemStack>();

        foreach (var itemStack in itemSlots)
        {
            if (itemStack != null && itemStack.item != null)
            {
                serializableList.Add(new SerializableItemStack(
                    itemStack.item.itemName,
                    itemStack.amount
                ));
            }
            else
            {
                serializableList.Add(null);
            }
        }

        return serializableList;
    }

    /// <summary>
    /// Converts the current Equipment to a Serializable format
    /// </summary>
    public List<SerializableItemStack> GetSerializableEquipment()
    {
        List<SerializableItemStack> serializableList = new List<SerializableItemStack>();

        foreach (var itemStack in equipment)
        {
            if (itemStack != null && itemStack.item != null)
            {
                serializableList.Add(new SerializableItemStack(
                    itemStack.item.itemName,
                    itemStack.amount
                ));
            }
            else
            {
                serializableList.Add(null);
            }
        }

        return serializableList;
    }

    /// <summary>
    /// Loads Inventory from Serializable data
    /// </summary>
    public void LoadSerializableInventory(List<SerializableItemStack> serializableInventory)
    {
        if (serializableInventory == null) return;

        // Clear current inventory
        for (int i = 0; i < itemSlots.Count; i++)
            itemSlots[i] = null;

        // Load each item
        for (int i = 0; i < serializableInventory.Count && i < itemSlots.Count; i++)
        {
            if (serializableInventory[i] != null)
            {
                ItemStack itemStack = serializableInventory[i].ToItemStack();
                if (itemStack != null)
                    itemSlots[i] = itemStack;
            }
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Loads Equipment from Serializable data
    /// </summary>
    public void LoadSerializableEquipment(List<SerializableItemStack> serializableEquipment)
    {
        if (serializableEquipment == null) return;

        // Clear current equipment
        for (int i = 0; i < equipment.Length; i++)
            equipment[i] = null;

        // Load each item
        for (int i = 0; i < serializableEquipment.Count && i < equipment.Length; i++)
        {
            if (serializableEquipment[i] != null)
            {
                ItemStack itemStack = serializableEquipment[i].ToItemStack();
                if (itemStack != null)
                    equipment[i] = itemStack;
            }
        }

        OnInventoryChanged?.Invoke();
    }
}
