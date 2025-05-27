using UnityEngine;
using Unity.Netcode;

public class ShopMultipleManager : MonoBehaviour
{
    /// <summary>
    /// 通用購買邏輯（由按鈕呼叫）
    /// </summary>
    public void OnBuyButtonClicked(string itemId)
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            var player = NetworkManager.Singleton.LocalClient.PlayerObject;
            var inventory = player.GetComponent<PlayerInventorySync>();

            if (inventory != null && inventory.IsOwner)
            {
                inventory.BuyItemServerRpc(itemId);
            }
        }
        else
        {
            Debug.LogWarning("🛒 非連線狀態，請勿使用多人商店！");
        }
    }

    // ✅ 提供給 UI Button 綁定的公開方法（Unity OnClick 不支援傳參數）
    public void BuyCarrot() => OnBuyButtonClicked("carrot");
    public void BuyWheat() => OnBuyButtonClicked("wheat");
    public void BuyTomato() => OnBuyButtonClicked("tomato");
}

