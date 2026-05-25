[System.Serializable]
public class ItemStack
{
    public ItemData item;
    public int amount;

    public ItemStack(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
