# BatBoss Chapter 4 — Test TODO

Checklist test core feature của BatBoss. Có thể mở rộng bằng cách thêm issue mới ở cuối file.

## 0. Setup

- [ ] Scene có Player, `PlayerManager`, BatBoss và `BossArenaController`.
- [ ] Bat root có `CharacterStats`, `BatHealth`, `BatBossController`, `Animator`, `Rigidbody2D`, tag `Enemy`.
- [ ] Rigidbody2D của Bat là Kinematic, Gravity Scale = 0.
- [ ] Gán `batSpherePrefab`, `pillarPrefab`, `holePrefab`.
- [ ] Gán các `pillarSpawnPoints`, boss camera và Collider2D trigger của arena.
- [ ] Pillar prefab có `CharacterStats`, `Health`, `Pillar`, Collider2D và `EnemyHPBar`; HP hiện tại = 20.
- [ ] `BossDefeatCutsceneTrigger` gán đúng boss và `SequencePlayer` nếu dùng cutscene.

## 1. Arena và wake-up

- [ ] Player đi vào reveal trigger đúng một lần.
- [ ] Boss được bật và `BeginEncounter()` chạy đúng một lần.
- [ ] Camera zoom tới boss; `onRevealComplete` chỉ gọi sau khi reveal hoàn tất.
- [ ] Boss không tự thức hoặc tự đánh khi Player còn ngoài vùng arena.
- [ ] `Bat_WakeUp` gọi `OnWakeUpComplete()` và chuyển sang `Combat`.

## 2. Combat và vị trí

- [ ] Boss giữ nguyên độ cao và không bị gravity/va chạm đẩy lệch.
- [ ] Khi Player ngoài `attackRange`, boss truy đuổi theo trục X.
- [ ] Khi Player trong `attackRange`, boss chọn `Atk1` hoặc `Atk2` sau cooldown.
- [ ] Boss không tấn công trước khi `OnWakeUpComplete()`.
- [ ] `Atk1` gọi `DropSphere()` đúng một lần.
- [ ] `Atk2` gọi `SpawnAoECircle()` đúng một lần.
- [ ] Attack state quay lại `Combat`, không bị kẹt nếu animation event chạy.

## 3. Damage filter

- [ ] Melee/PlayerNA bị deflect.
- [ ] Stand bị deflect.
- [ ] Projectile có `DamageSourceType.Ranged` gây damage × 1.5.
- [ ] Damage thiếu `DamageSource` bị deflect.
- [ ] `System` damage được nhận bình thường.
- [ ] `Pillar` damage được nhận và gọi `ForceHurtState()`.
- [ ] Armor/shield của `Health` vẫn được áp dụng sau filter.
- [ ] Boss HP bar cập nhật đúng nhưng giữ nguyên màu `Fill` trong prefab.

## 4. Pillar

- [ ] Pillar spawn theo cooldown và không vượt `maxActivePillars`.
- [ ] Point quá xa Player hoặc quá gần Pillar khác không được chọn.
- [ ] Player normal attack phá được Pillar.
- [ ] Stand phá được Pillar.
- [ ] Projectile phá được Pillar.
- [ ] Tất cả damage đi qua `Health`; không còn phụ thuộc `Pillar.TakeHit()`.
- [ ] Pillar chết sau đủ damage và health bar về 0.
- [ ] Pillar chết gây burst damage = 25% MaxHP boss.
- [ ] Boss flash/hurt đúng một lần.
- [ ] Pillar bị destroy được loại khỏi danh sách active.

## 5. BatSphere và Bat-Hole

- [ ] BatSphere spawn tại `Player.x`, `Boss.y + dropHeight`.
- [ ] BatSphere rơi và chỉ land một lần khi chạm ground.
- [ ] Landing gây burst AoE một lần.
- [ ] HazardZone được tạo đúng duration nếu prefab được gán.
- [ ] HazardZone gây DoT đúng interval/duration.
- [ ] Bat-Hole spawn tại `Player.x`, `holeSpawnY`.
- [ ] Bat-Hole có Collider2D trigger.
- [ ] Player ở trong vùng tại event `DealDamageNow()` thì nhận damage.
- [ ] Player rời vùng trước event thì không nhận damage.
- [ ] Bat-Hole chạy animation một lần rồi `DestroyAfterAnimation()`.

## 6. Death và progression

- [ ] Boss chết khi HP về 0, kể cả khi đang ở trạng thái chưa awake.
- [ ] `Die` animation chạy và boss destroy sau khoảng 2 giây.
- [ ] Death callback chỉ chạy một lần.
- [ ] Boss không spawn Pillar/hazard sau khi chết.
- [ ] Health bar ẩn sau death.
- [ ] Loot spawn tối đa một lần.
- [ ] `OnBossDefeated` được gọi và cutscene chạy đúng một lần.
- [ ] Boss chết giữa attack không để progression bị kẹt.
- [ ] Hazard đang tồn tại sau boss death được kiểm tra theo thiết kế: hủy ngay hoặc được phép chạy hết.

## 7. Regression và edge cases

- [ ] Player chết trong lúc boss đang attack.
- [ ] Boss chết giữa `Combat`, `Hurt` và animation attack.
- [ ] Player vào/ra arena nhiều lần.
- [ ] Boss prefab disable/enable nhiều lần không tạo duplicate event.
- [ ] Thiếu prefab reference cho warning rõ ràng, không spam exception mỗi frame.
- [ ] Scene không có `PlayerManager` vẫn không NullReference.
- [ ] Nhiều projectile va chạm cùng frame không gây damage lặp sai.
- [ ] Scene transition giữ đúng Player spawn position đã cấu hình.

## Issues phát hiện trong quá trình test

Thêm issue theo mẫu:

```text
- [ ] [P?] Mô tả ngắn
  - Repro:
  - Expected:
  - Actual:
  - Evidence: file/line hoặc screenshot
  - Follow-up:
```
