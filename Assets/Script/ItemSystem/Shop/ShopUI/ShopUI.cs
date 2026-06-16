using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUI : MenuUI
{
    [Header("Panels")]
    public CategoryUI categoryUI;
    public ItemListUI itemListUI;
    public ItemDetailUI itemDetailUI;

    [Header("Data")]
    public ShopManager shopManager;

    void OnEnable() 
    {
        if (shopManager == null || itemDetailUI == null || categoryUI == null || itemListUI == null)
            return;

        shopManager.UnlockByChapter(ChapterManager.Instance.CurrentChapterNumber);
        itemDetailUI.Init(shopManager, this);
        categoryUI.OnCategorySelected += HandleCategorySelected;
        itemListUI.OnItemSelected += HandleItemSelected;
        categoryUI.SelectDefault();
    }

    void OnDisable()
    {
        if (categoryUI != null)
            categoryUI.OnCategorySelected -= HandleCategorySelected;
        if (itemListUI != null)
            itemListUI.OnItemSelected -= HandleItemSelected;
    }

    void HandleCategorySelected(ItemType category) 
    {
        if (shopManager?.entries == null) return;

        var filtered = category.ToString() == "All"
            ? shopManager.entries.Where(e => e.isUnlocked).ToList()
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