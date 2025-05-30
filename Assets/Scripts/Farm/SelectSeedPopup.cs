using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectSeedPopup : MonoBehaviour
{
    public Transform gridParent;
    public GameObject slotPrefab;
    FarmUIManager manager;

    void Awake() => manager = FindFirstObjectByType<FarmUIManager>();

    public void Show(FarmTile tile)
    {
        gameObject.SetActive(true);

        // 清除舊有按鈕
        foreach (Transform c in gridParent)
            Destroy(c.gameObject);

        // 使用正確的方法取得背包資料
        foreach (var item in InventoryManager.Instance.GetInventoryData())
        {
            // 👉 如果只要顯示「種子」，可加條件：
            if (!item.itemId.ToLower().Contains("seed")) continue;

            var go = Instantiate(slotPrefab, gridParent);
            go.GetComponentInChildren<TMP_Text>().text = item.itemId;

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                manager.PlantSelected(item.itemId);
            });
        }
    }

    void Start() { }

    void Update() { }
}
