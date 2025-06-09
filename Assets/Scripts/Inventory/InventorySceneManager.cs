using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class InventorySceneManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayRefresh());
    }

    IEnumerator DelayRefresh()
    {
        yield return new WaitForSeconds(0.3f); // ✅ 等 UI 都載入好

        if (InventoryManager.Instance != null)
        {
            Debug.Log("🔁 嘗試重新從雲端載入背包資料");
            Task task = InventoryManager.Instance.ReloadFarmDataFromCloud();
            yield return new WaitUntil(() => task.IsCompleted);
        }
        else
        {
            Debug.LogError("❌ InventoryManager.Instance 為 null，請確認場景有掛該物件");
        }
    }
}



