using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class HybridInfo
{
    public string hybridName;
    public string parentA;
    public string parentB;

    public HybridInfo(string hybridName, string parentA, string parentB)
    {
        this.hybridName = hybridName;
        this.parentA = parentA;
        this.parentB = parentB;
    }
}

public class BreedingUIManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject breedingPanel;
    public Button breedingOpenButton;
    public Button closeButton;
    public Button breedButton;

    public TMP_Dropdown hybridDropdown;
    public TMP_Dropdown quantityDropdown;

    public TextMeshProUGUI parentAText;
    public TextMeshProUGUI parentBText;

    private List<HybridInfo> hybridList = new List<HybridInfo>
    {
        new HybridInfo("星星草莓", "草莓", "香菇"),
        new HybridInfo("火焰玉米", "玉米", "番茄"),
        new HybridInfo("魂之蘿蔔", "蘿蔔", "香菇"),
        new HybridInfo("幻影仙人掌", "仙人掌", "草莓"),
        new HybridInfo("泡泡蘑菇", "馬鈴薯", "蘿蔔"),
        new HybridInfo("笑笑蘿蔔", "蘿蔔", "南瓜")
    };

    private void Start()
    {
        breedingPanel.SetActive(true);

        closeButton.onClick.AddListener(() =>
        {
            Debug.Log("🌾 返回 Farm 場景");
            SceneManager.LoadScene("Farm");
        });

        breedButton.onClick.AddListener(OnBreedButtonClicked);
        hybridDropdown.onValueChanged.AddListener(UpdateParentTexts);

        SetupDropdowns();
        UpdateParentTexts(0);
    }

    private void SetupDropdowns()
    {
        hybridDropdown.ClearOptions();
        quantityDropdown.ClearOptions();

        List<string> hybridNames = new List<string>();
        foreach (var hybrid in hybridList)
            hybridNames.Add(hybrid.hybridName);
        hybridDropdown.AddOptions(hybridNames);

        List<string> quantities = new List<string> { "1", "2", "3", "4", "5" };
        quantityDropdown.AddOptions(quantities);
    }

    private void UpdateParentTexts(int index)
    {
        var hybrid = hybridList[index];
        parentAText.text = $"親代 A：{hybrid.parentA}";
        parentBText.text = $"親代 B：{hybrid.parentB}";
    }

    private async void OnBreedButtonClicked()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ 無法取得 InventoryManager 實例");
            return;
        }

        int index = hybridDropdown.value;
        int quantity = quantityDropdown.value + 1;

        var hybrid = hybridList[index];

        // 載入 ItemData（需要有正確命名的 ScriptableObject 資源）
        ItemData parentA = Resources.Load<ItemData>("Items/" + hybrid.parentA);
        ItemData parentB = Resources.Load<ItemData>("Items/" + hybrid.parentB);
        ItemData seed = Resources.Load<ItemData>("Items/" + hybrid.hybridName + "種子");

        if (parentA == null || parentB == null || seed == null)
        {
            Debug.LogWarning("❌ 找不到對應的物品（請確認 Resources/Items 下的資產命名是否正確）");
            return;
        }

        // 從 InventoryManager 中取得數量
        int haveA = CountOf(parentA.id);
        int haveB = CountOf(parentB.id);

        if (haveA < quantity || haveB < quantity)
        {
            Debug.Log("❌ 材料不足，請檢查背包中是否有足夠的作物！");
            return;
        }

        // 非同步移除素材
        bool removedA = await InventoryManager.Instance.RemoveItemAsync(parentA.id, quantity);
        bool removedB = await InventoryManager.Instance.RemoveItemAsync(parentB.id, quantity);

        if (!removedA || !removedB)
        {
            Debug.LogError("❌ 移除素材失敗，請稍後再試");
            return;
        }

        // 新增種子
        InventoryManager.Instance.AddItemToInventory(seed.id, quantity);

        Debug.Log($"✅ 成功交配！獲得 {hybrid.hybridName}種子 x{quantity}");
    }

    // 本地方法來計算某 itemId 的數量（從 InventoryManager 的資料）
    private int CountOf(string itemId)
    {
        var list = InventoryManager.Instance.GetInventoryData();
        var slot = list.Find(s => s.itemId == itemId);
        return slot?.count ?? 0;
    }
}
