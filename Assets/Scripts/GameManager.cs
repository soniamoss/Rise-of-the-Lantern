using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Run State")]
    public int levelIndex = 1;     // shown on HUD
    public int coins = 0;          // total run coins
    public int coinsThisLevel = 0; // coins picked in current level
    public int lives = 3;          // number of lives
    public int maxLives = 3;       

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
    public void LoseLife()
    {
        lives--;
        UIHud.I?.Refresh();

        if (lives <= 0)
        {
            // restart whole run
            lives = maxLives;
            coins = 0;
            levelIndex = 1;
            coinsThisLevel = 0;
            SceneManager.LoadScene("Level_01");
        }
        else
        {
            var player = FindObjectOfType<PlayerController2D>();
            if (player)
            {
                player.Respawn(); 
            }
        }

    }

    public void GainLife(int amount = 1)
    {
        lives = Mathf.Min(maxLives, lives + amount);
        UIHud.I?.Refresh();
    }


    public void EnterNextLevel(string nextScene)
    {
        levelIndex++;
        coinsThisLevel = 0; // reset per level
        SceneManager.LoadScene(nextScene);
    }
}
