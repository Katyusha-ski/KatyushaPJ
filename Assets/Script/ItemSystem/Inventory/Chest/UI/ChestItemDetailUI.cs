using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestItemDetailUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemStatsText;

    public void ShowItem(ItemData item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = item.itemIcon != null;
        }

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        if (itemStatsText != null)
        {
            itemStatsText.text = BuildStatsText(item);
            itemStatsText.gameObject.SetActive(
                !string.IsNullOrEmpty(itemStatsText.text)
            );
        }
    }

    public void Clear()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (itemNameText != null)
            itemNameText.text = string.Empty;

        if (itemDescriptionText != null)
            itemDescriptionText.text = string.Empty;

        if (itemStatsText != null)
        {
            itemStatsText.text = string.Empty;
            itemStatsText.gameObject.SetActive(false);
        }
    }

    private static string BuildStatsText(ItemData item)
    {
        if (!item.IsEquipment() || !item.GetStats().HasStats())
            return string.Empty;

        System.Text.StringBuilder builder =
            new System.Text.StringBuilder();

        foreach (StatModifierConfig config in item.GetStats().statConfigs)
        {
            string prefix = config.modifierType == ModifierType.Multiplicative
                ? " x"
                : " +";

            builder.Append(ItemStats.GetDisplayName(config.statType));
            builder.Append(":");
            builder.Append(prefix);
            builder.Append(config.value);
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd('\r', '\n');
    }
}
