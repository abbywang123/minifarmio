using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

public class CloudSaveTester : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button confirmBtn;
    public Button goFarmBtn;
    public TMP_Text outputText;
    public GameObject namePanel;

    private bool hasSavedData = false;

    async void Start()
    {
        outputText.text = "初始化中...";
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        string id = AuthenticationService.Instance.PlayerId;
        outputText.text = $"✅ 登入成功！ID: {id}";

        confirmBtn.onClick.AddListener(OnConfirm);
        goFarmBtn.onClick.AddListener(OnGoFarm);
        goFarmBtn.interactable = false; // 一開始不能按
    }

    private async void OnConfirm()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            outputText.text = "❌ 請輸入暱稱";
            return;
        }

        FarmData data = new()
        {
            playerName = nameInput.text,
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

        await CloudSaveHelper.SaveFarmData(data);

        string id = AuthenticationService.Instance.PlayerId;
        outputText.text = 
            $"✅ 資料已儲存\nID: {id}\n" +
            $"{data.playerName} 的資料：\n" +
            $"💰 {data.gold}G\n" +
            string.Join("\n", data.inventory.ConvertAll(i => $"- {i.itemId} × {i.count}"));

        namePanel.SetActive(false);
        goFarmBtn.interactable = true;
        hasSavedData = true;
    }

    private void OnGoFarm()
    {
        if (hasSavedData)
            SceneManager.LoadScene("Farm");
        else
            outputText.text = "❌ 尚未建立資料，請先按『確認』";
    }
}



