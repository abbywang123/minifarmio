using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // 🚀 載入多個 additive 場景
        LoadAdditiveScene("hyi");
        LoadAdditiveScene("SHOP");
        LoadAdditiveScene("weather data");
        LoadAdditiveScene("CropInfoPPanel");
    }

    void LoadAdditiveScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Debug.Log($"✅ 已載入 Additive 場景：{sceneName}");
        }
    }

    public void GoToFarm()
    {
        // 🚜 切換到 Farm 主場景（非 additive）
        SceneManager.LoadScene("Farm");
    }
}
