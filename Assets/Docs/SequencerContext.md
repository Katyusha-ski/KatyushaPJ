# KatyushaPJ — Hướng dẫn lần đầu: Hệ thống Tương tác & Điều phối cốt truyện (Sequencer)

> Đây là tài liệu **từng bước** dành cho người mới lần đầu code hệ thống này.
> Bạn sẽ đọc → làm → dừng ở mỗi **Checkpoint (điểm dừng)** để người giám sát review code của bạn trước khi đi tiếp.

---

## 0. Bạn sắp xây gì? Hiểu bằng hình ảnh

Trước đây, mỗi cái "phim ngắn" trong game (mở rương → Hachi hiện ra → nói chuyện → teleport) phải viết thẳng trong code C#. Muốn sửa phim nào là phải mở script, sửa code, compile — vừa dài vừa dễ vỡ, và mọi thứ dính vào nhau. File `ChestEncounterSequence.cs` cũ là ví dụ.

Hệ thống mới tách thành **3 "đồ chơi" riêng biệt** phối hợp với nhau:

```
   ┌────────────────────┐
   │   KỊCH BẢN (Data)   │  ← Một ScriptableObject (CutsceneData), là "kịch bản" list lệnh
   └─────────┬──────────┘
             │  đọc từng lệnh
             ▼
   ┌────────────────────┐
   │  ĐIỀU PHỐI (Runner) │  ← SequencePlayer, "nhạc trưởng", chỉ biết gọi Execute()
   └─────────┬──────────┘
             │  yêu cầu
      ┌──────▼──────┐
      │   CÁC NGHỆ SĨ (Systems) │  ← DialogueManager, TeleportManager, Animator...
      └─────────────┘
```

**Ba nguyên tắc toán học của cả hệ thống:**

1. **Command Pattern (Mẫu Lệnh):** Mỗi hành động nhỏ (hiện thoại, teleport, animation) được gói vào một class con của `SequenceAction`, và mỗi class **bắt buộc** có hàm `Execute()`. Runner chỉ cần biết mỗi lệnh có `Execute()` — nó không cần biết lệnh đó làm gì bên trong.
2. **Data-Driven (Hướng Dữ Liệu):** Trình tự các lệnh nằm trong **dữ liệu** (CutsceneData ScriptableObject + `[SerializeReference]` list các action-class thuần `[Serializable]`), không nằm trong code. Nhờ đó bạn xếp kịch bản bằng chuột trong Inspector, không gõ code.
3. **Event-Driven (Hướng Sự Kiện):** Dialogue xong → phát loa event `OnDialogueEnded`. Ai cần nghe (Sequencer) mới lắng nghe. Các hệ thống không gọi nhau trực tiếp → "lỏng lẻo" (decoupled).

Thời gian code nên theo thứ tự xây móng trước, nhà sau:
**Phase 1: Dialogue Upgrade → Phase 2: Teleport & Fade → Phase 3: Sequencer (lõi)**.

---

## PHASE 1 — Nâng cấp Hệ thống Hội thoại (nền để sequencer gọi được)

### 1.1 Bối cảnh: vì sao phải đụng Dialogue trước?

Dialogue là "nghệ sĩ" được sequencer gọi nhiều nhất. Trước khi sequencer có thể chờ "hết thoại rồi mới làm tiếp", DialogueManager phải **biết phát loa**. Và một lỗi lặp code trong DialogueUI sẽ làm phần sau khó đọc.

### 1.2 Bước 1 — Dọn code lặp (DRY) trong `DialogueUI.cs`

