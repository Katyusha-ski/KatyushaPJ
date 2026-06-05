using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransactionCost
{
    public ItemData item;
    public int amount;
}

[CreateAssetMenu(fileName = "New Shop Entry", menuName = "Shop/Shop Entry")]
public class ShopEntrySO : ScriptableObject
{
    [Header("Unlock")]
    public int unlockChapter = 0;
    public bool isUnlocked = true;

    [Header("Item Info")]
    public ItemData item;
    public int amount = 1;
    public List<TransactionCost> costs;
    public int stock = -1; // -1 for infinite stock

}
