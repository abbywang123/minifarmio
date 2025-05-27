using TMPro;
using UnityEngine;

public class GoldDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    void Start()
    {
        if (PlayerWallet.Instance != null)
        {
            // 先顯示初始金額
            goldText.text = $"Gold: {PlayerWallet.Instance.CurrentMoney}";

            // 訂閱事件
            PlayerWallet.Instance.OnMoneyChanged += UpdateGoldText;
        }
    }

    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnMoneyChanged -= UpdateGoldText;
    }

    private void UpdateGoldText(int newAmount)
    {
        goldText.text = $"Gold: {newAmount}";
    }
}
