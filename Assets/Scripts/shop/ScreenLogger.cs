using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ScreenLogger : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 元件")]
    public TextMeshProUGUI logText;
    public GameObject panelToClose; // ✅ 要控制顯示/隱藏的面板（建議是背景整個 log panel）

    private Queue<string> logs = new Queue<string>();
    private int maxLines = 10;

    void Awake()
    {
        // 一開始隱藏面板
        if (panelToClose != null)
            panelToClose.SetActive(false);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 限制最多顯示的行數
        if (logs.Count >= maxLines)
            logs.Dequeue();

        logs.Enqueue(logString);
        logText.text = string.Join("\n", logs);

        // 有 log 時自動顯示面板
        if (panelToClose != null && !panelToClose.activeSelf)
            panelToClose.SetActive(true);
    }

    // 點擊面板任意位置關閉 log 視窗
    public void OnPointerClick(PointerEventData eventData)
    {
        if (panelToClose != null)
            panelToClose.SetActive(false);
    }
}
