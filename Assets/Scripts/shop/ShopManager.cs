using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public Button shopButton;
    public Transform contentParent;
    public GameObject shopItemUIPrefab;
    public Text playerMoneyText;
    public PlayerInventory playerInventory;

    void Start()
    {
        shopPanel.SetActive(false);
        shopButton.onClick.AddListener(() => shopPanel.SetActive(!shopPanel.activeSelf));
        LoadShopItems();
        UpdateMoneyUI();
    }

    void LoadShopItems()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        ShopItemInfo[] items = Resources.LoadAll<ShopItemInfo>("ShopItems");

        foreach (var item in items)
        {
            GameObject obj = Instantiate(shopItemUIPrefab, contentParent);
            obj.transform.Find("ItemNameText").GetComponent<Text>().text = item.itemName;
            obj.transform.Find("ItemPriceText").GetComponent<Text>().text = $"💰{item.buyPrice}/{item.sellPrice}";
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            var buyBtn = obj.transform.Find("BuyButton").GetComponent<Button>();
            var sellBtn = obj.transform.Find("SellButton").GetComponent<Button>();

            buyBtn.interactable = item.canBuy;
            sellBtn.interactable = item.canSell;

            buyBtn.onClick.AddListener(() =>
            {
                if (playerInventory.BuyItem(item))
                    UpdateMoneyUI();
            });

            sellBtn.onClick.AddListener(() =>
            {
                if (playerInventory.SellItem(item))
                    UpdateMoneyUI();
            });
        }
    }

    void UpdateMoneyUI()
    {
        playerMoneyText.text = $"金錢：{playerInventory.money}";
    }
}
