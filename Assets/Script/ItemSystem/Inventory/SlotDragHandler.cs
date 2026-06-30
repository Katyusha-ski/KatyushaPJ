using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ============================================================================
// SLOT DRAG HANDLER
// ============================================================================
// Handle drag-and-drop + double-click + right-click cho slot inventory.
//
// INPUT -> OUTPUT:
//   Drag inventory -> equipment: equip
//   Drag equipment -> inventory: unequip
//   Drag inventory -> inventory: swap
//   Double-click consumable: use item
//   Right-click item: open item detail
// ============================================================================
public class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    private InventoryUI inventoryUI;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Slot slot;
    private Image draggedImage;
    private GameObject draggedOJ;
    void Awake()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        slot = GetComponent<Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventoryUI.HideItemDetail();
        if (!slot.HasItem()) return;
        draggedOJ = new GameObject("DraggedItem");
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        draggedOJ.transform.SetParent(canvas.transform, false);
        draggedImage = draggedOJ.AddComponent<Image>();

        draggedImage.sprite = slot.itemIcon.sprite;
        draggedImage.raycastTarget = false;

        var dragRect = draggedOJ.GetComponent<RectTransform>();
        dragRect.sizeDelta = rectTransform.sizeDelta;
        dragRect.position = Input.mousePosition;

        slot.itemIcon.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedOJ != null)
        {
           draggedOJ.transform.position = Input.mousePosition;
        } 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedOJ != null)
        {
            Destroy(draggedOJ);
        }
        slot.itemIcon.color = Color.white;

        if (!eventData.pointerEnter) 
        {
            rectTransform.position = originalPosition; 
        } 
    }

    // ========================================================================
    // RIGHT-CLICK -> SHOW ITEM DETAIL
    // DOUBLE-CLICK -> USE CONSUMABLE
    // ========================================================================
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot.isEquipmentSlot) return;
        if (!slot.HasItem()) return;

        if (eventData.button != PointerEventData.InputButton.Right) return;

        inventoryUI.ShowItemDetailAt(slot.slotIndex, rectTransform.position);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlotHandler = eventData.pointerDrag?.GetComponent<SlotDragHandler>();
        if (draggedSlotHandler == null || draggedSlotHandler == this) return;

        if (draggedSlotHandler.slot.isEquipmentSlot && this.slot.isEquipmentSlot) return;

        if (this.slot.isEquipmentSlot)
        {
            ItemStack draggedStack = draggedSlotHandler.slot.isEquipmentSlot 
                ? Inventory.Instance.equipment[draggedSlotHandler.slot.slotIndex]
                : Inventory.Instance.itemSlots[draggedSlotHandler.slot.slotIndex];

            if (draggedStack == null || draggedStack.item.itemType != ItemType.Equipment ||
                (int)draggedStack.item.equipmentType != this.slot.slotIndex + 1)
                return;

            Inventory.Instance.SwapEquipItem(draggedSlotHandler.slot.slotIndex, this.slot.slotIndex, draggedSlotHandler.slot.isEquipmentSlot);
        }
        else if (draggedSlotHandler.slot.isEquipmentSlot)
        {
            Inventory.Instance.SwapEquipItem(draggedSlotHandler.slot.slotIndex, this.slot.slotIndex, true);
        }
        else
        {
            Inventory.Instance.SwapItem(draggedSlotHandler.slot.slotIndex, this.slot.slotIndex);
        }
    }
}
