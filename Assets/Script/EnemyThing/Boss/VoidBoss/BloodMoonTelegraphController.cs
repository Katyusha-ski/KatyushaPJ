using UnityEngine;

public class BloodMoonTelegraphController : MonoBehaviour
{
    [SerializeField] private float damageRadius = 2.5f;
    [SerializeField] private int damageAmount = 15;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private bool hasDealtDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        hasDealtDamage = false;
    }

    public void DealAoEDamage()
    {
        if (hasDealtDamage) return;
        hasDealtDamage = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius, playerLayer);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damageAmount, gameObject);
        }
    }

    public void OnExplosionAnimEnd()
    {
        ObjectPool pool = ObjectPool.Instance;
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
