using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI cooldownText;

    public void SetIcon(Sprite iconSprite)
    {
        icon.sprite = iconSprite;
    }

    public void SetCooldown(float time)
    {
        cooldownText.text = time > 0 ? time.ToString("F1") : "";
        cooldownText.enabled = time > 0;

        var color = icon.color;
        color.a = time > 0 ? 0.5f : 1f; 
        icon.color = color;
    }
}
