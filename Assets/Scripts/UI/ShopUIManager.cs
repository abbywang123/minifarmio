using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform contentParent;

    public List<ShopItemInfo> itemList;

    public bool isBuyMode = true;

    public void RefreshUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in itemList)
        {
            GameObject go = Instantiate(itemPrefab, contentParent);
            go.GetComponent<ShopItemUI>().Setup(item, isBuyMode);
        }
    }

    public void ShowBuy()
    {
        isBuyMode = true;
        RefreshUI();
    }

    public void ShowSell()
    {
        isBuyMode = false;
        RefreshUI();
    }
}
