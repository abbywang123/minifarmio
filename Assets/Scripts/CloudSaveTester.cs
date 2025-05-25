using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class CloudSaveTester : MonoBehaviour
{
    public TMP_InputField nameInput;     // 玩家輸入暱稱
    public Button saveBtn, loadBtn;      // 儲存 / 讀取按鈕
    public TMP_Text outputText;          // 輸出畫面
    public GameObject namePanel;         // ⬅️ 暱稱區塊（包含 Label + Input）

    private void Start()
    {
        // 📤 儲存按鈕邏輯
        saveBtn.onClick.AddListener(async () =>
        {
            if (string.IsNullOrWhiteSpace(nameInput.text))
            {
                outputText.text = "❌ 請輸入暱稱後再儲存";
                return;
            }

            // 建立資料結構
            FarmData data = new()
            {
                playerName = nameInput.text,
                gold = 999,
                inventory = new List<ItemSlot>
                {
                    new ItemSlot { itemId = "turnip", count = 3 },
                    new ItemSlot { itemId = "carrot", count = 5 }
                },
                farmland = new List<FarmlandTile>
                {
                    new FarmlandTile { x = 0, y = 0, cropId = "turnip", growDays = 2, isTilled = true },
                    new FarmlandTile { x = 1, y = 0, cropId = "", growDays = 0, isTilled = false }
                }
            };

            await CloudSaveHelper.SaveFarmData(data);

            // ✅ 儲存後自動隱藏輸入區塊
            namePanel.SetActive(false);
        });

        // 📥 讀取按鈕邏輯
        loadBtn.onClick.AddListener(async () =>
        {
            await CloudSaveHelper.LoadFarmData(data =>
            {
                if (data != null)
                {
                    outputText.text =
                        $"{data.playerName}\n" +
                        $"💰 {data.gold}G\n" +
                        $"作物：{data.inventory.Sum(i => i.count)} 個\n" +
                        string.Join("\n", data.inventory.ConvertAll(i => $"- {i.itemId} × {i.count}"));
                }
                else
                {
                    outputText.text = "❌ 沒有讀到資料";
                }
            });
        });
    }
}


