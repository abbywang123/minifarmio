using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class LobbyUIManager : MonoBehaviour
{
    [Header("UI 元件 (請在 Inspector 指派)")]
    [SerializeField] TMP_Dropdown modeDropdown;
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button enterButton;

    string currentJoinCode;

    async void Start()
    {
        if (!CheckUIRefs()) return;

        statusText.text = "🔄 初始化中…";
        enterButton.interactable = false;

        await EnsureServicesAsync();

        statusText.text = "✅ 請選擇模式並點擊開始";
        enterButton.interactable = true;
        enterButton.onClick.AddListener(() => _ = StartMultiplayerAsync());
    }

    async Task StartMultiplayerAsync()
    {
        enterButton.interactable = false;
        statusText.text = "🔄 連線處理中…";

        try
        {
            if (modeDropdown.value == 0) // Host
            {
                currentJoinCode = await CreateRelayAsync();
                joinCodeInput.text = currentJoinCode ?? "";
                statusText.text = $"✅ Host 成功！JoinCode: <color=yellow>{currentJoinCode}</color>";
            }
            else // Client
            {
                string code = joinCodeInput.text.Trim().ToUpper();
                if (string.IsNullOrEmpty(code))
                    throw new System.Exception("Join Code 不可空白！");
                await JoinRelayAsync(code);
                statusText.text = "✅ 加入成功！";
            }

            SceneManager.LoadScene("FarmScene_Multiplayer");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ 連線錯誤: " + ex);
            statusText.text = $"❌ 失敗：{ex.Message}";
            enterButton.interactable = true;
        }
    }

async Task<string> CreateRelayAsync()
{
    statusText.text = "🔄 建立 Relay 中…";

    Allocation alloc = await RelayService.Instance.CreateAllocationAsync(2);
    string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
    Debug.Log("✅ 分配完成，JoinCode: " + joinCode);

    // 🛠️ 加上防呆檢查
    if (NetworkManager.Singleton == null)
    {
        Debug.LogError("❌ NetworkManager.Singleton is null！");
        statusText.text = "❌ 找不到 NetworkManager";
        return null;
    }

    var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    if (transport == null)
    {
        Debug.LogError("❌ UnityTransport 組件不存在！");
        statusText.text = "❌ 找不到 UnityTransport";
        return null;
    }

    transport.SetRelayServerData(
        alloc.RelayServer.IpV4,
        (ushort)alloc.RelayServer.Port,
        alloc.AllocationIdBytes,
        alloc.Key,
        alloc.ConnectionData,
        alloc.ConnectionData,
        isSecure: false
    );

    NetworkManager.Singleton.StartHost();
    return joinCode;
}





async Task JoinRelayAsync(string joinCode)
{
    statusText.text = "🔄 加入 Relay 中…";

    JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

    var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    transport.SetRelayServerData(
        joinAlloc.RelayServer.IpV4,
        (ushort)joinAlloc.RelayServer.Port,
        joinAlloc.AllocationIdBytes,
        joinAlloc.Key,
        joinAlloc.ConnectionData,
        joinAlloc.HostConnectionData,  // ✅ Host 的 connection data
        isSecure: false                // ✅ 是否加密
    );

    NetworkManager.Singleton.StartClient();
}



    async Task EnsureServicesAsync()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("✅ 初始化與登入完成");
    }

    bool CheckUIRefs()
    {
        bool ok = true;
        if (modeDropdown == null) { Debug.LogError("❌ modeDropdown 未指派"); ok = false; }
        if (joinCodeInput == null) { Debug.LogError("❌ joinCodeInput 未指派"); ok = false; }
        if (statusText == null) { Debug.LogError("❌ statusText 未指派"); ok = false; }
        if (enterButton == null) { Debug.LogError("❌ enterButton 未指派"); ok = false; }
        return ok;
    }
}
