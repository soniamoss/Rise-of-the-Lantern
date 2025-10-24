using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Run State")]
    public int levelIndex = 1;     // shown on HUD
    public int coins = 0;          // total run coins
    public int coinsThisLevel = 0; // coins picked in current level

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        coinsThisLevel += amount;
        UIHud.I?.Refresh();
    }

    public void EnterNextLevel(string nextScene)
    {
        levelIndex++;
        coinsThisLevel = 0; // reset per level
        SceneManager.LoadScene(nextScene);
    }
}
