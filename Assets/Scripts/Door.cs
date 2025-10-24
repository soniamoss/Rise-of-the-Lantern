using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] int requiredCoins = 1;
    [SerializeField] string nextSceneName = "Level_02";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.I && GameManager.I.coinsThisLevel >= requiredCoins)
        {
            GameManager.I.EnterNextLevel(nextSceneName);
        }
        else
        {
            Debug.Log($"Door locked: need {requiredCoins}, have {GameManager.I?.coinsThisLevel ?? 0}");
            // TODO: show a small UI prompt near the door
        }
    }

    // Optional: visualize requirement in Scene view
    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
            $"Requires {requiredCoins} coin(s)");
    }
}
