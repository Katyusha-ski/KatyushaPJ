using System;
using UnityEngine;
using UnityEngine.UI;


public class ShopSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public Button selectBtn;

    [Header("Highlight")]
    public Image background;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    public event Action<ShopSlotUI, ShopEntrySO> OnClicked;
    private ShopEntrySO entry;

    public void Setup(ShopEntrySO shopEntry)
    {
        entry = shopEntry;
        itemIcon.sprite = entry.item.itemIcon;
        selectBtn.onClick.AddListener(() => OnClicked?.Invoke(this, entry));
        Refresh();
    }

    public void Refresh()
    {
        bool canAfford = true;
        foreach(var cost in entry.costs)
        {
            if (Inventory.Instance.GetItemCount(cost.item) < cost.amount)
            {
                canAfford = false;
                break;
            }
        }
        itemIcon.color = canAfford ? normalColor : selectedColor;
    }

    public void SetSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }
}