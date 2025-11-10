using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    public HealthBar healthBar;
    public AudioClip damageSFX;
    public AudioClip dieSFX;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public LootManager lootManager;

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);

        // Update health bar
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
    }

    private void Die()
    {
        if (gameObject.tag == "Enemy")
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            if (lootManager != null)
            {
                lootManager.SpawnLoot();
            }

            if (VictoryUI.Instance != null)
            {
                VictoryUI.Instance.ShowVictoryUI();
            }
            else
            {
                Debug.LogWarning("VictoryUI.Instance is null!");
            }
            Destroy(gameObject, 1f);
        }
        else if (gameObject.tag == "Player")
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
