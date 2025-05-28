using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthText;

    public void SetHealth(int health, int maxHealth)
    {
        slider.value = health;
        if (healthText != null)
            healthText.text = $"{health}/{maxHealth}";
    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        if (healthText != null)
            healthText.text = $"{maxHealth}/{maxHealth}";
    }
}
