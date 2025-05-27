using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TileNetworkSync : NetworkBehaviour
{
    public TMP_Text label; // 指向 Label 顯示用的 Text（請從 Inspector 指派）

    // 網路變數：用來同步格子的座標、作物種類、成長天數
    public NetworkVariable<int> x = new();
    public NetworkVariable<int> y = new();
    public NetworkVariable<string> cropId = new();
    public NetworkVariable<int> growDays = new();

    // 初始化格子資料（通常由 Host 建立格子時呼叫）
    public void SetTile(int _x, int _y, string _cropId, int _growDays)
    {
        x.Value = _x;
        y.Value = _y;
        cropId.Value = _cropId;
        growDays.Value = _growDays;
    }

    void Update()
    {
        // 顯示作物名稱與座標
        if (label != null)
        {
            label.text = $"{cropId.Value}\n({x.Value},{y.Value})";
        }

        // 📌 點擊行為不處理於此，交給 TileClickManager 控制
    }

    // ✅ 播種 RPC：任何人點擊後都可以要求 Server 幫他修改 cropId
    [ServerRpc(RequireOwnership = false)]
    public void PlantCropServerRpc(string crop)
    {
        cropId.Value = crop;
        growDays.Value = 0;
    }
}
