using UnityEngine;
using System.Collections.Generic;

public class SkillPanelUI : MonoBehaviour
{
    public Transform skillPanel; // Kéo thả SkillPanel vào đây
    public GameObject skillUIPrefab; // Kéo prefab SkillItem vào đây
    public PlayerSkillManager playerSkillManager;

    private List<SkillUI> skillUIs = new List<SkillUI>();

    private void Awake()
    {
        if (playerSkillManager == null)
            playerSkillManager = GetComponentInParent<PlayerSkillManager>();
    }

    private void Start()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnSkillMatrixChanged += RefreshSkills;
        else
            Debug.LogWarning("[SkillPanelUI] Inventory.Instance is missing during Start; skill matrix events will not be received.", this);

        RefreshSkills();
    }

    private void OnEnable()
    {
        RefreshSkills();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnSkillMatrixChanged -= RefreshSkills;
    }

    public void RefreshSkills()
    {
        if (skillPanel == null || skillUIPrefab == null)
        {
            Debug.LogWarning("[SkillPanelUI] Skill panel or prefab is not assigned.", this);
            return;
        }

        if (playerSkillManager == null)
        {
            Debug.LogWarning("[SkillPanelUI] PlayerSkillManager is missing.", this);
            return;
        }

        foreach (Transform child in skillPanel)
            Destroy(child.gameObject);
        skillUIs.Clear();

        foreach (var skill in playerSkillManager.GetSkills())
        {
            var go = Instantiate(skillUIPrefab, skillPanel);
            var ui = go.GetComponent<SkillUI>();
            if (ui == null)
            {
                Debug.LogWarning("[SkillPanelUI] SkillUIPrefab is missing SkillUI component.", go);
                continue;
            }

            ui.SetSkill(skill);

            skillUIs.Add(ui);
        }
    }

    public void UpdateSkillCooldowns(List<float> cooldowns)
    {
        for (int i = 0; i < skillUIs.Count && i < cooldowns.Count; i++)
        {
            skillUIs[i].SetCooldown(cooldowns[i]);
        }
    }

    private void Update()
    {
        if (playerSkillManager != null)
        {
            var cooldowns = new List<float>();
            foreach (var skill in playerSkillManager.GetSkills())
            {
                cooldowns.Add(skill != null ? skill.CooldownTimer : 0f);
            }
            UpdateSkillCooldowns(cooldowns);
        }
    }
}
