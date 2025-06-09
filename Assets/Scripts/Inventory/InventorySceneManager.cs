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
        yield return new WaitForSeconds(0.3f); // ✅ 等待 UI 載入

        if (InventoryManager.Instance != null)
        {
            Debug.Log("🔁 從雲端載入背包資料...");
            // 改用協程
            yield return StartCoroutine(InventoryManager.Instance.ReloadFarmDataFromCloudCoroutine());
        }
        else
        {
            Debug.LogError("❌ InventoryManager.Instance 為 null，請確認場景有掛該物件");
        }
    }
}
