using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FarmSceneManager : MonoBehaviour
{
    public void GoToInventory()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Inventory");
    }

    public void GoToShop()
    {
        SceneManager.LoadScene("Shop"); // 不需刷新 UI
    }

    public void ReturnToFarm()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Farm");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Inventory")
        {
            Debug.Log("✅ 進入 Inventory，準備刷新 UI");
            InventoryManager.Instance?.StartCoroutine(DelayedRefresh());
        }
        else if (scene.name == "Farm")
        {
            Debug.Log("🌿 回到 Farm，重新同步雲端資料");
            _ = InventoryManager.Instance?.ReloadFarmDataFromCloud();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator DelayedRefresh()
    {
        yield return null; // 等一幀，保證 UI 元件都載入
        InventoryManager.Instance?.RefreshInventoryUI();
    }
}
