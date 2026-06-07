using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemListUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject shopSlotPref;
    public ShopManager shopManager;

    public event Action<ShopEntrySO> OnItemSelected;

    private List<ShopSlotUI> spawnedSlots = new List<ShopSlotUI>();
    private ShopSlotUI currentSelected;

    public void Populate(List<ShopEntrySO> entries)
    {
        if (contentParent == null || shopSlotPref == null || shopManager == null) return;

        foreach (Transform t in contentParent) Destroy(t.gameObject);
        spawnedSlots.Clear();
        currentSelected = null;

        if (entries == null) return;
        foreach (var entry in entries)
        {
            var go = Instantiate(shopSlotPref, contentParent);
            var slot = go.GetComponent<ShopSlotUI>();
            slot.Setup(entry, shopManager);
            slot.OnClicked += HandleSlotClicked;
            spawnedSlots.Add(slot);
        }
    }

    public void RefreshSlots()
    {
        foreach (var slot in spawnedSlots) {
            slot.Refresh();
        }
    }

    private void HandleSlotClicked(ShopSlotUI clickedSlot, ShopEntrySO entry)
    {
        if (currentSelected != null && clickedSlot != currentSelected)
            currentSelected.SetSelected(false);

        currentSelected = clickedSlot;
        currentSelected.SetSelected(true);

        OnItemSelected?.Invoke(entry);
    }
}