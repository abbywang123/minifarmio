using UnityEngine;
using UnityEngine.UI;

public class ButtonTestController : MonoBehaviour
{
    public Button testButton;

    void Start()
    {
        Debug.Log("🟡 測試場景開始");

        if (testButton != null)
        {
            testButton.onClick.AddListener(() =>
            {
                Debug.Log("✅ 測試按鈕被點擊！");
            });
        }
        else
        {
            Debug.LogError("❌ testButton 沒有被指定！");
        }
    }
}
