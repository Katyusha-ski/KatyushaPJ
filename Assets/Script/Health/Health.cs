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
    private float damageReduction = 0f;
    
    public void SetDamageReduction(float reduction)
    {
        damageReduction = Mathf.Clamp01(reduction);
    }
    
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

    void Awake()
    {
        CharacterStats stats = GetComponent<CharacterStats>();
        maxHealth = (int)stats.MaxHP;
        currentHealth = maxHealth;
        stats.MaxHPChanged += OnMaxHPChanged;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if(damageSFX != null)
            AudioManager.Instance.PlaySFX(damageSFX);
        
        if (isUnDying && damage >= currentHealth)
        {
            return;
        }
        
        float finalDamage = damage * (1f - damageReduction);
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
        CharacterStats stats = GetComponent<CharacterStats>();
        if (stats != null)
        {
            stats.MaxHPChanged -= OnMaxHPChanged;
        }
            
    }
}
