using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopUIController1 : MonoBehaviour
{
    public GameObject openShopButtonObject;

    void Start()
    {
        if (openShopButtonObject != null)
        {
            Button btn = openShopButtonObject.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OpenShopScene);
            }
            else
            {
                Debug.LogError("❌ 找不到 Button 元件");
            }
        }
        else
        {
            Debug.LogError("❌ openShopButtonObject 未設定");
        }
    }

    void OpenShopScene()
    {
        SceneManager.LoadScene("SHOP"); // 這個名字要和你的場景名稱一致
    }
}
