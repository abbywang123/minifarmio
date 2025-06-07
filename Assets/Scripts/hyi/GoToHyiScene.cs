using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GoToHyiScene : MonoBehaviour
{
    [Header("🧩 指定 UI 按鈕")]
    public Button hyiButton;               

    [Header("🎯 場景名稱（必須加入 Build Settings）")]
    public string hyiSceneName = "hyi";    

    void Start()
    {
        if (hyiButton != null)
        {
            Debug.Log($"🟢 hyiButton 註冊中：{hyiButton.name}");
            hyiButton.onClick.RemoveAllListeners(); // 確保不重複註冊
            hyiButton.onClick.AddListener(GoToHyi);
        }
        else
        {
            Debug.LogError("❌ 請在 Inspector 中指定 hyiButton！");
        }
    }

    public void GoToHyi()
    {
        // 防止誤觸 UI 透明區域或被擋住
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("🛡️ 點擊在 UI 上，不執行場景切換");
            return;
        }

        if (string.IsNullOrEmpty(hyiSceneName))
        {
            Debug.LogError("❌ 場景名稱未指定！");
            return;
        }

        Debug.Log("🎯 正在切換到場景：" + hyiSceneName);
        SceneManager.LoadScene(hyiSceneName);
    }
}
