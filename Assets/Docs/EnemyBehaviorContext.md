# Enemy Behavior Context

## 1. Phạm vi

Tài liệu này mô tả behavior chung của enemy dựa trên `EnemyController`, `EnemyStateFactory`, `MovementManager`, các interface state và `Health`. Các boss hoặc enemy đặc biệt có thể override một phần luồng này.

## 2. Các thành phần chính

- `EnemyController`: điều phối state hiện tại, movement, combat, animation và chuyển state.
- `EnemyStateFactory`: tạo các state mặc định.
- `IEnemyState`: hợp đồng `OnEnter`, `OnUpdate`, `OnExit` cho mỗi state.
- `IEnemyMovement`: abstraction cho patrol, pursuit, quay hướng và khoảng cách tới Player.
- `IEnemyCombat`: abstraction cho attack, cooldown và animation combat.
- `IEnemyStateContext`: abstraction cho việc chuyển state bằng tên.
- `MovementManager`: thao tác với `Rigidbody2D`, `SpriteRenderer` và vận tốc ngang.
- `AnimationController`: gửi trigger/bool tới `Animator`.
- `Health`: xử lý damage, shield, hồi máu và lifecycle chết.

## 3. Lifecycle khởi tạo

### Health

`Health.Awake()` lấy `CharacterStats`, xác định `maxHealth`, đặt `currentHealth` bằng HP tối đa, khởi tạo health bar và đăng ký sự kiện thay đổi MaxHP.

### EnemyController

`EnemyController.Start()` thực hiện các bước:

1. Lấy `Rigidbody2D`, `SpriteRenderer`, `Animator` và `CharacterStats`.
2. Tìm Player thông qua `PlayerManager`.
3. Tạo `MovementManager`.
4. Tạo `AnimationController`.
5. Tạo `EnemyStateFactory`.
6. Cache các state mặc định.
7. Chuyển enemy vào state ban đầu bằng `ChangeState(GetIdleState())`.

Mỗi frame, `EnemyController.Update()` gọi `OnUpdate()` của `currentState`.

## 4. State machine mặc định

Luồng hiện tại:

```text
Idle
  ├─ thấy Player trong vision range → Pursuit
  └─ chưa thấy Player → Patrol

Pursuit
  ├─ Player ngoài vision range → Idle
  ├─ Player trong attack range → Attack
  └─ chưa đủ gần → Pursue

Attack
  ├─ Player ra ngoài attack range → Pursuit
  └─ còn trong tầm → chờ cooldown và ExecuteAttack

Hurt
  └─ hết thời gian hurt → thường chuyển Pursuit hoặc state được cấu hình

Die
  └─ hết thời gian animation → Destroy enemy
```

## 5. Behavior từng state

### IdleState

`IdleState` hiện đang kiêm cả hai vai trò:

- Trạng thái không giao chiến.
- Di chuyển patrol.

Nếu Player nằm trong `visionRange`, state chuyển sang `Pursuit`. Nếu chưa phát hiện Player, state gọi `MovementManager.Patrol()`.

Tên `IdleState` vì vậy chưa phản ánh đầy đủ behavior hiện tại. Enemy không thực sự đứng yên trong state này.

### BasePursuitState

Khi vào state, enemy bật animation Run.

Mỗi frame:

1. Tính khoảng cách tới Player bằng `GetDistanceToPlayer()`.
2. Nếu khoảng cách lớn hơn `visionRange`, chuyển về `Idle`.
3. Nếu nằm trong `attackRange`, chuyển sang `Attack`.
4. Nếu chưa đủ gần, gọi `movement.Pursue()`.

`MovementManager` chỉ so sánh tọa độ X để quyết định đi trái hoặc phải.

### BaseAttackState

Khi vào attack:

- Tắt animation Run.
- Quay về phía Player.

Trong lúc attack:

- Nếu Player ra ngoài attack range, chuyển về `Pursuit`.
- Nếu cooldown sẵn sàng, gọi `ExecuteAttack()` và `RecordAttack()`.

`ExecuteAttack()` thường chỉ phát animation trigger. Damage thực tế thường được gọi bởi Animation Event trên controller.

### HurtState

Khi `Health.TakeDamage()` làm enemy còn sống, enemy lấy hurt state và chuyển sang state đó.

Hurt state:

- Reset timer.
- Phát animation Hurt.
- Tắt animation Run.
- Chờ `hurtDuration`.
- Sau đó chuyển tiếp sang Pursuit hoặc return state.

