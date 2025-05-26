using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI 元件")]
    public TMP_InputField nameInput;
    public Button confirmButton;
    public Button goFarmButton;
    public TMP_Text outputText;

    private bool dataSaved = false;

    async void Start()
    {
        outputText.text = "🔄 初始化中...";
        confirmButton.interactable = false;
        goFarmButton.interactable = false;

        await TryLoginSafely();
        await WaitForFinalLoginState();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            outputText.text = $"✅ 已登入\nID：{playerId}";
            Debug.Log("✅ 登入成功！");

            // ✅ 嘗試載入 Cloud Save 資料
            try
            {
                var data = await CloudSaveAPI.LoadFarmData();

                if (data != null)
                {
                    Debug.Log("✅ Cloud Save 資料已存在");
                    dataSaved = true;
                    goFarmButton.interactable = true;
                }
                else
                {
                    Debug.Log("📂 尚無 Cloud Save 資料，請玩家輸入暱稱後建立");
                    outputText.text += "\n請輸入暱稱並點擊『確認』建立資料";
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ Cloud Save 載入失敗：" + e.Message);
                outputText.text += "\n資料載入錯誤，請重新啟動或檢查網路";
            }

            // 綁定按鈕事件
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirm);
            confirmButton.interactable = true;

            goFarmButton.onClick.RemoveAllListeners();
            goFarmButton.onClick.AddListener(OnEnterFarm);
        }
        else
        {
            outputText.text = "❌ 登入失敗，請重新啟動";
            Debug.LogError("❌ 登入最終失敗！");
        }
    }

    async Task TryLoginSafely()
    {
        try
        {
            await AuthHelper.EnsureSignedIn();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ 登入例外：" + ex.Message);
        }
    }

    async Task WaitForFinalLoginState()
    {
        int retries = 0;
        while (!AuthenticationService.Instance.IsSignedIn && retries++ < 100)
        {
            await Task.Delay(100);
        }
    }

    private async void OnConfirm()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            outputText.text = "❌ 請輸入暱稱";
            return;
        }

        string nickname = nameInput.text;
        Debug.Log("👤 暱稱輸入：" + nickname);

        FarmData data = new()
        {
            playerName = nickname,
            gold = 999,
            inventory = new List<ItemSlot>
            {
                new ItemSlot { itemId = "wheat", count = 3 },
                new ItemSlot { itemId = "carrot", count = 5 }
            },
            farmland = new List<FarmlandTile>
            {
                new FarmlandTile { x = 0, y = 0, cropId = "wheat", growDays = 2, isTilled = true },
                new FarmlandTile { x = 1, y = 0, cropId = "", growDays = 0, isTilled = false }
            }
        };

        await CloudSaveAPI.SaveFarmData(data);

        string id = AuthenticationService.Instance.PlayerId;
        outputText.text =
            $"✅ 資料建立完成\n👤 ID：{id}\n" +
            $"暱稱：{data.playerName}\n💰 金幣：{data.gold} G\n" +
            string.Join("\n", data.inventory.Select(i => $"🔹 {i.itemId} x{i.count}"));

        dataSaved = true;
        goFarmButton.interactable = true;
    }

    private void OnEnterFarm()
    {
        if (!dataSaved)
        {
            outputText.text = "❌ 請先按『確認』建立資料";
            return;
        }

        SceneManager.LoadScene("Farm");
    }
}





