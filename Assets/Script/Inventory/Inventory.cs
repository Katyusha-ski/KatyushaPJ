using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public int maxSlots = 30;
    public List<ItemStack> itemSlots = new List<ItemStack>(30);

    public int equipmentSlots = 5;
    public ItemStack[] equipment = ItemStack[5];
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

    public bool AddItem(ItemData item, int amount){
        for (int i = 0; i < itemSlots.Count; i++){
            if (itemSlots[i] == null){
                itemSlots[i] = new ItemStack(item, amount);
                return true;
            }
            else if (itemSlots[i].item == item){
                itemSlots[i].amount += amount;
                return true;
            }
        }
        return false;
    }
    public bool EquipItem(int slotIndex)
    {
        var stack = itemSlots[slotIndex];
        if (stack == null || stack.item.itemType != ItemType.Equipment) return false;

        int equipIndex = (int)stack.item.equipmentType;
        if (equipIndex < 1 || equipIndex > 4) return false; 

        // if already has equipment in this slot, return to inventory
        if (equipmentSlots[equipIndex] != null)
        {
            AddItem(equipmentSlots[equipIndex].item, equipmentSlots[equipIndex].amount);
        }
        equipmentSlots[equipIndex] = stack;
        itemSlots[slotIndex] = null;
        return true;
    }
}
