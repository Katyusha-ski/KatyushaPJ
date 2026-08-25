using UnityEngine;

public class ChestTransferUIController : MonoBehaviour
{
    [SerializeField] private ItemContainerSlotPanelUI playerPanel;
    [SerializeField] private ItemContainerSlotPanelUI chestPanel;
    [SerializeField] private ChestItemDetailUI itemDetailUI;

    private IItemContainer playerAdapter;
    private ChestInventory chest;

    public void Bind(
        IItemContainer playerContainer,
        ChestInventory chestInventory)
    {
        Unbind();

        playerAdapter = playerContainer;
        chest = chestInventory;

        if (playerPanel != null)
        {
            playerPanel.ItemClicked += HandlePlayerItemClicked;
            playerPanel.ItemDoubleClicked +=
                HandlePlayerItemDoubleClicked;
        }

        if (chestPanel != null)
        {
            chestPanel.ItemClicked += HandleChestItemClicked;
            chestPanel.ItemDoubleClicked +=
                HandleChestItemDoubleClicked;
        }
    }

    public void Unbind()
    {
        if (playerPanel != null)
        {
            playerPanel.ItemClicked -= HandlePlayerItemClicked;
            playerPanel.ItemDoubleClicked -=
                HandlePlayerItemDoubleClicked;
        }

        if (chestPanel != null)
        {
            chestPanel.ItemClicked -= HandleChestItemClicked;
            chestPanel.ItemDoubleClicked -=
                HandleChestItemDoubleClicked;
        }

        playerAdapter = null;
        chest = null;
    }

    private void OnDestroy()
    {
        Unbind();
    }

    private void HandlePlayerItemClicked(
        ItemContainerSlotUI slotUI)
    {
        ShowDetail(slotUI);
    }

    private void HandleChestItemClicked(
        ItemContainerSlotUI slotUI)
    {
        ShowDetail(slotUI);
    }

    private void ShowDetail(
        ItemContainerSlotUI slotUI)
    {
        if (itemDetailUI == null || slotUI == null)
            return;

        ItemStack stack = slotUI.CurrentStack;
        itemDetailUI.ShowItem(stack?.item);
    }

    private void HandlePlayerItemDoubleClicked(
        ItemContainerSlotUI slotUI)
    {
        if (slotUI == null)
            return;

        bool transferred =
            ChestTransferService.TransferToChest(
            slotUI.SlotIndex,
            playerAdapter,
            chest
        );

        if (transferred && itemDetailUI != null)
            itemDetailUI.Clear();
    }

    private void HandleChestItemDoubleClicked(
        ItemContainerSlotUI slotUI)
    {
        if (slotUI == null)
            return;

        bool transferred =
            ChestTransferService.TransferToPlayer(
            slotUI.SlotIndex,
            chest,
            playerAdapter
        );

        if (transferred && itemDetailUI != null)
            itemDetailUI.Clear();
    }
}
