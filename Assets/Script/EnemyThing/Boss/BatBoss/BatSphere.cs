using UnityEngine;

public class BatSphere : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject hazardZonePrefab;
    [SerializeField] private float hazardDuration = 5f;
    [SerializeField] private int dotDamage = 3;
    [SerializeField] private float dotInterval = 1f;
    [SerializeField] private float dotDuration = 3f;

    [Header("AoE Explosion")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int explosionDamage = 5;

    [Header("Animation")]
    [SerializeField] private float explosionAnimDuration = 0.5f;

    private Transform player;
    private Animator animator;
    private bool isExploding = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Transform playerTarget)
    {
        player = playerTarget;
    }

    private void Update()
    {
        if (isExploding) return;

        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int groundMask = groundLayer;
        if ((groundMask & (1 << collision.gameObject.layer)) != 0)
        {
            Land();
            return;
        }

    }

    private void Land()
    {
        isExploding = true;

        if (animator != null)
            animator.SetTrigger("Explore");

        ExplosionBurst();

        if (hazardZonePrefab != null)
        {
            GameObject zone = Instantiate(hazardZonePrefab, transform.position, Quaternion.identity);
            HazardZone haz = zone.GetComponent<HazardZone>();
            if (haz != null)
                haz.Init(dotDamage, dotInterval, dotDuration, hazardDuration);
            else
                Destroy(zone, hazardDuration);
        }

        Destroy(gameObject, explosionAnimDuration);
    }

    private void ExplosionBurst()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        var damagedHealth = new System.Collections.Generic.HashSet<Health>();
        foreach (var hit in hits)
        {
            Health h = hit.GetComponentInParent<Health>();
            if (h != null && damagedHealth.Add(h))
                h.TakeDamage(explosionDamage, DamageSource.SystemSource);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Gizmos.color = new Color(1f, 0.2f, 0.05f, 0.15f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}


