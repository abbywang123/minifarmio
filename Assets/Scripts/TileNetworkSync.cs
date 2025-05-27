using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TileNetworkSync : NetworkBehaviour
{
    public TMP_Text label; // 顯示用的 Label（請在 Inspector 指派）

    // ✅ 同步格子的資料
    public NetworkVariable<int> x = new();
    public NetworkVariable<int> y = new();
    public NetworkVariable<string> cropId = new();
    public NetworkVariable<int> growDays = new();

    // ✅ 提供外部存取用屬性（例如存檔用）
    public int X => x.Value;
    public int Y => y.Value;
    public string CropId => cropId.Value;
    public int GrowDays => growDays.Value;

    // ✅ 初始化格子資料（由 Host 在 Spawn 時設定）
    public void SetTile(int _x, int _y, string _cropId, int _growDays)
    {
        x.Value = _x;
        y.Value = _y;
        cropId.Value = _cropId;
        growDays.Value = _growDays;
    }

    void Update()
    {
        // ✅ 更新 UI 顯示（作物名稱 + 座標）
        if (label != null)
        {
            string display = string.IsNullOrEmpty(cropId.Value) ? "空地" : cropId.Value;
            label.text = $"{display}\n({x.Value},{y.Value})";
        }
    }

    // ✅ 播種 RPC（由 Client 呼叫，Server 執行）
    [ServerRpc(RequireOwnership = false)]
    public void PlantCropServerRpc(string crop)
    {
        cropId.Value = crop;
        growDays.Value = 0;
    }

    // ✅ 收成 RPC（清除作物）
    [ServerRpc(RequireOwnership = false)]
    public void HarvestServerRpc()
    {
        cropId.Value = "";
        growDays.Value = 0;
    }

    // ✅ 清空格子 RPC（可用於重置農地）
    [ServerRpc(RequireOwnership = false)]
    public void ClearTileServerRpc()
    {
        cropId.Value = "";
        growDays.Value = 0;
    }
}
