# Reverse Prompt dành cho System Architect

> Prompt này đảo ngược vai trò từ `Directives.md`: thay vì Architect ra luật cho AI Agent (Lead Programmer), đây là prompt do AI Agent gửi lên Architect để trình bày ràng buộc, xin xác nhận và yêu cầu quyết định.
> Ngôn ngữ: Tiếng Việt (phân tích/thảo luận), tiếng Anh (code/syntax).

---

## PROMPT (gửi tới Architect)

Bạn là **System Architect / Tech Lead** của dự án **KatyushaPJ** (2D Action RPG, Unity URP). Tôi là **Lead Unity Gameplay Programmer** (AI Agent) thực thi kỹ thuật.

Dưới đây là toàn bộ ràng buộc bất di bất dịch tôi cam kết tuân thủ khi làm việc trên codebase. Tôi cần bạn:

1. **Xác nhận** từng luật dưới đây còn hiệu lực hay đã thay đổi.
2. **Quyết định** các mục còn treo ở phần cuối prompt.
3. **Approve / Reject** mọi thay đổi kiến trúc tôi đề xuất trong tương lai theo đúng quy trình.

### Phần 1 — Vai trò & thái độ

- Tôi **không tự ý sáng tạo thêm tính năng**; chỉ thực thi khi có lệnh từ Architect.
- Mọi thay đổi kiến trúc phải được Architect **approve trước khi thực thi**.
- Báo cáo, phân tích, thảo luận bằng **Tiếng Việt**; code và comment bằng **Tiếng Anh**.

### Phần 2 — Quy tắc FSM (Enemy State Machine)

- Chỉ dùng `animator.SetTrigger()`; **nghiêm cấm** `animator.Play()` (ngoại lệ duy nhất: sleep animation khởi tạo ban đầu như `BatBossController.Start()`).
- Cache mọi state instance trong `Dictionary<string, IEnemyState>`, khởi tạo **một lần** trong `CacheBossStates()`. Không `new` trong hot path. `SwitchTo()` chỉ lookup từ cache.
- State → State chỉ qua `ChangeState()` / `SwitchTo(string)`.
- Animation Event chỉ gọi **public method** trên Controller, KHÔNG chuyển state; dùng timer fallback trong `OnUpdate()`.
- Logic đặc thù (vd `BatHoverState`) giữ class riêng; không gộp vào generic states. Generic states mở rộng bằng constructor overloading, không phá vỡ backward compatibility.

### Phần 3 — Bảo toàn Game Design (BẮT BUỘC giữ nguyên)

- Không xoá/sửa `GetHurtState()` override của `BatBossController` (trả về `currentState` → chống flinch, bảo vệ cơ chế Deflect).
- Không xoá `GetDieState()` override (trả về `stateCache["Die"]` → đảm bảo death sequence gọi `HandleEnemyDeath()`).
- Boss `BatHealth` xử lý damage qua override, không bypass `Health.TakeDamage()` base. `DamageSource.sourceType` là nguồn quyết định duy nhất: `Melee/Stand/EnemySkill/null` → Deflect; `Ranged` → 1.5x; `Pillar` → damage + `ForceHurtState()`; `System` → damage bình thường, không đổi state.

### Phần 4 — Quy tắc code

- **DRY**: không tạo custom state khi generic đã đủ; chỉ tạo khi logic thực sự unique.
- **No Magic Numbers**: hardcoded value phải đặt tên rõ / khai báo const.
- **Interface Injection**: state không truy cập trực tiếp `MonoBehaviour` fields; dùng `IEnemyMovement`, `IEnemyCombat`, `IEnemyStateContext`; chỉ cast `ctx is BatBossController` khi thật cần thiết.
- **Animation Events**: chỉ gọi public method, method ngắn gọn (spawn prefab / gọi Init); prefab tự quản lý vòng đời.

### Phần 5 — Quy trình làm việc

1. Đọc context trước khi sửa.
2. Hỏi Architect approve trước khi đổi kiến trúc / thêm state / sửa generic states.
3. Commit message tiếng Anh, pattern `scope: message`.
4. Mọi thay đổi state machine / health mechanic phải cập nhật vào `Docs/Contexts/`.
5. Cập nhật file `.md` cho khớp hệ thống thực tế sau mỗi thay đổi code.

---

## Các câu hỏi cần Architect quyết định (treo)

| # | Vấn đề | Cần quyết định |
|---|--------|----------------|
| 1 | `AssisterController` legacy/unused | Giữ hay xoá? |
| 2 | 18/20 skill ItemData thiếu `itemIcon` | Thuần design art — ai làm? có ưu tiên không? |
| 3 | Tra cứu itemName (chưa migrate sang itemId) | Nợ kỹ thuật — có kế hoạch migrate không? |
| 4 | DuoGolem hazard skills `// TODO: chờ prefab`, values placeholder | Cần Architect chốt số liệu + giao prefab |
| 5 | DuoGolem Animator Controller chưa tạo (trigger: Punch/Run/Die) | Ai tạo, deadline? |
| 6 | DuoGolem `partnerGolem` phải gán tay trong Inspector | Hướng dẫn thao tác, script nào tự động hoá? |
| 7 | DuoGolem chưa gán Chapter | Boss này thuộc chapter nào? |

---

## Hướng phản hồi mong đợi từ Architect

- Trả lời theo cấu trúc: **Xác nhận từng luật** (vẫn áp dụng / điều chỉnh / bãi bỏ), kèm lý do ngắn gọn.
- Trả lời bảng "Câu hỏi treo" bằng quyết định rõ ràng.
- Nếu có thay đổi nào so với prompt này, cập nhật trực tiếp vào `Assets/Docs/Directives.md` và thông báo để tôi đồng bộ.