Mở `Assets/Script/Dialogue/DialogueUI.cs`. Bạn sẽ thấy **2 hàm gần như giống hệt nhau**: `Show(DialogueLine line)` và `UpdateLine(DialogueLine line)` — cùng set tên, set avatar, set text. Đây là vi phạm DRY (Don't Repeat Yourself).

**Cách làm (đúng theo thứ tự để khỏi vỡ):**

1. Thêm một hàm riêng chịu trách nhiệm **duy nhất** là "nhét 1 dòng thoại vào UI":

```csharp
private void SetDialogueUI(DialogueLine line)
{
    if (line == null) return;

    var speaker = line.speaker;
    if (speaker != null)
    {
        if (nameText != null)
            nameText.text = speaker.characterName;

        if (portraitImage != null)
        {
            if (speaker.portrait != null)
            {
                portraitImage.sprite = speaker.portrait;
                portraitImage.enabled = true;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }
    }

    if (lineText != null)
        lineText.text = line.text;
}
```

2. Rút gọn `Show` — chỉ bật panel rồi gọi hàm trên:

```csharp
public void Show(DialogueLine line)
{
    if (panel != null)
        panel.SetActive(true);

    SetDialogueUI(line);
}
```

3. Rút gọn `UpdateLine` — chỉ gọi hàm trên (giữ phép kiểm tra panel như cũ):

```csharp
public void UpdateLine(DialogueLine line)
{
    if (panel == null) return;

    SetDialogueUI(line);
}
```

**Lưu ý cho người mới:**
- Đừng bỏ `if (panel == null) return;` trong `UpdateLine` — nó là phòng thủ.
- `line == null` trong `SetDialogueUI`: hồi thoại lỡ null thì UI không sập. **Người giám sát sẽ xem bạn có guard hay không.**

### 1.3 Bước 2 — Event `OnDialogueEnded` trong `DialogueManager.cs`

**Giải thích "Event" một câu:** Event là cái loa phóng thanh. DialogueManager vừa nói xong thì bấm loa: "Ai quan tâm nghe này, tôi vừa xong cuộc thoại X!". Bất kỳ ai subscribe (đăng ký nghe) sẽ được báo, người không đăng ký thì thôi.

Event `OnDialogueEnded` đã được thêm sẵn cho hệ thống này — bạn **không cần viết mới**, chỉ cần **đọc và hiểu** nó, vì sequencer sẽ dựa vào nó:

```csharp
public event Action<DialogueData> OnDialogueEnded;
```

Và trong hàm `EndDialogue()`:

```csharp
private void EndDialogue()
{
    isDialogueActive = false;
    DialogueData endedData = currentData;   // "vớt" dữ liệu thoại vừa xong TRƯỚC
    currentData = null;
    currentLineIndex = 0;

    OnDialogueEnded?.Invoke(endedData);     // phát loa, kèm DATA của thoại vừa xong

    SetPlayerMovementCanMove(true);
    // ... UI.Hide() ...
}
```

**Điều quan trọng nhất — hiểu CHỮ KÝ (signature):**
- Event mang tham số `DialogueData`. Không phải `Action` rỗng.
- Lý do: khi sequencer mở 2 cuộc thoại liên tiếp, nó cần biết "cái event này là của thoại A hay thoại B" để không chạy nhầm. So khớp bằng `endedData == (thoại của tôi)`.

### ✅ Checkpoint 1 (nghỉ, cho giám sát review)
Trước khi qua Phase 2, nhờ giám sát review: (a) `SetDialogueUI` đã được cả `Show` và `UpdateLine` dùng chung chưa, (b) có guard null, (c) bạn hiểu `?.Invoke(endedData)`.

---

## PHASE 2 — Teleport & Chuyển cảnh Tường thuật (Narrative Fade)

### 2.1 Nó làm gì?

`TeleportManager` chịu trách nhiệm: dời Player bằng Rigidbody2D, kèm hiệu ứng màn hình đen (Fade) + một câu chữ dẫn truyện kiểu *"sau một lúc bay trong không gian..."*. **Chỉ làm 2 việc này thôi** (SRP).

### 2.2 Tạo Canvas UI (trong Unity Editor, không phải code)

1. Tạo **Canvas**: `GameObject > UI > Canvas`.
2. Trên Canvas components, gán một `CanvasScaler` (Scale With Screen Size, chuẩn 1920x1080) và `GraphicRaycaster` nếu chưa có.
3. Tạo con: **FadePanel** — một `Image` màu đen, **mở rộng phủ toàn màn hình** (Strech: anchors 0→1, offsets 0). Cài `Color = (0,0,0,0)` **alpha 0** (trong suốt) khi mới tạo.
4. Tạo con: **LoadingText** — một `TextMeshProUGUI`, đặt giữa màn hình, `alpha = 0`, chữ màu trắng (hoặc xám). Đây là chỗ hiện câu dẫn truyện.
5. Lưu toàn bộ thành **Prefab** `FadeUI`. Trong game, có một object `DontDestroyOnLoad` giữ Canvas này (GameManager đã dùng mẫu này — xem `Awake()` của nó).

### 2.3 Viết `TeleportManager.cs` (Singleton + SRP)

File mới: `Assets/Script/Teleport/TeleportManager.cs`.

**Hiểu 3 khái niệm có trong code dưới đây:**
- **Singleton:** biến `Instance` tĩnh, mọi nơi gọi `TeleportManager.Instance.Teleport(...)` mà không cần phải tìm object. Vì Particle player cần đặt vào màn đen trong lúc dịch chuyển.
- **Coroutine:** hàm `IEnumerator` chạy "từng nấc" — dừng lại khi `yield return`, tiếp tục khi frame sau. Lý tưởng cho chuỗi "tối dần → chờ → dời → sáng dần".
- **rb.position:** nói với Unity Physical engine "đưa vật này đến chỗ (x,y)". Quy tắc bất biến: TUYỆT ĐỐI KHÔNG dùng `transform.position` cho Player — nó bỏ qua physics, gây bug lồng/trượt vật lý (Player đang di chuyển bằng Rigidbody2D).

**Lưu ý về DOTween:** chức năng này cần package `DOTween` (thường ở folder `Assets/Plugins/Demigiant`). Code dùng `image.DOFade(...)`, `text.DOFade(...)`, `SetEase(...)`, `From(...)`... — đây là các hàm tiện của thư viện DOTween tween alpha. Nếu project chưa có DOTween thì báo giám sát trước, đừng tự viết fade bằng tay.

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Image fadePanel;          // ảnh đen toàn màn
    [SerializeField] private TextMeshProUGUI loadingText; // câu "bay trong không gian..."

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float holdSeconds = 1.0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Dịch Player. Nếu loadingMessage rỗng thì chỉ fade, không in chữ.</summary>
    public IEnumerator Teleport(Rigidbody2D playerRb, Vector2 destination, string loadingMessage)
    {
        if (playerRb == null)
        {
            Debug.LogError("[TeleportManager] playerRb null!");
            yield break;
        }

        // 1) Fade đen
        yield return FadeToBlack(fadeDuration);

        // 2) In chữ dẫn truyện (nếu có)
        if (!string.IsNullOrEmpty(loadingMessage))
        {
            loadingText.text = loadingMessage;
            DOTween.Kill(loadingText);                 // phòng khi coroutine cũ chưa tắt
            yield return loadingText.DOFade(1f, fadeDuration).WaitForCompletion();
        }

        // 3) Chờ người đọc câu chữ
        yield return new WaitForSeconds(holdSeconds);

        // 4) DỜI PLAYER — BẮT BUỘC QUA RIGIDBODY
        playerRb.position = destination;               // rb.position, KHÔNG phải transform.position!
        // (Nếu muốn ưu tiên vật lý: playerRb.MovePosition(destination) — nhưng với teleport
        //  súy đột ngột thì rb.position là chuẩn và tiết kiệm tính toán.)

        // 5) Tắt chữ, mở sáng màn hình
        if (!string.IsNullOrEmpty(loadingMessage))
        {
            yield return loadingText.DOFade(0f, fadeDuration).WaitForCompletion();
        }
        yield return FadeFromBlack(fadeDuration);
    }

    private IEnumerator FadeToBlack(float duration)
    {
        DOTween.Kill(fadePanel);
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        yield return fadePanel.DOFade(1f, duration).SetEase(Ease.InQuad).WaitForCompletion();
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        DOTween.Kill(fadePanel);
        yield return fadePanel.DOFade(0f, duration).SetEase(Ease.OutQuad).WaitForCompletion();
    }
}
```

**Điểm giám sát sẽ soi:**
- Dòng dịch chuyển phải là `playerRb.position = ...` hoặc `rb.MovePosition(...)`. **Gặp `transform.position =` trên Player là báo lỗi ngay.**
- `FadePanel`/`LoadingText` phải gán qua SerializeField, không đi kiếm `FindObjectOfType` lung tung.
- Không có logic "đổi BG" hay "gọi Quest" trong đây (SRP).

### 2.4 Đổi Background — KHÔNG nằm trong TeleportManager!

Camera `CameraFollowParallax` (gắn trên BG của OutskirtsScene) **chỉ** parallax scroll, nó **không** làm đổi sprite. Nếu project cần đổi nền khi teleport (đổi ảnh của OutskirtsScene), chúng ta tạo một action riêng của Sequencer (Phase 3) là `BackgroundAction`, hoặc ghi chú thêm sau. **Đừng đem logic đổi ảnh nhét vào `TeleportManager`** — vi phạm SRP.

### ✅ Checkpoint 2 (nghỉ, cho giám sát review)
Trước khi làm Phase 3, nhờ review: (a) `rb.position` đúng chưa, (b) coroutine có `yield break` khi null, (c) DOTween kill trước khi chạy lại (tránh tween bị treo), (d) Singleton Instance đúng mẫu (giống DialogueManager).

---

## PHASE 3 — Data-Driven Sequencer (trái tim hệ thống)

### 3.1 Trước khi viết — NẮM CHẮC 3 khái niệm này

1. **`class` abstract:** class không thể tạo object trực tiếp, chỉ làm "khuôn" để class con kế thừa. Ta cần `SequenceAction` abstract vì mỗi loại lệnh khác nhau (Dialogue, Teleport, Anim...) nhưng đều phải có `Execute()`.
2. **`[SerializeReference]` khác `[SerializeField]` chỗ nào?**
   - `[SerializeField] private DialogueData x;` → Inspector gán **1 kiểu cụ thể** (DialogueData).
   - `[SerializeField][SerializeReference] private SequenceAction x;` → Inspector cho phép **chọn bất kỳ class con nào của SequenceAction** (DialogueAction, TeleportAction...) ngay trong Inspector, và giữ NGUYÊN data khi chuyển. Nhờ attribute này ta gom nhiều loại lệnh vào **một list** `List<SequenceAction>`.
3. **IEnumerator + Coroutine:** mỗi `Execute()` trả về `IEnumerator` để Runner có thể `yield return` — tức "chạy và CHỜ tới khi lệnh hoàn thành rồi mới xử lệnh kế". Nếu trả về `void` thì không chờ được.

### 3.2 Bước 1 — Class gốc `SequenceAction` (abstract)

File mới: `Assets/Script/Sequencer/SequenceAction.cs`.

```csharp
using System.Collections;

