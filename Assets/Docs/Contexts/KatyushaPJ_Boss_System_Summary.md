# KatyushaPJ — Tổng Hợp Hệ Thống Boss

> File này tổng hợp toàn bộ thiết kế và kiến trúc cốt lõi của hệ thống Boss trong dự án KatyushaPJ (2D Side-scrolling Action RPG, Unity URP). Chỉ giữ lại **kết quả cuối cùng đã chốt**, không bao gồm lịch sử sửa lỗi, tranh luận, hay các bản nháp đã bị thay thế.

---

## 0.0 QUY ĐỊNH VAI TRÒ LÀM VIỆC (ROLEPLAY PROTOCOL) — BẮT BUỘC ĐỌC TRƯỚC

| Vai trò | Bên | Quyền hạn |
|---|---|---|
| **System Architect / Tech Lead** | User (Katyusha) | Nắm quyền quyết định Game Design, kiến trúc tổng thể, roadmap |
| **Lead Unity Gameplay Programmer** | AI | Thực thi kỹ thuật, đề xuất giải pháp, tuân thủ kiến trúc đã duyệt |

- Giao tiếp chuyên nghiệp, đi thẳng vào giải pháp kỹ thuật, không rườm rà.
- **Nghiêm cấm tự ý sáng tạo thêm tính năng/cơ chế** nếu không có lệnh từ Architect. Mọi ý tưởng mới rẽ nhánh khỏi thiết kế đã chốt trong file này đều phải hỏi lại Architect trước.
- Mọi thay đổi kiến trúc (thêm state, sửa generic state, đổi cơ chế lọc sát thương...) phải được Architect approve trước khi thực thi — không tự ý sửa và báo cáo sau.
- **Ngôn ngữ:** Phân tích, thảo luận, báo cáo — 100% Tiếng Việt. Code C#, syntax, tên class/method — Tiếng Anh. Comment trong code — Tiếng Anh (tối thiểu).
- Khi phát hiện lỗ hổng logic hoặc mâu thuẫn trong thiết kế, phải chủ động nêu ra và chờ Architect chốt phương án, không tự ý chọn 1 hướng rồi code luôn.
- Nếu nhận thấy dấu hiệu bị tràn context (quên chi tiết, tự bịa thông tin không có trong file này), phải dừng lại và báo cáo ngay, không được tiếp tục "đoán" rồi trình bày như sự thật.

---

## 0. TRIẾT LÝ THIẾT KẾ CỐT LÕI (Áp dụng cho mọi Boss)

### 0.1 Triết lý Chiến đấu (Combat Philosophy)
- **Attrition (Chiến tranh tiêu hao):** Không bao giờ có đòn One-shot (chết sốc). Áp lực sinh tồn đến từ nhịp độ dồn dập, ép góc và cấu rỉa liên tục.
- **Thang Sát thương chuẩn hóa (1–5):**
  - Mức 0: CC thuần túy (không sát thương).
  - Mức 1–2: Sát thương cảnh cáo.
  - Mức 3: Khống chế (CC) kèm sát thương HOẶC đòn đánh chay cận chiến của Boss.
  - Mức 4 (Max): Kỹ năng sát thương chủ lực.
  - Mức 5: **CẤM SỬ DỤNG** (không có one-shot).

### 0.2 Kiến trúc Hệ thống
- **Decoupling (Độc lập phân luồng):** AI của Boss (FSM) và các hệ thống phụ trợ (môi trường, kỹ năng) chạy song song, độc lập hoàn toàn.
- **Telegraphing (Minh bạch cảnh báo) — bắt buộc mọi cơ chế gây sát thương/CC:**
  - UI Toàn cục: Logo cảnh báo chớp nháy báo loại kỹ năng sắp ra.
  - Chỉ thị Không gian: Vùng đỏ bán trong suốt dưới đất, kèm thanh Fill/Scale báo chính xác vị trí và thời gian.

