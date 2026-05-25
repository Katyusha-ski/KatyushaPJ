using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoriesUI : MonoBehaviour
{
    public event Action<ItemType> OnCategorySelected;

    public void SelectCategory(String itemTypeName)
    {
        if (Enum.TryParse(itemTypeName, out ItemType category))
            OnCategorySelected?.Invoke(category);
    }
}
