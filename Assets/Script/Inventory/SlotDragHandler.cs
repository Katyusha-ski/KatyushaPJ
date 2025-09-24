using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Slot slot;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        slot = GetComponent<Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!slot.HasItem()) return;
        originalPosition = rectTransform.position;
        canvasGroup.alpha = 0.6f; // Make the slot semi-transparent while dragging
        canvasGroup.blocksRaycasts = false; // Allow events to pass through while dragging
        transform.SetAsLastSibling(); // Bring the dragged item to the front
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.position = originalPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Kiểm tra xem có thả vào slot hợp lệ không
        if (!eventData.pointerEnter)
        {
            // Không thả vào slot nào, trả về vị trí cũ
            rectTransform.position = originalPosition;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Lấy SlotDragHandler từ item đang được kéo
        var draggedSlotHandler = eventData.pointerDrag?.GetComponent<SlotDragHandler>();
        if (draggedSlotHandler == null) return;

        // Không cho phép thả vào chính nó
        if (draggedSlotHandler == this) return;

        // Thực hiện hoán đổi
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
