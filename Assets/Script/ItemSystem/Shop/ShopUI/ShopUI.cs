using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("Panels")]
    public CategoryUI categoryUI;
    public ItemListUI itemListUI;
    public ItemDetailUI itemDetailUI;

    [Header("Data")]
    public ShopManager shopManager;

    void OnEnable()
    {
        itemDetailUI.Init(shopManager, this);
        categoryUI.OnCategorySelected += HandleCategorySelected;
        itemListUI.OnItemSelected += HandleItemSelected;
        categoryUI.SelectDefault();
    }

    void OnDisable()
    {
        categoryUI.OnCategorySelected -= HandleCategorySelected;
        itemListUI.OnItemSelected -= HandleItemSelected;
    }

    void HandleCategorySelected(ItemType category) 
    {
        var filtered = category.ToString() == "None" // use "None" to apply no filter, since "All" is reserved for item types   
            ? shopManager.entries
                .Where(e => e.item.itemType == category)
                .ToList()
            : shopManager.entries
                .Where(e => e.isUnlocked)
                .Where(e => e.item.itemType == category)
                .ToList();


        itemListUI.Populate(filtered);
        itemDetailUI.Clear();
    }

    void HandleItemSelected(ShopEntrySO entry)
    {
        itemDetailUI.ShowEntry(entry);
    }

    public void RefreshAll()
    {
        itemListUI.RefreshSlots();
        itemDetailUI.RefreshButton();
    }
}