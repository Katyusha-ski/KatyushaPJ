using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private GameObject bossNameText;

    private Health trackedHealth;

    public void SetBoss(Health bossHealth)
    {
        trackedHealth = bossHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.MaxHealth;
            healthSlider.value = bossHealth.CurrentHealth;
        }
        gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (trackedHealth == null) return;

        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, trackedHealth.CurrentHealth, Time.deltaTime * 10f);
        }
        if (fillImage != null && healthGradient != null)
        {
            fillImage.color = healthGradient.Evaluate(healthSlider.normalizedValue);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        trackedHealth = null;
    }
}
