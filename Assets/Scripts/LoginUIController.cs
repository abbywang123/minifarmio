using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Netcode;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI 元件")]
    public TMP_InputField nameInput;
    public Button confirmButton;
    public Button goFarmButton;           // 單機農場按鈕
    public Button goMultiplayerButton;    // 多人農場按鈕
    public TMP_Text outputText;

    private bool dataSaved = false;

    async void Start()
    {
        outputText.text = "🔀 初始化中...";
        confirmButton.interactable = false;
        goFarmButton.interactable = false;
        goMultiplayerButton.interactable = false;

        await TryLoginSafely();
        await WaitForFinalLoginState();
        Debug.Log("✅ NetworkManager Singleton 是否存在？" + (NetworkManager.Singleton != null));

        if (AuthenticationService.Instance.IsSignedIn)
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            outputText.text = $"✅ 已登入\nID：{playerId}";
            Debug.Log("✅ 登入成功");

            try
            {
                var data = await CloudSaveAPI.LoadFarmData();
                if (data != null)
                {
                    Debug.Log("✅ Cloud Save 資料已存在");
                    dataSaved = true;
                    goFarmButton.interactable = true;
                    goMultiplayerButton.interactable = true;
                }
                else
                {
                    outputText.text += "\n請輸入曉名並點擊『確認』建立資料";
                }
            }
            catch (System.Exception e)
            {
                outputText.text += "\n❌ Cloud Save 載入失敗：" + e.Message;
            }

            confirmButton.onClick.AddListener(OnConfirm);
            goFarmButton.onClick.AddListener(OnEnterFarmSingle);
            goMultiplayerButton.onClick.AddListener(OnEnterFarmMultiplayer);

            confirmButton.interactable = true;
        }
        else
        {
            outputText.text = "❌ 登入失敗，請重新啟動";
        }
    }

    async Task TryLoginSafely()
    {
        try { await AuthHelper.EnsureSignedIn(); }
        catch (System.Exception ex) { Debug.LogWarning("⚠️ 登入例外：" + ex.Message); }
    }

    async Task WaitForFinalLoginState()
    {
        int retries = 0;
        while (!AuthenticationService.Instance.IsSignedIn && retries++ < 100)
            await Task.Delay(100);
    }

    private async void OnConfirm()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            outputText.text = "❌ 請輸入曉名";
            return;
        }

        string nickname = nameInput.text.Trim();
        Debug.Log("👤 曉名輸入：" + nickname);

        FarmData data = new()
        {
            playerName = nickname,
            gold = 1000,
            inventory = new List<ItemSlot>
            {
                new ItemSlot { itemId = "wheat", count = 3 },
                new ItemSlot { itemId = "carrot", count = 5 },
                new ItemSlot { itemId = "carrotseed", count = 10 } // 新增種子
            },
            farmland = new List<FarmlandTile>
            {
                new FarmlandTile { x = 0, y = 0, cropId = "wheat", growDays = 2, isTilled = true },
                new FarmlandTile { x = 1, y = 0, cropId = "", growDays = 0, isTilled = false }
            }
        };

        await CloudSaveAPI.SaveFarmData(data);

        PlayerPrefs.SetString("playerName", nickname);
        var wrapper = new InventoryWrapper { inventory = data.inventory };
        PlayerPrefs.SetString("inventoryData", JsonUtility.ToJson(wrapper));

        outputText.text = $"✅ 資料建立完成\n曉名：{data.playerName}\n💰 金幣：{data.gold}G\n" +
                          string.Join("\n", data.inventory.Select(i => $"🔹 {i.itemId} x{i.count}"));

        dataSaved = true;
        goFarmButton.interactable = true;
        goMultiplayerButton.interactable = true;
    }

    private void OnEnterFarmSingle()
    {
        if (!dataSaved)
        {
            outputText.text = "❌ 請先按『確認』建立資料";
            return;
        }

        SceneManager.LoadScene("Farm");
    }

    private void OnEnterFarmMultiplayer()
    {
        if (!dataSaved)
        {
            outputText.text = "❌ 請先按『確認』建立資料";
            return;
        }

        SceneManager.LoadScene("LobbyScene");
    }
}

[System.Serializable]
public class InventoryWrapper
{
    public List<ItemSlot> inventory;
}
