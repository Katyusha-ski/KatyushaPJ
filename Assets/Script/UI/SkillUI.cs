using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI cooldownText;

    private bool hasLoggedCooldownState;
    private bool lastCooldownActive;

    public void SetSkill(SkillBase skill)
    {
        Debug.Log($"[SkillPanelDebug] SkillUI.SetSkill: skill={(skill != null ? skill.name : "NULL")}, icon ref={(icon != null ? "OK" : "NULL")}, icon.sprite={(icon != null && icon.sprite != null ? icon.sprite.name : "NULL")}", this);
        if (skill == null)
        {
            Clear();
            return;
        }

        SetIcon(skill.icon);
    }

    public void SetIcon(Sprite iconSprite)
    {
        Debug.Log($"[SkillPanelDebug] SkillUI.SetIcon: icon ref={(icon != null ? "OK" : "NULL")}, incoming sprite={(iconSprite != null ? iconSprite.name : "NULL")}", this);
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
            if (!hasLoggedCooldownState || lastCooldownActive)
                Debug.Log($"[SkillPanelDebug] SkillUI.SetCooldown skipped: icon.enabled=false, time={time:F1}", this);

            hasLoggedCooldownState = true;
            lastCooldownActive = false;
            cooldownText.text = "";
            cooldownText.enabled = false;
            return;
        }

        if (!hasLoggedCooldownState || cooldownActive != lastCooldownActive)
            Debug.Log($"[SkillPanelDebug] SkillUI.SetCooldown state changed: active={cooldownActive}, time={time:F1}", this);

        hasLoggedCooldownState = true;
        lastCooldownActive = cooldownActive;
        cooldownText.text = time > 0 ? time.ToString("F1") : "";
        cooldownText.enabled = cooldownActive;

        var color = icon.color;
        color.a = cooldownActive ? 0.5f : 1f; 
        icon.color = color;
    }

    public void Clear()
    {
        hasLoggedCooldownState = false;
        lastCooldownActive = false;

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
