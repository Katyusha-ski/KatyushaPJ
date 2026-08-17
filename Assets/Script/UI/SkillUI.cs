using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI cooldownText;

    public void SetSkill(SkillBase skill)
    {
        if (skill == null)
        {
            Clear();
            return;
        }

        SetIcon(skill.icon);
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (icon == null) return;

        icon.sprite = iconSprite;
        icon.enabled = iconSprite != null;
    }

    public void SetCooldown(float time)
    {
        if (cooldownText == null || icon == null) return;

        bool cooldownActive = time > 0f;

        if (!icon.enabled)
        {
            cooldownText.text = "";
            cooldownText.enabled = false;
            return;
        }

        cooldownText.text = time > 0 ? time.ToString("F1") : "";
        cooldownText.enabled = cooldownActive;

        var color = icon.color;
        color.a = cooldownActive ? 0.5f : 1f;
        icon.color = color;
    }

    public void Clear()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (cooldownText != null)
        {
            cooldownText.text = "";
            cooldownText.enabled = false;
        }
    }
}
