using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;

    public HealthBar healthBar;
    public AudioClip damageSFX;
    public AudioClip dieSFX;

    // UI references
    public GameObject gameOverUI;
    public GameObject victoryUI;

    public int CurrentHealth => currentHealth;

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

            // Hiện UI Victory khi enemy chết
            if (victoryUI != null)
            {
                victoryUI.SetActive(true);
                GameManager.Instance.PauseGame();
            }
            
            Destroy(gameObject, 1f);
        }
        else if (gameObject.tag == "Player")
        {
            // Hiện UI GameOver khi player chết
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
                GameManager.Instance.PauseGame();
            }
        }
        
        if (dieSFX != null)
        {
            AudioManager.Instance.PlaySFX(dieSFX);
        }
        Debug.Log($"{gameObject.name} has died.");
    }
}
