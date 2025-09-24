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
}
