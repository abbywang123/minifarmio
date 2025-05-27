using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Button breedingOpenButton;         // ✅ 開啟面板的按鈕
    public Button closeButton;                // ✅ 關閉面板的按鈕
    public Button breedButton;                // ✅ 交配按鈕

    public TMP_Dropdown hybridDropdown;       // ✅ 交配作物選擇
    public TMP_Dropdown quantityDropdown;     // ✅ 數量選擇

    public TextMeshProUGUI parentAText;       // ✅ 顯示 Parent A
    public TextMeshProUGUI parentBText;       // ✅ 顯示 Parent B

    [Header("背包參考")]
    public Inventory playerInventory;

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
        // 綁定按鈕事件
        breedingOpenButton.onClick.AddListener(() => breedingPanel.SetActive(true));
        closeButton.onClick.AddListener(() => breedingPanel.SetActive(false));
        breedButton.onClick.AddListener(OnBreedButtonClicked);
        hybridDropdown.onValueChanged.AddListener(UpdateParentTexts);

        SetupDropdowns();
        UpdateParentTexts(0);
        breedingPanel.SetActive(false); // 預設不顯示面板
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

    private void OnBreedButtonClicked()
    {
        int index = hybridDropdown.value;
        int quantity = quantityDropdown.value + 1;

        var hybrid = hybridList[index];

        ItemData parentA = Resources.Load<ItemData>("Items/" + hybrid.parentA);
        ItemData parentB = Resources.Load<ItemData>("Items/" + hybrid.parentB);
        ItemData seed = Resources.Load<ItemData>("Items/" + hybrid.hybridName + "種子");

        if (parentA == null || parentB == null || seed == null)
        {
            Debug.LogWarning("❌ 找不到對應的物品（請確認 Resources/Items 下的資產命名是否正確）");
            return;
        }

        int haveA = playerInventory.CountOf(parentA);
        int haveB = playerInventory.CountOf(parentB);

        if (haveA < quantity || haveB < quantity)
        {
            Debug.Log("❌ 材料不足，請檢查背包中是否有足夠的作物！");
            return;
        }

        playerInventory.Remove(parentA, quantity);
        playerInventory.Remove(parentB, quantity);
        playerInventory.Add(seed, quantity);

        Debug.Log($"✅ 成功交配！獲得 {hybrid.hybridName}種子 x{quantity}");
    }
}
