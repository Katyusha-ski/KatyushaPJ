using System;
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;
    public event Action OnSkillMatrixChanged;
    public int maxSlots = 30;
    public List<ItemStack> itemSlots = new List<ItemStack>(30);

    public int equipmentSlots = 4;
    public ItemStack[] equipment = new ItemStack[4];
    public int maxStack = 99;

    public ItemStack[,] skillMatrix = new ItemStack[4, 5];

    public static int SkillTypeToRow(SkillType type)
    {
        return (int)type - 1;
    }

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

    public int GetItemCount(ItemData item)
    {
        int count = 0;
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (!IsSlotEmpty(itemSlots[i]) && itemSlots[i].item == item)
            {
                count += itemSlots[i].amount;
            }
        }
        return count;
    }
    private bool IsSlotEmpty(ItemStack slot) => slot == null || slot.item == null;

    public bool CanAddItem(ItemData item, int amount = 1)
    {
        if(amount <= 0) return false; // Invalid amount
        int freeSpace = 0;
        // Check for non-stackable items
        if (!item.isStackable)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (IsSlotEmpty(itemSlots[i]))
                {
                    freeSpace++;
                }
            }
            return freeSpace >= amount;
        }

        // Check for non-null slots with same item
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (!IsSlotEmpty(itemSlots[i]) && itemSlots[i].item == item && itemSlots[i].amount < maxStack)
            {
                freeSpace += (maxStack - itemSlots[i].amount);
            }
        }
        // Check for null slots
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (IsSlotEmpty(itemSlots[i]))
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
            if (!IsSlotEmpty(itemSlots[i]) && itemSlots[i].item == item)
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
                if (IsSlotEmpty(itemSlots[i]))
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
                if (!IsSlotEmpty(itemSlots[i]) && itemSlots[i].item == item && itemSlots[i].amount < maxStack)
                {
                    int canAdd = Mathf.Min(amount, maxStack - itemSlots[i].amount);
                    itemSlots[i].amount += canAdd;
                    amount -= canAdd;
                }
            }

            // Add to empty slots
            for (int i = 0; i < itemSlots.Count && amount > 0; i++)
            {
                if (IsSlotEmpty(itemSlots[i]))
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

        for (int i = 0; i < itemSlots.Count && remaining > 0; i++)
        {
            if (!IsSlotEmpty(itemSlots[i]) && itemSlots[i].item == item)
            {
                int takeAmount = Mathf.Min(remaining, itemSlots[i].amount);
                itemSlots[i].amount -= takeAmount;
                remaining -= takeAmount;

                if (itemSlots[i].amount <= 0)
                {
                    itemSlots[i] = null;
                }
            }
        }

        OnInventoryChanged?.Invoke();
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
    /// Universal equipment swap - handles both equip and unequip
    /// isUnequipping = false: inventory ? equipment (equip)
    /// isUnequipping = true: equipment ? inventory (unequip)
    /// </summary>
    public bool SwapEquipItem(int sourceSlotIndex, int targetSlotIndex, bool isUnequipping = false)
    {
        if (isUnequipping)
        {
            // Unequip mode: equipment ? inventory
            var stack = equipment[sourceSlotIndex];
            if (stack == null) return false;

            // Direct swap
            var temp = itemSlots[targetSlotIndex];
            itemSlots[targetSlotIndex] = stack;
            equipment[sourceSlotIndex] = temp;
        }
        else
        {
            // Equip mode: inventory ? equipment
            var stack = itemSlots[sourceSlotIndex];
            if (stack == null || stack.item.itemType != ItemType.Equipment) return false;

            if ((int)stack.item.equipmentType != targetSlotIndex + 1) return false;

            // Direct swap
            var temp = equipment[targetSlotIndex];
            equipment[targetSlotIndex] = stack;
            itemSlots[sourceSlotIndex] = temp;
        }

        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();
        return true;
    }

    // ========================================================================
    // UseItem(int slotIndex)
    // ========================================================================
    // Cau noi giua UI (SlotDragHandler) va Player (ConsumableManager).
    // Flow:
    //   SlotDragHandler (double-click)
    //   -> Inventory.UseItem(slotIndex)
    //   -> tim Player (tag), lay ConsumableManager
    //   -> ConsumableManager.Use(item)
    //   -> tao StatusEffect, apply vao StatusEffectController
    //   -> RemoveItem(1) neu Use() thanh cong
    // ========================================================================
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= itemSlots.Count) return;
        ItemStack stack = itemSlots[slotIndex];
        if (stack == null || stack.item == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Khong tim thay Player de su dung item");
            return;
        }

        if (stack.item.itemType == ItemType.Consumable)
        {
            ConsumableManager consumableManager = player.GetComponent<ConsumableManager>();
            if (consumableManager == null)
            {
                Debug.LogError("Player can co component ConsumableManager", player);
                return;
            }
            if (consumableManager.Use(stack.item))
                RemoveItem(stack.item, 1);
        }
        else if (stack.item.itemType == ItemType.Skill)
        {
            PlayerSkillManager skillManager = player.GetComponent<PlayerSkillManager>();
            if (skillManager == null)
            {
                Debug.LogError("Player can co component PlayerSkillManager", player);
                return;
            }
            if (skillManager.UseItem(stack.item))
                RemoveItem(stack.item, 1);
        }
    }

    public SkillBase GetHighestSkill(int type)
    {
        if (type < 0 || type >= skillMatrix.GetLength(0)) return null;
        for (int i = skillMatrix.GetLength(1) - 1; i >= 0; i--)
        {
            var stack = skillMatrix[type, i];
            if (stack != null && stack.item != null && stack.item.skillData != null)
                return stack.item.skillData.skill;
        }
        return null;
    }

    public void SetSkill(int row, int col, ItemStack stack)
    {
        if (row < 0 || row >= skillMatrix.GetLength(0) || col < 0 || col >= skillMatrix.GetLength(1)) return;
        skillMatrix[row, col] = stack;
        OnSkillMatrixChanged?.Invoke();
    }

    public bool ClearInventory()
    {
        for (int i = 0; i < itemSlots.Count; i++)
            itemSlots[i] = null;
        for (int i = 0; i < equipment.Length; i++)
            equipment[i] = null;
        for (int r = 0; r < skillMatrix.GetLength(0); r++)
            for (int c = 0; c < skillMatrix.GetLength(1); c++)
                skillMatrix[r, c] = null;
        OnInventoryChanged?.Invoke();
        OnSkillMatrixChanged?.Invoke();
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

        for (int i = 0; i < equipment.Length; i++)
            equipment[i] = null;

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

    public List<List<SerializableItemStack>> GetSerializableSkillMatrix()
    {
        var result = new List<List<SerializableItemStack>>();
        int rows = skillMatrix.GetLength(0);
        int cols = skillMatrix.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            var row = new List<SerializableItemStack>();
            for (int c = 0; c < cols; c++)
            {
                var stack = skillMatrix[r, c];
                if (stack != null && stack.item != null)
                    row.Add(new SerializableItemStack(stack.item.itemName, stack.amount));
                else
                    row.Add(null);
            }
            result.Add(row);
        }
        return result;
    }

    public void LoadSerializableSkillMatrix(List<List<SerializableItemStack>> serialized)
    {
        if (serialized == null) return;
        int rows = skillMatrix.GetLength(0);
        int cols = skillMatrix.GetLength(1);
        for (int r = 0; r < rows && r < serialized.Count; r++)
        {
            for (int c = 0; c < cols && c < serialized[r].Count; c++)
            {
                skillMatrix[r, c] = serialized[r][c]?.ToItemStack();
            }
        }
        OnSkillMatrixChanged?.Invoke();
    }
}
