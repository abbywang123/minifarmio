using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToHyiScene : MonoBehaviour
{
    [Header("🎯 場景名稱（必須加入 Build Settings）")]
    public string hyiSceneName = "hyi";

    public void GoToHyi()
    {
        if (string.IsNullOrEmpty(hyiSceneName))
        {
            Debug.LogError("❌ 場景名稱未指定！");
            return;
        }

        Debug.Log("🎯 正在切換到場景：" + hyiSceneName);
        SceneManager.LoadScene(hyiSceneName);
    }
}
