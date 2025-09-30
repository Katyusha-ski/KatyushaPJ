using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Slot slot;
    private Image draggedImage;
    private GameObject draggedOJ;
    


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

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlotHandler = eventData.pointerDrag?.GetComponent<SlotDragHandler>();
        if (draggedSlotHandler == null || draggedSlotHandler == this) return;

        // If both slots are equipment slots, do nothing
        if (draggedSlotHandler.slot.isEquipmentSlot && this.slot.isEquipmentSlot) return;

        // If the target slot is an equipment slot, check if the dragged item is of the correct type
        if (this.slot.isEquipmentSlot)
        {
            var draggedStack = Inventory.Instance.itemSlots[draggedSlotHandler.slot.slotIndex];
            if (draggedStack == null || draggedStack.item.itemType != ItemType.Equipment ||
                (int)draggedStack.item.equipmentType != this.slot.slotIndex + 1)
                return;
        }

        Inventory.Instance.SwapItem(draggedSlotHandler.slot.slotIndex, this.slot.slotIndex);
    }

}
