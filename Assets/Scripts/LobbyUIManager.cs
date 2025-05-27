using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using QF = Unity.Services.Lobbies.Models.QueryFilter;   // 別名

public class LobbyUIManager : MonoBehaviour
{
    [Header("UI 元件")]
    public TMP_Dropdown modeDropdown;
    public TMP_InputField joinCodeInput;
    public TMP_Text statusText;
    public Button enterButton;

    private string currentJoinCode;

    void Start() => enterButton.onClick.AddListener(() => _ = StartMultiplayer());

    async Task StartMultiplayer()
    {
        int mode = modeDropdown.value;
        try
        {
            statusText.text = "🔄 建立連線中...";

            if (mode == 0)            // Host
            {
                statusText.text = "🟢 建立房主中...";
                currentJoinCode = await CreateRelay();
                statusText.text = $"✅ 房間代碼：{currentJoinCode}";

                await LobbyService.Instance.CreateLobbyAsync(
                    "MyFarmLobby",
                    2,
                    new CreateLobbyOptions
                    {
                        IsPrivate = false,
                        Data = { { "S1", new DataObject(DataObject.VisibilityOptions.Public, currentJoinCode) } }
                    });

                NetworkManager.Singleton.StartHost();
            }
            else                      // Client
            {
                string code = joinCodeInput.text.Trim();
                if (string.IsNullOrEmpty(code))
                {
                    statusText.text = "❌ 請輸入房間代碼";
                    return;
                }

                statusText.text = "🔵 加入房間中...";
                await JoinRelay(code);
                await LobbyService.Instance.QuickJoinLobbyAsync(
                    new QuickJoinLobbyOptions
                    {
                        Filter = new List<QueryFilter>    // ✅ List 形式
                        {
                            new QF(
                                QF.FieldOptions.S1,
                                code,
                                QF.OpOptions.EQ)
                        }
                    });

                NetworkManager.Singleton.StartClient();
            }

            statusText.text += "\n🚀 進入農場...";
            await Task.Delay(1000);
            SceneManager.LoadScene("FarmScene_Multiplayer");
        }
        catch (Exception ex)
        {
            statusText.text = $"❌ 發生錯誤：{ex.Message}";
            Debug.LogError(ex);
        }
    }

    /* ---------- Relay ---------- */

    async Task<string> CreateRelay()
    {
        Allocation alloc = await RelayService.Instance.CreateAllocationAsync(1);
        string joinCode  = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetHostRelayData(
            alloc.RelayServer.IpV4,
            (ushort)alloc.RelayServer.Port,
            alloc.AllocationIdBytes,
            alloc.Key,
            alloc.ConnectionData);

        return joinCode;
    }

    async Task JoinRelay(string joinCode)
    {
        JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetClientRelayData(
            alloc.RelayServer.IpV4,
            (ushort)alloc.RelayServer.Port,
            alloc.AllocationIdBytes,
            alloc.Key,
            alloc.ConnectionData,
            alloc.HostConnectionData);
    }
}




