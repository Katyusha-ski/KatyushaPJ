using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public List<SkillBase> skills = new List<SkillBase>();
    public CharacterStats characterStats;

    private StandAnimationController standAnimation;

    private void Awake()
    {
        standAnimation = GetComponentInChildren<StandAnimationController>(true);
    }

    private void Start()
    {
        if (characterStats == null)
            characterStats = GetComponent<CharacterStats>();

        ReloadSkills();
    }

    public void ReloadSkills()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("[PlayerSkillManager] Inventory.Instance is null; skill matrix is not loaded.");
            return;
        }

        const int skillRows = 4;
        while (skills.Count < skillRows)
            skills.Add(null);

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i] = Inventory.Instance.GetHighestSkill(i);
            if (skills[i] != null)
                skills[i].Initialize(characterStats);
        }

        // Bắn event ĐÚNG MỘT LẦN, sau khi mọi thay đổi (UnlockSkill + ReloadSkills)
        // đã hoàn tất, để subscriber (SkillPanelUI/SkillSystemUI) đọc được dữ liệu mới nhất.
        Inventory.Instance.NotifySkillMatrixChanged();
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
        {
            SkillBase skill = skills[skillIndex];
            if (skill == null) return;

            skill.Activate(gameObject, direction);

            // Animation routing (tập trung 1 điểm duy nhất):
            // - Melee/Defend/Range dùng chung stance "Def" của Hachi.
            // - Dash có animation riêng (dashTriggerName), xử lý trong DashSkill.
            // StandAnimationController là nơi duy nhất gọi SetTrigger("Def").
            if (skill.skillType == SkillType.Melee ||
                skill.skillType == SkillType.Defend ||
                skill.skillType == SkillType.Range)
            {
                standAnimation?.TriggerCastStance();
            }
        }
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
        for (int i = 4; i >= 0; i--)
        {
            if (Inventory.Instance.IsSkillUnlocked(row, i))
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

        ItemData expectedItem = Inventory.Instance.GetSkillItemAt(row, newLevel - 1);
        if (expectedItem != skillItem)
        {
            Debug.LogWarning($"Item {skillItem?.itemName} không khớp layout tại vị trí " +
                $"({row},{newLevel - 1}) — expected {expectedItem?.itemName}.");
            return false;
        }

        Inventory.Instance.UnlockSkill(row, newLevel - 1);
        ReloadSkills(); // bắn NotifySkillMatrixChanged 1 lần sau khi mọi thứ xong
        return true;
    }
}