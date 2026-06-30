using UnityEngine;

public class DeathExplosion : MonoBehaviour
{
    [Header("Burst")]
    [SerializeField] private float burstRadius = 2.5f;
    [SerializeField] private int burstDamage = 10;

    [Header("Poison Zone")]
    [SerializeField] private float zoneLifeTime = 5f;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private int poisonDamagePerTick = 3;

    private Health playerHealth;
    private float dmgTickTimer;

    private void Start()
    {
        ExplosionBurst();
        Destroy(gameObject, zoneLifeTime);
    }

    private void ExplosionBurst()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, burstRadius, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(burstDamage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<Health>();
            dmgTickTimer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        dmgTickTimer += Time.deltaTime;
        if (collision.CompareTag("Player") && playerHealth != null)
        {
            if (dmgTickTimer >= tickInterval)
            {
                playerHealth.TakeDamage(poisonDamagePerTick);
                dmgTickTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = null;
            dmgTickTimer = 0f;
        }
    }
}
