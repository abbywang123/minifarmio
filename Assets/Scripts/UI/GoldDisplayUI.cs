using UnityEngine;
using TMPro;

public class GoldDisplayUI : MonoBehaviour
{
    public PlayerWallet playerWallet;
    public TMP_Text goldText;

    void Update()
    {
        if (playerWallet != null && goldText != null)
        {
            goldText.text = $"金幣：{playerWallet.GetGold()}";
        }
    }
}
