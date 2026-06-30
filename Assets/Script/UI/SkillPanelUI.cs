using UnityEngine;
using System.Collections.Generic;

public class SKillPanelUI : MonoBehaviour
{
    public Transform skillPanel; // Kéo thả SkillPanel vào đây
    public GameObject skillUIPrefab; // Kéo prefab SkillItem vào đây
    public PlayerSkillManager playerSkillManager; // TODO: -> PlayerSkillManager

    private List<SkillUI> skillUIs = new List<SkillUI>();

    private void Start()
    {
        if (playerSkillManager != null)
        {
            SetSkills(playerSkillManager.GetSkills()); // TODO: -> PlayerSkillManager
        }
    }

    public void SetSkills(List<SkillBase> skills)
    {
        foreach (Transform child in skillPanel)
            Destroy(child.gameObject);
        skillUIs.Clear();

        foreach (var skill in skills)
        {
            var go = Instantiate(skillUIPrefab, skillPanel);
            var ui = go.GetComponent<SkillUI>();
            if (skill != null)
                ui.SetIcon(skill.icon);
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
        if (playerSkillManager != null) // TODO: -> PlayerSkillManager
        {
            var cooldowns = new List<float>();
            foreach (var skill in playerSkillManager.GetSkills()) // TODO
            {
                cooldowns.Add(skill != null ? skill.CooldownTimer : 0f);
            }
            UpdateSkillCooldowns(cooldowns);
        }
    }

}
