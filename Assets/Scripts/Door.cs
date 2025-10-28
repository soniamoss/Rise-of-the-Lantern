using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] string nextSceneName = "Level_02";
    [SerializeField] int coinsRequired = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Door] Trigger by '{other.name}' (tag={other.tag})");
        if (!other.CompareTag("Player")) return;

        if (GameManager.I == null) { Debug.LogError("[Door] No GameManager."); return; }

        int have = GameManager.I.Coins;
        Debug.Log($"[Door] Coins={have}, Required={coinsRequired}");

        if (have < coinsRequired) { Debug.Log("[Door] Not enough coins."); return; }

        if (!IsSceneInBuild(nextSceneName))
        {
            Debug.LogError($"[Door] Scene '{nextSceneName}' not in Build Settings."); return;
        }

        Debug.Log($"[Door] Loading '{nextSceneName}'");
        SceneManager.LoadScene(nextSceneName);
    }

    bool IsSceneInBuild(string name)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (sceneName == name) return true;
        }
        return false;
    }
}