Field `previousState` hiện không hoàn toàn khôi phục state trước đó; trong một nhánh logic, state vẫn chuyển sang `Pursuit`. Cần xác định rõ đây là chủ ý hay cần sửa thành khôi phục state thật sự.

### DieState

Khi HP giảm xuống 0:

```text
Health.TakeDamage()
  → Health.Die()
  → OnDied
  → EnemyController.GetDieState()
  → ChangeState(DieState)
  → animation Die
  → hết dieDuration
  → callback onDeath nếu có
  → Destroy(enemy)
```

`Health.isDead` ngăn damage, heal, `Die()` và `OnDied` bị xử lý lặp lại.

`SetHealth(0)` chỉ đặt HP về 0, không tự phát `OnDied`. Duo Golem dùng hành vi này cho trạng thái bị hạ tạm thời trước khi hồi sinh.

## 6. Movement hiện tại

`MovementManager` hiện hỗ trợ:

- Patrol theo hướng cuối cùng.
- Di chuyển tới Player theo trục X.
- Lùi khỏi Player theo trục X.
- Quay mặt theo hướng di chuyển.
- Đảo hướng khi va vào object có tag `Obstacle`.

Movement hiện chưa có:

- Hai giới hạn trái/phải cho patrol.
- Home position.
- Khoảng cách truy đuổi tối đa.
- Kiểm tra mép platform.
- Kiểm tra nền phía trước.
- Trạng thái đang rơi.
- Phân biệt ground enemy và flying enemy.
- Điều hướng giữa nhiều tầng hoặc platform.

## 7. Các giới hạn behavior hiện tại

### Enemy rời khỏi vùng patrol

Luồng hiện tại là:

```text
Enemy đang Patrol
  → phát hiện Player
  → Pursuit
  → Player ra khỏi vision range
  → Idle
  → Patrol tại vị trí hiện tại
```

Enemy không quay về vị trí hoặc khu vực patrol ban đầu. Vì vậy Player có thể kéo enemy ra xa khỏi vị trí thiết kế.

### Phát hiện Player theo khoảng cách 2D

`GetDistanceToPlayer()` dùng `Vector2.Distance`, bao gồm cả X và Y. Logic chưa kiểm tra:

- Player có cùng tầng hay không.
- Player có bị tường che hay không.
- Player có nằm phía trước enemy hay không.
- Phía trước enemy có nền hay không.
- Player ở bên kia vực hay không.

Enemy mặt đất có thể cố chạy ngang tới Player ở tầng khác hoặc ngoài platform.

### Enemy đi qua mép platform

Movement hiện chỉ nhìn Player và đặt vận tốc ngang. Không có ground probe hoặc edge detection. Nếu Player nằm bên kia mép, enemy có thể bước ra khỏi platform và rơi xuống.

### Enemy rơi trong lúc truy đuổi

Hiện chưa có `FallingState`. `PursuitState` không biết enemy đang đứng trên nền, đang rơi hay đã rơi xuống vị trí không hợp lệ.

### State khởi tạo có side effect

`EnemyController.CacheStates()` gọi các method provider dạng virtual. Enemy con có thể override các method này, vì vậy logic riêng của class con có thể chạy trong lúc base class chưa hoàn tất khởi tạo.

Đây là rủi ro đáng chú ý với các controller có state phụ thuộc vào field riêng, ví dụ Duo Golem.

## 8. Giải pháp Home và ReturnToPost

Nên tách vị trí hoạt động của enemy khỏi vùng truy đuổi.

Các dữ liệu có thể có:

```text
homePosition
patrolLeft
patrolRight
maxChaseDistance
```

State flow đề xuất:

```text
Patrol
  → thấy Player hợp lệ → Pursuit

Pursuit
  ├─ vào attack range → Attack
  ├─ mất Player → ReturnToPost
  ├─ vượt maxChaseDistance → ReturnToPost
  └─ gặp mép platform → Turn hoặc ReturnToPost

ReturnToPost
  ├─ về gần HomePosition → Patrol
  └─ phát hiện lại Player hợp lệ → Pursuit
```

`ReturnToPostState` chỉ cần điều khiển enemy về gần `homeX` hoặc waypoint gần nhất. Không nên chuyển trực tiếp về `Idle` sau khi mất mục tiêu.

