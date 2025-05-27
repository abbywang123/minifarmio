using UnityEngine;
using Unity.Netcode;

public class NetcodeDiag : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"[Diag] NetworkManager.Singleton = {NetworkManager.Singleton}");
        if (NetworkManager.Singleton != null)
        {
            Debug.Log($"[Diag] Default Player Prefab = {NetworkManager.Singleton.NetworkConfig.PlayerPrefab}");
            Debug.Log($"[Diag] Transport = {NetworkManager.Singleton.NetworkConfig.NetworkTransport}");
        }
    }
}
