using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemContainerSlotUI : MonoBehaviour, IPointerClickHandler
{
    private const float DoubleClickThreshold = 0.3f;

    [Header("Visuals")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;

    private float lastClickTime = -1f;
    private ItemStack currentStack;

    public int SlotIndex { get; private set; }
    public ItemStack CurrentStack => currentStack;

    public event Action<ItemContainerSlotUI> ItemClicked;
    public event Action<ItemContainerSlotUI> ItemDoubleClicked;

    public void Initialize(int slotIndex)
    {
        SlotIndex = slotIndex;
        lastClickTime = -1f;
        ClearSlot();
    }

    public void SetItem(ItemStack stack)
    {
        currentStack = stack;

        if (itemIcon != null)
        {
            itemIcon.sprite = stack?.item?.itemIcon;
            itemIcon.enabled = stack?.item?.itemIcon != null;
        }

        if (quantityText != null)
        {
            bool showQuantity = stack?.item != null &&
                                stack.item.isStackable &&
                                stack.amount > 1;

            quantityText.text = showQuantity
                ? stack.amount.ToString()
                : string.Empty;

            quantityText.gameObject.SetActive(showQuantity);
        }
    }

    public void ClearSlot()
    {
        currentStack = null;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        float now = Time.unscaledTime;
        bool isDoubleClick =
            lastClickTime >= 0f &&
            now - lastClickTime <= DoubleClickThreshold;

        lastClickTime = now;
        ItemClicked?.Invoke(this);

        if (isDoubleClick)
        {
            lastClickTime = -1f;
            ItemDoubleClicked?.Invoke(this);
        }
    }
}
