using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemDetailUI : MonoBehaviour
{
    [Header("Item Info")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI itemStatText;

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
        itemIcon.gameObject.SetActive(true);
        itemNameText.text = entry.item.itemName;       
        descriptionText.text = entry.item.description; 
        BuildCostIcons();
        BuildStatsText(entry.item.stats);
        RefreshButton();
    }

    public void Clear()
    {
        currentEntry = null;
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        itemNameText.text = "";
        descriptionText.text = "";
        itemStatText.text = "";
        foreach (Transform t in costsContainer) Destroy(t.gameObject);
        buyButton.interactable = false;
        buyButtonText.text = "Select Item";
    }

    void BuildCostIcons()
    {
        foreach (Transform t in costsContainer) Destroy(t.gameObject);
        if (currentEntry.costs == null) return;
        foreach (var cost in currentEntry.costs)
        {
            if (cost?.item == null) continue;
            var go = Instantiate(costIconPrefab, costsContainer);
            var icon = go.transform.Find("ItemIcon")?.GetComponent<Image>();
            if (icon != null) icon.sprite = cost.item.itemIcon;
            var amount = go.transform.Find("Amount")?.GetComponent<TextMeshProUGUI>();
            if (amount != null) amount.text = $"x{cost.amount}";
        }
    }

    void BuildStatsText(ItemStats stats)
    {
        if (stats == null || !stats.HasStats())
        {
            itemStatText.text = "";
            return;
        }

        var sb = new System.Text.StringBuilder();

        if (stats.atk > 0) sb.AppendLine($"Atk: {stats.atk}");
        if (stats.critChance > 0) sb.AppendLine($"Crit Rate: {stats.critChance}");
        if (stats.critDamage > 0) sb.AppendLine($"Crit Damage: {stats.critDamage}");
        if (stats.armorPierce > 0) sb.AppendLine($"Armor Pierce: {stats.armorPierce}");
        if (stats.armor > 0) sb.AppendLine($"Armor: {stats.armor}");
        if (stats.health > 0) sb.AppendLine($"Max HP: {stats.health}");
        if (stats.movementSpeed > 0) sb.AppendLine($"Movement Speed: {stats.movementSpeed}");
        if (stats.controlResistance > 0) sb.AppendLine($"CC Res: {stats.controlResistance}");
        if (stats.lifesteal > 0) sb.AppendLine($"Life Steal: {stats.lifesteal}");
        if (stats.cooldownReduction > 0) sb.AppendLine($"CDR: {stats.cooldownReduction}");
        if (stats.hpRegen > 0) sb.AppendLine($"HP Regen: {stats.hpRegen}");
        if (stats.dmgR > 0) sb.AppendLine($"DmgR: {stats.dmgR}");
        if (stats.skillAmp > 0) sb.AppendLine($"Skill Amp: {stats.skillAmp}");

        itemStatText.text = sb.ToString().TrimEnd('\n', '\r');
    }

    public void RefreshButton()
    {
        if (currentEntry == null) return;

        bool canAfford = shopManager.CanAfford(currentEntry);
        bool inStock = currentEntry.stock != 0;

        buyButton.interactable = canAfford && inStock;
        buyButtonText.text = !inStock ? "Soldout"
                           : !canAfford ? "Insufficient"
                           : "Buy";
        buyButtonText.color = !inStock ? Color.yellow
                                : !canAfford ? Color.red
                                : Color.green;
    }

    public void OnBuyClicked()
    {
        if (currentEntry == null) return;
        if (shopManager.Purchase(currentEntry))
            shopUI.RefreshAll();
    }
}