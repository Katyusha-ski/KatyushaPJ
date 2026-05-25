using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject UI;
    public Transform equipmentSlot; // Parent object for equipment slots
    public Transform slotParent; // Parent object for inventory slots
    public GameObject slotPrefab; // Prefab for individual inventory slots

    private List<Slot> slotScripts = new List<Slot>();
    private List<Slot> equipmentSlotScripts = new List<Slot>();

    public void ShowInventory()
    {
        UI.SetActive(true);
    }

    public void HideInventory()
    {
        UI.SetActive(false);
    }

    private void Awake()
    {
        //Create slot and equipment slot only 1 time
        for (int i = 0; i < Inventory.Instance.maxSlots; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotScript = slotGO.GetComponent<Slot>();
            slotScript.slotIndex = i;
            slotScripts.Add(slotScript);
        }

        
        for (int i = 0; i < Inventory.Instance.equipmentSlots; i++)
        {
            var slotGO = Instantiate(slotPrefab, equipmentSlot);
            var slotScript = slotGO.GetComponent<Slot>();
            slotScript.slotIndex = i;
            slotScript.isEquipmentSlot = true;
            equipmentSlotScripts.Add(slotScript);
        }
    }

    private void OnEnable()
    {
        Inventory.Instance.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        Inventory.Instance.OnInventoryChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        // update the slot's content
        var slots = Inventory.Instance.itemSlots;
        for (int i = 0; i < slotScripts.Count; i++)
        {
            var stack = slots[i];
            if (stack != null && stack.item != null)
            {
                slotScripts[i].SetItem(stack.item, stack.amount);
            }
            else
            {
                slotScripts[i].ClearSlot();
            }
        }

        // update the equipment slot's content
        var equips = Inventory.Instance.equipment;
        for (int i = 0; i < equipmentSlotScripts.Count; i++)
        {
            var stack = equips[i];
            if (stack != null && stack.item != null)
            {
                equipmentSlotScripts[i].SetItem(stack.item, stack.amount);
            }
            else
            {
                equipmentSlotScripts[i].ClearSlot();
            }
        }
    }
}