[System.Serializable]
public abstract class SequenceAction
{
    public abstract IEnumerator Execute();
}
```

> Ghi chú nhỏ cho người mới: `SequenceAction` là abstract class **thuần `[System.Serializable]`**, KHÔNG kế thừa ScriptableObject. Lý do: `CutsceneData.actions` dùng `[SerializeReference]`, và `[SerializeReference]` KHÔNG hoạt động với kiểu kế thừa `UnityEngine.Object` (ScriptableObject/MonoBehaviour) — Unity sẽ bỏ qua attribute và hiện picker tìm asset thay vì menu chọn loại action. Với class thuần, mỗi action sống trực tiếp bên trong list và có thể chọn loại qua menu ⛭/gear hoặc dropdown type trong Inspector.

### 3.3 Bước 2 — Các lệnh con (Action layer)

#### a) `DialogueAction` — chạy 1 cuộc thoại, ĐỢI xong mới thoát

```csharp
using System.Collections;
using UnityEngine;

public class DialogueAction : SequenceAction
{
    public DialogueData dialogue;

    private bool waiting;

    public override IEnumerator Execute()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("[DialogueAction] dialogue null!"); 
            yield break;                            // null thì thôi, đừng sập
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[DialogueAction] DialogueManager missing!");
            yield break;
        }

        waiting = false;                            // ⚠️ reset TRƯỚC khi gọi
        DialogueManager.Instance.OnDialogueEnded += OnEnded;   // subscribe

        // KHÔNG cần PausePlayer/ResumePlayer — StartDialogue() tự khóa
        // PlayerMovementController.CanMove=false, EndDialogue() tự mở khóa lại.
        DialogueManager.Instance.StartDialogue(dialogue);

        while (!waiting)                            // đợi event báo xong
            yield return null;

        DialogueManager.Instance.OnDialogueEnded -= OnEnded;   // unsubscribe — bắt buộc!
    }

    private void OnEnded(DialogueData ended)
    {
        if (ended == dialogue)   // 1 event có thể cho nhiều thoại, chỉ nhận đúng thoại mình
            waiting = true;
    }
}
```

**Giải thích theo thứ tự bạn phải thuộc lòng (đây là trái tim của toàn hệ thống):**

- **KHÔNG có `PausePlayer`/`ResumePlayer`** — 2 hàm này không tồn tại trong `DialogueManager` và KHÔNG cần thêm. `StartDialogue()` (trong `DialogueManager.cs`) **đã tự** `SetPlayerMovementCanMove(false)` khi bắt đầu thoại, và `EndDialogue()` **đã tự** `SetPlayerMovementCanMove(true)` khi kết thúc. Khóa input là việc của DialogueManager (đã có sẵn), `DialogueAction` chỉ việc gọi `StartDialogue` và chờ event.
- `waiting = false;` **trước** `StartDialogue(...)`: nếu đặt sau, có thể event đã fire xong mà cờ vẫn còn giá trị cũ → thoát nhầm hoặc treo. Đây là fix race-condition, MUST FIX.
- Subscribe `+=` trước khi bắt đầu thoại, **unsubscribe `-=` sau khi xong** (đặt trong `finally` nếu bạn tự tin hơn). Quên unsubscribe = memory leak + event cháy nhiều lần. **Giám sát sẽ bắt lỗi này nếu thấy thiếu.**
- `while (!waiting) yield return null;` — một cách tối giản để "chờ". Có thể thay bằng `WaitUntil(() => waiting)` (dòng chữ tự fluent hơn).
- `ended == dialogue` — so khớp dữ liệu để không nhận nhầm thoại khác.
- **Lưu ý:** sau khi thoại xong, `EndDialogue()` mở khóa `CanMove` NGAY. Nếu sequence cần giữ Player đứng yên thêm (vd chạy `TeleportAction` kế tiếp), thì `TeleportManager`/`SequencePlayer` tự khóa `CanMove` ở tầng của mình — đừng nhét vào DialogueAction (SRP).

#### b) `TeleportAction` — gọi TeleportManager, đợi hết fade

```csharp
using System.Collections;
using UnityEngine;

