using UnityEngine;


[RequireComponent(typeof(CharacterStats))]
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 300;
    [SerializeField] private int currentHealth;
    [SerializeField] private MonoBehaviour healthBar;
    [SerializeField] private int virtualShield; 
    public int VirtualShield => virtualShield;
    public void SetShield(int amount) => virtualShield = Mathf.Max(0, amount);
    public void AddShield(int amount) => virtualShield += amount;
    private IHealthBar _healthBar;
    public AudioClip damageSFX;
    public AudioClip dieSFX;
    public LootManager lootManager;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    private bool isUnDying = false;
    private bool isInvulnerable = false;
    private bool isDead = false;

    private CharacterStats characterStats;
    private float regenTimer = 0f;
    private const float REGEN_INTERVAL = 5f; //heal every 5 seconds


    public System.Action<int> OnDamaged;
    public event System.Action<Health> OnDied;

    public void SetUnDying(bool value)
    {
        isUnDying = value;
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);

        if (_healthBar != null)
        {
            _healthBar.SetHealth(currentHealth, maxHealth);
        }

    }

    private void Update()
    {
        // HP Regeneration every 5 seconds
        if (currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer >= REGEN_INTERVAL && characterStats != null)
            {
                int regenAmount = (int)characterStats.HPRegen;
                if (regenAmount > 0)
                {
                    Heal(regenAmount);
                }
                regenTimer = 0f;
            }
        }
        else
        {
            regenTimer = 0f;
        }
    }

    private void Awake()
    {
        _healthBar = healthBar as IHealthBar;
        characterStats = GetComponent<CharacterStats>();
        if (characterStats != null)
        {
            maxHealth = (int)characterStats.MaxHP;
            characterStats.MaxHPChanged += OnMaxHPChanged;
        }
        currentHealth = maxHealth;
        _healthBar?.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    public virtual void TakeDamage(int damage, GameObject damageSource)
    {
        if (isDead || isInvulnerable)
            return;

        if (damageSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(damageSFX);

        if (virtualShield > 0)
        {
            int absorbed = Mathf.Min(damage, virtualShield);
            virtualShield -= absorbed;
            damage -= absorbed;

            if (damage <= 0)
            {
                _healthBar?.SetHealth(currentHealth, maxHealth);
                return;
            }
        }

        float dmgReduction = characterStats != null ? characterStats.DmgR / 100f : 0f;
        float armor = characterStats != null ? characterStats.Armor : 0f;

        if (damageSource != null)
        {
            CharacterStats attackerStats = damageSource.GetComponent<CharacterStats>();
            if (attackerStats != null && attackerStats.ArmorPierce > 0f)
                armor *= (1f - attackerStats.ArmorPierce / 100f);
        }

        float finalDamage = Mathf.Max(1f, (damage - armor) * (1f - dmgReduction));
        if (isUnDying && (int)finalDamage >= currentHealth)
            return;

        if (finalDamage == 0) return;

        OnDamaged?.Invoke((int)finalDamage);
        currentHealth -= (int)finalDamage;
        
        _healthBar?.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if(gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = GetComponent<EnemyController>();
            if (enemyController != null && !(enemyController.GetCurrentState() is HurtState))
            {
                IEnemyState hurtState = enemyController.GetHurtState(enemyController.GetCurrentState());
                if (hurtState != enemyController.GetCurrentState())
                    enemyController.ChangeState(hurtState);
            }
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        _healthBar?.SetHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        OnDied?.Invoke(this);

        if (gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = GetComponent<EnemyController>();
            if (enemyController != null)
            {
                IEnemyState dieState = enemyController.GetDieState();
                enemyController.ChangeState(dieState);
            }
        }
        else if (gameObject.CompareTag("Player"))
        {
            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.ShowGameOverUI();
            }
            else
            {
                Debug.LogWarning("GameOverUI.Instance is null!");
            }
        }
        
        if (dieSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(dieSFX);
        }
        Debug.Log($"{gameObject.name} has died.");
    }

    private void OnMaxHPChanged(float newMaxHP)
    {
        maxHealth = (int)newMaxHP;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        _healthBar?.SetHealth(currentHealth, maxHealth);
    }

    private void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.MaxHPChanged -= OnMaxHPChanged;
        }
    }
}
