using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public HealthBar healthBar;
    public AudioClip damageSFX;
    public AudioClip dieSFX;
    public LootManager lootManager;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    private bool isUnDying = false;

    private CharacterStats characterStats;
    private float regenTimer = 0f;
    private const float REGEN_INTERVAL = 5f; //heal every 5 seconds

    public void SetUnDying(bool value)
    {
        isUnDying = value;
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
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

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        maxHealth = (int)characterStats.MaxHP;
        currentHealth = maxHealth;
        characterStats.MaxHPChanged += OnMaxHPChanged;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (damageSFX != null)
            AudioManager.Instance.PlaySFX(damageSFX);

        if (isUnDying && damage >= currentHealth)
        {
            return;
        }
        // calculate final damage after armor and damage reduction (will add armor piercing in future)
        float dmgReduction = characterStats != null ? characterStats.DmgR / 100f : 0f;
        float armor = characterStats != null ? characterStats.Armor : 0f;
        float finalDamage = Mathf.Max(1f, (damage - armor) * (1f - dmgReduction));
        currentHealth -= (int)finalDamage;
        
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if(gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = GetComponent<EnemyController>();
            if (enemyController != null)
            {
                IEnemyState hurtState = enemyController.GetHurtState(enemyController.GetCurrentState());
                enemyController.ChangeState(hurtState);
            }
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
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
        
        if (dieSFX != null)
        {
            AudioManager.Instance.PlaySFX(dieSFX);
        }
        Debug.Log($"{gameObject.name} has died.");
    }

    private void OnMaxHPChanged(float newMaxHP)
    {
        maxHealth = (int)newMaxHP;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    private void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.MaxHPChanged -= OnMaxHPChanged;
        }
    }
}
