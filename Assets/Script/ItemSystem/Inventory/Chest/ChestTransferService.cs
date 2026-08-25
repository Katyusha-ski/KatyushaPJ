using UnityEngine;

public static class ChestTransferService
{
    public static bool TransferToChest(
        int playerSlotIndex,
        IItemContainer playerAdapter,
        ChestInventory chest)
    {
        if (playerAdapter == null || chest == null)
        {
            Debug.LogWarning(
                "[ChestTransferService] Cannot transfer to chest: " +
                "source or destination is null."
            );
            return false;
        }

        ItemStack playerStack =
            playerAdapter.GetItemAt(playerSlotIndex);

        if (playerStack == null ||
            playerStack.item == null ||
            playerStack.amount <= 0)
        {
            Debug.LogWarning(
                $"[ChestTransferService] Cannot transfer player slot " +
                $"{playerSlotIndex}: slot is empty."
            );
            return false;
        }

        ItemData item = playerStack.item;
        int amount = playerStack.amount;

        if (!chest.TryAddItem(item, amount))
        {
            Debug.LogWarning(
                $"[ChestTransferService] Cannot transfer " +
                $"{amount}x '{item.itemName}' to chest " +
                $"'{chest.chestId}': destination is full or " +
                "does not have enough capacity."
            );
            return false;
        }

        if (!playerAdapter.TryRemoveItemAt(
                playerSlotIndex,
                amount))
        {
            RollbackFromChest(chest, item, amount);
            Debug.LogError(
                $"[ChestTransferService] Added {amount}x " +
                $"'{item.itemName}' to chest, but failed to " +
                "remove it from the player inventory. " +
                "The source/destination state requires inspection."
            );
            return false;
        }

        return true;
    }

    public static bool TransferToPlayer(
        int chestSlotIndex,
        ChestInventory chest,
        IItemContainer playerAdapter)
    {
        if (chest == null || playerAdapter == null)
        {
            Debug.LogWarning(
                "[ChestTransferService] Cannot transfer to player: " +
                "source or destination is null."
            );
            return false;
        }

        ItemStack chestStack =
            chest.GetItemAt(chestSlotIndex);

        if (chestStack == null ||
            chestStack.item == null ||
            chestStack.amount <= 0)
        {
            Debug.LogWarning(
                $"[ChestTransferService] Cannot transfer chest slot " +
                $"{chestSlotIndex}: slot is empty."
            );
            return false;
        }

        ItemData item = chestStack.item;
        int amount = chestStack.amount;

        if (!playerAdapter.TryAddItem(item, amount))
        {
            Debug.LogWarning(
                $"[ChestTransferService] Cannot transfer " +
                $"{amount}x '{item.itemName}' to player: " +
                "destination is full or does not have enough capacity."
            );
            return false;
        }

        if (!chest.TryRemoveItemAt(
                chestSlotIndex,
                amount))
        {
            RollbackFromContainer(playerAdapter, item, amount);
            Debug.LogError(
                $"[ChestTransferService] Added {amount}x " +
                $"'{item.itemName}' to player, but failed to " +
                "remove it from the chest. " +
                "The source/destination state requires inspection."
            );
            return false;
        }

        return true;
    }

    private static void RollbackFromChest(
        ChestInventory chest,
        ItemData item,
        int amount)
    {
        int remaining = amount;

        for (int i = chest.itemSlots.Count - 1;
             i >= 0 && remaining > 0;
             i--)
        {
            ItemStack stack = chest.GetItemAt(i);
            if (stack == null || stack.item != item)
                continue;

            int amountToRemove =
                Mathf.Min(remaining, stack.amount);

            if (chest.TryRemoveItemAt(i, amountToRemove))
                remaining -= amountToRemove;
        }

        if (remaining > 0)
        {
            Debug.LogError(
                $"[ChestTransferService] Could not fully rollback " +
                $"{remaining}x '{item.itemName}' from chest."
            );
        }
    }

    private static void RollbackFromContainer(
        IItemContainer container,
        ItemData item,
        int amount)
    {
        int remaining = amount;

        for (int i = container.MaxSlots - 1;
             i >= 0 && remaining > 0;
             i--)
        {
            ItemStack stack = container.GetItemAt(i);
            if (stack == null || stack.item != item)
                continue;

            int amountToRemove =
                Mathf.Min(remaining, stack.amount);

            if (container.TryRemoveItemAt(i, amountToRemove))
                remaining -= amountToRemove;
        }

        if (remaining > 0)
        {
            Debug.LogError(
                $"[ChestTransferService] Could not fully rollback " +
                $"{remaining}x '{item.itemName}' from container " +
                $"'{container.ContainerId}'."
            );
        }
    }
}
