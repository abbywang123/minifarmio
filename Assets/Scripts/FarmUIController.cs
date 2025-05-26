using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FarmUIController : MonoBehaviour
{
    public Button openInventoryButton; // 把按鈕拉進這裡

    void Start()
    {
        openInventoryButton.onClick.AddListener(OpenInventoryScene);
    }

    public void OpenInventoryScene()
    {
        Debug.Log("✅ 點擊成功：切換到背包場景");
        SceneManager.LoadScene("Iventory");
    }
}

