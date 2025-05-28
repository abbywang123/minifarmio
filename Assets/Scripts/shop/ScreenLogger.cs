using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class ScreenLogger : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI logText;
    public GameObject panelToClose;

    private Regex chineseRegex = new Regex(@"[\u4e00-\u9fff]"); // 判斷是否含有中文

    void Start()
    {
        if (panelToClose != null)
            panelToClose.SetActive(false); // ✅ 一開始就隱藏面板
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
        if (!chineseRegex.IsMatch(logString))
            return;

        logText.text = logString;

        if (panelToClose != null && !panelToClose.activeSelf)
            panelToClose.SetActive(true); // ✅ 有中文字才顯示
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (panelToClose != null)
            panelToClose.SetActive(false); // ✅ 點擊隱藏
    }
}
