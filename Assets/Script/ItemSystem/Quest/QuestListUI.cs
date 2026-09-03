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
        Debug.Log($"[QuestListUI] Refresh: Inventory.Instance null = {Inventory.Instance == null}");
        Debug.Log($"[QuestListUI] Refresh: contentParent null = {contentParent == null}" +
                  (contentParent != null
                      ? $", name = '{contentParent.name}', path = '{GetTransformPath(contentParent)}'"
                      : string.Empty));
        Debug.Log($"[QuestListUI] Refresh: slotPrefab null = {slotPrefab == null}" +
                  (slotPrefab != null ? $", name = '{slotPrefab.name}'" : string.Empty));

        if (Inventory.Instance != null)
            Debug.Log($"[QuestListUI] Refresh: Inventory.questItems.Count = {Inventory.Instance.questItems.Count}");

        if (Inventory.Instance == null || contentParent == null || slotPrefab == null)
        {
            Debug.LogWarning("[QuestListUI] Refresh stopped because one or more required references are null.");
            return;
        }

        foreach (var slot in spawnedSlots)
            Destroy(slot);
        spawnedSlots.Clear();

        bool selectedItemStillExists = false;
        int instantiatedSlotCount = 0;
        foreach (ItemData item in Inventory.Instance.questItems)
        {
            if (item == selectedItem)
                selectedItemStillExists = true;

            GameObject slotGO = Instantiate(slotPrefab, contentParent);
            instantiatedSlotCount++;
            slotGO.SetActive(true);
            QuestSlotUI slotUI = slotGO.GetComponent<QuestSlotUI>();
            if (slotUI == null)
            {
                Debug.LogError($"[QuestListUI] Spawned object '{slotGO.name}' has no QuestSlotUI component.", slotGO);
            }
            else
            {
                slotUI.Setup(item, OnSlotClicked);
            }
            spawnedSlots.Add(slotGO);
        }

        Debug.Log($"[QuestListUI] Refresh: instantiated slot count = {instantiatedSlotCount}");

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
            if (slotUI != null && slotUI.Item == selectedItem)
            {
                slotUI.SelectButton();
                break;
            }
        }

        if (detailUI != null)
            detailUI.ShowDetail(item);
    }

    private static string GetTransformPath(Transform target)
    {
        string path = target.name;
        Transform current = target.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
