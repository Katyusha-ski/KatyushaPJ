using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ============================================================================
// SLOT DRAG HANDLER
// ============================================================================
// Handle drag-and-drop + double-click cho slot inventory.
//
// INPUT -> OUTPUT:
//   Drag inventory -> equipment: equip
//   Drag equipment -> inventory: unequip
//   Drag inventory -> inventory: swap
//   Double-click consumable: use item
//
// VIEC SU DUNG ITEM:
//   OnPointerClick() phat hien double-click
//   -> goi Inventory.Instance.UseItem(slotIndex)
//   -> (xem Inventory.UseItem() de biet flow tiep)
// ============================================================================
public class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Slot slot;
    private Image draggedImage;
    private GameObject draggedOJ;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        slot = GetComponent<Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
    // DOUBLE-CLICK -> USE CONSUMABLE
    // ========================================================================
    // Chi click vao inventory slot (khong phai equipment slot).
    // Neu item la Consumable -> goi Inventory.UseItem()
    // ========================================================================
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (slot.isEquipmentSlot) return; // Equipment khong the "dung"
        if (!slot.HasItem()) return;

        // Phat hien double-click trong khoang thoi gian threshold
        if (Time.unscaledTime - lastClickTime < doubleClickThreshold)
        {
            ItemStack stack = Inventory.Instance.itemSlots[slot.slotIndex];
            if (stack != null && stack.item.itemType == ItemType.Consumable)
            {
                // chuyen cho Inventory xu ly (tim Player, apply effect, remove stack)
                Inventory.Instance.UseItem(slot.slotIndex);
            }
        }
        lastClickTime = Time.unscaledTime;
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
