# BatBoss Chapter 4 — Test TODO

Checklist để test toàn bộ core feature của BatBoss. Có thể mở rộng bằng cách thêm issue mới vào mục cuối file.

## 0. Setup tối thiểu

- [ ] Dựng scene test tối thiểu có Player, `PlayerManager`, Bat prefab và `BatBossController`.
- [ ] Kiểm tra Bat root có `CharacterStats`, `BatHealth`, `BatBossController`, `Animator`, `Rigidbody2D`.
- [ ] Kiểm tra Bat root có tag `Enemy`.
- [ ] Kiểm tra Rigidbody Bat là Kinematic và `Gravity Scale = 0`.
- [ ] Gán `batSpherePrefab`, `pillarPrefab`, `holePrefab`.
- [ ] Tạo và gán tối thiểu 3 `pillarSpawnPoints` trong vùng test.
- [ ] Gán `BatHealth` với HP boss theo giá trị design được chốt sau này.
- [ ] Xác nhận Pillar prefab có HP = `20`.

## 1. Smoke test khởi động

- [ ] Play scene test: không có NullReferenceException từ BatBoss/Health/Animator.
- [ ] Bat phát `Bat_WakeUp` đúng một lần.
- [ ] Animation event `OnWakeUpComplete` chuyển boss sang `Hover`.
- [ ] Boss không bị rơi bởi gravity và không bị đẩy lệch bởi physics.
- [ ] Boss hover quanh vị trí gốc, không bị lệch X theo thời gian.
- [ ] Boss không bắt đầu AI trước khi wake-up hoàn tất.

## 2. FSM và animation

- [ ] Sau khoảng 2 giây ở `Hover`, boss chọn `Atk1` hoặc `Atk2`.
- [ ] `Atk1` gọi `DropSphere()` đúng một lần.
- [ ] `Atk2` gọi `SpawnAoECircle()` đúng một lần.
- [ ] Attack state tự quay về `Hover` sau timer.
- [ ] `OnAttackAnimEnd()` không làm state bị chuyển lặp hoặc kẹt.
- [ ] Hurt state chỉ xuất hiện từ Pillar explosion.
- [ ] Die state phát animation `Die` và không quay lại `Hover`.

## 3. Damage filter

- [ ] Melee damage bị Deflect.
- [ ] Stand damage bị Deflect.
- [ ] Projectile có `DamageSourceType.Ranged` gây damage × `1.5`.
- [ ] Damage không có `DamageSource` bị Deflect.
- [ ] `System` damage được nhận bình thường.
- [ ] Pillar damage được nhận bình thường và gọi `ForceHurtState()`.
- [ ] Armor/shield của `Health` vẫn được áp dụng đúng sau damage filter.
- [ ] Health bar phản ánh HP hiện tại và max HP chính xác.

## 4. Pillar system

- [ ] Pillar spawn theo cooldown.
- [ ] Không vượt quá `maxActivePillars`.
- [ ] Không spawn tại point quá xa Player.
- [ ] Không spawn quá gần Pillar khác.
- [ ] Player normal attack phá được Pillar.
- [ ] Stand phá được Pillar.
- [ ] Projectile phá được Pillar.
- [ ] Pillar có HP = `20` và destroy sau đủ damage.
- [ ] Pillar destroy gây burst damage bằng `25%` max HP boss.
- [ ] Pillar destroy làm boss flash/hurt đúng một lần.
- [ ] Pillar bị destroy được remove khỏi danh sách active.

## 5. BatSphere và Bat-Hole

- [ ] BatSphere rơi đúng hướng và dừng khi chạm ground.
- [ ] BatSphere gây burst AoE một lần khi landing.
- [ ] BatSphere tạo HazardZone nếu prefab đã gán.
- [ ] HazardZone gây DoT đúng interval và duration.
- [ ] Bat-Hole có Collider2D `Is Trigger`.
- [ ] Player vào Bat-Hole được ghi nhận bởi `OnTriggerEnter2D`.
- [ ] Animation event `DealDamageNow` gây damage đúng một lần.
- [ ] Player rời Bat-Hole trước event thì không bị damage.

## 6. Death và progression

- [ ] Boss chết khi HP xuống 0.
- [ ] Death callback chỉ chạy một lần.
- [ ] Boss không tiếp tục spawn Pillar/hazard sau khi chết.
- [ ] Health bar ẩn sau khi boss chết.
- [ ] Loot spawn đúng một lần nếu có LootManager.
- [ ] `OnBossDefeated` được phát đúng một lần.
- [ ] Boss arena/ChapterManager nhận callback và complete boss chapter.
- [ ] Save/load sau khi complete không đưa Player quay lại trạng thái chưa hoàn thành.

## 7. Regression và edge cases

- [ ] Player chết trong lúc boss đang attack.
- [ ] Boss chết trong lúc BatSphere/HazardZone còn tồn tại.
- [ ] Boss chết giữa Hurt state.
- [ ] Player vào/ra vùng test nhiều lần.
- [ ] Disable/enable boss prefab nhiều lần.
- [ ] Thiếu một prefab reference thì có warning rõ ràng, không spam exception mỗi frame.
- [ ] Không có PlayerManager khi scene khởi động.
- [ ] Test với nhiều projectile va chạm cùng frame.

## Issues phát hiện trong quá trình test

Thêm issue mới theo mẫu:

```text
- [ ] [P?] Mô tả ngắn
  - Repro: 
  - Expected: 
  - Actual: 
  - Evidence: file/line hoặc screenshot
```

