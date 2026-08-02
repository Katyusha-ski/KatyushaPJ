using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [System.Serializable]
    public class TabEntry
    {
        public string tabName;
        public Button button;
        public Image buttonBackground;
        public Image buttonIcon;
        public GameObject contentPanel;
    }

    [Header("Danh sách tab, thứ tự tuỳ ý")]
    public TabEntry[] tabs;

    [Header("Màu khi tab đang được chọn")]
    public Color activeBackgroundColor = Color.black;
    public Color activeIconColor = Color.white;

    [Header("Màu khi tab không được chọn")]
    public Color inactiveBackgroundColor = Color.white;
    public Color inactiveIconColor = Color.black;

    [Tooltip("Tab nào sẽ mở sẵn khi panel này vừa bật lên")]
    public int defaultTabIndex = 0;

    private void OnEnable()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            tabs[i].button.onClick.RemoveListener(() => SelectTab(index));
            tabs[i].button.onClick.AddListener(() => SelectTab(index));
        }
        SelectTab(defaultTabIndex);
    }

    public void SelectTab(int selectedIndex)
    {
        if (tabs == null || selectedIndex < 0 || selectedIndex >= tabs.Length) return;

        for (int i = 0; i < tabs.Length; i++)
        {
            bool isActive = (i == selectedIndex);

            if (tabs[i].contentPanel != null)
                tabs[i].contentPanel.SetActive(isActive);

            if (tabs[i].buttonBackground != null)
                tabs[i].buttonBackground.color = isActive ? activeBackgroundColor : inactiveBackgroundColor;

            if (tabs[i].buttonIcon != null)
                tabs[i].buttonIcon.color = isActive ? activeIconColor : inactiveIconColor;
        }
    }
}
