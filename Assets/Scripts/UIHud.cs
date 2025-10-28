using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHud : MonoBehaviour
{
    public static UIHud I;

    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI coinsText;

    void Awake()
    {
        I = this;
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
        Refresh();
    }

    public void Refresh()
    {
        if (GameManager.I == null) return;
        if (livesText) livesText.text = $"Lives {GameManager.I.Lives}";
        if (levelText) levelText.text = $"Level {GameManager.I.LevelIndex}";
        if (coinsText) coinsText.text = $"Coins {GameManager.I.Coins}";
    }
}
