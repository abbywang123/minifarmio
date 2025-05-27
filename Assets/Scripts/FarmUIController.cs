using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class FarmUIController : MonoBehaviour
{
    public GameObject openInventoryButtonObject; // ✅ GameObject 類型

    void Start()
    {
        Debug.Log("🟡 Start() 被執行了");

        // 取出 Button 元件並綁定事件
        var btn = openInventoryButtonObject.GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("❌ 無法在 openInventoryButtonObject 上取得 Button 元件！");
        }
        else
        {
            btn.onClick.AddListener(OpenInventoryScene);
            Debug.Log("✅ Button 組件成功取得並綁定事件！");
        }

        // 啟用 Input Actions（新輸入系統）
        var inputModule = EventSystem.current?.GetComponent<InputSystemUIInputModule>();
        if (inputModule != null && inputModule.actionsAsset != null)
        {
            inputModule.actionsAsset.Enable();
            Debug.Log("✅ 已強制啟用 Input Actions 資產：" + inputModule.actionsAsset.name);
        }
        else
        {
            Debug.LogWarning("⚠️ 無法啟用 Input Actions：未找到 InputSystemUIInputModule 或 ActionsAsset");
        }
    }

    public void OpenInventoryScene()
    {
        Debug.Log("✅ 點擊成功：切換到背包場景");

        // 額外測試行為：改變顏色（可刪）
        openInventoryButtonObject.GetComponent<Image>().color = Color.red;

        // 切換場景
        SceneManager.LoadScene("Inventory");
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                Debug.Log("🟠 點到了 UI 元件！");
            else
                Debug.Log("⚪ 點擊畫面沒碰到 UI");
        }
    }
}