## 9. Tách Idle và Patrol

State nên được phân biệt:

```text
Idle
  → đứng yên hoặc chờ

Patrol
  → di chuyển giữa các waypoint
```

Patrol có thể dùng hai `Transform`:

```text
PatrolLeft ---------------- PatrolRight
```

Behavior ở biên:

```text
đến waypoint
  → dừng ngắn hoặc phát animation LookAround
  → đảo hướng
  → tiếp tục Patrol
```

Waypoint là lựa chọn phù hợp với level 2D vì dễ cấu hình và nhìn thấy trực tiếp trong Unity Editor.

## 10. Xử lý mép platform

Đối với ground enemy không được phép rơi, trước khi di chuyển nên kiểm tra:

1. Có vật cản phía trước không?
2. Có nền ở vị trí phía trước không?
3. Chênh lệch độ cao có nằm trong giới hạn không?

Nếu không có nền:

```text
Pursuit → Turn hoặc ReturnToPost
```

Logic raycast nên nằm trong `MovementManager`, không đặt trực tiếp toàn bộ trong state.

Các capability có thể cung cấp:

```text
IsGrounded()
CanMoveForward()
IsAtPlatformEdge()
```

## 11. Xử lý enemy được phép rơi

Nếu design cho phép enemy rơi xuống platform thấp hơn, nên có state riêng:

```text
Pursuit
  → mất nền
Falling
  → chạm nền
Land/Recover
  ├─ Player còn hợp lệ → Pursuit
  ├─ quá xa HomePosition → ReturnToPost
  └─ không còn mục tiêu → Patrol
```

Trong `FallingState`:

- Không attack.
- Không liên tục đổi hướng theo Player.
- Để Rigidbody2D xử lý gravity.
- Có thời gian rơi tối đa.
- Kiểm tra nền khi tiếp đất.
- Có xử lý nếu không tìm thấy nền.

Enemy bay nên dùng movement riêng, không dùng chung ground-edge logic.

## 12. Cải thiện detection

Đối với ground enemy, nên tách khoảng cách ngang và dọc:

```text
horizontalDistance = abs(player.x - enemy.x)
verticalDistance = abs(player.y - enemy.y)
```

Điều kiện phát hiện có thể gồm:

```text
horizontalDistance <= visionRange
verticalDistance <= maxDetectHeight
Player nằm phía trước hoặc đang trong vùng phản ứng
Không bị tường che
```

Nên dùng hai ngưỡng khác nhau để tránh đổi state liên tục:

```text
visionRange     : khoảng cách bắt đầu phát hiện
loseTargetRange : khoảng cách mất mục tiêu
loseTargetDelay : thời gian mất mục tiêu
```

Có thể thêm `lastKnownPlayerPosition` và `SearchState` sau này nếu cần behavior tìm kiếm.

## 13. Các hướng thiết kế và trade-off

### Phương án tối thiểu

Thêm:

- `homeX`.
- `maxChaseDistance`.
- `ReturnToPostState`.

Ưu điểm:

- Ít thay đổi.
- Giải quyết enemy đuổi quá xa.
- Không cần viết lại toàn bộ FSM.

Nhược điểm:

- Chưa xử lý mép platform.
- Chưa phân biệt tầng.

### Phương án cân bằng

Thêm:

- Tách `IdleState` và `PatrolState`.
- Hai waypoint patrol.
- `ReturnToPostState`.
- Detection theo X/Y.
- Ground-edge check.

Đây là phương án phù hợp nhất cho enemy mặt đất thông thường.

### Phương án mở rộng

Thêm:

- `Ground`, `Flying`, `Jumping` movement capability.
- `FallingState` và `RecoverState`.
- `SearchState`.
- Encounter/Enemy Registry.
- Movement lock hoặc navigation service riêng.

Ưu điểm là mở rộng tốt cho enemy phức tạp.

Nhược điểm là cần thay đổi nhiều interface, state và dữ liệu prefab.

## 14. Checklist review

### State machine

- `Idle` có nên tách khỏi `Patrol` không?
- Khi mất Player, enemy nên `ReturnToPost` hay `Search`?
- `HurtState` có cần quay lại state trước đó thật sự không?
- Attack có kiểm tra Player còn sống và cùng tầng không?
- Có cần tránh state transition lặp trong cùng frame không?

### Patrol và leash

