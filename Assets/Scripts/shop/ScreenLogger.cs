using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ScreenLogger : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 元件")]
    public TextMeshProUGUI logText;
    public GameObject panelToClose; // ✅ 這是整個 log UI 面板（例如一個含背景的 Panel）

    private Queue<string> logs = new Queue<string>();
    private int maxLines = 10;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;

        // 啟動時預設不顯示
        if (panelToClose != null)
            panelToClose.SetActive(false);
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logs.Count >= maxLines)
            logs.Dequeue();

        logs.Enqueue(logString);
        logText.text = string.Join("\n", logs);

        // ✅ 有訊息時顯示 + 浮到最上層
        if (panelToClose != null)
        {
            panelToClose.SetActive(true);
            panelToClose.transform.SetAsLastSibling(); // 確保在最上層顯示
        }
    }

    // ✅ 點擊任意區域關閉 Log 面板
    public void OnPointerClick(PointerEventData eventData)
    {
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }
    }
}
