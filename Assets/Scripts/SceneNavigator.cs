using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class SceneNavigator : MonoBehaviour
{
    public Button backButton;
    public string farmSceneName = "FarmScene";
    public TMP_Text saveStatusText;
    public InventoryManager inventoryManager;

    void Start()
    {
        saveStatusText.gameObject.SetActive(false);
        backButton.onClick.AddListener(() => _ = SaveAndReturn());
    }

    async Task SaveAndReturn()
    {
        // ✅ 儲存 Cloud Save 資料
        FarmData farmData = await CloudSaveAPI.LoadFarmData();
        farmData.inventory = inventoryManager.GetInventoryData();
        await CloudSaveAPI.SaveFarmData(farmData);

        // ✅ 顯示提示
        saveStatusText.text = "儲存成功！返回農場中...";
        saveStatusText.gameObject.SetActive(true);

        await Task.Delay(1500); // 等待 1.5 秒

        SceneManager.LoadScene(farmSceneName);
    }
}
