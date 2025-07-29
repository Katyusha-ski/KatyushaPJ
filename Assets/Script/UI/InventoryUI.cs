using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject UI;
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
    }
}
