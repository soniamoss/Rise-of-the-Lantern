using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Run State (public for compatibility)")]
    public int lives = 3;
    public int maxLives = 3;
    public int coins = 0;
    public int levelIndex = 1;      
    public bool worldLit = false;   // after Level_04 brighten

    public int Lives => lives;
    public int Coins => coins;
    public int LevelIndex => levelIndex;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelIndex = scene.buildIndex;      // 0-based index tp show actual scene number
        if (levelIndex < 1) levelIndex = 1;

        // Refresh HUD whenever a scene loads
        UIHud.I?.Refresh();

        // If we’re already lit from before, ensure the lantern is full bright
        var lantern = FindObjectOfType<LanternMaskController>();
        if (lantern && worldLit)
        {
            lantern.SetFullBright(true);
        }
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        UIHud.I?.Refresh();
        Debug.Log($"[GameManager] Coins = {coins}");
    }

    public void LoseLife()
    {
        lives--;
        UIHud.I?.Refresh();

        if (lives <= 0)
        {
            // reset entire run
            coins = 0;
            lives = maxLives;
            levelIndex = 1;
            worldLit = false;   // return to darkness on a new run
            SceneManager.LoadScene("Level_01");
            return;
        }

        var player = FindObjectOfType<PlayerController2D>();
        if (player != null) player.Respawn();
    }

    public void EnterNextLevel(string nextSceneName)
    {
        if (!IsSceneInBuild(nextSceneName))
        {
            Debug.LogError($"[GameManager] Scene '{nextSceneName}' not in Build Settings.");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }

    bool IsSceneInBuild(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }

    public void ResetRun()
    {
        coins = 0;
        lives = maxLives;
        levelIndex = 1;
        worldLit = false;
        UIHud.I?.Refresh();
    }
}
