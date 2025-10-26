using UnityEngine;
using TMPro;

public class UIHud : MonoBehaviour
{
    public static UIHud I;

    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text livesText;   


    void Awake()
    {
        I = this;
        Refresh();
    }

    public void Refresh()
    {
        if (GameManager.I == null) return;
        if (coinsText) coinsText.text = $"Coins: {GameManager.I.coins}";
        if (levelText) levelText.text = $"Level {GameManager.I.levelIndex}";
        if (livesText) livesText.text = $"Lives: {GameManager.I.lives}";

    }
}
