using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoToHyiScene : MonoBehaviour
{
    public Button hyiButton;               // 指定 UI Button
    public string hyiSceneName = "hyi";    // 場景名稱（需在 Build Settings 中加入）

    void Start()
    {
        if (hyiButton != null)
        {
            hyiButton.onClick.AddListener(GoToHyi);
        }
        else
        {
            Debug.LogError("❌ 請在 Inspector 中指定 hyiButton！");
        }
    }

    void GoToHyi()
    {
        Debug.Log("🎯 正在切換到場景：" + hyiSceneName);
        SceneManager.LoadScene(hyiSceneName);
    }
}
