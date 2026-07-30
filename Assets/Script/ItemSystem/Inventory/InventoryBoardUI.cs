using UnityEngine;
using UnityEngine.UI;

/*
 * Setup:
 *   1. Tao GameObject "InventoryBoard", gan InventoryBoardUI.
 *   2. Keo InventoryPanel cu (UI cua InventoryUI) vao inventoryRoot.
 *   3. Tao SkillPanel con (gom InventorySkillPanelUI + 4 slot prefab), keo vao skillRoot.
 *   4. Tao 2 Button "Tab_Inventory" va "Tab_Skills", gan vao inventoryTabButton / skillTabButton.
 *   5. UI toggle button (mo Inventory) chuyen target tu InventoryUI.UI sang InventoryBoard.
 *
 * Flow: OnEnable() mac dinh show Inventory tab.
 *       Click tab button => an panel cu, hien panel moi, disable button dang active.
 */
public class InventoryBoardUI : MonoBehaviour
{
    public GameObject inventoryRoot;
    public GameObject skillRoot;

    public Button inventoryTabButton;
    public Button skillTabButton;

    private void Awake()
    {
        if (inventoryTabButton != null)
            inventoryTabButton.onClick.AddListener(() => ShowTab(Tab.Inventory));
        if (skillTabButton != null)
            skillTabButton.onClick.AddListener(() => ShowTab(Tab.Skill));
    }

    private void OnEnable()
    {
        ShowTab(Tab.Inventory);
    }

    private void ShowTab(Tab tab)
    {
        bool showInventory = tab == Tab.Inventory;
        inventoryRoot.SetActive(showInventory);
        skillRoot.SetActive(!showInventory);

        inventoryTabButton.interactable = !showInventory;
        skillTabButton.interactable = showInventory;
    }

    private enum Tab { Inventory, Skill }
}