public class TeleportAction : SequenceAction
{
    public Vector2 destination;
    [TextArea] public string loadingMessage;   // câu "bay trong không gian..."

    public override IEnumerator Execute()
    {
        if (TeleportManager.Instance == null)
        {
            Debug.LogWarning("[TeleportAction] TeleportManager missing!");
            yield break;
        }

        Rigidbody2D playerRb = FindObjectOfType<PlayerMovementController>()?.GetRigidbody();
        if (playerRb == null)
        {
            Debug.LogWarning("[TeleportAction] playerRb not found!");
            yield break;
        }

        yield return TeleportManager.Instance.Teleport(playerRb, destination, loadingMessage);
    }
}
```

**Điểm giám sát soi:** `TeleportAction` **KHÔNG được** tự sơn đen màn hình hay tự `transform.position =`. Nó chỉ "gọi người khác làm" (delegate) và chờ. Đây là biểu hiện của Command Pattern.

#### c) `AnimationAction` — chạy animation cutscene (Rương mở, Hachi...)

```csharp
using System.Collections;
using UnityEngine;

public class AnimationAction : SequenceAction
{
    public GameObject target;                    // object cần trigger anim
    public bool activateOnStart = true;          // bật object trước khi chạy anim
    public string triggerName = "Open";          // tên trigger trên Animator (vd "Open","Appear")
    public float waitSecondsAfterTrigger = 0.1f; // đợi 1 nhịp để anim bắt đầu

