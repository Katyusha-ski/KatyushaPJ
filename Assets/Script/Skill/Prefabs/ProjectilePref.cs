using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePref : MonoBehaviour, IProjectilePref
{
    public string targetTag = "Enemy";
    public float castingTime = 0f;
    public float lifeTime = 2f;
    public GameObject explodeEffect;

    private ProjectileConfig config;
    private Animator animator;
    private bool isCasting = true;
    private float castingTimer = 0f;
    private bool hasLaunched = false;
    private int moveDirection = 1;
    private float damage;
    private int hitCount = 0;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    void Start()
    {
        animator = GetComponent<Animator>();
        castingTimer = castingTime;
        if (castingTime <= 0f)
        {
            isCasting = false;
            hasLaunched = true;
        }
    }

    void Update()
    {
        if (isCasting)
        {
            castingTimer -= Time.deltaTime;
            if (castingTimer <= 0f)
            {
                isCasting = false;
                Launch();
            }
        }
        else if (hasLaunched)
        {
            transform.Translate(Vector2.right * moveDirection * config.speed * Time.deltaTime);
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0f)
                Destroy(gameObject);
        }
    }

    private void Launch()
    {
        hasLaunched = true;
        if (animator != null)
            animator.SetTrigger("Launch");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag))
            return;

        if (hitTargets.Contains(collision.gameObject))
            return;

        Health targetHealth = collision.GetComponent<Health>();
        if (targetHealth == null)
            return;

        targetHealth.TakeDamage((int)damage);
        hitTargets.Add(collision.gameObject);
        hitCount++;

        ApplyEffects(collision.gameObject);

        if (explodeEffect != null)
            Instantiate(explodeEffect, collision.ClosestPoint(transform.position), Quaternion.identity);

        if (hitCount > config.pierceCount)
            Destroy(gameObject);
    }

    private void ApplyEffects(GameObject target)
    {
        StatusEffectController sec = target.GetComponent<StatusEffectController>();
        if (sec == null)
            sec = target.AddComponent<StatusEffectController>();

        if (config.applyPoison)
        {
            var dot = new DoTEffect(config.poisonDuration, target, config.poisonDamagePerTick, config.poisonTickInterval);
            sec.ApplyEffect(dot);
        }

        if (config.applySlow)
        {
            var slowConfigs = new List<StatModifierConfig>
            {
                new StatModifierConfig { statType = StatType.MovementSpeed, value = -config.slowAmount, modifierType = ModifierType.Multiplicative }
            };
            var slow = new StatModifierEffect(config.slowDuration, target, slowConfigs, true);
            sec.ApplyEffect(slow);
        }

        if (config.applyArmorDebuff)
        {
            var armorConfigs = new List<StatModifierConfig>
            {
                new StatModifierConfig { statType = StatType.Armor, value = -config.armorDebuffAmount, modifierType = ModifierType.Additive }
            };
            var armorDebuff = new StatModifierEffect(config.armorDebuffDuration, target, armorConfigs, true);
            sec.ApplyEffect(armorDebuff);
        }
    }

    public void SetDirection(int direction, int isItNeedToFlip = 1)
    {
        moveDirection = direction;

        if ((direction == -1) != (isItNeedToFlip < 0))
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue;
    }

    public void SetProjectileConfig(ProjectileConfig projectileConfig)
    {
        config = projectileConfig;
    }
}
