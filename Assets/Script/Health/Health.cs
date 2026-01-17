using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    public HealthBar healthBar;
    public AudioClip damageSFX;
    public AudioClip dieSFX;
    public LootManager lootManager;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if(damageSFX != null)
            AudioManager.Instance.PlaySFX(damageSFX);
        currentHealth -= damage;
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
}
