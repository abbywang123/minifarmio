using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections; // ✅ 引入 FixedString32Bytes

public class PlayerNameSync : NetworkBehaviour
{
    public TMP_Text nameText;

    public NetworkVariable<FixedString32Bytes> playerName =
        new NetworkVariable<FixedString32Bytes>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string localName = PlayerPrefs.GetString("playerName", "玩家");
            playerName.Value = new FixedString32Bytes(localName);
        }

        // 初次顯示名稱
        nameText.text = playerName.Value.ToString();

        // 訂閱變更事件
        playerName.OnValueChanged += (_, newValue) =>
        {
            nameText.text = newValue.ToString();
        };
    }
}
