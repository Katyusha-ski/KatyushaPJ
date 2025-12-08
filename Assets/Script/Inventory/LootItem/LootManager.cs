using UnityEngine;

public class LootManager : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private GameObject itemFloatPref;
    [SerializeField] private string poolTag = "ItemLoot";
    [SerializeField] private float dropRad;
    [SerializeField] private float dropForce;

    public void SpawnLoot()
    {
        if (lootTable == null)
            return;

        var loot = lootTable.GetRandomLoot();

        foreach (var (item, amount) in loot)
        {
            SpawnLootItem(item, amount);
        }
    }

    private void SpawnLootItem(ItemData item, int amount)
    {
        Vector2 dropPos = GetRandomDropPosition();
        GameObject LootGO = ObjectPool.Instance.SpawnFromPool(poolTag, dropPos, Quaternion.identity);
        if (LootGO != null)
        {
            ItemFloat itemFloat = LootGO.GetComponent<ItemFloat>();
            if(itemFloat != null)
            {
                itemFloat.Initialize(item, amount);
                ApplyRandomVelocity(LootGO.GetComponent<Rigidbody2D>());
            }
        }
        
    }

    private Vector3 GetRandomDropPosition()
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(0f, dropRad);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
            Mathf.Sin(angle * Mathf.Deg2Rad) * distance,
            0
        );

        return transform.position + offset;
    }

    private void ApplyRandomVelocity(Rigidbody2D rb)
    {
        if (rb == null)
            return;
        
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * dropForce;
    }
}
