using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TileNetworkSync : NetworkBehaviour
{
    public TMP_Text label; // 用於顯示格子資訊（作物名稱 + 座標）

    // 可同步的變數
    public NetworkVariable<int> x = new NetworkVariable<int>();
    public NetworkVariable<int> y = new NetworkVariable<int>();
    public NetworkVariable<string> cropId = new NetworkVariable<string>();
    public NetworkVariable<int> growDays = new NetworkVariable<int>();

    // 設定格子的基本資料（由 Host 呼叫）
    public void SetTile(int _x, int _y, string _cropId, int _growDays)
    {
        x.Value = _x;
        y.Value = _y;
        cropId.Value = _cropId;
        growDays.Value = _growDays;
    }

    // 每幀更新 UI 顯示（只在本地運行，無需同步）
    void Update()
    {
        if (label != null)
        {
            label.text = $"{cropId.Value}\n({x.Value},{y.Value})";
        }
    }
}
