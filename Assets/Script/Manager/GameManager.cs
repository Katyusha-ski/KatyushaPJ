using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Gameplay,
    Pause,
}

public class GameManager : Singleton<GameManager>
{
    public const string UsagiShopChestId = "usagi_shop_storage";

    public GameState CurrentGameState { get; private set; }
    public ChestInventory UsagiShopChest { get; private set; }

    [Header("Game state")]
    private float playTime = 0f;
    private SaveData tempSaveData;

    protected override void OnSingletonAwake()
    {
        UsagiShopChest = new ChestInventory(UsagiShopChestId);
        SceneManager.sceneLoaded += OnAnySceneLoaded;
    }

    protected override void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnAnySceneLoaded;
        base.OnDestroy();
    }

    private void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireCameraFollowToPlayer();
        RestoreGameplayUI(scene);
        SetPlayerActiveForScene(scene);
    }

    private void WireCameraFollowToPlayer()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerTransform == null)
            return;

        CameraController cameraFollow = FindFirstObjectByType<CameraController>();
        if (cameraFollow == null) return;

        cameraFollow.SetTarget(PlayerManager.Instance.PlayerTransform);
    }

    private void RestoreGameplayUI(Scene scene)
    {
        // Menu đã tắt HUD (ResetForMainMenu): vào scene chơi thì bật lại.
        if (scene.name == "MainMenuScene") return;
        if (UIManager.Instance != null)
            UIManager.Instance.SetGameplayUIActive(true);
    }

    private void SetPlayerActiveForScene(Scene scene)
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerTransform == null)
            return;
        bool gameplay = scene.name != "MainMenuScene" && scene.name != "CreditsScene";
        PlayerManager.Instance.PlayerTransform.gameObject.SetActive(gameplay);
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Gameplay)
        {
            playTime += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        CurrentGameState = GameState.Pause;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        CurrentGameState = GameState.Gameplay;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Save all game state (called from UI or auto save)
    /// </summary>
    public void SaveGame()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance was not found!");
            return;
        } 

        Scene currentScene = SceneManager.GetActiveScene();

        // Get player state
        int playerHealth = 0;
        bool hasHachi = false;
        Vector3 playerPosition = Vector3.zero;

        if (PlayerManager.Instance != null)
        {
            if (PlayerManager.Instance.PlayerHealth != null)
                playerHealth = PlayerManager.Instance.PlayerHealth.CurrentHealth;

            if (PlayerManager.Instance.PlayerController != null)
                hasHachi = PlayerManager.Instance.PlayerController.HasHachi;

            if (PlayerManager.Instance.PlayerTransform != null)
                playerPosition = PlayerManager.Instance.PlayerTransform.position;
        }
        else
        {
            Debug.LogError("PlayerManager was not found!");
            return;
        }

        List<SerializableShopEntry> shopData = new List<SerializableShopEntry>();
        ShopManager shop = ShopManager.Instance;
        if (shop != null)
            shop.GetSerializableData(shopData);

        List<ChestInventorySave> chestData = new List<ChestInventorySave>
        {
            new ChestInventorySave(
                UsagiShopChest.chestId,
                UsagiShopChest.GetSerializableItems()
            )
        };

        int chapterNumber = ChapterManager.Instance != null ? ChapterManager.Instance.CurrentChapterNumber : 0;

        var skillMatrix2D = Inventory.Instance.GetSerializableSkillUnlocked();

        SaveData data = new SaveData
        {
            currentChapter = chapterNumber,
            // Inventory data
            inventoryItem = Inventory.Instance.GetSerializableInventory(),
            equipmentItem = Inventory.Instance.GetSerializableEquipment(),
            skillUnlocked = skillMatrix2D,
            questItems = Inventory.Instance.GetSerializableQuestItems(),
            // Scene info
            currentSceneIndex = currentScene.buildIndex,
            currentSceneName = currentScene.name,
            // Player state
            playerHealth = playerHealth,
            hasHachi = hasHachi,
            playerPositionX = playerPosition.x,
            playerPositionY = playerPosition.y,
            playerPositionZ = playerPosition.z,
            // Shop data
            shopEntries = shopData,
            // Chest data
            chestInventories = chestData,
            // Scene state (quái chết, đồ ẩn/hiện, trigger đã chạy)
            sceneStates = SceneStateTracker.ExportStates(),
            // Metadata
            saveDataTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            playTime = this.playTime
        };

        SaveManager.SaveGame(data);
        Debug.Log("Game saved successfully!");
    }

    /// <summary>
    /// Load game and switch to saved scene
    /// </summary>
    public void LoadGame()
    {
        // Load save data
        SaveData saveData = SaveManager.LoadGame();

        // Check if save data is valid
        if (saveData == null)
        {
            Debug.LogError("No valid save data found!");
            return;
        }

        // Load game state
        if (ChapterManager.Instance != null)
            ChapterManager.Instance.SetChapter(saveData.currentChapter);
        this.playTime = saveData.playTime;

        // Store save data temporarily
        tempSaveData = saveData;

        // Load-file: bỏ nháp RAM của tracker để tiến trình sau lần save
        // (quái giết, trigger fire) không kẹt lại. Refresh khi scene mới
        // load sẽ merge file vào RAM trống = đúng trạng thái file.
        SceneStateTracker.ClearAllStates();

        // Load the saved scene
        // TODO: sau này build map xong thì check theo sceneName thay vì sceneIndex
        if (saveData.currentSceneIndex > 0)
        {
            Debug.Log($"Loading scene: {saveData.currentSceneName} (Index: {saveData.currentSceneIndex})");

            // Register to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoadedAfterLoadGame;

            // Load scene
            SceneManager.LoadScene(saveData.currentSceneIndex);
        }
        else
        {
            Debug.LogError("Invalid scene index in save data!");
        }
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame()
    {
        SaveManager.DeleteSave();

        if (ChapterManager.Instance != null)
            ChapterManager.Instance.SetChapter(1);
        playTime = 0f;

        if (Inventory.Instance != null)
        {
            Inventory.Instance.ClearInventory();
        }

        PlayerManager.Instance?.GetComponent<PlayerSkillManager>()?.ReloadSkills();

        UsagiShopChest?.Clear();

        SceneStateTracker.ClearAllStates();

        tempSaveData = SaveData.Default();
        SceneManager.sceneLoaded += OnNewGameSceneLoaded;

        Debug.Log("New game started!");

        if (ChapterManager.Instance != null)
            ChapterManager.Instance.GoToMainScene();
    }

    private void OnNewGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnNewGameSceneLoaded;
        Invoke(nameof(RestorePlayerState), 0.2f);
    }

    /// <summary>
    /// Check if save file exists
    /// </summary>
    public bool HasSaveFile()
    {
        return SaveManager.HasSaveFile();
    }

    /// <summary>
    /// Callback after scene is loaded - restore inventory and player
    /// </summary>
    private void OnSceneLoadedAfterLoadGame(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe to prevent multiple calls
        SceneManager.sceneLoaded -= OnSceneLoadedAfterLoadGame;

        if (tempSaveData == null) return;

        // Load inventory after scene is ready
        if (Inventory.Instance != null)
        {
            Inventory.Instance.LoadSerializableInventory(tempSaveData.inventoryItem);
            Inventory.Instance.LoadSerializableEquipment(tempSaveData.equipmentItem);
            Inventory.Instance.LoadSerializableSkillUnlocked(tempSaveData.skillUnlocked);
            PlayerManager.Instance?.GetComponent<PlayerSkillManager>()?.ReloadSkills();
            Inventory.Instance.LoadSerializableQuestItems(tempSaveData.questItems);
            Debug.Log("Inventory, equipment, skills and quest items restored!");
        }
        else
        {
            Debug.LogWarning("Inventory instance not found after scene load!");
        }

        // Restore shop data
        ShopManager shop = ShopManager.Instance;
        if (shop != null)
            shop.LoadSerializableData(tempSaveData.shopEntries);

        ChestInventorySave savedChest = null;
        if (tempSaveData.chestInventories != null)
        {
            savedChest = tempSaveData.chestInventories.Find(
                chest => chest != null && chest.chestId == UsagiShopChestId
            );
        }

        UsagiShopChest.LoadSerializableItems(savedChest?.items);

        // Nạp scene state rồi apply ngay cho scene vừa load: bỏ nháp RAM,
        // chép lại đúng file (thay vì union cộng dồn) rồi mới refresh.
        SceneStateTracker.ClearAllStates();
        SceneStateTracker.ImportStates(tempSaveData.sceneStates);
        SceneStateTracker.RefreshForScene(scene);

        // Restore player state with delay (ensure player is spawned)
        Invoke(nameof(RestorePlayerState), 0.2f);

        Debug.Log($"Game loaded! Chapter: {ChapterManager.Instance.CurrentChapterNumber}, Play time: {playTime:F1}s");
    }

    /// <summary>
    /// Restore player state from save data
    /// </summary>
    private void RestorePlayerState()
    {
        if (tempSaveData == null) return;

        // Find player
        if (PlayerManager.Instance == null)  // ← Kiểm tra PlayerManager
        {
            Debug.LogError("PlayerManager not found!");
            return;
        }

       
            // Restore position
            Vector3 savedPosition = new Vector3(
                tempSaveData.playerPositionX,
                tempSaveData.playerPositionY,
                tempSaveData.playerPositionZ
            );
            if (PlayerManager.Instance.PlayerTransform != null)
                PlayerManager.Instance.PlayerTransform.position = savedPosition;

            if (PlayerManager.Instance.PlayerHealth != null)
                PlayerManager.Instance.PlayerHealth.Revive(tempSaveData.playerHealth);

            if (PlayerManager.Instance.PlayerController != null)
                PlayerManager.Instance.PlayerController.SetHachiAppeared(tempSaveData.hasHachi);
        

        tempSaveData = null; 
    }

    /// <summary>
    /// Get save info for UI display
    /// </summary>
    public SaveData GetSaveInfo()
    {
        return SaveManager.LoadGame();
    }
}
