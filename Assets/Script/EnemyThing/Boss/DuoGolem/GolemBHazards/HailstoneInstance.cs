using UnityEngine;

public class HailstoneInstance : MonoBehaviour
{
    [SerializeField] private float defaultFallSpeed = 8f;
    [SerializeField] private float defaultLifetime = 4f;
    [SerializeField] private LayerMask playerLayer;

    private float fallSpeed;
    private float lifetime;
    private int damage;
    private bool active;

    private void Awake()
    {
        if (playerLayer.value == 0)
            playerLayer = LayerMask.GetMask("Player");
    }

    private void OnEnable()
    {
        active = false;
    }

    public void Initialize(int hailDamage, float speed, float duration)
    {
        damage = hailDamage;
        fallSpeed = speed > 0f ? speed : defaultFallSpeed;
        lifetime = duration > 0f ? duration : defaultLifetime;
        active = true;
    }

    private void Update()
    {
        if (!active) return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active || (playerLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        Health health = other.GetComponentInParent<Health>();
        if (health != null && damage > 0)
            health.TakeDamage(damage, DamageSource.SystemSource);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (!active) return;
        active = false;
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