### 0.3 Quy chuẩn Trạng thái (FSM Standards)
- **DRY (Don't Repeat Yourself):** Ưu tiên tái sử dụng Generic State (`GenericAttackState`, `HurtState`, `DieState`) thay vì viết state riêng cho từng Boss. Chỉ viết Custom State khi logic thực sự độc quyền, không thể tham số hóa (ví dụ: kiểu di chuyển đặc thù).
- **Animation Control:** Bắt buộc dùng `animator.SetTrigger()`. Nghiêm cấm `animator.Play()` (trừ trường hợp khởi tạo trạng thái ngủ ban đầu trước khi FSM active).
- **Snap Initialization:** Khi chuyển Phase, chỉ số (tốc độ, sát thương) cập nhật ngay lập tức, không có thời gian Warm-up.
- **Trạng thái Bao Cát (Target Dummy):** Khi Boss rơi vào cửa sổ xả sát thương (DPS Window) do cơ chế đặc biệt, nó nhận 100% sát thương, KHÔNG khiên, KHÔNG I-frame.
- **Trạng thái Tê liệt/Chết tạm:** Vô hiệu hóa hoàn toàn Collider2D/Hurtbox. Player đi xuyên qua được, mọi đòn đánh/raycast tự động bỏ qua thực thể này.
- **Symmetric Logic:** Các cơ chế liên kết sinh mệnh hoặc hoán đổi vai trò phải đối xứng hai chiều, không hard-code cố định một vai trò.

### 0.4 Cơ chế Cuồng nộ (Enrage)
- Scale theo 4 mốc HP: **100% → 75% → 50% → 25%**, tính trên thanh HP riêng của từng thực thể (không cộng gộp nếu là Duo Boss).
- Máu càng thấp, tốc độ ra đòn, vận tốc di chuyển và mật độ bẫy càng tăng.

---

## 1. BAT BOSS (Chapter 4)

### 1.1 Tổng quan
- Thực thể bay lơ lửng trên không, di chuyển bằng cách bay nhấp nhô (Sine/Cosine), không đáp đất.
- Không dùng `SkillManager` chung của Enemy — mọi đòn đánh được điều khiển hoàn toàn qua **Animation Events** gọi thẳng vào Controller.

### 1.2 Hệ thống Máu & Lọc Sát thương (`BatHealth` override `Health`)
| Nguồn sát thương | Hiệu ứng lên Boss |
|---|---|
| Melee / Stand (Hachiware) | Deflect — 0 sát thương, phát SFX dội đòn |
| Ranged (đạn/skill tầm xa) | Nhận **1.5x sát thương bonus** — khuyến khích Player bắn xa |
| Pillar (nổ cột) | Sát thương thường + kích hoạt `ForceHurtState()` |
| System (nội bộ) | Sát thương thường, không đổi state |

- **"Khiên chống khựng":** `GetHurtState()` override trả về `currentState` — Boss KHÔNG BAO GIỜ bị flinch (khựng) khi ăn đòn thường hay Ranged. Chỉ vào `HurtState` khi bị nổ Cột (Pillar) tác động.
- **`GetDieState()`:** Trả về `stateCache["Die"]` — gọi callback `HandleEnemyDeath()` (VFX, ẩn thanh máu, rơi đồ, bắn event `OnBossDefeated`).

### 1.3 FSM State Map
```
Sleep (init, animator.Play) → OnWakeUpComplete() → Hover
Hover (BatHoverState — UNIQUE, bay Sin/Cos)
  ├─ Timer 2s → PickNextAttack() → random 50/50
  │     ├─ DropSphere (Trigger "Atk1")
  │     └─ SpawnDoT/AoE (Trigger "Atk2")
  └─ Pillar spawn timer chạy song song, độc lập, trong Update()
GenericAttackState (dùng chung cho mọi đòn tấn công)
  → OnEnter: SetTrigger animation
  → Animation Event giữa clip gọi method thực thi (DropSphere() / SpawnAoECircle())
  → Timer hết → quay về Hover
HurtState (generic, playHurtTrigger=false) — chỉ kích hoạt qua ForceHurtState() khi Pillar nổ
DieState (generic, duration=2f, callback=HandleEnemyDeath)
```

### 1.4 Cơ chế Tấn công
- **Atk1 — DropSphere:** Spawn `batSpherePrefab` thẳng tại `(player.position.x, transform.position.y)` — trên đầu Player. Quả cầu rơi thẳng đứng xuống (không homing), chạm đất nổ burst AoE + sinh `HazardZone` (vùng gây DoT).
- **Atk2 — SpawnAoECircle (hole):** Spawn `holePrefab` (vòng tròn nổ chậm) tại vị trí Player (tọa độ Y = mặt đất, trừ đi `hoverHeight`).

### 1.5 Hệ thống Cột (Pillar Spawn Manager) — chạy ngầm, độc lập với đòn tấn công
| Tham số | Giá trị |
|---|---|
| Số Cột tối đa cùng lúc | 3 |
| Cooldown spawn | 7 giây |
| Khoảng cách tối đa tới Player | 12f |
| Khoảng cách tối thiểu giữa các Cột | 5f |

- Timer chỉ chạy khi số Cột đang tồn tại < 3. Phá hủy 1 Cột sẽ mở slot để Timer tiếp tục đếm (không tự reset ngay lập tức).
- Khi spawn: lọc các điểm trong `pillarSpawnPoints[]` theo 2 điều kiện trên → chọn ngẫu nhiên 1 điểm hợp lệ.
- Cột có HP riêng (nhỏ), khi bị phá hủy sẽ gọi `bossHealth.TakeDamage(PillarBurstDamage, gameObject)` với `DamageSourceType.Pillar` → gây sát thương lớn (25% MaxHP Boss) + kích hoạt `ForceHurtState()`.

### 1.6 Vật lý / Layer
- Golem/Boss KHÔNG va chạm vật lý với Player (Layer Collision Matrix tắt tương tác Player–Enemy) → cả hai đi xuyên qua nhau, không có contact damage.
- Các Prefab Hitbox (chiêu thức) dùng Layer riêng (ví dụ `EnemyAttack`) để không bị loại trừ va chạm với Player.

---

## 2. VOID BOSS (Chapter 6)

### 2.1 Tổng quan & Ngoại hình
- Thực thể bóng tối sâu thẳm, hình dạng biến hóa nhưng chủ đạo có 6 chi + 1 đầu.
- Di chuyển trên mặt đất bằng 6 chi (đi bộ/bò), KHÔNG bay lơ lửng.
- Chuyên phép thuật tầm xa kèm CC/Debuff, dồn Player vào thế bí trước khi áp sát dùng đòn cận chiến sát thương cao.

### 2.2 Bộ Đòn Đánh Thường (NA — Sát thương chính, dùng khi cận chiến)
- **NA1 (Stomp — Dậm chân):** Gây sát thương diện rộng (AoE) + `StunEffect`.
- **NA2 (Spike Pierce — Cọc nhọn đâm):** Biến chân thành cọc đâm thẳng, sát thương lớn.
- Cả 2 đòn đều đẻ ra **Prefab chứa Trigger Collider** để check hitbox qua vật lý (`OnTriggerEnter2D`/`OverlapCircle`), **tuyệt đối không** dùng `Vector2.Distance` check cứng trong Controller.

### 2.3 Bộ Kỹ Năng (Skill — Toàn bộ tầm xa, thiên về CC/Ép góc)
- **Skill 1 (Void Sphere):** Bắn 1 quả cầu Homing (dí theo Player), tự nổ sau timer hoặc khi va chạm.
  - Cooldown ngắn, dùng thường xuyên vì khó trúng.
  - Trúng đòn: sát thương trung bình + Micro-Stun nhanh + Slow + **Debuff giảm giáp** (`StatModifierEffect`).
  - Cách né chính: Dash đúng lúc hoặc dùng Defend Skill.
- **Skill 2 (Ambush Summon):** Triệu hồi 1 "con đệ" xuất hiện đột ngột dưới chân Player.
  - **Bản chất:** Là một **Trap/Prefab tự hủy (Untargetable)**, KHÔNG PHẢI Enemy thực thụ — trồi lên, tấn công 1 nhát rồi biến mất, không có thanh máu.
  - Hiệu ứng: sát thương cao hơn Skill 1 + `SilentEffect` (câm lặng, không dùng được Skill). Player vẫn có thể chạy thoát khỏi phạm vi (trừ khi bị ép vào rìa map).
- **Skill 3:** Đã bị cắt bỏ khỏi thiết kế.
- **Skill 4 (Ultimate — Blood Moon):** Đòn mạnh nhất, kích hoạt theo **Timer/Cooldown cực dài** (không phụ thuộc ngưỡng máu).
  - Cơ chế: Vùng cảnh báo đỏ (Telegraph) xuất hiện quanh Player, random cả trục X (`Player.X ± a`) và trục Y (`Player.Y + b`).
  - **Anti-Overlap:** Các vị trí random không được cách nhau quá gần — áp dụng thuật toán kiểm tra khoảng cách tối thiểu (`minSpacing`) giữa các điểm trong cùng 1 wave, random lại nếu vi phạm (có giới hạn `maxAttempts`).
  - Sau delay, vùng nổ/chiếu tia sáng đỏ gây sát thương DPS cực lớn — đủ giết Player dù mang giáp tốt nhất Chapter nếu ăn đủ đòn. Lặp lại nhiều wave, ép Player di chuyển + nhảy liên tục.

### 2.4 Tư duy AI (Decision Logic) — Dựa trên Khoảng cách + Ưu tiên Cooldown
```
Nếu Distance <= MeleeRange:
    Roll ngẫu nhiên → NA1 hoặc NA2
Nếu Distance > MeleeRange:
    Nếu Skill 2 sẵn sàng → Ưu tiên dùng Skill 2 (mạnh hơn)
    Nếu Skill 2 đang CD mà Skill 1 sẵn sàng → Dùng Skill 1
    Nếu cả 2 đang CD → Pursuit (đi bộ tiếp cận Player)
```

### 2.5 FSM State Map
- **Dùng lại Generic State:** NA1, NA2, Skill 1, Skill 2 đều dùng `GenericAttackState`. `HurtState`, `DieState` dùng bản generic.
- **Custom State bắt buộc (không thể tham số hóa):**
  - `VoidIdleState` — bộ não AI: kiểm tra khoảng cách + cooldown mỗi frame, quyết định hành vi theo mục 2.4.
  - `VoidPursuitState` — truy đuổi ngắt quãng: đang đi bộ áp sát nhưng vẫn liên tục check cooldown Skill 1/2, sẵn sàng dừng lại xả chiêu ngay khi hồi xong (tham khảo logic `NecromancerE` có sẵn trong project).
  - `BloodMoonState` — Ultimate: quản lý multi-wave telegraph, sub-timer riêng, không phù hợp gói vào `GenericAttackState`.

### 2.6 Hệ thống Máu & Va chạm
- **KHÔNG có cơ chế lọc sát thương phức tạp** như BatBoss — Boss nhận sát thương bình thường từ mọi nguồn (Melee, Ranged, Skill). Độ khó đến từ chỉ số HP/Armor cao và debuff dồn dập lên Player, không phải từ cơ chế miễn nhiễm.
- **Super Armor:** Override `GetHurtState()` trả về `currentState` — Boss KHÔNG BAO GIỜ bị khựng dưới bất kỳ hình thức nào, đảm bảo chuỗi combo CC không bị ngắt.
- **Facing Lock:** Trong mọi `GenericAttackState`, Boss bị khóa không lật mặt (flip) theo Player để bảo toàn animation.
- **Ghost Collision:** Không xử lý va chạm vật lý giữa Boss và Player bằng code — dùng Layer Collision Matrix để 2 bên tự do đi xuyên qua nhau (không cần Dash để xuyên).
- **Cleanup on Death:** Khi Boss chết (`HandleEnemyDeath()`), phải hủy toàn bộ `VoidSphere`, `AmbushTrap`, `BloodMoon` đang tồn tại trên map để tránh giết Player sau khi đã thắng.

---

## 3. GDD CHI TIẾT: BAT BOSS

> Bản GDD đầy đủ, mở rộng từ mục 1. Kèm phần Nhận xét & Phản biện để lưu lại các câu hỏi thiết kế chưa chốt — không phải lỗi, mà là điểm cần Architect xác nhận thêm nếu có ai định code tiếp.

### 3.1 Tổng quan
- Solo Boss, thực thể bay (dơi khổng lồ), không đáp đất trừ khi có cơ chế đặc biệt buộc phải hạ cánh.
- **Triết lý:** Ép Player chuyển từ chiến thuật cận chiến (Melee/Stand) sang buộc phải dùng kỹ năng tầm xa (Ranged) để gây sát thương hiệu quả; song song đó quản lý hệ thống "bẫy ngầm" (Cột) chạy độc lập với nhịp tấn công của Boss.
- Kiến trúc độc lập: FSM của Boss và cơ chế sinh Cột chạy trên 2 luồng logic tách biệt, không phụ thuộc lẫn nhau về thời gian.

### 3.2 Bộ Chiêu Thức (Animation-Driven, không dùng SkillManager chung)
Toàn bộ đòn tấn công thực thi qua Animation Event gọi thẳng method Controller — FSM chỉ bắn Trigger, không tự tính thời điểm gây sát thương bằng code/timer.

- **Đòn 1 — Thả Cầu Độc (Trigger "Atk1"):** Spawn 1 quả cầu tại điểm ngẫu nhiên trong danh sách spawn cố định (trên không). Quả cầu tự bay/rơi hướng về phía Player (không có homing phức tạp, chỉ định hướng đơn giản lúc spawn). Chạm đất: tự nổ (Burst AoE tức thời, bán kính nhỏ) + để lại vùng khói độc (Hazard Zone, DoT vài giây rồi biến mất).
- **Đòn 2 — Bẫy Nổ Chậm / Hố (Trigger "Atk2"):** Spawn 1 "Hố" tại vị trí hiện tại của Player (tọa độ Y = mặt đất, không phải tọa độ đang bay của Boss). Cảnh báo trước bằng animation vòng tròn, sau delay sẽ kích hoạt gây sát thương diện rộng nếu Player còn đứng trong vùng.

### 3.3 Nhận xét & Phản biện — Bat Boss
Các điểm còn mơ hồ, cần Architect xác nhận thêm trước khi giao cho ai code tiếp:

1. **Đòn 1 có homing hay không?** Câu "tự bay hướng về Player" dễ hiểu nhầm là có tracking runtime. Cần khẳng định dứt khoát: bắn thẳng 1 hướng cố định lúc spawn (không tracking), hay có homing (và mức độ mạnh bao nhiêu)?
2. **Burst AoE của quả cầu chưa quy vào thang Mức 1-5.** Cần gán rõ: Burst = Mức mấy, DoT tiếp theo = Mức mấy.
3. **Delay trước khi Hố (Đòn 2) nổ chưa có con số cụ thể.** Cần định lượng (ví dụ 1.2s) để code timer chính xác.
4. **Không có cơ chế Phase/Enrage nào cho BatBoss.** Khác với Duo Golem và VoidBoss đều có enrage theo % HP — có thể là chủ đích (độ khó chỉ đến từ Cột), nhưng cần 1 câu xác nhận rõ để tránh ai đó tự thêm Enrage không được duyệt.
5. **Tỷ lệ chọn Đòn 1 vs Đòn 2 chưa nêu rõ logic AI.** Random 50/50, luân phiên cố định, hay theo điều kiện khoảng cách như VoidBoss? Cần chốt.
6. **Ranged 1.5x tính trước hay sau Armor?** Nhân vào sát thương gốc trước khi trừ Armor, hay nhân vào kết quả cuối cùng sau khi đã trừ Armor/DmgR — 2 cách cho kết quả khác nhau đáng kể ở late game khi Player có Armor cao.

---

## 4. GDD CHI TIẾT: VOID BOSS

> Bản GDD đầy đủ, mở rộng từ mục 2. **Đã loại bỏ 2 chi tiết bị hallucinate** từng xuất hiện ở một bản báo cáo trước đó (đã được Agent grep code xác nhận không tồn tại):
> - ~~"Trục Y dùng Raycast cắm xuống đất, không lơ lửng"~~ → **SAI**. Code thật (`SpawnBloodMoonWave()`) dùng `Random.Range(-b, b)` thuần túy trên trục Y — đúng như thiết kế gốc, vùng nổ **có lơ lửng trên không**.
> - ~~"`meleeCooldownTimer` — cơ chế Anti-Spam cho NA1/NA2"~~ → **KHÔNG tồn tại**, không có trong code, không có trong bất kỳ bản GDD/context nào trước đó. Đã xác nhận qua grep toàn bộ codebase.

### 4.1 Tổng quan
- Solo Boss, thực thể bóng tối biến hình, di chuyển bằng 6 chi trên mặt đất.
- **Triết lý:** Chuyên gia CC/Debuff tầm xa dọn đường cho đòn cận chiến sát thương cao; đỉnh điểm là 1 đòn Ultimate mang tính "kiểm tra sinh tồn" định kỳ, không phụ thuộc lượng máu.
- Kiến trúc độc lập: Bộ não AI (`VoidIdleState`) tự quyết định hành vi dựa trên khoảng cách và cooldown mỗi frame — không phụ thuộc animation hay báo hiệu bên ngoài để chuyển trạng thái.

### 4.2 Bộ Đòn Đánh Thường (NA)
| Đòn | Cơ chế | Hiệu ứng |
|---|---|---|
| NA1 — Stomp (Dậm chân) | Dậm chân tại chỗ, sát thương AoE quanh thân | + Stun |
| NA2 — Spike Pierce (Cọc nhọn đâm) | Biến chân thành cọc, đâm thẳng về trước | Sát thương lớn, không CC |

- Cả 2 đòn spawn Prefab chứa Trigger Collider, check hitbox bằng vật lý thực tế (`OverlapCircle`/`OnTriggerEnter2D`) — tuyệt đối không check `Vector2.Distance` cứng trong Controller.
- Sau `PickMeleeAttack()`, Boss vào `GenericAttackState` — timer animation chạy hết rồi về `VoidIdle` — **không có cooldown riêng biệt nào khác** ngoài timer đó (xem lưu ý ở đầu mục 4).

### 4.3 Bộ Kỹ Năng (Tầm xa, chuyên CC/Ép góc)
- **Skill 1 (Void Sphere):** Bắn 1 quả cầu Homing (dí theo Player), tự nổ sau timer hoặc khi va chạm. Cooldown ngắn (khó trúng nên dùng thường xuyên). Trúng: sát thương trung bình + Micro-Stun + Slow + Debuff giảm giáp (`StatModifierEffect`). Né bằng Dash đúng lúc hoặc Defend Skill.
- **Skill 2 (Ambush Summon):** Triệu hồi 1 "con đệ" xuất hiện đột ngột dưới chân Player — là **Trap/Prefab tự hủy (Untargetable)**, không phải Enemy có máu. Trồi lên, tấn công 1 nhát, biến mất. Sát thương cao hơn Skill 1 + `SilentEffect`. Player vẫn thoát được nếu chưa bị ép vào rìa map. Khi cả Skill 1 và 2 sẵn sàng cùng lúc, **luôn ưu tiên Skill 2**.
- **Skill 3:** Đã cắt khỏi thiết kế.
- **Skill 4 — Ultimate (Blood Moon):** Kích hoạt theo Timer/Cooldown cực dài, không phụ thuộc % HP.
  - Nhiều vùng Telegraph đỏ xuất hiện quanh Player: trục X random `Player.X ± a`, **trục Y random thuần túy `Player.Y ± b`** (random đối xứng cả 2 phía, **không dùng Raycast, không dán đất** — tạo cả vùng nổ lơ lửng trên không, ép Player kết hợp Nhảy + Chạy).
  - **Object Pool (Architect Decision):** Prefab vụ nổ Telegraph phải được lấy/trả qua hệ thống `ObjectPool` sẵn có của project (`Pattern/ObjectPool.cs` — generic singleton pool), KHÔNG dùng `Instantiate`/`Destroy` trực tiếp. Do 1 wave sinh ra nhiều prefab liên tục tại các tọa độ X/Y khác nhau trong thời gian ngắn, việc cấp phát bộ nhớ liên tục sẽ gây GC spike — bắt buộc tái sử dụng object qua Pool.
  - Anti-Overlap: các điểm trong cùng 1 wave không được cách nhau gần hơn `minSpacing`, random lại nếu vi phạm (giới hạn `maxAttempts`).
  - Sau delay, các vùng đồng loạt nổ/chiếu tia sáng đỏ — sát thương DPS cực lớn, đủ giết Player dù giáp tốt nhất Chapter nếu ăn đủ đòn. Lặp nhiều wave trong 1 lần kích hoạt.

### 4.4 Tư duy AI (Decision Logic)
```
Nếu Distance <= MeleeRange:
    Roll ngẫu nhiên → NA1 hoặc NA2
Nếu Distance > MeleeRange:
    Nếu Skill 2 sẵn sàng → Ưu tiên dùng Skill 2 (mạnh hơn)
    Nếu Skill 2 đang CD mà Skill 1 sẵn sàng → Dùng Skill 1
    Nếu cả 2 đang CD → Pursuit (đi bộ tiếp cận Player)
```
- `VoidPursuitState`: đang đi bộ áp sát vẫn liên tục check cooldown Skill 1/2 mỗi frame — hồi xong giữa chừng thì dừng lại xả chiêu ngay, không đi tới tận nơi mới đánh.

### 4.5 FSM State Map
- **Generic State (DRY):** NA1, NA2, Skill 1, Skill 2 → `GenericAttackState`. `HurtState`, `DieState` dùng bản generic.
- **Custom State bắt buộc:**
  - `VoidIdleState` — bộ não AI (mục 4.4).
  - `VoidPursuitState` — truy đuổi ngắt quãng.
  - `BloodMoonState` — Ultimate: quản lý multi-wave telegraph, sub-timer riêng, không gói vừa `GenericAttackState`.

### 4.6 Hệ thống Máu & Vật lý
- Không có cơ chế lọc sát thương (Deflect/Bonus) như BatBoss — nhận sát thương bình thường từ mọi nguồn. Độ khó đến từ HP/Armor nền cao + Debuff dồn dập, không phải miễn nhiễm.
- Super Armor: `GetHurtState()` trả `currentState` — không bao giờ khựng.
- Facing Lock trong `GenericAttackState`.
- Ghost Collision: Layer Collision Matrix tắt tương tác vật lý Boss–Player.
- Cleanup on Death: hủy toàn bộ `VoidSphere`, `AmbushTrap`, `BloodMoon` đang tồn tại khi Boss chết.

### 4.7 Nhận xét & Phản biện — Void Boss
1. **Không có Enrage theo % HP.** Toàn bộ độ khó đến từ Ultimate theo Timer cố định — Boss đánh y hệt lúc 100% HP và lúc 5% HP. Nếu là chủ đích ("trận đấu bền vững, không leo thang") thì ổn, cần xác nhận rõ để tránh về sau ai đó tự thêm Enrage phá cân bằng.
2. **Chưa quy định điều gì xảy ra nếu Ultimate trùng lúc NA/Skill khác đang chờ.** Ví dụ Boss đang Pursuit, Ultimate cooldown hết đúng lúc Player lọt vào MeleeRange — ai được ưu tiên? Cần 1 quy tắc dứt khoát (ví dụ: Ultimate có quyền ngắt bất kỳ state chờ nào, nhưng không ngắt ngang một `GenericAttackState` đang chạy dở).
3. **Cleanup on Death chưa nói rõ Ultimate đang xả dở có bị hủy giữa chừng không.** Nếu Player giết Boss ngay giữa lúc Blood Moon còn vài wave chưa nổ, các wave đó có huỷ ngay hay vẫn nổ nốt? Rủi ro "thắng nhưng vẫn chết" nếu code sai.
4. **Chưa định lượng `MeleeRange`, `a`, `b`, `minSpacing`, `maxAttempts`.** Toàn bộ đang là biến số chưa có giá trị cụ thể — cần chốt số liệu thật trước khi code.
5. **Hình dạng/animation của "con đệ" (Skill 2) chưa được nhắc tới.** Vấn đề mỹ thuật nhưng ảnh hưởng tới việc set Collider/kích thước vùng kích hoạt.
6. **Giới hạn Arena (map bounds) cho vị trí spawn Blood Moon — từng bị gạt bỏ ("không ảnh hưởng gameplay").** Cần xác nhận lại khi map thật của Chapter 6 dựng xong, vì lúc quyết định Arena chưa tồn tại, đây là giả định tạm thời chưa kiểm chứng bằng map thực tế.

---

## 5. DUO GOLEM (Trận Chiến Tiêu Hao) — *Đã code*

### 5.1 Tổng quan
- Duo Boss gồm **Golem A** và **Golem B** (phân biệt màu sắc).
- **Triết lý:** Trừng phạt lối chơi "gọt máu lệch" (min-max 1 con trước). Golem chỉ có **1 đòn đánh thường duy nhất** (lù đù, đi bộ, đấm chay — Sát thương Mức 3, không gầm gừ báo hiệu). Toàn bộ độ khó thực sự đến từ **Môi trường (ArenaHazardController)**, chạy hoàn toàn độc lập với AI của Boss.

### 5.2 Cơ chế Liên kết Sinh mệnh (Resurrection Loop)
> **Tính đối xứng:** Toàn bộ cơ chế dưới đây áp dụng hai chiều — nếu Golem B cạn HP trước, vai trò A/B hoán đổi tương ứng trong mọi state.

- Khi Golem A cạn HP (0%) → vào state **Tê liệt (Paralyzed)**: vô hiệu hóa hoàn toàn Collider2D/Hurtbox, Player đi xuyên qua được, mọi đòn đánh/raycast tự động bỏ qua nó.
- Ngay lúc đó, Golem B ngắt mọi hành vi, vào state **Vận công Hồi sinh (Revive Channeling)**, đứng bất động **8 giây**.
- **Trạng thái Bao Cát:** Trong 8 giây này, Golem B KHÔNG I-frame, KHÔNG khiên — nhận 100% sát thương (DPS Window thuần túy).
- **Kịch bản Thua (lặp vô tận):** Nếu Player không hạ được Golem B trong 8 giây, Golem A hồi sinh với HP **chính xác bằng** HP hiện tại của Golem B. Không có Hard Enrage Timer tổng — vòng lặp có thể diễn ra vô hạn nếu Player cứ đánh lệch máu.
- **Kịch bản Thắng:** Nếu Player hạ được Golem B về 0 HP ngay trong lúc nó đang Vận công, Golem B **chết vĩnh viễn** (không vào Tê liệt) → cả 2 Golem cùng ở mốc 0 HP → **Chiến thắng ngay lập tức**.
- **Snap Initialization:** Ngay khi hồi sinh thành công, Golem A snap thẳng vào Phase tương ứng với HP mới, kế thừa ngay tốc độ/sát thương của Phase đó, không có Warm-up.

### 5.3 Cơ chế Cuồng nộ & Bù trừ Môi trường
- Phase tính theo mốc **100% → 75% → 50% → 25%** trên HP **độc lập** của từng Golem (không cộng gộp).
- Khi 1 Golem gục (vào Vận công Hồi sinh):
  - Tắt hoàn toàn 2 kỹ năng môi trường gắn với con gục.
  - Giáng 1 cấp Phase kỹ năng môi trường của con đang Vận công (tạo DPS Window).
  - *Edge case:* Nếu con đang Vận công đã ở Phase 1 (thấp nhất), giữ nguyên Phase 1 (không tắt hẳn) để tránh Player quá an toàn.
- Bản thể Boss (tốc độ di chuyển, tốc độ đấm) cũng tăng theo Phase, nhưng sát thương đấm chay **khóa trần ở Mức 3**.

### 5.4 Thang Sát thương & Cảnh báo
| Mức | Áp dụng |
|---|---|
| 5 (One-shot) | Không tồn tại |
| 4 (Cấu rỉa mạnh) | Chạm trần của 2 skill Sát thương Môi trường ở Phase cuối |
| 3 (CC + Sát thương / Đấm chay) | Chạm trần của 2 skill CC Môi trường ở Phase cuối + đòn đấm chay Boss |
| 1–2 (Cảnh cáo) | 2 skill Sát thương Môi trường ở Phase 1 & 2 |
| 0 (Không sát thương) | 2 skill CC Môi trường ở Phase 1 & 2 |

- **Telegraph:** UI Logo chớp nháy báo loại bẫy + vùng đỏ bán trong suốt dưới đất (kèm thanh Fill/Scale) báo vị trí & thời gian chính xác.

### 5.5 Bộ Kỹ Năng Môi Trường (mỗi Golem giữ 1 CC + 1 Sát thương)

#### 🔴 Golem A — Combo Trục Ngang: "Đường Ống Tử Thần"
**CC — Tường Đá Ép (Snap Trap):**
- `WaitTime` (thời gian đứng im trước khi sập) = **hằng số cố định 1.5s** ở mọi Phase (để Player rèn phản xạ/muscle memory).
- `SlamSpeed` (vận tốc 2 tường lao vào nhau) = biến số **tăng dần theo Phase**.
- Phase 1–2 (0 dmg): tường mỏng mọc cản đường ngẫu nhiên.
- Phase 3 (dmg Mức 2): 2 tường tạo ngõ cụt, đứng im 1.5s rồi sập nhanh. Bắt buộc nhảy né (Dash vô hiệu). Nếu Player thoát ra trước hạn, tường vẫn sập (cắn không khí).
- Phase 4 (dmg Mức 3): SlamSpeed cực đại (chớp nhoáng). Trúng đòn → **Trói (Root)**. Nếu đang lơ lửng mà bị kẹp → xích đá ghim và kéo giật cắm đầu xuống đất cực mạnh.

**Sát thương — Đá Lăn:**
- Phase 1–2 (Mức 1-2): hitbox vừa, cooldown dài.
- Phase 3–4 (Mức 3 → Max 4): hitbox to hơn, cooldown cực nhanh, tạo luồng đạn dồn dập.

#### 🔵 Golem B — Combo Trục Dọc: "Bẫy Chuột Trùng Trình"
**CC — Dư Chấn & Mưa Đá:**
- Phase 1 (0 dmg): rung nhẹ, Slow nhẹ.
- Phase 2 (0 dmg): rung mạnh, tăng % Slow + kéo dài thời gian.
- Phase 3 (Mức 2): thêm đá vụn rớt từ trần (chip damage).
- Phase 4 (Mức 3): đá to hơn 1 chút, mật độ mưa đá kiểu Bullet Hell, kết hợp Slow nặng cực kỳ nguy hiểm.

**Sát thương — Cọc Đá Đâm** (tuyệt đối KHÔNG bám đuổi/tracking):
- Phase 1–2 (Mức 1-2): vùng cảnh báo AOE nhỏ, delay dài.
- Phase 3–4 (Mức 3 → Max 4): vùng cảnh báo AOE khổng lồ, delay cực ngắn, cooldown liên tục — ép Player Dash liên tục để thoát.

---

## 5.6 Implementation Notes

**File structure** `Assets/Script/EnemyThing/Boss/DuoGolem/` (11 files):

```
DuoGolem/
├── GolemController.cs           # Base: HP phase, death loop, animation stubs
├── GolemA.cs                    # SnapTrap + RollingStone factory
├── GolemB.cs                    # TremorHailstorm + StoneSpikeStab factory
├── IEnvironmentSkill.cs         # Interface: Enable(phase), Disable()
├── ArenaHazardController.cs     # 2-slot plain class (not MB), phase gating
├── States/
│   ├── GolemAttackState.cs      # Single punch (Mức 3, timer-based)
│   ├── ParalyzedState.cs        # Disable ALL colliders + hazards
│   └── RevivalChannelingState.cs# 8s timer, HP snap, death check
└── GolemAHazards/
│   ├── SnapTrapSkill.cs         # Wall trap (CC), physical Dash block
│   └── RollingStoneSkill.cs     # Boulder spawn (Damage)
└── GolemBHazards/
    ├── TremorHailstormSkill.cs  # Slow tremor + falling debris (CC→Damage)
    └── StoneSpikeStabSkill.cs   # Ground AoE spikes (Damage)
```

**Key implementation decisions** (documented in code comments):
- SnapTrap blocks Dash via **collision matrix** (TrapWall layer), not by disabling the Dash skill
- Downward force in SnapTrap Phase 4 uses direct `Rigidbody2D.linearVelocity.y = value`, NOT `AddForce`
- ParalyzedState calls `GetComponents<Collider2D>()` → disable **all** colliders (prevent Hachiware lifesteal exploit)
- Revival HP snap: `revivedHP = partnerHealth.CurrentHP` (exact mirror per GDD)
- Phase gate: `hazard.SetPhase(Mathf.Max(0, myPhase - 1))` — edge case at Phase 0 (keeps Phase 1)
- No separate Manager/referee class per GDD; symmetric FSM with re-entrancy guard (`isParalyzedOrReviving`)

**Pending setup** (not in code — marked as `// TODO` or XML `"chua chot so lieu"`):
- Hazard prefab instantiation needs real prefab references
- Damage/cooldown/hitbox values all placeholder for Architect tuning
- `partnerGolem` reference must be assigned in Inspector
- Animator Controller needs triggers: "Punch", "Run", "Die"
- Chapter assignment TBD

---

## 5.7 Die Animation TODO (All Bosses)

⚠️ **Animation chết (Die) chưa có asset chính thức cho bất kỳ Boss nào.**

- `VoidBoss_Die.anim` đã tạo placeholder (6 sprite keyframe từ sheet VoidBoss, non-looping, 0.83s) — nhưng dùng sprite Idle làm frame.
- BatBoss cũng chưa có Die animation riêng — hiện tại dùng Idle fallback qua Animator.
- Duo Golem chưa có animation (code đã có, chờ Animator Controller + sprite).

**Cần:** Asset artist tạo sprite Die riêng cho từng Boss, cập nhật .anim clip tương ứng.

---

## 6. GHI CHÚ TỔNG HỢP

| Boss | Trạng thái | Đặc trưng cơ chế lọc sát thương | Nguồn gây khó chính |
|---|---|---|---|
| BatBoss (Ch.4) | Đã code, đã refactor FSM (dùng Generic State) | Deflect Melee, 1.5x Ranged, Pillar → Hurt | Bay lơ lửng + Pillar hệ thống ngầm |
| VoidBoss (Ch.6) | Đã code Controller + 5 hitbox prefab + controller + BossHealthBarUI | Không lọc, sát thương thường từ mọi nguồn | CC/Debuff tầm xa dồn dập + Ultimate Blood Moon |
| Duo Golem | **Đã code** (11 files: controller, 2 classes, 3 states, 4 hazards, 2 interfaces) | Không áp dụng cho Golem (chỉ 1 đòn đấm chay) | Toàn bộ độ khó đến từ Môi trường (4 skill theo Phase) |