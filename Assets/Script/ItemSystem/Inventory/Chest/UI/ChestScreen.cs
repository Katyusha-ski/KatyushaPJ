using UnityEngine;

public class ChestScreen : MonoBehaviour
{
    [SerializeField] private ItemContainerSlotPanelUI chestGrid;
    [SerializeField] private ItemContainerSlotPanelUI playerGrid;
    [SerializeField] private ChestItemDetailUI itemDetailUI;
    [SerializeField] private ChestTransferUIController transferController;
    [SerializeField] private GameObject raycastBlocker;

    private PlayerInventoryAdapter playerAdapter;
    private ChestInventory chest;

    public void Open()
    {
        if (chestGrid == null ||
            playerGrid == null ||
            itemDetailUI == null ||
            transferController == null ||
            raycastBlocker == null)
        {
            Debug.LogError(
                "[ChestScreen] Cannot open because one or more " +
                "Inspector references are missing.",
                this
            );
            return;
        }

        GameManager gameManager = GameManager.Instance;
        Inventory inventory = Inventory.Instance;

        if (gameManager == null ||
            gameManager.UsagiShopChest == null ||
            inventory == null)
        {
            Debug.LogWarning(
                "[ChestScreen] Cannot open: required game data " +
                "instances are not available.",
                this
            );
            return;
        }

        gameObject.SetActive(true);
        raycastBlocker.SetActive(true);

        chest = gameManager.UsagiShopChest;
        playerAdapter = new PlayerInventoryAdapter(inventory);

        chestGrid.Bind(chest);
        playerGrid.Bind(playerAdapter);
        transferController.Bind(playerAdapter, chest);
        itemDetailUI.Clear();
    }

    public void Close()
    {
        if (transferController != null)
            transferController.Unbind();

        if (chestGrid != null)
            chestGrid.Unbind();

        if (playerGrid != null)
            playerGrid.Unbind();

        if (itemDetailUI != null)
            itemDetailUI.Clear();

        if (raycastBlocker != null)
            raycastBlocker.SetActive(false);

        playerAdapter = null;
        chest = null;
        gameObject.SetActive(false);
    }
}
