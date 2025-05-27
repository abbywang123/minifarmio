using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerNameSync : NetworkBehaviour
{
    public TMP_Text nameText;

    public NetworkVariable<string> playerName = new NetworkVariable<string>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string localName = PlayerPrefs.GetString("playerName", "玩家");
            playerName.Value = localName;
        }

        nameText.text = playerName.Value;

        playerName.OnValueChanged += (_, newValue) =>
        {
            nameText.text = newValue;
        };
    }
}
