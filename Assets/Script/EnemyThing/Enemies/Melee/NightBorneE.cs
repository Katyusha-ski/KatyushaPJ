using UnityEngine;

public class NightBorneE : EnemyController
{
    [Header("Explosion Burst")]
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private int explosionDamage = 15;

    [Header("Hazard Zone")]
    [SerializeField] private GameObject hazardZonePrefab;

    private LayerMask playerLayer;

    protected override void Start()
    {
        base.Start();
        playerLayer = LayerMask.GetMask("Player");
    }

    public override void HandleEnemyDeath()
    {
        base.HandleEnemyDeath();

        ExplosionBurst();

        if (hazardZonePrefab != null)
        {
            Instantiate(hazardZonePrefab, transform.position, Quaternion.identity);
        }
    }

    private void ExplosionBurst()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, explosionRadius, playerLayer);

        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(explosionDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
