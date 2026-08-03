using UnityEngine;
using System.Collections.Generic;

public class QuestListUI : MonoBehaviour
{
    [Header("Kéo GameObject 'Content' của ScrollView vào đây")]
    public Transform contentParent;

    [Header("Kéo prefab slot (có gắn QuestSlotUI) vào đây")]
    public GameObject slotPrefab;

    [Header("Kéo QuestDetailUI đang hiển thị chi tiết vào đây")]
    public QuestDetailUI detailUI;

    private readonly List<GameObject> spawnedSlots = new List<GameObject>();

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnQuestItemsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnQuestItemsChanged -= Refresh;
    }

    public void Refresh()
    {
        if (Inventory.Instance == null || contentParent == null || slotPrefab == null) return;

        foreach (var slot in spawnedSlots)
            Destroy(slot);
        spawnedSlots.Clear();

        foreach (ItemData item in Inventory.Instance.questItems)
        {
            GameObject slotGO = Instantiate(slotPrefab, contentParent);
            QuestSlotUI slotUI = slotGO.GetComponent<QuestSlotUI>();
            if (slotUI != null)
                slotUI.Setup(item, OnSlotClicked);
            spawnedSlots.Add(slotGO);
        }
    }

    private void OnSlotClicked(ItemData item)
    {
        if (detailUI != null)
            detailUI.ShowDetail(item);
    }
}
