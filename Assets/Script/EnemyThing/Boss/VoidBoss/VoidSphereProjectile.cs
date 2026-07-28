using System.Collections.Generic;
using UnityEngine;

public class VoidSphereProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float slowAmount = 0.5f;
    [SerializeField] private float armorDebuffDuration = 3f;
    [SerializeField] private float armorDebuffAmount = 15f;
    [SerializeField] private LayerMask playerLayer;

    private List<StatModifierConfig> slowConfigs;
    private List<StatModifierConfig> armorConfigs;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = PlayerManager.Instance != null ? PlayerManager.Instance.PlayerTransform : null;

        slowConfigs = new List<StatModifierConfig>
        {
            new StatModifierConfig { statType = StatType.MovementSpeed, value = -slowAmount, modifierType = ModifierType.Multiplicative }
        };
        armorConfigs = new List<StatModifierConfig>
        {
            new StatModifierConfig { statType = StatType.Armor, value = -armorDebuffAmount, modifierType = ModifierType.Additive }
        };
    }

    private void Start()
    {
        Invoke(nameof(HandleTimeout), lifeTime);
    }

    private void HandleTimeout()
    {
        if (hasHit) return;
        hasHit = true;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        if (animator != null)
            animator.SetTrigger("Explode");
    }

    private void FixedUpdate()
    {
        if (player == null || hasHit) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;
        hasHit = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Health health = other.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage, gameObject);

        StatusEffectController sec = other.GetComponent<StatusEffectController>();
        if (sec == null) sec = other.gameObject.AddComponent<StatusEffectController>();

        sec.ApplyEffect(new StunEffect(stunDuration, other.gameObject));
        sec.ApplyEffect(new StatModifierEffect(slowDuration, other.gameObject, slowConfigs, true));
        sec.ApplyEffect(new StatModifierEffect(armorDebuffDuration, other.gameObject, armorConfigs, true));

        if (animator != null)
            animator.SetTrigger("Explode");
    }

    public void OnVoidSphereExplodeEnd()
    {
        Destroy(gameObject);
    }
}
