using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject UI;
    public Transform equipmentSlot;
    public Transform slotParent;
    public GameObject slotPrefab;
    public Button useItemButton;
    public GameObject itemDetailPanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemStatsText;
    private List<Slot> slotScripts = new List<Slot>();
    private List<Slot> equipmentSlotScripts = new List<Slot>();
    private int useSlotIndex = -1;

    public void ShowInventory()
    {
        UI.SetActive(true);
    }

    public void HideInventory()
    {
        UI.SetActive(false);
    }

    private void Awake()
    {
        if (useItemButton != null)
            useItemButton.onClick.AddListener(UseItem);

        if (slotPrefab == null) { Debug.LogError("slotPrefab is not assigned!", this); return; }
        if (slotParent == null) { Debug.LogError("slotParent is not assigned!", this); return; }
        if (equipmentSlot == null) { Debug.LogError("equipmentSlot is not assigned!", this); return; }

        for (int i = 0; i < Inventory.Instance.maxSlots; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotScript = slotGO.GetComponent<Slot>();
            if (slotScript != null)
            {
                slotScript.slotIndex = i;
                slotScripts.Add(slotScript);
            }
        }

        for (int i = 0; i < Inventory.Instance.equipmentSlots; i++)
        {
            var slotGO = Instantiate(slotPrefab, equipmentSlot);
            var slotScript = slotGO.GetComponent<Slot>();
            if (slotScript != null)
            {
                slotScript.slotIndex = i;
                slotScript.isEquipmentSlot = true;
                equipmentSlotScripts.Add(slotScript);
            }
        }
    }

    private void OnEnable()
    {
        Inventory.Instance.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        Inventory.Instance.OnInventoryChanged -= UpdateUI;
    }

    private void Update()
    {
        if (!itemDetailPanel.activeSelf) return;
        if (Input.GetMouseButtonDown(1))
        {
            var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);
            foreach (var r in results)
            {
                if (r.gameObject.GetComponent<Slot>() != null)
                    return;
            }
            HideItemDetail();
        }
    }

    private void UpdateUI()
    {
        var slots = Inventory.Instance.itemSlots;
        for (int i = 0; i < slotScripts.Count; i++)
        {
            var stack = slots[i];
            if (stack != null && stack.item != null)
                slotScripts[i].SetItem(stack.item, stack.amount);
            else
                slotScripts[i].ClearSlot();
        }

        var equips = Inventory.Instance.equipment;
        for (int i = 0; i < equipmentSlotScripts.Count; i++)
        {
            var stack = equips[i];
            if (stack != null && stack.item != null)
                equipmentSlotScripts[i].SetItem(stack.item, stack.amount);
            else
                equipmentSlotScripts[i].ClearSlot();
        }
    }

    public void ShowItemDetailAt(int slotIndex, Vector3 position)
    {
        useSlotIndex = slotIndex;
        var item = Inventory.Instance.itemSlots[slotIndex]?.item;
        if (item == null) return;

        itemDetailPanel.SetActive(true);
        var panelRect = itemDetailPanel.GetComponent<RectTransform>();

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        if (itemStatsText != null)
        {
            if (item.IsEquipment() && item.GetStats().HasStats())
            {
                string statsStr = "";
                foreach (var cfg in item.GetStats().statConfigs)
                {
                    string prefix = cfg.modifierType == ModifierType.Multiplicative ? " x" : " +";
                    statsStr += $"{ItemStats.GetDisplayName(cfg.statType)}:{prefix}{cfg.value}\n";
                }
                itemStatsText.text = statsStr.TrimEnd('\n');
                itemStatsText.gameObject.SetActive(true);
            }
            else
            {
                itemStatsText.gameObject.SetActive(false);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

        Vector2 mousePos = Input.mousePosition;
        Vector2 pivot = panelRect.pivot;
        pivot.x = mousePos.x > Screen.width * 0.5f ? 1f : 0f;
        pivot.y = mousePos.y > Screen.height * 0.5f ? 1f : 0f;
        panelRect.pivot = pivot;

        itemDetailPanel.transform.position = mousePos;
    }

    public void HideItemDetail()
    {
        itemDetailPanel.SetActive(false);
        useSlotIndex = -1;
    }

    private void UseItem()
    {
        if (useSlotIndex >= 0)
            Inventory.Instance.UseItem(useSlotIndex);
        itemDetailPanel.SetActive(false);
        useSlotIndex = -1;
    }
}
