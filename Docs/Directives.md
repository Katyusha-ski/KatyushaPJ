# Directives.md — Bộ luật làm việc cho AI/Dev

> File này định nghĩa các quy tắc bất di bất dịch khi làm việc với codebase KatyushaPJ.
> Mọi AI Agent và Developer phải đọc và tuân thủ tuyệt đối.

---

## 1. VAI TRÒ & THÁI ĐỘ

### Roleplay Protocol

| Vai trò | Bên | Quyền hạn |
|---|---|---|
| **System Architect / Tech Lead** | User | Nắm quyền quyết định Game Design, kiến trúc tổng thể, roadmap |
| **Lead Unity Gameplay Programmer** | AI Agent | Thực thi kỹ thuật, đề xuất giải pháp, tuân thủ kiến trúc đã duyệt |

- Giao tiếp chuyên nghiệp, đi thẳng vào giải pháp kỹ thuật.
- **Nghiêm cấm tự ý sáng tạo thêm tính năng** nếu không có lệnh từ Architect.
- Mọi thay đổi kiến trúc phải được Architect approve trước khi thực thi.

### Ngôn ngữ

| Ngữ cảnh | Ngôn ngữ |
|---|---|
| Phân tích, thảo luận, báo cáo | **Tiếng Việt** (100%) |
| Code C#, Syntax, tên class/method | **Tiếng Anh** |
| Comment trong code | Tiếng Anh (tối thiểu) |

---

## 2. QUY TẮC FSM (Enemy State Machine)

### 2.1 Animation Control

```
BẮT BUỘC: animator.SetTrigger()
NGHIÊM CẤM: animator.Play()
```

- **Lý do:** `SetTrigger()` cho phép Animation System tự động chuyển transition dựa trên Animator Controller graph. `animator.Play()` phá vỡ cơ chế blended transition và gây khó debug.
- **Ngoại lệ duy nhất:** Sleep animation khởi tạo ban đầu (trước khi FSM active), sử dụng `animator.Play("Bat_Sleep", 0, 0f)` như đã thấy trong `BatBossController.Start()`.

### 2.2 State Caching

```csharp
// Bắt buộc: Cache tất cả state instances trong Dictionary<string, IEnemyState>
// Chỉ khởi tạo MỘT LẦN duy nhất trong CacheBossStates() / CacheStates()
// Không tạo state mới trong Update() hay trong vòng lặp

stateCache = new Dictionary<string, IEnemyState>();
stateCache["Hover"] = new BatHoverState();     // unique, singleton
stateCache["Hurt"]  = new HurtState("Hover");   // generic, singleton
```

- Không dùng `new` trong hot path.
- `SwitchTo()` lookup từ cache, không tạo instance mới.

### 2.3 State Transition Rules

- **State → State:** Chỉ qua `ChangeState()` hoặc `SwitchTo(string)`.
- **Animation Event → Public Method:** Animation Event gọi trực tiếp method trên Controller (ví dụ `DropSphere()`, `SpawnAoECircle()`).
- **Animation Event → State:** KHÔNG chuyển state từ Animation Event. Dùng timer fallback trong `GenericAttackState.OnUpdate()`.

### 2.4 Architecture Boundary

```
Đặc thù (giữ nguyên class riêng):
    BatHoverState — Sine/cosine hover logic, Boss-specific

Dùng chung (generic states trong States/Common/):
    GenericAttackState   — (trigger, duration, returnState)
    HurtState            — (returnState, playHurtTrigger)
    DieState             — (duration, onDeath callback)
```

- Tuyệt đối không gộp logic đặc thù (như `BatHoverState`) vào class bộ binh dùng chung.
- Generic states có thể mở rộng tham số (constructor overloading) nhưng không phá vỡ backward compatibility.

---

## 3. QUY TẮC BẢO TOÀN GAME DESIGN

### 3.1 Boss Health & Deflect

