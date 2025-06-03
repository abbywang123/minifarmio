using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoToShopScene : MonoBehaviour
{
    public Button shopButton;           // 指定 UI Button
    public string shopSceneName = "SHOP";  // 場景名稱（需在 Build Settings 中加入）

    void Start()
    {
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(GoToShop);
        }
        else
        {
            Debug.LogError("❌ 請在 Inspector 中指定 shopButton！");
        }
    }

    void GoToShop()
    {
        Debug.Log("🛒 切換到場景：" + shopSceneName);
        SceneManager.LoadScene(shopSceneName);
    }
}
