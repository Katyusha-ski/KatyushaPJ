using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryUI : MonoBehaviour
{
    [System.Serializable]
    public class CategoryButton
    {
        public Button button;
        public ItemType itemType;
    }

    public List<CategoryButton> categoryButtons;
    public event Action<ItemType> OnCategorySelected;

    private Button currentSelected;

    private void Awake()
    {
        foreach (var cb in categoryButtons) {
            var cat = cb.itemType;
            var btn = cb.button;
            btn.onClick.AddListener(() => HandleClick(cat, btn));
        }
    }

    private void HandleClick(ItemType category, Button btn)
    {
        if (currentSelected != null)
            currentSelected.interactable = true;
        btn.interactable = false;
        currentSelected = btn;

        OnCategorySelected?.Invoke(category);
    }

    public void SelectDefault()
    {
        var first = categoryButtons[0];
        HandleClick(first.itemType, first.button);
    }
}