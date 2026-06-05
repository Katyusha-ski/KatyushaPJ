using UnityEngine;
using System.Collections.Generic;
using System;

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
        foreach (Transform t in contentParent) Destroy(t.gameObject);
        spawnedSlots.Clear();
        foreach (var entry in entries) {
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
        if (clickedSlot == currentSelected) 
            currentSelected.SetSelected(false);

        currentSelected = clickedSlot;
        currentSelected.SetSelected(true);

        OnItemSelected?.Invoke(entry);
    }
}