- Mỗi enemy có HomePosition không?
- Patrol boundary dùng waypoint hay distance?
- `maxChaseDistance` tính từ home hay từ waypoint gần nhất?
- Khi quay về, enemy có được phát hiện Player lại không?
- Có cần giới hạn thời gian chase không?

### Platform và falling

- Enemy nào được phép bước qua mép?
- Enemy nào được phép rơi?
- Enemy rơi xuống tầng thấp có tiếp tục chase không?
- Nếu rơi quá lâu thì Destroy, respawn hay ReturnToPost?
- Có cần ground probe ở chân và phía trước không?

### Detection

- Detection dùng khoảng cách ngang hay Vector2 distance?
- Có vertical tolerance không?
- Có line of sight không?
- Có kiểm tra hướng nhìn không?
- Có thời gian mất mục tiêu không?

### Lifecycle

- `OnDied` chỉ phát một lần chưa?
- `SetHealth(0)` có được phân biệt với chết vĩnh viễn không?
- Có API riêng cho `Kill()` và `Revive()` không?
- Enemy thường có gọi callback spawn loot không?
- Base class có gọi virtual method trước khi class con khởi tạo xong không?

## 15. Hướng triển khai khuyến nghị

Thứ tự thay đổi nên là:

1. Xác định lại semantics của `Idle`, `Patrol`, `Pursuit` và `ReturnToPost`.
2. Thêm HomePosition và MaxChaseDistance.
3. Thêm `ReturnToPostState`.
4. Tách patrol boundary thành hai waypoint nếu level cần kiểm soát chính xác.
5. Bổ sung detection ngang/dọc và thời gian mất mục tiêu.
6. Bổ sung edge detection cho ground enemy.
7. Chỉ thêm `FallingState` cho loại enemy thực sự được phép rơi.
8. Review lại việc base class gọi virtual method trong lúc khởi tạo.
9. Khi có wave spawner hoặc object pool, chuyển từ snapshot event sang Enemy Registry.

## 16. Luồng behavior mục tiêu

```text
Idle
  └─ chờ hoặc quan sát

Patrol
  ├─ đi giữa PatrolLeft và PatrolRight
  ├─ dừng/quay ở biên
  └─ phát hiện Player hợp lệ → Pursuit

Pursuit
  ├─ Player trong attack range → Attack
  ├─ Player mất dấu → Search hoặc ReturnToPost
  ├─ vượt MaxChaseDistance → ReturnToPost
  ├─ gặp mép không được rơi → Turn hoặc ReturnToPost
  └─ mất nền và được phép rơi → Falling

Attack
  ├─ thực hiện animation và damage event
  └─ Player ra ngoài tầm → Pursuit

Hurt
  └─ hết thời gian → Pursuit, Patrol hoặc state trước đó tùy thiết kế

Falling
  └─ chạm nền → Recover, Pursuit hoặc ReturnToPost

ReturnToPost
  └─ về vùng hoạt động → Patrol

Die
  └─ animation kết thúc → Destroy

## 17. Phạm vi áp dụng leash

Leash, dead zone và patrol recovery áp dụng cho toàn bộ enemy thường, tức mọi enemy không phải boss.

Boss giữ nguyên movement và state machine riêng. Không áp dụng logic `ReturnToPost`, khôi phục tọa độ hoặc timeout teleport của enemy thường vào boss.

Việc phân biệt boss phải dựa trên loại controller hoặc capability rõ ràng, không dựa vào tên GameObject. Các controller boss hiện tại có movement/state riêng, đặc biệt boss bay và Duo Golem.

## 18. Thiết kế Leash và Re-engage

### Dữ liệu runtime

Enemy thường cần các dữ liệu sau:

```text
homePosition        : tọa độ cố định lúc enemy spawn
patrolMinX          : biên trái của patrol zone
patrolMaxX          : biên phải của patrol zone
maxChaseDistance    : khoảng cách X tối đa được phép rời home
visionRange         : tầm phát hiện Player, dùng để bắt đầu/duy trì Pursuit
loseTargetDelay     : thời gian Player phải liên tục nằm ngoài visionRange
                       trước khi bị coi là mất dấu
reEngageDistance    : maxChaseDistance - visionRange, clamp về giá trị tối thiểu hợp lệ
homeTolerance       : sai số cho phép khi xác định đã về home
recoveryTimeout     : thời gian tối đa được phép bị kẹt khi quay về,
                       được reset lại mỗi lần enemy vào ReturnToPost
