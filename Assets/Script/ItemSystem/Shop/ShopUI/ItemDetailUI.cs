using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemDetailUI : MonoBehaviour
{
    [Header("Item Info")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;

    [Header("Costs")]
    public Transform costsContainer;
    public GameObject costIconPrefab;

    [Header("Action")]
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;

    private ShopEntrySO currentEntry;
    private ShopManager shopManager;
    private ShopUI shopUI;

    public void Init(ShopManager manager, ShopUI ui)
    {
        shopManager = manager;
        shopUI = ui;
        Clear();

    }

    public void ShowEntry(ShopEntrySO entry)
    {
        if (entry?.item == null) return;
        currentEntry = entry;
        itemIcon.sprite = entry.item.itemIcon;             
        itemNameText.text = entry.item.itemName;       
        descriptionText.text = entry.item.description; 
        BuildCostIcons();
        RefreshButton();
    }

    public void Clear()
    {
        currentEntry = null;
        itemIcon.sprite = null;
        itemNameText.text = "";
        descriptionText.text = "";
        foreach (Transform t in costsContainer) Destroy(t.gameObject);
        buyButton.interactable = false;
        buyButtonText.text = "Chọn vật phẩm";
    }

    void BuildCostIcons()
    {
        foreach (Transform t in costsContainer) Destroy(t.gameObject);
        if (currentEntry.costs == null) return;
        foreach (var cost in currentEntry.costs)
        {
            if (cost?.item == null) continue;
            var go = Instantiate(costIconPrefab, costsContainer);
            go.GetComponentInChildren<Image>().sprite = cost.item.itemIcon;
            go.GetComponentInChildren<TextMeshProUGUI>().text = $"x{cost.amount}";
        }
    }

    public void RefreshButton()
    {
        if (currentEntry == null) return;

        bool canAfford = shopManager.CanAfford(currentEntry);
        bool inStock = currentEntry.stock != 0;

        buyButton.interactable = canAfford && inStock;
        buyButtonText.text = !inStock ? "Hết hàng"
                           : !canAfford ? "Không đủ nguyên liệu"
                           : "Mua";
    }

    public void OnBuyClicked()
    {
        if (currentEntry == null) return;
        if (shopManager.Purchase(currentEntry))
            shopUI.RefreshAll();
    }
}