using System;

public sealed class PlayerInventoryAdapter : IItemContainer
{
    private readonly Inventory inventory;

    public PlayerInventoryAdapter(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public string ContainerId => "player_inventory";
    public int MaxSlots => inventory != null ? inventory.maxSlots : 0;
    public int OccupiedSlots => inventory != null && inventory.itemSlots != null
        ? inventory.itemSlots.Count
        : 0;

    public event Action OnInventoryChanged
    {
        add
        {
            if (inventory != null)
                inventory.OnInventoryChanged += value;
        }
        remove
        {
            if (inventory != null)
                inventory.OnInventoryChanged -= value;
        }
    }

    public ItemStack GetItemAt(int index)
    {
        if (inventory == null || inventory.itemSlots == null || index < 0 || index >= inventory.itemSlots.Count)
            return null;

        return inventory.itemSlots[index];
    }

    public bool TryAddItem(ItemData item, int amount)
    {
        if (inventory == null || item == null || amount <= 0 || !inventory.CanAddItem(item, amount))
            return false;

        inventory.AddItem(item, amount);
        return true;
    }

    public bool TryRemoveItemAt(int index, int amount)
    {
        ItemStack stack = GetItemAt(index);
        if (stack == null || stack.item == null || amount <= 0 || amount > stack.amount)
            return false;

        inventory.RemoveItem(stack.item, amount);
        return true;
    }
}
