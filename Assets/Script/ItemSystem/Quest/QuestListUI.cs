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
    private ItemData selectedItem;
    private bool hasStarted;
    private bool subscribed;

    private void Start()
    {
        hasStarted = true;
        SubscribeToInventory();
        Refresh();
    }

    private void OnEnable()
    {
        if (hasStarted)
            SubscribeToInventory();
        Refresh();
    }

    private void OnDisable()
    {
        if (subscribed && Inventory.Instance != null)
            Inventory.Instance.OnQuestItemsChanged -= Refresh;
        subscribed = false;
    }

    private void SubscribeToInventory()
    {
        if (subscribed || Inventory.Instance == null) return;

        Inventory.Instance.OnQuestItemsChanged += Refresh;
        subscribed = true;
    }

    public void Refresh()
    {
        if (Inventory.Instance == null || contentParent == null || slotPrefab == null) return;

        foreach (var slot in spawnedSlots)
            Destroy(slot);
        spawnedSlots.Clear();

        bool selectedItemStillExists = false;
        foreach (ItemData item in Inventory.Instance.questItems)
        {
            if (item == selectedItem)
                selectedItemStillExists = true;

            GameObject slotGO = Instantiate(slotPrefab, contentParent);
            slotGO.SetActive(true);
            QuestSlotUI slotUI = slotGO.GetComponent<QuestSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(item, OnSlotClicked);
                slotUI.SetSelected(item == selectedItem);
            }
            spawnedSlots.Add(slotGO);
        }

        if (!selectedItemStillExists)
            selectedItem = null;

        if (selectedItem == null && Inventory.Instance.questItems.Count > 0)
            OnSlotClicked(Inventory.Instance.questItems[0]);
        else if (Inventory.Instance.questItems.Count == 0 && detailUI != null)
            detailUI.Clear();
    }

    private void OnSlotClicked(ItemData item)
    {
        selectedItem = item;

        foreach (GameObject slot in spawnedSlots)
        {
            QuestSlotUI slotUI = slot.GetComponent<QuestSlotUI>();
            if (slotUI != null)
                slotUI.SetSelected(slotUI.Item == selectedItem);
        }

        if (detailUI != null)
            detailUI.ShowDetail(item);
    }
}
