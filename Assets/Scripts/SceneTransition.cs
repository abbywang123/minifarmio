using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneTransition : MonoBehaviour
{
    [Header("這個傳送門的 ID")]
    public string portalId;

    // 定義傳送對應表： key = 當前場景 + 傳送門 ID，value = 目標場景與 spawn 點
    private static readonly Dictionary<string, (string scene, string spawn)> portalMap = new Dictionary<string, (string, string)>
    {
        // Map 傳送門
        { "Map_ToForest", ("Forest", "SpawnPoint_Forest") },
        { "Map_ToFarm", ("Farm", "SpawnPoint_Farm") },
        { "Map_ToMountain", ("Mountain", "SpawnPoint_Mountain") },
        { "Map_ToPond", ("Pond", "SpawnPoint_Pond") },

        // Forest 傳送門
        { "Forest_ToMagicForest", ("MagicForest", "SpawnPoint_MagicForest") },

        // Mountain 傳送門
        { "Mountain_ToSand", ("Sand", "SpawnPoint_Sand") },
        { "Mountain_ToRiver", ("River", "SpawnPoint_River") },
        { "Mountain_ToCanyon", ("Canyon", "SpawnPoint_Canyon") }
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        string currentScene = SceneManager.GetActiveScene().name;
        string key = $"{currentScene}_{portalId}";

        if (!portalMap.TryGetValue(key, out var target))
        {
            Debug.LogWarning($"找不到傳送目標：場景 {currentScene}, 傳送門 ID {portalId}");
            return;
        }

        // 設定出生點
        SpawnManager.Instance?.SetSpawnPoint(target.spawn);

        // 載入新場景
        SceneManager.LoadScene(target.scene);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(portalId))
        {
            Debug.LogWarning($"[SceneTransition] 傳送門未設定 portalId，物件名稱：{gameObject.name}");
        }
    }
#endif
}
