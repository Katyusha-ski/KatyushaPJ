using UnityEngine;

public class BatHole : MonoBehaviour
{
    [SerializeField] private float damage = 8f;

    // Cache the player GameObject when it enters the trigger
    private GameObject playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject == playerInRange)
            playerInRange = null;
    }

    // Call this method from an Animation Event to deal instant damage
    public void DealDamageNow()
    {
        if (playerInRange == null) return;

        Health hp = playerInRange.GetComponent<Health>();
        if (hp != null)
        {
            int dmg = Mathf.RoundToInt(damage);
            hp.TakeDamage(dmg, gameObject);
        }
    }
}