```

`homePosition` không được cập nhật lại sau mỗi lần enemy truy đuổi. `maxChaseDistance` và `reEngageDistance` được tính theo độ lệch tuyệt đối trên trục X so với `homePosition.x`. HP và các dữ liệu combat không bị reset khi enemy được đưa về home.

Patrol zone chỉ xét trục X. `homePosition.y` là độ cao khôi phục mặc định.

`loseTargetDelay` tồn tại để tránh vòng lặp đổi state tương tự dead zone, nhưng xảy ra ở biên `visionRange` thay vì biên `maxChaseDistance`: nếu Player di chuyển lượn ngay rìa tầm nhìn, enemy có thể bị flap liên tục giữa Pursuit và ReturnToPost chỉ vì sai số vị trí rất nhỏ. Player chỉ được coi là mất dấu khi nằm ngoài `visionRange` liên tục qua hết `loseTargetDelay`, không phải ngay khung hình đầu tiên ra khỏi vùng.

### Dead zone

```text
abs(enemy.x - homePosition.x) <= reEngageDistance
	vùng có thể phát hiện và truy đuổi Player

reEngageDistance < abs(enemy.x - homePosition.x) <= maxChaseDistance
	dead zone; không được re-engage

abs(enemy.x - homePosition.x) > maxChaseDistance
	không tiếp tục pursuit; chuyển về ReturnToPost
```

`reEngageDistance` được tính từ khoảng cách leash trừ vision range và phải được clamp để không nhỏ hơn giá trị tối thiểu. Dead zone ngăn vòng lặp enemy vừa quay về vừa nhìn thấy Player rồi lập tức quay lại đuổi.

Toàn bộ quyết định enemy có được re-engage hay không phải dùng chung một rule, ví dụ `ShouldReengage()`. Rule này được dùng bởi `ReturnToPostState` và logic kết thúc `HurtState`.

## 19. Leash state flow

```text
Patrol / Idle
  ├─ Player hợp lệ trong vùng re-engage → Pursuit
  └─ vị trí X vượt maxChaseDistance so với homePosition (do bị đẩy/lệch,
     không liên quan detection) và enemy đang grounded → ReturnToPost

Pursuit
 ├─ Player trong attack range → Attack
 ├─ vượt maxChaseDistance → ReturnToPost
  └─ mất Player liên tục qua loseTargetDelay → ReturnToPost hoặc Search tùy thiết kế

Attack
  ├─ chưa vượt maxChaseDistance → tiếp tục attack hiện tại
  ├─ đã vượt maxChaseDistance → không bắt đầu attack mới
  └─ attack hiện tại kết thúc → ReturnToPost

ReturnToPost
 ├─ đang trong dead zone → tiếp tục về home, bỏ qua detection
  ├─ trong re-engage range và Player hợp lệ → Pursuit
 ├─ X đã vào patrol zone và enemy đang grounded → khôi phục Y về homePosition.y và chuyển Patrol
 ├─ X đã vào patrol zone nhưng chưa grounded → giữ nguyên ReturnToPost,
    để Rigidbody2D rơi tự nhiên, kiểm tra lại grounded mỗi frame
  └─ vượt recoveryTimeout → teleport toàn bộ về homePosition và chuyển Patrol

Hurt
  └─ hết thời gian → đánh giá lại cùng rule re-engage/dead zone,
	 không mặc định chuyển thẳng sang Pursuit
```

Khi `maxChaseDistance` bị vượt, enemy không dùng grace period theo thời gian và không bắt đầu đòn mới chỉ vì Player còn trong attack range. Nếu một attack đã thực sự bắt đầu, attack được phép hoàn tất; sau đó enemy phải ReturnToPost. Điều kiện leash trong `AttackState` phải được kiểm tra mỗi frame trong `OnUpdate()`, không chỉ một lần lúc `OnEnter()`.

Sau khi ReturnToPost đã vào vùng re-engage, Player có thể kéo enemy ra khỏi dead zone lần nữa. Nếu Player vẫn ở gần và hợp lệ, enemy được Pursuit và Attack lại theo behavior bình thường. Điều này áp dụng như nhau bất kể ReturnToPost được vào từ nhánh Pursuit hay từ nhánh Patrol/Idle do bị đẩy lạc vị trí — cả hai dùng chung một `ReturnToPostState`.

### Enemy bị đẩy lạc khỏi patrol zone khi chưa giao chiến

`ReturnToPost` trong bản thiết kế trước chỉ được kích hoạt từ `Pursuit`. Nếu enemy đang `Patrol`/`Idle` (chưa từng phát hiện Player) mà bị đẩy ra ngoài vùng hoạt động bởi một lực bất kỳ ngoài combat hiện có (hazard, moving platform, hoặc cơ chế đẩy được thêm sau này), enemy sẽ tiếp tục patrol tại vị trí sai đó vô thời hạn vì không có đường nào dẫn nó về ReturnToPost.

Để tránh việc này, `Patrol`/`Idle` cần một check độc lập với detection, chạy định kỳ (không cần mỗi frame, có thể throttle 0.2–0.5s):

```text
abs(enemy.x - homePosition.x) > maxChaseDistance
  AND enemy đang grounded
  → chuyển ReturnToPost
