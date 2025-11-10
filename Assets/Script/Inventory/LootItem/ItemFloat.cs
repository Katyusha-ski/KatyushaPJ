using UnityEngine;

public class ItemFloat : MonoBehaviour
{
    [Header("References")]
    private SpriteRenderer sr;
    private BoxCollider2D pickupCollider;

    [SerializeField]private ItemData itemData;
    [SerializeField]private int amount;

    void Start()
    {
        pickupCollider = GetComponent<BoxCollider2D>();
    }

    public void Initialize(ItemData item, int itemAmount)
    {
        itemData = item;
        amount = itemAmount;
        sr = GetComponent<SpriteRenderer>();

        if (sr != null && item.itemIcon != null)
        {
            sr.sprite = item.itemIcon;
        }

        gameObject.name = $"Loot_{item.itemName}_{amount}"; 
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Inventory.Instance != null)
            {
                bool added = Inventory.Instance.AddItem(itemData, amount);
                if (added)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
