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

    public bool AddItem(ItemData item, int amount = 1)
    {
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
            if (amount > 0) return false;
            OnInventoryChanged?.Invoke();
            return true;
        }
        for (int i = 0; i < itemSlots.Count && amount > 0; i++)
        {
            if (itemSlots[i] != null && itemSlots[i].item == item && itemSlots[i].amount < maxStack)
            {
                int canAdd = Mathf.Min(amount, maxStack - itemSlots[i].amount);
                itemSlots[i].amount += canAdd;
                amount -= canAdd;
            }
        }
        for (int i = 0; i < itemSlots.Count && amount > 0; i++)
        {
            if (itemSlots[i] == null)
            {
                int stackAmount = Mathf.Min(amount, maxStack);
                itemSlots[i] = new ItemStack(item, stackAmount);
                amount -= stackAmount;
            }
        }
        OnInventoryChanged?.Invoke();
        return amount == 0;
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

        if (!AddItem(stack.item, stack.amount)) return false;

        equipment[slotIndex] = null;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i] != null && itemSlots[i].item == item)
            {
                itemSlots[i].amount -= amount;
                if (itemSlots[i].amount <= 0)
                    itemSlots[i] = null;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
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
