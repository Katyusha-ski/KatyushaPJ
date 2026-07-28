using UnityEngine;

public class AmbushTrapController : MonoBehaviour
{
    [SerializeField] private float delayBeforeStrike = 0.5f;
    [SerializeField] private int damage = 35;
    [SerializeField] private float silentDuration = 2f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float maxDashDistance = 4f;

    private SpriteRenderer spriteRenderer;
    private bool hasDealtDamage;
    private Vector3 dashDirection;
    private float distanceTraveled;
    private bool isDashing;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Invoke(nameof(BeginDash), delayBeforeStrike);
    }

    private void BeginDash()
    {
        Transform player = PlayerManager.Instance != null ? PlayerManager.Instance.PlayerTransform : null;
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = player.position;
        dashDirection = (targetPos - transform.position).normalized;
        dashDirection.z = 0f;
        if (spriteRenderer != null)
            spriteRenderer.flipX = dashDirection.x > 0f;
        distanceTraveled = 0f;
        isDashing = true;
    }

    private void Update()
    {
        if (!isDashing) return;

        float step = dashSpeed * Time.deltaTime;
        transform.position += dashDirection * step;
        distanceTraveled += step;

        if (distanceTraveled >= maxDashDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDashing || hasDealtDamage) return;
        if (!other.CompareTag("Player")) return;

        hasDealtDamage = true;

        Health health = other.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage, gameObject);

        StatusEffectController sec = other.GetComponent<StatusEffectController>();
        if (sec == null) sec = other.gameObject.AddComponent<StatusEffectController>();
        sec.ApplyEffect(new SilentEffect(silentDuration, other.gameObject));

        Destroy(gameObject);
    }
}