```

Dùng `maxChaseDistance` làm ngưỡng, không dùng biên patrol zone, để tránh trigger quá nhạy chỉ vì enemy turn-around tự nhiên ở mép patrol. Điều kiện `grounded` tránh việc chuyển state ngay giữa lúc enemy còn đang bị đẩy bay, trước khi vật lý ổn định lại.

## 20. Recovery khi enemy rơi hoặc lệch độ cao

Đối với enemy thường, khi enemy đang ReturnToPost, tọa độ X đi vào patrol zone, và enemy đang grounded:

```text
giữ nguyên CurrentHealth
reset vận tốc Rigidbody2D
đặt position.y = homePosition.y
đưa enemy về patrol
```

Recovery không reset HP, không hồi sinh enemy đã chết và không reset các giá trị balance/combat không liên quan.

Việc snap Y là chủ đích thiết kế cho các platform lơ lửng tách rời: enemy mặt đất không có khả năng tự đi bộ ngang giữa các platform để quay về home nếu bị rơi hoặc bị đẩy xuống một nền thấp hơn, nên cần teleport thẳng lên độ cao gốc. Vì vậy một enemy có thể rơi từ `homePosition.y = 1` xuống `y = 0`, sau đó khi quay lại đúng vùng X sẽ được đưa trở lại độ cao `y = 1`.

Điều kiện `grounded` chỉ nhằm đảm bảo việc snap Y không xảy ra đúng lúc enemy còn đang rơi tự do giữa không trung (ví dụ vừa bị đẩy khỏi mép platform trung gian, đang free-fall khi X vừa khớp patrol zone). Nếu chưa grounded, ReturnToPost tiếp tục để Rigidbody2D rơi tự nhiên và kiểm tra lại grounded ở frame sau, tránh hiện tượng enemy bị "hút ngược" lên không trung giữa cú rơi.

Level designer phải bảo đảm `homePosition` là vị trí hợp lệ, không nằm trong collider hoặc vật cản. Nếu map phức tạp, có thể bổ sung recovery marker hoặc ground validation trước khi đặt lại Y.

Knockback, Hurt và các nguyên nhân khác cũng có thể làm enemy rời nền. Logic phát hiện airborne/falling không được chỉ gắn với Pursuit; ground enemy cần dùng movement/lifecycle check chung.

## 21. Recovery timeout

`recoveryTimeout` là fail-safe cho enemy bị kẹt, rơi sai vị trí hoặc không thể tự đi về patrol zone.

```text
mỗi lần vào ReturnToPost (bất kể từ Pursuit, Patrol/Idle hay Hurt)
 → reset và bắt đầu lại recovery timer từ đầu

đã vào patrol zone và grounded trước khi hết timeout
 → khôi phục Y, reset velocity, Patrol

hết timeout
 → teleport toàn bộ vị trí (X và Y) về homePosition
 → giữ nguyên HP
  → reset velocity
  → chuyển Patrol
