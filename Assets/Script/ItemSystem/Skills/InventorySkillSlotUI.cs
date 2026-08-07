using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySkillSlotUI : MonoBehaviour
{
    public Image icon;
    public Image[] levelPips;
    public TMP_Text levelText;

    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    public void SetSkill(SkillBase skill, int level)
    {
        if (skill == null)
        {
            ClearSlot();
            return;
        }

        if (icon != null)
        {
            icon.sprite = skill.icon;
            icon.enabled = skill.icon != null;
        }

        for (int i = 0; i < levelPips.Length; i++)
        {
            if (levelPips[i] != null)
            {
                levelPips[i].color = i < level ? unlockedColor : lockedColor;
                levelPips[i].enabled = true;
            }
        }

        if (levelText != null)
        {
            levelText.text = level > 0 ? $"Lv.{level}" : "Locked";
            levelText.gameObject.SetActive(true);
        }
    }

    public void ClearSlot()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        foreach (var pip in levelPips)
        {
            if (pip != null)
                pip.enabled = false;
        }

        if (levelText != null)
            levelText.gameObject.SetActive(false);
    }
}
