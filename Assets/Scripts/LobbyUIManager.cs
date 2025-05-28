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
    [SerializeField] Button startGameButton;
    [SerializeField] Button backToLoginButton;

    string currentJoinCode;

    async void Start()
    {
        if (!CheckUIRefs()) return;

        statusText.text = "🔄 初始化中…";
        enterButton.interactable = false;
        startGameButton.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false); // 預設先隱藏，待判斷顯示

        // ✅ 綁定返回登入事件
        backToLoginButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("login");
        });

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
                joinCodeInput.gameObject.SetActive(false);

                currentJoinCode = await CreateRelayAsync();
                joinCodeInput.text = currentJoinCode ?? "";

                statusText.text = $"✅ Host 成功！JoinCode: <color=yellow>{currentJoinCode}</color>";

                // ✅ 顯示「開始遊戲」按鈕，僅 Host 可用
                startGameButton.gameObject.SetActive(true);
                startGameButton.onClick.RemoveAllListeners();
                startGameButton.onClick.AddListener(() =>
                {
                    if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
                    {
                        statusText.text = "🌾 正在載入農場場景中…";
                        NetworkManager.Singleton.SceneManager.LoadScene("FarmScene_Multiplayer", LoadSceneMode.Single);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ 非 Host 嘗試切場景，操作被忽略");
                    }
                });
            }
            else // Client
            {
                joinCodeInput.gameObject.SetActive(true);

                string code = joinCodeInput.text.Trim().ToUpper();
                if (string.IsNullOrEmpty(code))
                    throw new System.Exception("Join Code 不可空白！");

                await JoinRelayAsync(code);
                statusText.text = "✅ 加入成功！等待房主開始遊戲…";

                // ❌ 不自動切場景，等 Host 切換
            }
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
            joinAlloc.HostConnectionData,
            isSecure: false
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
        if (startGameButton == null) { Debug.LogError("❌ startGameButton 未指派"); ok = false; }
        if (backToLoginButton == null) { Debug.LogError("❌ backToLoginButton 未指派"); ok = false; }
        return ok;
    }
}
