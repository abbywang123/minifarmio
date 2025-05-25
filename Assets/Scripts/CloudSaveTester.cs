using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CloudSaveTester : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button saveBtn, loadBtn;
    public TMP_Text outputText;

    private void Start()
    {
        saveBtn.onClick.AddListener(async () =>
        {
            FarmData data = new()
            {
                playerName = nameInput.text,
                gold = 999,
                inventory = new()
                {
                    new ItemSlot { itemId = "turnip", count = 3 },
                    new ItemSlot { itemId = "carrot", count = 5 }
                },
                farmland = new()
                {
                    new FarmlandTile { x = 0, y = 0, cropId = "turnip", growDays = 2, isTilled = true },
                    new FarmlandTile { x = 1, y = 0, cropId = "", growDays = 0, isTilled = false }
                }
            };

            await CloudSaveHelper.SaveFarmData(data);
        });

        loadBtn.onClick.AddListener(async () =>
        {
            await CloudSaveHelper.LoadFarmData(data =>
            {
                if (data != null)
                {
                    outputText.text = $"👤 {data.playerName}\n💰 {data.gold}G\n作物：{data.inventory.Count} 個";
                }
                else
                {
                    outputText.text = "❌ 沒有讀到資料";
                }
            });
        });
    }
}
