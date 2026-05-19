using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour, IHealthBar
{
    public Image fillImage;
    public Text healthText;
    private Material _materialInstance;

    private static readonly int HealthProperty = Shader.PropertyToID("_Health");

    private void Awake()
    {
        if (fillImage != null && fillImage.material != null)
        {
            _materialInstance = new Material(fillImage.material);
            fillImage.material = _materialInstance;
        }
    }

    public void SetHealth(int health, int maxHealth)
    {
        SetHealthValue(health, maxHealth);
        if (healthText != null)
            healthText.text = $"{health}/{maxHealth}";
    }

    public void SetMaxHealth(int maxHealth)
    {
        SetHealthValue(maxHealth, maxHealth);
        if (healthText != null)
            healthText.text = $"{maxHealth}/{maxHealth}";
    }

    private void SetHealthValue(int health, int maxHealth)
    {
        if (_materialInstance == null || maxHealth <= 0)
            return;

        float normalizedHealth = Mathf.Clamp01((float)health / maxHealth);
        _materialInstance.SetFloat(HealthProperty, normalizedHealth);
    }
}
