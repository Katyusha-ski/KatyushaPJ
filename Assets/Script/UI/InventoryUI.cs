using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject UI;
    public Transform equipmentSlot; // Parent object for equipment slots
    public Transform slotParent; // Parent object for inventory slots
    public GameObject slotPrefab; // Prefab for individual inventory slots
    

    public void ShowInventory()
    {
        UI.SetActive(true);
    }

    public void HideInventory()
    {
        UI.SetActive(false);
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
        // Clear existing slots
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
        // Create new slots based on the inventory items
        var slots = Inventory.Instance.itemSlots;
        for (int i = 0; i < slots.Count; i++)
        {
            var stack = slots[i];
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotScript = slotGO.GetComponent<Slot>();
            if (stack != null && stack.item != null)
            {
                slotScript.SetItem(stack.item, stack.amount);
            }
            else
            {
                slotScript.ClearSlot();
            }
        }

        // Update equipment slots
        foreach (Transform child in equipmentSlot)
        {
            Destroy(child.gameObject);
        }
        var equips = Inventory.Instance.equipment;
        for (int i = 0; i < equips.Length; i++)
        {
            var stack = equips[i];
            var slotGO = Instantiate(slotPrefab, equipmentSlot); // Nếu có prefab riêng thì dùng equipmentSlotPrefab
            var slotScript = slotGO.GetComponent<Slot>();
            if (stack != null && stack.item != null)
            {
                slotScript.SetItem(stack.item, stack.amount);
            }
            else
            {
                slotScript.ClearSlot();
            }
        }
    }

    void CreateSlots()
    {
        for(int i = 0; i < Inventory.Instance.maxSlots; i++)
        {
            Instantiate(slotPrefab, slotParent);
        }
    }
}
