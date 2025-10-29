## ROADMAP: Unity + Firebase - Chuẩn Bị Portfolio Intern 🎯

**Mục tiêu:** Có portfolio trong 3 tháng (12 tuần) để apply intern Unity Developer tại các công ty ở Việt Nam (VNG, Amanotes, Sky Mavis, startups...).

**Timeline tổng quan:** 12 tuần (2-3 giờ/ngày trong tuần, 5-6 giờ/ngày cuối tuần). Budget = $0 (chỉ dùng tiers miễn phí).

**Target:** Vị trí intern Unity ở Việt Nam (startup & mid/large companies).

---

## Mục Lục

- [1. HEADER & MỤC TIÊU](#header--mục-tiêu)
- [2. MỤC LỤC (Table of Contents)](#mục-lục-table-of-contents)
- [3. GIAI ĐOẠN 1: HOÀN THIỆN PROJECT HIỆN TẠI (Tuần 1-4)](#giai-đoạn-1-hoàn-thiện-project-hiện-tại-tuần-1-4)
  - [Tuần 1-2: Clean up KatyushaPJ](#tuần-1-2-clean-up-katyushapj)
  - [Tuần 3-4: Hoàn thiện Save System JSON](#tuần-3-4-hoàn-thiện-save-system-json)
- [4. GIAI ĐOẠN 2: DESIGN PATTERNS CƠ BẢN (Tuần 5-6)](#giai-đoạn-2-design-patterns-cơ-bản-tuần-5-6)
- [5. GIAI ĐOẠN 3: FIREBASE INTEGRATION (Tuần 7-9)](#giai-đoạn-3-firebase-integration-tuần-7-9)
- [6. GIAI ĐOẠN 4: PORTFOLIO & DEPLOYMENT (Tuần 10-11)](#giai-đoạn-4-portfolio--deployment-tuần-10-11)
- [7. GIAI ĐOẠN 5: INTERVIEW PREPARATION (Tuần 12)](#giai-đoạn-5-interview-preparation-tuần-12)
- [8. APPENDIX: TÀI NGUYÊN HỌC TẬP](#appendix-tài-nguyên-học-tập)
- [9. CHECKLIST TRƯỚC KHI APPLY](#checklist-trước-khi-apply)
- [10. JOB APPLICATION STRATEGY](#job-application-strategy)

---

## 1. HEADER & MỤC TIÊU

### Tiêu đề
**ROADMAP: Unity + Firebase - Chuẩn Bị Portfolio Intern** 🎯

### Mục tiêu (SMART)
- Specific: Hoàn thành 1 project Unity (KatyushaPJ) với integration Firebase, demo video, deployed WebGL, và GitHub portfolio.
- Measurable: 12 tuần, từng tuần có deliverable (video, build, tính năng hoạt động).
- Achievable: 2-3h/ngày (weekday), 5-6h/ngày (weekend).
- Relevant: Tập trung kỹ năng cần cho intern (Unity core, patterns, Firebase, deploy, Git).
- Time-bound: Hoàn thành trong 12 tuần.

### Timeline tổng quan: 12 tuần
- Tuần 1-4: Clean up & hoàn thiện Save System
- Tuần 5-6: Design patterns cơ bản
- Tuần 7-9: Firebase integration (Auth, Database, Storage, Analytics)
- Tuần 10-11: Portfolio, deployment, demo video
- Tuần 12: Interview prep

---

## 2. MỤC LỤC (Table of Contents)

Xem mục lục đầu file (links anchor). Dùng anchor để nhảy nhanh đến từng phần.

---

## 3. GIAI ĐOẠN 1: HOÀN THIỆN PROJECT HIỆN TẠI (Tuần 1-4)

### Tuần 1-2: Clean up KatyushaPJ ✅

Mục tiêu cụ thể (SMART): Trong 2 tuần, làm cho repository sẵn sàng public và có hướng dẫn rõ ràng để người khác (recruiter/peer) chạy project trong 30 phút.

Thời gian ước tính: 2-3h/ngày weekdays, 5h mỗi ngày cuối tuần.

Deliverables:
- README.md hoàn chỉnh
- .gitignore chuẩn cho Unity
- Folder structure rõ ràng, code cleaned (no dead code)

Checklist:
- [ ] Viết README chính (mục tiêu, cách build/run, dependency)
- [ ] Thêm .gitignore cho Unity
- [ ] Code cleanup: remove unused using, tối giản public fields
- [ ] Đặt scenes trong thư mục `Assets/Scenes` và ghi rõ scene chính

Template README.md (mẫu) 📝

```markdown
# KatyushaPJ

> Short description: game/tech demo for portfolio.

## Yêu cầu
- Unity 2020.3+ (LTS) hoặc phiên bản tương thích

## Build & Run
1. Open Unity Hub -> Add project -> open
2. Open scene `Assets/Scenes/Main.unity`
3. Play

## WebGL build
- See `Assets/Script/HuongDan/` for WebGL tips

## Contact
- Your Name - email@example.com
```

.gitignore cho Unity (mẫu):

```text
# ===============
# Unity generated
# ===============
Library/
Temp/
Obj/
Build/
Builds/
MemoryCaptures/
UserSettings/

# OS files
*.DS_Store
*.Thumbs.db

# Visual Studio
.vs/
*.csproj
*.sln
*.user
```

Tips: commit small, descriptive commits. Use branches for features (feature/save-system).

---

### Tuần 3-4: Hoàn thiện Save System JSON ✅

Mục tiêu cụ thể: Triển khai Save/Load JSON đúng theo `HUONG_DAN_SAVE_LOAD_SYSTEM.md`, có test, demo video 2-3 phút, và upload WebGL build lên itch.io.

Thời gian: 2-3h/ngày weekdays, 5-6h cuối tuần.

Deliverables:
- Save/Load working with JSON
- Tests (manual checklist)
- Demo video (2-3 phút script)
- WebGL build uploaded (itch.io) và URL

Testing checklist:
- [ ] Save creates a JSON file (localPlayerPrefs or file in persistentDataPath)
- [ ] Load restores player state exactly
- [ ] Corrupted JSON handling (recover gracefully)
- [ ] Cross-platform path handling (Editor, Windows, WebGL fallback)

Demo video script (mẫu 2-3 phút):

> 0:00-0:10 — Intro: tên, mục tiêu demo
> 0:10-0:40 — Show gameplay & features
> 0:40-1:30 — Show Save: make changes, press Save, show JSON file briefly
> 1:30-2:10 — Show Load: restart scene, press Load -> state restored
> 2:10-2:30 — Conclusion: link GitHub & itch.io

Hướng dẫn upload itch.io WebGL build:
1. Build Settings -> Platform = WebGL -> Build
2. zip thư mục Build
3. Tạo tài khoản itch.io -> Create new project -> Kind: HTML
4. Upload zip -> Set to unlisted nếu muốn
5. Test trên trình duyệt

Fallback nếu WebGL gặp vấn đề: upload Windows build + video demo.

---

## 4. GIAI ĐOẠN 2: DESIGN PATTERNS CƠ BẢN (Tuần 5-6)

Mục tiêu: Nắm 3 pattern cơ bản (Singleton, Object Pool, Observer) và áp dụng vào KatyushaPJ.

Thời gian: 2-3h/ngày weekdays, 5-6h cuối tuần.

Deliverables:
- Implement Singleton cho managers
- Implement BulletPool, VFXPool
- Replace direct coupling bằng event system
- Mini-benchmarks cho object pooling

### Singleton Pattern

Tại sao cần? ✅
- Quản lý global state (GameManager, AudioManager) dễ truy cập.

Khi nào dùng?
- Manager thực sự là singleton, tồn tại xuyên scene và chỉ một instance.

Lưu ý (pitfalls): ⚠️
- Lạm dụng Singleton gây tight-coupling, khó test.
- Không dùng cho mọi class; prefer dependency injection cho code lớn.

Interview answer (ngắn):
> "Singleton đảm bảo một instance duy nhất. Trong Unity thường dùng cho Manager cần truy cập global. Cẩn thận race condition và lifecycle khi dùng DontDestroyOnLoad."

Code example (Unity C#):

```csharp
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }
            }
            return _instance;
        }
    }
    
    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else if (_instance != this) Destroy(gameObject);
    }
}

// Usage
public class GameManager : Singleton<GameManager>
{
    public int Score;
}
```

Comments: hỗ trợ DontDestroyOnLoad và tránh duplicate instance. Test: create two GameManager in scene -> second sẽ bị destroy.

### Object Pool Pattern

Motivation: giảm GC và chi phí Instantiate/Destroy khi spawn nhiều object (bullets, VFX).

When to use: bullets, particle effects, UI popups, enemies spawn/respawn.

Pitfalls: quản lý size, memory leak nếu không return object.

Benchmarks: đo FPS/GC before/after (capture 60s with and without pooling). Mục tiêu: giảm spikes GC và tăng frame stability.

Code example: BulletPool

```csharp
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialSize = 50;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var b = Instantiate(bulletPrefab);
            b.SetActive(false);
            pool.Enqueue(b);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0) obj = pool.Dequeue();
        else obj = Instantiate(bulletPrefab);

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

// Bullet script should call BulletPool.Instance.Return(this.gameObject) on disable/collision
```

Benchmarking tips:
- Use Unity Profiler -> Record -> simulate 100 bullets spawn/return -> compare GC.alloc and frame times.

### Observer Pattern (Event System)

Motivation: Giảm coupling bằng event-driven architecture.

UnityEvent vs C# event comparison:

| Aspect | UnityEvent | C# event |
|---|---:|---|
| Serialized in Inspector | ✅ | ❌ |
| Runtime performance | Slightly slower | Faster |
| Type safety | Less | More |
| Use-case | Designer wiring | Code-only decoupling |

Code example: Health system with events

```csharp
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHP = 100;
    public int CurrentHP { get; private set; }

    public event Action<int> OnHealthChanged; // C# event

    void Start() => CurrentHP = MaxHP;

    public void TakeDamage(int dmg)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - dmg);
        OnHealthChanged?.Invoke(CurrentHP);
        if (CurrentHP == 0) Die();
    }

    void Die()
    {
        // publish death event / handle logic
        Debug.Log("Dead");
    }
}

// Subscriber example
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    void OnEnable() => playerHealth.OnHealthChanged += UpdateBar;
    void OnDisable() => playerHealth.OnHealthChanged -= UpdateBar;
    void UpdateBar(int hp) { /* update UI */ }
}
```

Bài tập áp dụng vào KatyushaPJ:
- Replace direct calls between player and UI with events
- Pool bullets and VFX
- Create GameManager singleton

---

## 5. GIAI ĐOẠN 3: FIREBASE INTEGRATION (Tuần 7-9) 🚀

Mục tiêu: Tích hợp Firebase core features: Auth, Realtime Database (save/load), Cloud Storage, Analytics, Remote Config.

Thời gian: 2-3h/ngày weekdays, 6h cuối tuần.

Deliverables:
- Firebase project + Unity SDK cài đặt
- Auth (Anonymous + Email)
- Save/Load to Realtime DB
- Upload screenshot to Cloud Storage
- Analytics event

Lưu ý: chỉ dùng free tier. Không upload secrets vào Git.

### Tuần 7: Setup & Authentication

Steps (mô tả từng bước — bạn nên chụp screenshot từng bước):
1. Vào https://console.firebase.google.com -> Create project -> đặt tên
2. Add app -> Web/Android/iOS (Unity thường dùng Web/Android) — lưu config file (google-services.json cho Android, GoogleService-Info.plist cho iOS)
3. Download Firebase Unity SDK (từ https://firebase.google.com/download/unity) và import `.unitypackage` vào Unity Editor
4. Trong Unity: Window -> Package Manager -> kiểm tra dependency (play-services-resolver)
5. Trong Firebase Console -> Authentication -> Sign-in method -> enable Anonymous & Email/Password

Code example: Anonymous + Email/Password (async/await)

```csharp
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;

    async void Start()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            return;
        }
        auth = FirebaseAuth.DefaultInstance;
    }

    public async Task<FirebaseUser> SignInAnonymouslyAsync()
    {
        try
        {
            var userCredential = await auth.SignInAnonymouslyAsync();
            Debug.Log($"Signed in anonymously: {userCredential.User.UserId}");
            return userCredential.User;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Anonymous sign-in failed: {e}");
            return null;
        }
    }

    public async Task<FirebaseUser> SignInWithEmailAsync(string email, string password)
    {
        try
        {
            var cred = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"Signed in: {cred.User.Email}");
            return cred.User;
        }
        catch (FirebaseException fe)
        {
            Debug.LogError($"Email sign-in failed: {fe.Message}");
            return null;
        }
    }
}
```

Notes: check Firebase Unity support for async/await version. Use Unity 2018+ or appropriate SDK.

### Tuần 8: Realtime Database - Save/Load

Database structure design (example):

```
/users/{uid}/profile
/users/{uid}/saves/{saveId}
/leaderboards/global/{scoreId}
```

Code example SaveToFirebase / LoadFromFirebase (async/await)

```csharp
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

[System.Serializable]
public class PlayerProfile
{
    public string uid;
    public string name;
    public int level;
}

public class FirebaseDatabaseManager : MonoBehaviour
{
    private DatabaseReference rootRef;

    void Start() => rootRef = FirebaseDatabase.DefaultInstance.RootReference;

    public async Task SaveProfileAsync(PlayerProfile profile)
    {
        string json = JsonUtility.ToJson(profile);
        try
        {
            await rootRef.Child("users").Child(profile.uid).Child("profile").SetRawJsonValueAsync(json);
            Debug.Log("Profile saved");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e}");
        }
    }

    public async Task<PlayerProfile> LoadProfileAsync(string uid)
    {
        try
        {
            var snapshot = await rootRef.Child("users").Child(uid).Child("profile").GetValueAsync();
            if (snapshot.Exists)
            {
                var json = snapshot.GetRawJsonValue();
                var profile = JsonUtility.FromJson<PlayerProfile>(json);
                return profile;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e}");
        }
        return null;
    }
}
```

Error handling & offline mode:
- Use `FirebaseDatabase.DefaultInstance.GoOffline()` / `GoOnline()` for testing offline behavior.
- Catch exceptions, retry with exponential backoff.

Testing checklist:
- [ ] Save then reload -> identical state
- [ ] Simulate network loss -> app queues writes when back online
- [ ] Security rules: read/write restricted to authenticated users for `/users/{uid}`

### Tuần 9: Advanced Features

- Cloud Storage: upload screenshots/replays

```csharp
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public class FirebaseStorageManager : MonoBehaviour
{
    private Firebase.Storage.FirebaseStorage storage;

    void Start() => storage = Firebase.Storage.FirebaseStorage.DefaultInstance;

    public async Task<string> UploadBytesAsync(byte[] bytes, string path)
    {
        var storageRef = storage.GetReference(path);
        try
        {
            var metadata = await storageRef.PutBytesAsync(bytes);
            string url = await storageRef.GetDownloadUrlAsync();
            Debug.Log($"Uploaded: {url}");
            return url;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Upload failed: {e}");
            return null;
        }
    }
}
```

- Firebase Analytics: log event on level complete
- Remote Config: fetch and apply tuning parameters
- Leaderboard sample: push score under `/leaderboards/global`

Code examples for these topics should follow similar patterns (use SDK docs). Ensure you don't push service account keys to Git.

---

## 6. GIAI ĐOẠN 4: PORTFOLIO & DEPLOYMENT (Tuần 10-11)

Mục tiêu: Public portfolio trên GitHub + itch.io với demo video và docs.

Thời gian: 2-3h/ngày weekdays, 6h cuối tuần.

Deliverables:
- GitHub repo đẹp (profile README, pinned)
- At least 1 deployed WebGL (itch.io) + demo video
- LinkedIn cập nhật

### GitHub Portfolio

Profile README template (ngắn):

```markdown
# Your Name

Software/Game Developer • Unity • C#

## Projects
- KatyushaPJ — Unity + Firebase — [itch.io link] | [repo link]

## Contact
- Email: your.email@example.com
```

Commit best practices: small, atomic commits, use PR even cho feature branches.

Screenshot guidelines:
- 16:9 screenshots, 1280x720 or 1920x1080
- Show UI, gameplay, and WebGL link

### Demo Video

Script template chi tiết (5 phần):
1. Intro 10s (name, role you want)
2. Problem & What you built 20s
3. Live demo 2-3 minutes
4. Technical deep-dive 1-2 minutes (arch, key code snippet)
5. How to run & links 20s

Recording tools:
- OBS (free) for screen capture
- Unity Recorder for high-quality clips

Editing tips:
- Keep it concise, add captions, highlight interactions
- Export 1080p H264

### itch.io Deployment: WebGL build settings

Optimization tips:
- Reduce build size: strip unused assets, use Addressables
- Compression: gzip/brotli on server (itch.io handles this)

Game page setup: description, screenshots, tags, social links, set to public/unlisted.

### LinkedIn Profile

Headline template: "Unity Developer Intern • C# • Firebase • Game Dev Portfolio"

About section: 2-3 paragraphs about what you build, tech stack, link to demo.

Project entries: add repo link, one-line summary, screenshot.

---

## 7. GIAI ĐOẠN 5: INTERVIEW PREPARATION (Tuần 12) 🧭

Mục tiêu: Chuẩn bị trả lời technical & behavioral, practice pitch và demo.

Thời gian: 2-3h/ngày weekdays, mock interviews cuối tuần 5-6h.

### Technical Questions

20 câu Unity phổ biến + đáp án (tóm tắt): include topics: Update vs FixedUpdate, memory management, script lifecycle, prefab vs instance, coroutines, physics layers, UI batching, Addressables, ECS intro, Profiler.

10 câu Design Patterns: Singleton, Factory, Object Pool, State, Observer, Command, MVC, Dependency Injection — include short answers and when to use.

5 câu Firebase: auth flow, security rules, offline persistence, Realtime vs Firestore, Cloud Storage usage.

Code challenges thường gặp: implement object pool, implement save/load JSON, simple leaderboard sort.

### Behavioral Questions

STAR method giải thích (Situation, Task, Action, Result) — chuẩn bị 5 stories.

Sample stories (prepare 5):
- Tối ưu hóa performance
- Fix bug lớn
- Làm feature từ A-Z
- Làm việc nhóm / conflict
- Học tool mới nhanh

Questions to ask interviewer (10 câu hay): team size, tech stack, intern expectations, mentorship, project examples, next steps.

### Portfolio Presentation

- Pitch 2 phút: who you are, what you built, impact
- Demo walkthrough 5 phút: show core mechanics, save/load, firebase features
- Technical deep-dive 10 phút: show code snippet (pooling or firebase save) and explain decisions

---

## 8. APPENDIX: TÀI NGUYÊN HỌC TẬP 📚

<details>
<summary>Firebase</summary>

- Official docs: https://firebase.google.com/docs
- Unity SDK: https://firebase.google.com/download/unity
- YouTube: Firebase Channel, Fireship.io
- Sample projects: Firebase Unity quickstarts on GitHub
</details>

<details>
<summary>Unity Patterns & Learning</summary>

- Game Programming Patterns (book): https://gameprogrammingpatterns.com/
- Unity Learn: https://learn.unity.com/
- YouTube channels: Brackeys (archive), Jason Weimann, GameDevTV
- GitHub repos: Unity-Technologies samples
</details>

<details>
<summary>Interview Prep</summary>

- LeetCode for algorithm basics (focus on problem solving)
- Glassdoor / LinkedIn for company-specific experiences
- Reddit r/gamedev for community insights
</details>

---

## 9. CHECKLIST TRƯỚC KHI APPLY ✅

- [ ] GitHub profile hoàn chỉnh
- [ ] Minimum 1 project với demo video
- [ ] Firebase integration working
- [ ] Resume 1 trang
- [ ] LinkedIn updated
- [ ] Chuẩn bị 5 STAR stories
- [ ] Practice 20 technical questions
- [ ] Deployed WebGL URL

---

## 10. JOB APPLICATION STRATEGY

Target companies (VN) — ví dụ 12 công ty:

1. VNG
2. Amanotes
3. Sky Mavis
4. Glass Egg
5. VTC
6. TopeBox
7. Gameloft (VN)
8. KardiaGames
9. Appota
10. Table7 Studios
11. Small local startups
12. Indie studios

Timeline apply (gợi ý week-by-week):
- Week 1-4: polish project + GitHub
- Week 5-8: add Firebase & deploy
- Week 9-10: make demo video, prepare resume
- Week 11-12: apply & interview practice

Email template xin intern:

> Subject: [Intern Application] Unity Developer Intern — {Your Name}

> Hi {Hiring Manager Name},
>
> I'm {Your Name}, a 3rd-year student passionate about Unity game development. I built *KatyushaPJ* (Unity + Firebase) demonstrating save/load, cloud features and WebGL deployment. You can find my demo here: {itch.io link} and source: {GitHub link}.
>
> I'm applying for an internship role at {Company}. I'm available from {start date} and eager to learn/work on {areas}. Attached is my resume. Thank you for your time.
>
> Best,
> {Your Name} — {email} — {phone}

Follow-up strategy:
- If no reply in 7 days -> polite follow-up (short)
- If rejected -> ask for feedback and iterate

Rejection handling & iteration:
- Keep a log (company, date, reason)
- Improve one thing per rejection (resume, demo, code quality)

---

## FORMAT & STYLE GUIDELINES (cho sinh viên)

- Tone: friendly, practical, action-oriented.
- Time budgeting: realistic (2-3h weekdays, 5-6h weekends).
- Budget: $0 only free tiers.
- Focus: deliverables > theory. Breadth > depth (show variety of skills).

> **Quan trọng:** Đảm bảo mỗi deliverable đo được (demo video link, deployed URL, PR, commit history).

---

## QUICK START (Ngày 0)

1. Fork/clone repo
2. Open Unity project
3. Run scene `Assets/Scenes/Main.unity`
4. Follow Week 1 checklist: update README, add .gitignore, make a small commit

---

## FALLBACK PLAN nếu bị stuck 🛟

- Issue: Firebase config fails -> Fallback: store saves in local persistentDataPath and record demo video showing local save. Mark Firebase as bonus feature.
- Issue: WebGL build too large -> Fallback: provide Windows standalone + video + hosted screenshots.

---

## FINISH LINE — WHAT “DONE” LOOKS LIKE ✅

- Public GitHub repo with clear README and commit history
- Deployed WebGL on itch.io with working demo
- Demo video (YouTube unlisted) linked in README
- Firebase features (auth + save/load + storage) working in build
- One-page resume + LinkedIn updated

---

## RESOURCES & LINKS (important)

- Firebase docs: https://firebase.google.com/docs
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- Game Programming Patterns: https://gameprogrammingpatterns.com/

> Nếu bạn muốn, tôi có thể tiếp theo: 1) thêm checklist cụ thể vào `Issues` trên GitHub, 2) tạo template PR & issue, 3) generate demo video storyboard chi tiết cho project.

---

**Chúc bạn may mắn — bắt đầu từ hôm nay, commit nhỏ, ship thường xuyên, và giữ momentum!** 🚀
