using UnityEngine;
using UnityEngine.UI;
using TMPro;
/* ============================================================================
 * GHI CHÚ: Tab UI (Inventory / Skill Matrix / Quest Item) + Skill Matrix Refactor
 * Cập nhật lần cuối: [điền ngày bạn dán note này]
 * ============================================================================
 *
 * ĐÃ HOÀN THÀNH (đã Play test xác nhận bằng mắt):
 * -----------------------------------------------------------------------
 * 1. Tab UI khung 3 tab dọc (Inventory/Skill/Quest) — TabController.cs
 *    - Bấm tab nào hiện đúng panel đó, đảo màu nút (active = nền đen/icon trắng)
 *    - Gắn trên GameObject "Inventory UI", mở mặc định tab Inventory (index 0)
 *    - Nút mở panel cũ (InventoryUI.ShowInventory/HideInventory) không đổi gì,
 *      TabController tự SelectTab(0) mỗi lần panel bật lên qua OnEnable()
 *
 * 2. Skill Matrix — tách LAYOUT (cố định) khỏi UNLOCK STATE (runtime):
 *    - SkillMatrixLayout.asset (ScriptableObject) — 20 ItemData cố định theo
 *      row (0=Range,1=Dash,2=Defend,3=Melee) x col (0=Lv1...4=Lv5)
 *    - Inventory.skillUnlocked (bool[20]) — trạng thái mở khóa runtime, thay
 *      cho ItemStack[,] cũ (đã xoá field skillMatrix cũ)
 *    - Học skill (PlayerSkillManager.UseItem) giờ gọi Inventory.UnlockSkill(row,col)
 *      thay vì tạo ItemStack mới — nút "Học skill" đã nối vào InventoryUI
 *      (trước đây CHƯA từng hoạt động vì nút Use chỉ hiện cho Consumable)
 *    - Save/Load: chỉ lưu List<bool> skillUnlocked (nhẹ hơn, không cần
 *      Resources.Load lại theo tên item như bản cũ)
 *
 * 3. SkillSystemUI.cs — sửa lỗi Unity không serialize được mảng 2 chiều:
 *    - cells: SkillCell[,] → SkillCell[20] (mảng phẳng, index = row*5+col)
 *    - Refresh() đọc Inventory.Instance.IsSkillUnlocked(r,c)
 *    - 20 ô UI đã wiring đúng vào cells (chạy qua menu 1 lần:
 *      Tools > Setup > Repair Skill Cells Wiring — do bug thứ tự
 *      SetActive(false) cắt ngang Awake() lúc dựng scene, đã fix trong
 *      InventoryTabSceneSetup.cs, không lặp lại nếu dựng scene mới từ đầu)
 *
 * 4. Quest Item UI — code đã viết xong (QuestSlotUI.cs, QuestListUI.cs,
 *    QuestDetailUI.cs), đang tự gán UI trong Editor (list slot to hơn có
 *    icon+tên bên trái dạng ScrollView dọc, panel description bên phải)
 *
 *
 * VIỆC PHỤ CÒN LẠI (không gấp, làm khi rảnh):
 * -----------------------------------------------------------------------
 * [ ] Gán skillIcon cho 20 ô trong SkillSystemUI (hiện để trống — ô mở khóa
 *     chỉ đổi khung nền, chưa có icon hiện ra)
 * [ ] 18/20 ItemData skill chưa có itemIcon (chỉ Range Lv1 + Dash Lv1 có sẵn
 *     lúc đầu) — cần thiết kế/gán icon riêng từng skill
 * [ ] Icon tab Skill (đang tạm dùng "charged1") và tab Quest (đang tạm dùng
 *     "I_Scroll") — thay bằng icon thật khi có
 * [ ] Bug SlotDragHandler.cs: field originalPosition chưa từng được gán giá
 *     trị → thả item ra ngoài UI bị teleport slot về (0,0,0). Chưa sửa,
 *     phát hiện lúc điều tra bug leak ghost kéo-thả, khác phạm vi lúc đó.
 * [ ] Đổi tên GameObject "Inventory UI" (giờ chứa cả Skill+Quest, tên cũ dễ
 *     gây hiểu lầm) — nếu đổi, nhớ sửa 2 chỗ hardcode "Inventory UI" trong
 *     InventoryTabSceneSetup.cs (Build() và RepairSkillCellsWiring()), và tự
 *     gán lại nút "Inventory Button" trong Inspector nếu đổi cả tên class
 *     InventoryUI.cs (Unity lưu tên class dạng string trong scene, đổi tên
 *     class mà không gán lại tay sẽ làm nút mất liên kết, không báo lỗi)
 *
 *
 * LƯU Ý KỸ THUẬT (để không lặp lại bug tương tự sau này):
 * -----------------------------------------------------------------------
 * - Unity Inspector KHÔNG serialize được mảng 2 chiều (T[,]) hay list lồng
 *   list (List<List<T>>) — đã gặp 2 lần (SkillSystemUI.cells, Inventory.
 *   skillMatrix cũ). Luôn dùng mảng phẳng 1 chiều + quy đổi index = row*cols+col.
 * - JsonUtility cũng KHÔNG serialize List<List<T>> (khác nguyên nhân nhưng
 *   cùng triệu chứng "field biến mất khỏi Inspector/JSON không báo lỗi gì cả").
 * - Trong Editor (không Play), GameObject mới tạo bằng code KHÔNG đảm bảo
 *   Awake() chạy ngay lập tức trong cùng lệnh — nếu SetActive(false) ngay
 *   sau khi tạo, Awake() có thể bị bỏ lỡ. Muốn chắc chắn dữ liệu được ghi
 *   lúc dựng scene bằng Editor script, tự ghi trực tiếp thay vì dựa vào
 *   vòng đời MonoBehaviour.
 * ============================================================================
 */
public class QuestDetailUI : MonoBehaviour
{
    [Header("Tham chiếu UI (tự kéo trong Inspector)")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    [Tooltip("GameObject cha chứa toàn bộ khung detail — ẩn khi chưa chọn item nào")]
    public GameObject detailRoot;

    private void Awake()
    {
        Clear();
    }

    public void ShowDetail(ItemData item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (detailRoot != null)
            detailRoot.SetActive(true);

        if (icon != null)
            icon.sprite = item.itemIcon;
        if (nameText != null)
            nameText.text = item.itemName;
        if (descriptionText != null)
            descriptionText.text = item.description;
    }

    public void Clear()
    {
        if (detailRoot != null)
            detailRoot.SetActive(false);
    }
}
