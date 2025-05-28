using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    public HealthBar healthBar;

    public int CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
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
            Destroy(gameObject);
        }
        Debug.Log($"{gameObject.name} has died.");
    }
}
