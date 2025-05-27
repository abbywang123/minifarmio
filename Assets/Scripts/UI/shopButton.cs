using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    public GameObject shopPanel;        // 商店面板，預設設為 inactive
    public Button shopToggleButton;     // 開關商店的按鈕

    void Start()
    {
        shopPanel.SetActive(false);     // 開始時隱藏商店

        // 按鈕綁定事件，點擊切換商店顯示/隱藏
        shopToggleButton.onClick.AddListener(() =>
        {
            bool isActive = shopPanel.activeSelf;
            shopPanel.SetActive(!isActive);
        });
    }
}
