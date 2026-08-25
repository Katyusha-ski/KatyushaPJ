using System;

public interface IItemContainer
{
    string ContainerId { get; }
    int MaxSlots { get; }
    int OccupiedSlots { get; }

    event Action OnInventoryChanged;

    ItemStack GetItemAt(int index);
    bool TryAddItem(ItemData item, int amount);
    bool TryRemoveItemAt(int index, int amount);
}