    public override IEnumerator Execute()
    {
        if (target == null)
        {
            Debug.LogWarning("[AnimationAction] target null!");
            yield break;
        }

        if (activateOnStart)
            target.SetActive(true);                 // Hachi lúc đầu ẩn, giờ hiện ra

        Animator animator = target.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("[AnimationAction] no Animator on target!");
            yield break;
        }

        animator.SetTrigger(triggerName);           // ✅ FSM: dùng SetTrigger

        yield return null;                          // cho 1 frame để Animator nhận trigger
        yield return new WaitForSeconds(waitSecondsAfterTrigger);
    }
}
```

**Quy tắc vàng FSM:** trong hệ thống này, mọi animation cutscene **chỉ được dùng `animator.SetTrigger("TênTrigger")`**. Nghiêm cấm `animator.Play("StateName")` — `Play()` nhảy thẳng đến State, phá FSM của nhân vật/kẻ địch. **Giám sát sẽ dừng review nếu thấy `Play()`.**

**Giải thích vì sao `yield return null` trước `WaitForSeconds`:** giúp Animator có 1 frame cập nhật trạng thái — đáng để có thói quen đó.

#### d) Shop — một tính năng UI độc lập, không phải lệnh của Sequencer

Shop là **tính năng UI bình thường**, không phải phân cảnh cutscene, nên không có `OpenShopAction`. NPC mở shop sau khi đối thoại xong sẽ dùng một component riêng độc lập, gắn trên chính NPC (vd Usagi):

```csharp
using UnityEngine;

public class ShopKeeperDialogue : MonoBehaviour
{
    [SerializeField] private DialogueData greeting;
    [SerializeField] private ShopUI shopUI;            // ShopUI kế thừa MenuUI

    private void OnEnable()  => DialogueManager.Instance.OnDialogueEnded += OnEnded;
    private void OnDisable() => DialogueManager.Instance.OnDialogueEnded -= OnEnded;

    private void OnEnded(DialogueData ended)
    {
        if (ended != greeting) return;               // đúng thoại chào của mình mới mở
        if (shopUI != null) shopUI.ShowMenuAndPause();   // mở shop + pause game
    }
}
```

**Giải thích tại sao (SRP):** `DialogueManager` chỉ hiện chữ, `ShopKeeperDialogue` chỉ mở shop khi thoại kết thúc, `Sequencer` hoàn toàn không dính líu. `OnDialogueEnded` chính là cơ chế được thêm vào cho đúng case này.

**Về pause:** dùng `ShowMenuAndPause()` (có sẵn trong `MenuUI`) — nó `SetActive(true)` + `GameManager.PauseGame()`. Shop xuất hiện sau khi dọn sạch quái nên pause không ảnh hưởng gameplay, nhưng còn lợi ích phụ: `EndDialogue()` vừa mở khóa `CanMove` cho player, nếu shop không pause thì player vẫn di chuyển được lúc mở shop — trông không tự nhiên. Nếu bạn không muốn pause, thay bằng `shopUI.JustShowMenu()`.

### 3.4 Bước 3 — Container kịch bản `CutsceneData` (Data layer)

File mới: `Assets/Script/Sequencer/CutsceneData.cs`.

```csharp
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCutscene", menuName = "Scriptable Objects/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [SerializeReference] public List<SequenceAction> actions = new();
}
```

**Cách xài trong Inspector (đây là sức mạnh của hệ thống):**
- Tạo asset: *Create > Scriptable Objects > CutsceneData*.
- Trong Inspector, kéo danh sách `actions`, mỗi phần tử bấm vào icon cài răng cưa → chọn loại lệnh: **DialogueAction**, **TeleportAction**, **AnimationAction**... (Shop không nằm trong list này — xem mục 3.3.d.)
- Sắp xếp trật tự tùy ý: Thoại → Anim rương → Thoại → Anim Hachi → Teleport. **Không cần sửa code.** Đó là Data-Driven hoàn chỉnh.

### 3.5 Bước 4 — Runner `SequencePlayer` (Runner layer)

File mới: `Assets/Script/Sequencer/SequencePlayer.cs`. Đây là component gắn trên các Trigger trong game.

```csharp
using System.Collections;
using UnityEngine;

