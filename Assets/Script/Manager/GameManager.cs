using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Gameplay,
    Pause,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; }

    [Header("Game state")]
    private float playTime = 0f;
    private SaveData tempSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        Vector3 playerPosition = Vector3.zero;

        if (PlayerManager.Instance != null)
        {
            if (PlayerManager.Instance.PlayerHealth != null)
                playerHealth = PlayerManager.Instance.PlayerHealth.CurrentHealth;

            if (PlayerManager.Instance.PlayerTransform != null)
                playerPosition = PlayerManager.Instance.PlayerTransform.position;
        }
        else
        {
            Debug.LogError("PlayerManager was not found!");
            return;
        }

        List<SerializableShopEntry> shopData = new List<SerializableShopEntry>();
        ShopManager shop = FindFirstObjectByType<ShopManager>();
        if (shop != null)
            shop.GetSerializableData(shopData);

        int chapterNumber = ChapterManager.Instance != null ? ChapterManager.Instance.CurrentChapterNumber : 0;

        SaveData data = new SaveData
        {
            currentChapter = chapterNumber,
            // Inventory data
            inventoryItem = Inventory.Instance.GetSerializableInventory(),
            equipmentItem = Inventory.Instance.GetSerializableEquipment(),
            skillMatrix = Inventory.Instance.GetSerializableSkillMatrix(),
            // Scene info
            currentSceneIndex = currentScene.buildIndex,
            currentSceneName = currentScene.name,
            // Player state
            playerHealth = playerHealth,
            playerPositionX = playerPosition.x,
            playerPositionY = playerPosition.y,
            playerPositionZ = playerPosition.z,
            // Shop data
            shopEntries = shopData,
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

        tempSaveData = SaveData.Default();
        SceneManager.sceneLoaded += OnNewGameSceneLoaded;

        Debug.Log("New game started!");

        SceneManager.LoadScene("Village");
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
            Inventory.Instance.LoadSerializableSkillMatrix(tempSaveData.skillMatrix);
            Debug.Log("Inventory, equipment and skills restored!");
        }
        else
        {
            Debug.LogWarning("Inventory instance not found after scene load!");
        }

        // Restore shop data
        ShopManager shop = FindFirstObjectByType<ShopManager>();
        if (shop != null)
            shop.LoadSerializableData(tempSaveData.shopEntries);

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
                PlayerManager.Instance.PlayerHealth.SetHealth(tempSaveData.playerHealth);
        

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
