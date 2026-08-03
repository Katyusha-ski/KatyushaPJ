using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDetailUI : MonoBehaviour
{
    [Header("Tham chiếu UI (tự kéo trong Inspector)")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    [Tooltip("GameObject cha chứa toàn bộ khung detail — ẩn khi chưa chọn item nào")]
    public GameObject detailRoot;

    private void Awake()
    {
        Clear();
    }

    public void ShowDetail(ItemData item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (detailRoot != null)
            detailRoot.SetActive(true);

        if (icon != null)
            icon.sprite = item.itemIcon;
        if (nameText != null)
            nameText.text = item.itemName;
        if (descriptionText != null)
            descriptionText.text = item.description;
    }

    public void Clear()
    {
        if (detailRoot != null)
            detailRoot.SetActive(false);
    }
}
