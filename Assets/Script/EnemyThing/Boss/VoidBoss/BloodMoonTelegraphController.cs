using UnityEngine;

public class BloodMoonTelegraphController : MonoBehaviour
{
    [SerializeField] private float damageRadius = 2.5f;
    [SerializeField] private int damageAmount = 30;

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

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);
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
