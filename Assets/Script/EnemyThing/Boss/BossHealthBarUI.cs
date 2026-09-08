using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour, IHealthBar
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject bossNameText;

    private Health trackedHealth;

    public void SetBoss(Health bossHealth)
    {
        trackedHealth = bossHealth;
        if (bossHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.MaxHealth;
            healthSlider.value = bossHealth.CurrentHealth;
        }
        if (bossHealth != null)
            gameObject.SetActive(true);
    }

    public void SetHealth(int health, int maxHealth)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = Mathf.Max(1, maxHealth);
        healthSlider.value = Mathf.Clamp(health, 0, healthSlider.maxValue);
    }

    public void SetMaxHealth(int maxHealth)
    {
        SetHealth(maxHealth, maxHealth);
    }

    private void LateUpdate()
    {
        if (trackedHealth == null) return;

        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, trackedHealth.CurrentHealth, Time.deltaTime * 10f);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        trackedHealth = null;
    }
}
