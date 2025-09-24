using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text quantityText;

    // Sets the item in the slot with the specified amount.
    public void SetItem(ItemData item, int amount)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        itemIcon.sprite = item.itemIcon;
        itemIcon.enabled = true;

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
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    public bool HasItem()
    {
        return itemIcon.sprite != null && itemIcon.sprite != null;
    }
}
