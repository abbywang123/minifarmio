using UnityEngine;
using System.Collections;

public class InventorySceneManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayRefresh());
    }

    IEnumerator DelayRefresh()
    {
        // 嘗試最多 5 次，每次間隔 0.1 秒，確保 InventoryManager 準備好
        const int maxAttempts = 5;
        const float interval = 0.1f;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.RefreshInventoryUI();
                Debug.Log("📦 InventorySceneManager：成功刷新背包 UI");
                yield break;
            }

            Debug.Log($"🔄 第 {i + 1} 次等待 InventoryManager 載入中...");
            yield return new WaitForSeconds(interval);
        }

        Debug.LogWarning("❌ 嘗試多次後仍找不到 InventoryManager 實例");
    }
}


