# Roadmap.md — Kế hoạch phát triển

> Định hướng công việc cho các Sprint sắp tới.
> Cập nhật bởi System Architect.

---

## Sprint Hiện tại: Chapter 4 — BatBoss Phase 1

**Mục tiêu:** Hoàn thiện BatBoss là boss có thể chiến đấu được.

### TODO List

| # | Task | Trạng thái | File liên quan |
|---|---|---|---|
| 1 | Refactor FSM: dùng generic states thay vì Bat-specific (`GenericAttackState`, `HurtState`, `DieState`) | ✅ Hoàn thành | `BatBossController.cs`, `GenericAttackState.cs`, `HurtState.cs`, `DieState.cs` |
| 2 | Restore `GetHurtState()` / `GetDieState()` override | ✅ Hoàn thành | `BatBossController.cs` |
| 3 | **Hoàn thiện code Pillar** — VFX nổ, damage trigger, `DestroyPillar()` logic | ⏳ Đang làm | `Pillar.cs` |
| 4 | **VFX Nổ `holePrefab`** — Animation circles, delayed burst, damage vùng | ⏳ Đang làm | `Hole.cs` (prefab script) |
| 5 | **Test cân bằng sát thương** — Verify 1.5x Ranged bonus, Melee deflect, Pillar burst damage (25% MaxHP) | ⏳ Chưa bắt đầu | `BatHealth.cs`, `Pillar.cs` |
| 6 | Tích hợp thanh máu `BossHealthBarUI` | ⏳ Chưa bắt đầu | `BossHealthBarUI.cs`, `BatBossController.cs` |

### Checklist Testing Phase 1

- [ ] Ranged attack → damage × 1.5, Boss không flinch (no HurtState)
- [ ] Melee/Stand attack → 0 damage, deflect SFX
- [ ] Pillar spawn đúng vị trí (trong 12f, cách pillar khác 5f)
- [ ] Pillar bị phá → 25% MaxHP damage + Boss vào HurtState (flash đỏ)
- [ ] `holePrefab` AoE circle bùng nổ tại vị trí Player sau delay
- [ ] Boss chết → VFX, loot, `OnBossDefeated` event, destroy sau 2s

---

## Sprint Tiếp theo: Chapter 4 — BatBoss Phase 2

**Mục tiêu:** Enrage mechanic khi Boss máu < 50%.

### Thiết kế sơ bộ

```
BatBoss Phase 2 (Enrage):
  ├── Tốc độ hover tăng (hoverSpeed × 1.5)
  ├── Atk1: DropSphere → 2 quả cầu thay vì 1
  │         (gọi DropSphere() 2 lần từ Animation Event)
  ├── Atk2: holePrefab → AoE rộng hơn 1.5x
  ├── Pillar spawn: cooldown giảm 7s → 4s
  └── Active shield: deflecting first hit mỗi 10s (VirtualShieldEffect)
```

### Implementation Plan

1. Thêm `private bool isEnraged;` trong `BatBossController`
2. `Update()` kiểm tra `Health.CurrentHealth < cachedMaxHP * 0.5f` → set `isEnraged = true`
3. Điều chỉnh parameter tương ứng khi `isEnraged` active
4. Test cân bằng: Phase 2 không được quá khó (1 hit kill) hoặc quá dễ

---

## Các Sprint Sau (Backlog)

| Sprint | Nội dung | Ghi chú |
|---|---|---|
| 3 | Boss Health Bar UI + BossArenaController (camera locking, gate) | Chương 4 |
| 4 | Chapter 4 Boss Scene — tilemap, arena design | Phối hợp với Level Designer |
| 5 | Mini-boss Chapter 1-3 system (nếu có) | Mở rộng hệ thống |
| 6 | Cross-chapter progression balance | Tuning |

---

## Technical Debt (Nợ kỹ thuật)

### Critical (Cần xử lý gấp)

| # | Vấn đề | Mô tả | Giải pháp |
|---|---|---|---|
| TD-1 | Save/Load dùng `itemName` string | `SerializableItemStack` lưu `itemName` → `Resources.Load<ItemData>(itemName)` tìm trong subfolders: `Items/`, `Items/Consumables/`, `Items/Equipments/`, `Items/Materials/`, `Items/Quest/`, `Items/Skills/`. Load theo tên dễ gãy nếu rename. | Migrate sang lưu `itemId` (GUID từ ScriptableObject) và dùng `Resources.Load` với đường dẫn cố định hoặc dictionary mapping. |

### Medium

| # | Vấn đề | Mô tả |
|---|---|---|
| TD-2 | `ArmorPierce` stat tồn tại nhưng chưa dùng | Cập nhật `Health.TakeDamage()` formula: `finalDamage = max(1, (incoming - armor * (1 - armorPierce/100)) * (1 - dmgR))` |
| TD-3 | `AssisterController` legacy | Xác nhận unused → xoá |
| TD-4 | Pillar detect projectile bằng `GetComponent<ProjectilePref>()` | Migrate sang tag/layer based detection |
| TD-5 | `GenericAttackState.animDuration` hardcode 1.2s | Cần sync với clip length thực tế |

---

## Timeline Ước Lượng

```
Sprint 1 (hiện tại): Phase 1 combat-ready        ████████████░░░░░░  [60%]
Sprint 2 (tiếp theo): Phase 2 enrage mechanic     ░░░░░░░░░░░░░░░░░░  [0%]
Sprint 3: Boss UI + Arena                          ░░░░░░░░░░░░░░░░░░  [0%]
Sprint 4: Chapter 4 Scene                          ░░░░░░░░░░░░░░░░░░  [0%]
```

> **Note:** Timeline có thể thay đổi dựa trên priority từ Architect.
