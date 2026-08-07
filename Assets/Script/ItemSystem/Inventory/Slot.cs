using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotIndex;
    public Image itemIcon;
    public TMP_Text quantityText;
    public bool isEquipmentSlot = false;
    private ItemData currentItem;

    // Sets the item in the slot with the specified amount.
    public void SetItem(ItemData item, int amount)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        currentItem = item;

        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
        }
        else
        {
            Debug.LogWarning($"[Slot] itemIcon is missing on slot {slotIndex}.", this);
        }

        if (quantityText != null)
            quantityText.enabled = true;

        if (quantityText != null)
        {
            // if the item is stackable, show the quantity
            if (item.isStackable && amount > 1)
            {
                quantityText.text = amount.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    public bool HasItem()
    {
        return currentItem != null;
    }

    public ItemData GetItemData()
    {
        return currentItem;
    }
}
