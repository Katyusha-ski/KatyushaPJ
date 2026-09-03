using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSlotUI : MonoBehaviour
{
    [Header("Tham chiếu UI (tự kéo trong Inspector)")]
    public Image icon;
    public TMP_Text nameText;
    public Button button;

    private ItemData questItem;
    private System.Action<ItemData> onClickCallback;

    public ItemData Item => questItem;

    public void Setup(ItemData item, System.Action<ItemData> onClick)
    {
        questItem = item;
        onClickCallback = onClick;

        if (icon != null)
            icon.sprite = item != null ? item.itemIcon : null;
        if (nameText != null)
            nameText.text = item != null ? item.itemName : "";

        if (button != null)
        {
            button.colors = ColorBlock.defaultColorBlock;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickCallback?.Invoke(questItem));
        }
    }

    public void SelectButton()
    {
        if (button != null)
            button.Select();
    }
}