public class SequencePlayer : MonoBehaviour
{
    [SerializeField] private CutsceneData cutscene;

    public event System.Action OnSequenceCompleted;   // hook cuối: cấp lại quyền điều khiển

    public bool IsPlaying { get; private set; }

    public void Play()
    {
        if (IsPlaying || cutscene == null) return;    // chống chạy 2 lần
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        IsPlaying = true;

        foreach (var action in cutscene.actions)
        {
            if (action == null) continue;             // an toàn khi list có ô trống
            yield return StartCoroutine(action.Execute());
            // ↑ tuyệt vời: chờ TỪNG action xong mới qua action kế.
            // Runner KHÔNG cần biết action đó làm gì bên trong (Command Pattern).
        }

        IsPlaying = false;
        OnSequenceCompleted?.Invoke();
    }
}
```

**Truy vết dòng số "có ma thuật":** `yield return StartCoroutine(action.Execute())` — Runner gọi `Execute()` của action 1, chờ nó hoàn toàn chạy xong, rồi vòng `foreach` tự nhảy sang action 2. Đây chính là lý do abstract chỉ cần 1 chữ ký `Execute()`, nhưng hàng trăm loại lệnh đều chạy được.

**Gắn vào Trigger trong game:**
- Tạo 1 `BoxCollider2D` `IsTrigger` (hoặc play trên collision), gắn `SequencePlayer`, kéo `cutscene` asset vào, khi Player đi vào → gọi `GetComponent<SequencePlayer>().Play()` (thường qua 1 script `OnTriggerEnter2D`, hoặc chính SequencePlayer cũng có thể tự có trigger — tùy dự án).

### ✅ Checkpoint 3 (nghỉ, cho giám sát review — QUAN TRỌNG NHẤT)
Trước khi hoàn tất, nhờ review toàn diện: (a) subscribe/unsubscribe có cân bằng không, (b) `waiting` reset trước `StartDialogue`, (c) KHÔNG có `transform.position` trên Player và KHÔNG có `animator.Play()`, (d) Tạo 1 asset CutsceneData mẫu (Thoại → Teleport) và chạy thử trong Editor xem luồng đúng không.

---

## PHASE 4 — Chạy thử & Củng cố

1. Trong Editor: tạo `CutsceneData` mẫu 2 bước — (1) `DialogueAction` nói vài câu, (2) `TeleportAction` dời player kèm câu "vượt không gian..."
2. Kéo vào 1 `SequencePlayer` trên 1 trigger. Vào Play mode, cho Player đi vào trigger.
3. Kỳ vọng: thoại chạy → player đứng yên → hết thoại → màn đen + câu chữ → dời vị trí → mở sáng. Nếu đúng => hệ thống hoạt động.

---

## 5. Ràng buộc Bất di bất dịch (Red Flags — giám sát soi từng commit)

1. **Chỉ can thiệp Rigidbody:** mọi dịch chuyển Player qua `rb.position`/`rb.MovePosition`. Gặp `transform.position =` trên Player → **lỗi review ngay lập tức**.
2. **FSM Animation:** trong `AnimationAction` chỉ `animator.SetTrigger(...)`. Gặp `animator.Play(...)` → **lỗi review ngay**.
3. **SRP:** `DialogueManager` chỉ hiện chữ. `TeleportManager` chỉ fade + dời vị trí. `SequencePlayer` chỉ điều phối. Không gộp logic chéo nhau. `BackgroundAction` (đổi nền) là action riêng, không nằm trong TeleportManager.
4. **Event subscribe/unsubscribe cân bằng:** luôn `-=` khi xong (hoặc trong `finally`), tránh leak và cháy event nhiều lần.
5. **Guard null ở mọi `Execute()`:** `yield break` khi thiếu `dialogue`/`playerRb`/`target`, không để hệ thống sập âm thầm.

---

## Nhắc nhở cuối cùng cho người mới

- Code theo **từng Phase**, dừng ở **checkpoint** và gọi giám sát review trước khi sang Phase kế.
- Đọc lại mục 0 (3 nguyên tắc) mỗi khi lạc lối: **Lệnh gói `Execute()` / Kịch bản là data / Hệ thống giao tiếp bằng event**.
- Có 6 file mới trong Phase 3: `SequenceAction` (abstract), `DialogueAction`, `TeleportAction`, `AnimationAction`, `CutsceneData`, `SequencePlayer` — mỗi file nhỏ, không gộp. (Shop là component riêng `ShopKeeperDialogue` ở mục 3.3.d, không thuộc Sequencer.)

---

## TODO — Trạng thái hiện tại & bước tiếp theo (cập nhật 20/08/2026)

> Commit gần nhất: `c971fe9` "Add Hachi chest intro cutscene flow" (đã push lên `origin/main`).
> **Đang làm dở (chưa commit):** refactor click-pacing — xem mục "Công việc đang dở" bên dưới.

### Bối cảnh đã xong (đừng làm lại)

- **Phase 1 (Dialogue):** `DialogueManager` có event `OnDialogueEnded` (để Sequencer chờ hết thoại); `DialogueUI` đã DRY qua `SetDialogueUI(...)`. Đã commit.
- **Phase 3 (Sequencer):** 6 file core + trigger + custom editor đã commit:
  - `SequenceAction` là class abstract `[System.Serializable]` (**KHÔNG** phải ScriptableObject) vì `[SerializeReference]` không hoạt động trên `UnityEngine.Object` — Inspector vanilla chỉ hiện object-picker, không có dropdown type.
  - Vì vanilla Inspector **không** vẽ dropdown cho danh sách `[SerializeReference]` đa hình, đã viết **custom editor** `Assets/Script/Editor/CutsceneDataDrawer.cs` (`[CustomEditor(typeof(CutsceneData))]`, dùng `TypeCache.GetTypesDerivedFrom<SequenceAction>()` + `managedReferenceValue`).
  - `SequencePlayer` chạy từng action bằng `Execute()` trả về `IEnumerator`; **Red Flags mục 5** bắt buộc tuân thủ (chỉ `rb.position`, chỉ `SetTrigger`, subscribe/unsubscribe cân bằng, guard null `yield break`).
- **Cutscene mở rương Hachi (Ch.1) — đã hoàn tất ở `c971fe9`:**
  - Assets thoại: `Resources/DialogueSO/Dialogues/HachiChest{1..4}_Dialogue.asset`; cutscene: `Resources/DialogueSO/Cutscenes/HachiChestCutscene.asset`.
  - `HachiChest.controller` (Animator) có trigger `Open`; `HachiChest.prefab` gắn thẳng `SequencePlayer` (thay cho trigger test trong scene — OutskirtsScene đã gỡ trigger cũ).
  - Actions mới: `NarrationAction` (fade + chữ dẫn truyện, chờ click), `ShowHachiRevealAction`, `UnlockHachiAction`.
  - `SequenceAction` có field `[NonSerialized] Runner` — `SequencePlayer` inject GameObject đang chạy để action dùng đúng context runtime.
  - Player: thêm trạng thái `hasHachi` — bật/tắt HachiiKat, skill UI, inventory theo tiến trình cutscene.
  - `characterName` đã điền đủ cho 3 CharacterProfile (`Kati`, `Hachi`, `Usagi`). **Portrait vẫn còn trống** (chưa có ảnh).
- **FadeUI + TeleportManager:** đã tách khỏi scene → singleton prefab (`FadeUI.prefab`, `TeleportManager.prefab`, `GameUIRoot.prefab`) nạp qua bootstrap `CoreSystem` (commit `a1e8dfb`, `cbae68f`). Không còn việc "kéo FadeUI vào OutskirtsScene" như todo cũ.

### Công việc đang dở (working tree — 8 file modified, chưa commit)

**Refactor click-pacing:** đưa logic "chờ click" lên Runner thay vì giấu trong từng action.

- `SequenceAction.cs`: thêm field `waitForClick` + virtual `HandlesClickInternally` + method `WaitForClick()`.
- `SequencePlayer.cs`: sau mỗi action, nếu `action.waitForClick && !action.HandlesClickInternally` → `yield return action.WaitForClick()`.
- `DialogueAction.cs`, `NarrationAction.cs`, `TeleportAction.cs`: override `HandlesClickInternally => true` (đã tự xử lý click bên trong, tránh chờ click 2 lần).
- `NarrationAction.cs`: bỏ field `waitForClick` cục bộ + `WaitForClick()` riêng → dùng cơ chế chung của base.
- `HachiChest.prefab`: thêm child `HachiReveal` (SpriteRenderer + Animator) + mở rộng `BoxCollider2D` (m_Offset.y 0.63→1.85, m_Size.y 1.31→3.75).
- `GameUIRoot.prefab`: đảo `m_SortingOrder` giữa 2 Canvas (0↔1).
- `HachiChestCutscene.asset`: fix YAML nhỏ (đóng chuỗi/`{}`).

**Danh sách file đang modified:** `HachiChestCutscene.asset`, `HachiChest.prefab`, `GameUIRoot.prefab`, `DialogueAction.cs`, `NarrationAction.cs`, `SequenceAction.cs`, `SequencePlayer.cs`, `TeleportAction.cs`.

### Các bước tiếp theo (làm theo thứ tự)

1. **Hoàn tất refactor click-pacing (đang dở):**
   - Compile lại trong Unity, mở `HachiChestCutscene.asset` xác nhận các action vẫn giữ nguyên data sau khi `SequenceAction` thêm field mới.
   - Vào Play Mode: chạy `HachiChest` → thoại HachiChest1 → Hachi hiện ra (chờ click) → thoại tiếp → teleport. Kiểm tra click không bị chờ 2 lần, action `waitForClick` dừng đúng nhịp.
2. **Test hitbox + anim rương:** `Open.anim`/`HachiChest.controller` (trigger `Open`), `HachiReveal` hiện đúng lúc, collider mở rộng không cản Player oan.
3. **Commit working tree hiện tại** đúng phạm vi Sequencer (8 file trên). Không kéo theo file lạ.
4. **Wire shop Usagi (todo cũ #3):** gắn `ShopKeeperDialogue` lên NPC Usagi trong scene + tạo `DialogueData` thoại shop; điền portrait cho 3 `CharacterProfile`.
5. **Dựng map OutskirtsScene theo kịch bản Ch.1 (todo cũ #6):** Player + villagers + Usagi shop + chest/Hachi trigger.
6. **Push** sau khi commit xong.

---

## Ghi chú nghiên cứu: Tele qua scene khác + đổi ảnh BG (20/08/2026)

> Đã nghiên cứu xong hiện trạng code, **CHƯA code gì cả** — đây là bản ghi nhớ để làm sau (khi dựng xong 6 scene chapter còn thiếu).

### Hiện trạng

- **TeleportManager** (`Assets/Script/Teleport/TeleportManager.cs:44`) chỉ tele **trong cùng scene**: fade → chữ → click → `playerRB.position = destination` → fade. **Không có `SceneManager` nào.**
- **Load scene** hiện tại toàn bộ qua `GameSceneController.cs` + `ChapterManager.cs` (non-additive, single scene). `ChapterManager` dùng `SceneManager.LoadScene(bossSceneName / mainSceneName)`.
- **7 ChapterDataSO** (`Resources/ChapterSO/`) khai tên scene: `OutskirtsScene, RohokScene, KuriFarmScene, MiraScene, KynariteScene, MytharaScene, HyvoriaScene`. **Chỉ `OutskirtsScene` có file thật** — 6 scene còn lại chưa tồn tại, `EditorBuildSettings` chỉ có 5 scene (MainMenu, Grass, Snow, Stone, Outskirts).
- **BG** tồn tại 2 kiểu:
  - OutskirtsScene: 1 object world-space `BG` (SpriteRenderer) + `CameraFollowParallax` (`Assets/Script/Effect/CameraFollowParallax.cs`) — chỉ parallax scroll, **không swap sprite**.
  - Grass/Snow/StoneScene (test): BG dạng **UI Canvas** (`BG` Canvas + `BG Img`).
  - **Không có** `BackgroundAction`, `ChangeBackground`, `SwapBackground`, `ParallaxBackground` (grep = 0). BG swap là tính năng mới hoàn toàn.
- **FadeUI** là Singleton nhưng nằm **trong `GameUIRoot` đặt theo từng scene** → sau khi load scene khác mà scene đó thiếu `GameUIRoot` thì `FadeUI.Instance == null` → Teleport/Narration lỗi. `TeleportManager` tự nó là persistent (bootstrap từ `CoreSystem`), nhưng phụ thuộc FadeUI không bền.
- **TeleportAction** hiện chỉ có `Vector2 destination` + `loadingMessage` — chưa có field scene.

### Hướng triển khai khi làm (đúng kiến trúc hiện tại)

1. **TeleportManager**: thêm overload `TeleportToScene(sceneName, spawnPoint, loadingMessage)` — fade → chữ → click → `SceneManager.LoadScene(sceneName)` → chờ `SceneManager.sceneLoaded` → tìm lại `Rigidbody2D` của Player (playerRB bị destroy khi unload) → set `rb.position = spawnPoint` → fade sáng.
2. **BackgroundAction** (action mới của Sequencer): field `SpriteRenderer target` + `Sprite bg` + tùy chọn cập nhật `CameraFollowParallax` (sceneStartX/EndX, startOffsetX/endOffsetX). Swap `target.sprite`.
3. **TeleportAction**: thêm field `sceneName` — để trống = tele cùng scene như cũ (không phá asset cũ); có giá trị = gọi nhánh scene.
4. **Tách FadeUI khỏi GameUIRoot** sang persistent root (giống TeleportManager/CoreSystem), hoặc chấp nhận giới hạn: mọi scene target phải có `GameUIRoot`.
5. **Điều kiện tiên quyết**: tạo đủ 6 scene chapter còn thiếu + đưa vào `EditorBuildSettings` trước khi test tele liên scene.
6. **Note liên quan**: `GameManager.cs:159` còn TODO "check theo sceneName thay vì sceneIndex" — đổi cơ chế này trước khi tele theo tên scene hoạt động ổn định với save/load.