```

Timer được reset về 0 mỗi lần enemy **vào lại** ReturnToPost, kể cả khi trước đó đã từng re-engage rồi mất dấu lại nhiều lần trong cùng một lượt truy đuổi. `recoveryTimeout` chỉ nhằm bắt trường hợp enemy bị kẹt vật lý (rơi sai chỗ, va chạm địa hình, không thể tự đi về), không nhằm giới hạn số lần Player re-engage hợp lệ với enemy — nếu Player liên tục kéo enemy ra rồi để nó tự về đúng luật, đó là hành vi chiến đấu bình thường và không nên bị fail-safe này can thiệp.

Thời gian timeout phải dài hơn thời gian bình thường để enemy đi từ vị trí xa nhất trong leash về patrol zone. Timeout không được dùng để reset HP hoặc tạo lại enemy.

## 22. Chống interrupt và attack event

Enemy không được bị stunlock vô hạn bởi các hit nhỏ. Có thể dùng một trong các rule:

```text
damage nhỏ → chỉ mất HP, không tạo Hurt mới
poise đầy → chuyển Hurt
đòn nặng → được phép phá poise và interrupt
```

Ở mức tối thiểu, khi enemy đang Hurt thì không tạo lại HurtState cho mỗi hit, trừ khi damage đạt ngưỡng stagger đặc biệt.

Animation Event của attack luôn phải đi qua một validation point trước khi gây damage. Validation cần xác nhận:

```text
enemy chưa chết
attack window còn hợp lệ
đòn attack hiện tại chưa bị invalidate
target Player còn tồn tại và còn sống
```

Khi Attack bị interrupt, attack window/token phải được đóng để Animation Event cũ không gây damage ma.

## 23. Tương thích với boss

Leash của enemy thường không được đặt trực tiếp trong `EnemyController.Update()` theo cách bắt buộc mọi subclass phải chạy qua.

Các boss cần giữ nguyên behavior:

- Boss bay có movement/pursuit riêng và không dùng ground recovery.
- Boss có super armor có thể không chuyển sang Hurt như enemy thường.
- Duo Golem dùng `SetHealth(0)` cho trạng thái down tạm thời và có flow Paralyzed/Revival/RealDie riêng.
- Boss có death callback, phase, hazard hoặc attack state riêng không được reset bởi recovery của enemy thường.

Điểm tích hợp phù hợp là một capability hoặc policy của enemy thường, ví dụ `UsesLeash`, được mặc định bật cho enemy thường và tắt/override ở boss. Leash chỉ kiểm soát movement/target boundary; không được tự ý thay đổi HP, phase, hazard, death callback hoặc logic revive của boss.

Một số boss chỉ có một combat state duy nhất: khi đã vào trạng thái chiến đấu thì luôn chủ động tìm Player để tấn công, không patrol, không có khái niệm mất mục tiêu hay quay về vị trí ban đầu. Các boss này không kế thừa hoặc sử dụng `PatrolState`, `IdleState`, `PursuitState` hay `ReturnToPostState` của enemy thường, nên toàn bộ thay đổi ở mục 18–21 (dead zone, loseTargetDelay, gate grounded, recovery, recoveryTimeout) không ảnh hưởng tới nhóm boss này. Điều kiện bắt buộc là boss chỉ được dùng chung các thành phần nền tảng (`Health`, `MovementManager`, khung `EnemyController`), còn state chiến đấu phải được viết riêng, không đi qua các state class thuộc nhóm leash.

## 24. Thứ tự triển khai

```text
1. Xác định homePosition và patrolMinX/patrolMaxX cho enemy thường.
2. Tách Patrol khỏi Idle nếu cần behavior đứng yên riêng.
3. Thêm ReturnToPostState.
4. Gom rule re-engage/dead zone vào một helper duy nhất (ShouldReengage()).
5. Thêm loseTargetDelay để tránh flap ở biên visionRange.
6. Chặn attack mới sau khi vượt leash và cho attack đang chạy hoàn tất;
   check leash trong AttackState phải chạy mỗi frame.
7. Bổ sung IsGrounded() và dùng làm gate cho việc snap Y trong recovery.
8. Thêm recovery theo X, khôi phục Y về homePosition khi grounded.
9. Thêm check định kỳ ở Patrol/Idle để phát hiện enemy bị đẩy lạc khỏi
   maxChaseDistance dù chưa giao chiến, dẫn vào ReturnToPostState.
10. Thêm recoveryTimeout làm fail-safe, reset lại mỗi lần vào ReturnToPost.
11. Bổ sung edge/ground/airborne check dùng chung cho ground enemy.
12. Kiểm tra Hurt, knockback và Animation Event validation.
13. Xác nhận boss không bị áp dụng policy leash ngoài ý muốn — boss dùng
    state chiến đấu riêng (auto tìm Player khi vào combat, không patrol,
    không quên mục tiêu) và không kế thừa Patrol/Idle/Pursuit/ReturnToPost
    của enemy thường.
```