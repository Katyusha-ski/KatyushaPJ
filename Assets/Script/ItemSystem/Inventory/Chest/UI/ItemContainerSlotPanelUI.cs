using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainerSlotPanelUI : MonoBehaviour
{
    [Header("Slot UI")]
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;

    private readonly List<ItemContainerSlotUI> slotUIs =
        new List<ItemContainerSlotUI>();

    private IItemContainer container;

    public IItemContainer Container => container;

    public event Action<ItemContainerSlotUI> ItemClicked;
    public event Action<ItemContainerSlotUI> ItemDoubleClicked;

    public void Bind(IItemContainer newContainer)
    {
        Unbind();
        container = newContainer;

        if (container == null)
        {
            ClearSlots();
            return;
        }

        container.OnInventoryChanged += Refresh;
        BuildSlots();
        Refresh();
    }

    public void Unbind()
    {
        if (container != null)
            container.OnInventoryChanged -= Refresh;

        container = null;
    }

    private void OnDestroy()
    {
        Unbind();
    }

    private void BuildSlots()
    {
        ClearSlots();

        if (slotPrefab == null)
        {
            Debug.LogError(
                "[ItemContainerSlotPanelUI] Slot prefab is not assigned.",
                this
            );
            return;
        }

        Transform parent = slotParent != null ? slotParent : transform;
        GridLayoutGroup grid = parent.GetComponent<GridLayoutGroup>();

        if (grid == null)
        {
            Debug.LogWarning(
                "[ItemContainerSlotPanelUI] Slot parent has no GridLayoutGroup. " +
                "Slots will still be created, but layout must be configured manually.",
                parent
            );
        }

        for (int i = 0; i < container.MaxSlots; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, parent);
            slotObject.SetActive(true);
            ItemContainerSlotUI slotUI =
                slotObject.GetComponent<ItemContainerSlotUI>();

            if (slotUI == null)
            {
                Debug.LogError(
                    "[ItemContainerSlotPanelUI] Slot prefab is missing " +
                    "ItemContainerSlotUI.",
                    slotObject
                );
                Destroy(slotObject);
                continue;
            }

            slotUI.Initialize(i);
            slotUI.ItemClicked += HandleItemClicked;
            slotUI.ItemDoubleClicked += HandleItemDoubleClicked;
            slotUIs.Add(slotUI);
        }
    }

    private void ClearSlots()
    {
        foreach (ItemContainerSlotUI slotUI in slotUIs)
        {
            if (slotUI != null)
                Destroy(slotUI.gameObject);
        }

        slotUIs.Clear();
    }

    public void Refresh()
    {
        if (container == null)
            return;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            ItemStack stack = container.GetItemAt(i);

            if (stack != null && stack.item != null)
                slotUIs[i].SetItem(stack);
            else
                slotUIs[i].ClearSlot();
        }
    }

    private void HandleItemClicked(ItemContainerSlotUI slotUI)
    {
        ItemClicked?.Invoke(slotUI);
    }

    private void HandleItemDoubleClicked(ItemContainerSlotUI slotUI)
    {
        ItemDoubleClicked?.Invoke(slotUI);
    }
}
