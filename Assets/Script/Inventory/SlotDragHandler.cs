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

        SwapItems(draggedSlotHandler.slot, this.slot);
    }

    private void SwapItems(Slot fromSlot, Slot toSlot)
    {
        // Lưu thông tin item và số lượng của slot đích
        var toIcon = toSlot.itemIcon.sprite;
        var toQuantity = GetQuantity(toSlot);

        // Cập nhật slot đích với thông tin từ slot nguồn
        toSlot.itemIcon.sprite = fromSlot.itemIcon.sprite;
        if (toSlot.quantityText != null)
            toSlot.quantityText.text = fromSlot.quantityText?.text ?? "";

        // Cập nhật slot nguồn với thông tin đã lưu
        fromSlot.itemIcon.sprite = toIcon;
        if (fromSlot.quantityText != null)
            fromSlot.quantityText.text = toQuantity > 0 ? toQuantity.ToString() : "";

        // Cập nhật trạng thái hiển thị
        fromSlot.itemIcon.enabled = fromSlot.itemIcon.sprite != null;
        toSlot.itemIcon.enabled = toSlot.itemIcon.sprite != null;

        if (fromSlot.quantityText != null)
            fromSlot.quantityText.gameObject.SetActive(GetQuantity(fromSlot) > 1);
        if (toSlot.quantityText != null)
            toSlot.quantityText.gameObject.SetActive(GetQuantity(toSlot) > 1);
    }

    private int GetQuantity(Slot slot)
    {
        if (slot.quantityText == null || string.IsNullOrEmpty(slot.quantityText.text))
            return 0;
        return int.Parse(slot.quantityText.text);
    }
}
