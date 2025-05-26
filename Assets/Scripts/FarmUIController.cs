using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FarmUIController : MonoBehaviour
{
    public Button openInventoryButton;

    void Start()
    {
        Debug.Log("🟡 Start() 被執行了"); // 👉 加入這行確認 Start 是否有執行
        openInventoryButton.onClick.AddListener(OpenInventoryScene);
    }

    public void OpenInventoryScene()
    {
        Debug.Log("✅ 點擊成功：切換到背包場景");
        SceneManager.LoadScene("Inventory");
    }
}


