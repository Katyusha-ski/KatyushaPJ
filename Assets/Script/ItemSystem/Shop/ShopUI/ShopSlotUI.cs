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
    public Color unaffordableColor = new Color(1f, 0.6f, 0.6f, 1f);

    public event Action<ShopSlotUI, ShopEntrySO> OnClicked;
    private ShopEntrySO entry;
    private ShopManager shopManager;

    public void Setup(ShopEntrySO shopEntry, ShopManager manager)
    {
        entry = shopEntry;
        shopManager = manager;
        itemIcon.sprite = entry.item.itemIcon;
        selectBtn.onClick.AddListener(() => OnClicked?.Invoke(this, entry));
        Refresh();

    }

    public void Refresh()
    {
        bool canAfford = shopManager.CanAfford(entry);
        itemIcon.color = canAfford ? normalColor : unaffordableColor;
    }

    public void SetSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }
}