using System.Collections.Generic;
using UnityEngine;

/*
 * Setup:
 *   1. Tao GameObject "SkillPanel" (con cua InventoryBoard).
 *   2. Gan InventorySkillPanelUI.
 *   3. Keo Transform chua 4 skill slot vao skillSlotParent.
 *   4. Tao prefab 1 skill slot (gom InventorySkillSlotUI + icon + 5 pip Image),
 *      keo vao skillSlotPrefab.
 *
 * Flow: Awake() instantiate 4 slot tu prefab.
 *       OnEnable() subscribe OnSkillMatrixChanged + Refresh().
 *       Refresh() lay data tu Inventory.GetSkillPanelData(i).
 */
public class InventorySkillPanelUI : MonoBehaviour
{
    public Transform skillSlotParent;
    public GameObject skillSlotPrefab;

    private List<InventorySkillSlotUI> slots = new List<InventorySkillSlotUI>();

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            var go = Instantiate(skillSlotPrefab, skillSlotParent);
            slots.Add(go.GetComponent<InventorySkillSlotUI>());
        }
    }

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnSkillMatrixChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnSkillMatrixChanged -= Refresh;
    }

    public void Refresh()
    {
        if (Inventory.Instance == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            var data = Inventory.Instance.GetSkillPanelData(i);
            slots[i].SetSkill(data.activeSkill, data.currentLevel);
        }
    }
}