```csharp
// KHÔNG BAO GIỜ xoá hoặc modify GetHurtState() của BatBossController
public override IEnemyState GetHurtState(IEnemyState currentState) => currentState;

// Đây là "khiên chống khựng":
//   - Health.TakeDamage() tự động gọi GetHurtState() khi Enemy bị dính đòn
//   - Override này trả về currentState → ChangeState() là no-op
//   - Boss CHỈ vào HurtState qua ForceHurtState() (Pillar explosion)
//   - Nếu xoá, Boss sẽ NullReferenceException (stateFactory = null)
//     hoặc bị flinch khi ăn đạn Ranged → phá vỡ cơ chế Deflect
```

### 3.2 Boss Death Sequence

```csharp
// GIỮ NGUYÊN override này
public override IEnemyState GetDieState() => stateCache["Die"];

// Lý do:
//   - Health.Die() gọi GetDieState() khi HP về 0
//   - stateCache["Die"] là DieState(duration: 2f, callback: HandleEnemyDeath)
//   - Xoá override → NullReferenceException (stateFactory = null)
//     hoặc DieState mặc định (destroy 1s) không gọi HandleEnemyDeath()
```

### 3.3 Damage Source Integrity

- Boss `BatHealth` override xử lý damage source, KHÔNG bypass qua `Health.TakeDamage()` của base.
- `DamageSource.sourceType` là enum duy nhất quyết định cơ chế:
  - `Melee/Stand/EnemySkill/null` → Deflect (no damage, SFX)
  - `Ranged` → 1.5x bonus damage
  - `Pillar` → Normal damage + `ForceHurtState()`
  - `System` → Normal damage, no state change

---

## 4. QUY TẮC CODE

### 4.1 DRY (Don't Repeat Yourself)

- Boss không tạo custom state cho logic đã có generic state (`DieState`, `HurtState`, `GenericAttackState`).
- Chỉ tạo custom state khi logic thực sự unique không thể tham số hoá (ví dụ `BatHoverState`).

### 4.2 No Magic Numbers

- Hardcoded values phải được đặt tên rõ ràng hoặc khai báo hằng:
  ```csharp
  // Tốt
  private const float HOVER_TIMER = 2f;
  private const float ATTACK_ANIM_DURATION = 1.2f;
  // Không tốt
  if (timer <= 0f) { /* magic 2f ở đâu đó */ }
  ```

### 4.3 Interface Injection

- State không truy cập trực tiếp `MonoBehaviour` fields.
- Dùng interface parameters `IEnemyMovement`, `IEnemyCombat`, `IEnemyStateContext`.
- Chỉ cast `ctx is BatBossController` khi thực sự cần boss-specific logic.

### 4.4 Animation Events

- Animation Event chỉ gọi **public method** trên Controller.
- Controller method không chứa logic dài dòng — chỉ spawn prefab / gọi Init.
- Prefab tự quản lý vòng đời của nó.

---

## 5. QUY TẮC WORKFLOW

1. **Đọc hiểu context** — luôn đọc file liên quan trước khi sửa.
2. **Hỏi trước khi hành động** — mọi thay đổi kiến trúc / thêm state mới / sửa generic states cần Architect approve.
3. **Commit message** — Tiếng Anh, ngắn gọn, theo pattern `scope: message` (ví dụ: `batboss: restore GetHurtState override`).
4. **Tài liệu** — Mọi thay đổi state machine / health mechanic phải cập nhật vào `Docs/Contexts/`.

---

## 6. TỔNG KẾT

| Luật | Mức độ |
|---|---|
| `SetTrigger()` thay vì `animator.Play()` | **Bắt buộc** |
| Không xoá `GetHurtState()`/`GetDieState()` override | **Bắt buộc** |
| Cache state, không new trong hot path | **Bắt buộc** |
| DRY — dùng generic states | **Bắt buộc** |
| Hỏi Architect trước khi thay đổi kiến trúc | **Bắt buộc** |
