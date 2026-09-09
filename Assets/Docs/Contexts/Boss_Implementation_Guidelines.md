# Boss Implementation Guidelines

Tài liệu rút kinh nghiệm từ BatBoss để dùng làm checklist khi bắt đầu boss mới.

## 1. Chốt lifecycle trước khi viết attack

Boss cần có flow rõ ràng:

```text
Spawn/Inactive -> Reveal -> WakeUp -> Combat -> Hurt/Phase -> Die -> Defeated event -> Cleanup
```

Mỗi trạng thái phải có điều kiện vào, điều kiện ra và quyền sở hữu cleanup. Không để
`Update()` của boss chặn state `Die` chỉ vì một cờ gameplay như `isAwake` hoặc `isActive`.

## 2. Tách trách nhiệm giữa hệ thống

- Arena: trigger encounter, camera reveal, optional UnityEvent.
- Boss controller: AI, state transition, animation event callbacks.
- Health: HP, armor/shield và death entry point.
- Attack object: hitbox, damage timing và lifetime của chính nó.
- Cutscene/progression: lắng nghe boss defeated.
- Scene transition: quyết định player spawn.

Không để arena tự đóng map, tự spawn Player, tự điều khiển chapter progression và tự
phát cutscene cùng lúc nếu các hệ thống chuyên trách đã tồn tại.

## 3. Dùng damage pipeline chung

Boss phụ, weak point và object có thể bị đánh nên dùng `CharacterStats + Health` nếu
không có lý do mạnh để tạo hệ HP riêng. Điều này giữ cho melee, Stand, projectile,
armor, shield và health bar hoạt động đồng nhất.

Nếu boss có miễn nhiễm hoặc bonus damage, dùng `DamageSource` để filter tại Health.
Không kiểm tra kiểu projectile bằng component cụ thể ở từng attack object.

## 4. Animation event phải có fallback an toàn

Mỗi event quan trọng cần:

- tên method ổn định và public;
- không spawn lặp nếu event bị gọi hai lần;
- state timer không phụ thuộc tuyệt đối vào event cuối clip;
- clip và timer được kiểm tra cùng nhau khi đổi animation.

Object được spawn từ animation phải có lifecycle độc lập: landing/explosion, damage
once, animation one-shot và destroy rõ ràng.

## 5. Physics và vị trí phải là quyết định thiết kế

Ngay khi tạo boss cần chốt:

- boss có chịu gravity không;
- có bị Player/địa hình đẩy không;
- vị trí cố định hay navigation;
- trục nào được phép truy đuổi;
- hitbox/hurtbox dùng trigger hay collision;
- layer collision matrix.

Không dùng nhiều script cùng ghi `transform.position` trong các callback khác nhau nếu
không có thứ tự update rõ ràng. Với boss bay, ưu tiên một owner duy nhất cho vị trí.

## 6. UI không được chứa gameplay policy

Health bar chỉ nhận HP và hiển thị. Màu, layout, canvas mode và Fill nên do prefab/UI
artist cấu hình. Script không nên tự ghi đè màu theo gradient nếu design không yêu cầu.

Boss bar Screen Space và enemy bar World Space có thể dùng hai prefab UI khác nhau,
nhưng đều nên implement cùng interface `IHealthBar`.

## 7. Death cleanup phải được test như một feature riêng

Khi boss chết cần xác nhận:

- state death vẫn được tick trong mọi trạng thái trước đó;
- event defeated chỉ phát một lần;
- boss không spawn thêm attack/pillar;
- health bar, collider và Rigidbody không còn gây tác động ngoài ý muốn;
- loot/cutscene/progression không bị gọi lặp;
- projectile, hazard, summon còn tồn tại được xử lý theo quyết định thiết kế.

## 8. Bài học rút ra từ BatBoss

BatBoss cho thấy các lỗi boss thường không nằm ở một attack riêng lẻ mà nằm ở ranh
giới giữa arena, state machine, physics, damage và progression. Khi xây boss mới:

- **Không thức tỉnh ngoài ý muốn:** boss phải bắt đầu ở trạng thái ngủ/inactive và chỉ
  được `BeginEncounter()` từ arena hoặc trigger có chủ đích. Không gọi wake trong
  `Start()` nếu thiết kế yêu cầu Player phải bước vào vùng đánh thức.
- **Arena không ôm quá nhiều trách nhiệm:** arena chỉ reveal camera, bật encounter và
  phát event tùy chọn. Gate, spawn Player, load scene, đóng map và boss defeat phải do
  hệ thống chuyên trách xử lý.
- **Physics phải được chốt trước:** với boss bay hoặc boss cố định, cấu hình
  `Rigidbody2D`, gravity, collision và owner của position ngay từ đầu. Tránh để nhiều
  script cùng ghi `transform.position`.
- **Spawn point và offset phải là dữ liệu rõ ràng:** attack rơi từ trên cao, projectile
  trên mặt đất và pillar không nên dùng tọa độ ngầm hoặc nhầm với vị trí boss. Các
  offset quan trọng cần có field Inspector và gizmo kiểm tra trực quan.
- **AoE cần tách telegraph, hit timing và cleanup:** quyết định rõ damage xảy ra khi
  chạm Player hay khi chạm đất; damage chỉ một lần; object phải tự hủy sau animation.
- **Weak point nên dùng pipeline chung:** pillar/weak point dùng `CharacterStats +
  Health + Collider2D` để melee, Stand, projectile, armor và health bar dùng cùng một
  đường xử lý. Chỉ thêm damage filter riêng khi thiết kế thật sự cần.
- **Death phải có guard và fallback:** animation death, event defeated, loot, summon,
  health bar và destroy không được chạy hai lần. Animation event cần fallback nếu clip
  bị đổi hoặc event bị thiếu.
- **Serialized reference là một phần của implementation:** script đúng nhưng thiếu
  Collider2D, camera, prefab, spawn point hoặc SequencePlayer vẫn khiến encounter hỏng.
  Mỗi boss cần kiểm tra cả code, prefab và scene.
- **Gizmo phải phục vụ đúng câu hỏi gameplay:** chỉ vẽ attack range, safe/reveal range,
  AoE hoặc spawn point cần tuning; không dùng gizmo để che giấu logic chưa rõ.
- **Test theo lifecycle, không chỉ test đánh boss:** cần test ngủ, reveal, wake, attack,
  projectile/hazard, pillar, death giữa attack, cleanup và cutscene sau defeat.

## 9. Checklist trước khi commit boss mới

- [ ] Có README/context mô tả đúng code hiện tại.
- [ ] Có test TODO cho setup, wake, combat, damage, death và regression.
- [ ] Không còn tên state/class/field cũ trong tài liệu.
- [ ] Tất cả serialized reference quan trọng đã được kiểm tra trong prefab/scene.
- [ ] `git diff --check` sạch ngoài các cảnh báo line ending của Unity.
- [ ] Đã test cả happy path và death/disable/enable edge cases.
