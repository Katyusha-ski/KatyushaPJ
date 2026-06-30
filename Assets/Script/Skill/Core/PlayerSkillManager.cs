using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public List<SkillBase> skills = new List<SkillBase>();
    public CharacterStats characterStats;

    private void Start()
    {
        if (characterStats == null)
            characterStats = GetComponent<CharacterStats>();

        ReloadSkills();
    }

    public void ReloadSkills()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i] = Inventory.Instance.GetHighestSkill(i);
            if (skills[i] != null)
                skills[i].Initialize(characterStats);
        }
    }

    public List<SkillBase> GetSkills()
    {
        return skills;
    }

    void Update()
    {
        foreach (var skill in skills)
        {
            if (skill != null)
                skill.UpdateCooldown();
        }
    }

    public void ActivateSkill(int skillIndex, int direction)
    {
        if (skillIndex >= 0 && skillIndex < skills.Count)
            skills[skillIndex]?.Activate(gameObject, direction);
    }

    public bool UseItem(ItemData skillItem)
    {
        if (skillItem == null || skillItem.itemType != ItemType.Skill) return false;
        if (skillItem.skillData == null || skillItem.skillData.skill == null) return false;

        SkillType type = skillItem.skillData.skill.skillType;
        int newLevel = skillItem.skillData.Level;
        int row = Inventory.SkillTypeToRow(type);
        if (row < 0) return false;

        int currentLevel = 0;
        for (int i = Inventory.Instance.skillMatrix.GetLength(1) - 1; i >= 0; i--)
        {
            if (Inventory.Instance.skillMatrix[row, i] != null)
            {
                currentLevel = i + 1;
                break;
            }
        }

        if (newLevel != currentLevel + 1)
        {
            Debug.LogWarning($"Cannot add skill {type} level {newLevel}. Current: {currentLevel}. Need: {currentLevel + 1}.");
            return false;
        }

        Inventory.Instance.SetSkill(row, newLevel - 1, new ItemStack(skillItem, 1));
        ReloadSkills();
        return true;
    }
}