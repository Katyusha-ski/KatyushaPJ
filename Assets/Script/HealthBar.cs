using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText; 

    public void SetHealth(int health)
    {
        healthSlider.value = health;
        if (healthText != null)
            healthText.text = health.ToString();
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        if (healthText != null)
            healthText.text = ((int)maxHealth).ToString();
    }
}
