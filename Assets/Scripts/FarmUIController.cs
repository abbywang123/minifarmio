using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;


public class FarmUIController : MonoBehaviour
{
    public Button openInventoryButton;

    void Start()
    {
        Debug.Log("🟡 Start() 被執行了");

        if (openInventoryButton == null)
        {
            Debug.LogError("❌ openInventoryButton 沒有被指定！");
        }
        else
        {
            Debug.Log("✅ openInventoryButton 設定正確，準備綁定事件");
            openInventoryButton.onClick.AddListener(OpenInventoryScene);
        }

        // 嘗試強制啟用 Input Actions（新輸入系統用於 UI）
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
    // 測試功能：換顏色或隱藏按鈕
    openInventoryButton.GetComponent<Image>().color = Color.red;

    // 最終功能
    SceneManager.LoadScene("Inventory");
}

    void Update()
    {
            if (Mouse.current.leftButton.wasPressedThisFrame)
{
    if (EventSystem.current.IsPointerOverGameObject())
    {
        Debug.Log("🟠 點到了 UI 元件！");
    }
    else
    {
        Debug.Log("⚪ 點擊畫面沒碰到 UI");
    }
}

        }
    }


