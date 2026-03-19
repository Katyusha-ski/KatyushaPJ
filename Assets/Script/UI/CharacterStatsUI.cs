using TMPro;
using UnityEngine;

public class CharacterStatsUI : MonoBehaviour
{
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private Transform statsContainer; // Parent chứa stats
    [SerializeField] private TextMeshProUGUI statPrefab; // Prefab text

    private CharacterStats characterStats;

    private void Start()
    {
        characterStats = FindFirstObjectByType<CharacterStats>();
        if (characterStats == null)
            Debug.LogError("CharacterStats not found!");

        if (statsPanel != null)
            statsPanel.SetActive(false);
    }

    public void ToggleStats()
    {
        if (statsPanel == null)
            return;

        if (statsPanel.activeSelf)
            statsPanel.SetActive(false);
        else
        {
            statsPanel.SetActive(true);
            UpdateStats();
        }
    }

    public void ShowStats()
    {
        if (statsPanel == null)
            return;

        statsPanel.SetActive(true);
        UpdateStats();
    }

    public void HideStats()
    {
        if (statsPanel == null)
            return;

        statsPanel.SetActive(false);
    }

    private void UpdateStats()
    {
        if (characterStats == null || statsContainer == null)
            return;

        // Xóa stats cũ
        foreach (Transform child in statsContainer)
            Destroy(child.gameObject);

        // Tạo stats mới từ CharacterStats data
        CreateStatText("Armor", characterStats.Armor.ToString("F1"));
        CreateStatText("Life Steal", (characterStats.LifeSteal * 100).ToString("F1") + "%");
        CreateStatText("CC Res", characterStats.CCRes.ToString("F1"));
        CreateStatText("ATK", characterStats.Atk.ToString("F1"));
        CreateStatText("Crit Rate", characterStats.CritRate.ToString("F1") + "%");
        CreateStatText("Crit DMG", (characterStats.CritDamage * 100).ToString("F1") + "%");
        CreateStatText("Armor Pierce", characterStats.ArmorPierce.ToString("F1"));
        CreateStatText("CDR", characterStats.CDR.ToString("F1") + "%");
        CreateStatText("Max HP", characterStats.MaxHP.ToString("F0"));
        CreateStatText("Movement Speed", characterStats.MovementSpeed.ToString("F2"));
    }

    private void CreateStatText(string statName, string statValue)
    {
        if (statPrefab == null)
        {
            Debug.LogError("Stat Prefab not assigned!");
            return;
        }

        var textObj = Instantiate(statPrefab, statsContainer);
        textObj.text = $"{statName}: {statValue}";
    }
}
