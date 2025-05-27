using UnityEngine;

public class FarmController : MonoBehaviour
{
    public async void Save()
    {
        // 🔍 儲存前先強制寫入暱稱（可移除）
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
        {
            PlayerPrefs.SetString("PlayerName", "測試玩家");
            Debug.Log("⚠️ 尚未設定玩家名稱，自動填入 '測試玩家'");
        }

        var data = new FarmData
        {
            playerName = PlayerPrefs.GetString("PlayerName", "未命名"),
            gold = Random.Range(100, 999)
        };

        // ✅ 加上輸出確認
        Debug.Log($"📦 建立儲存資料: {data.playerName}, 金幣: {data.gold}");

        await CloudSaveAPI.SaveFarmData(data);
    }

    public async void Load()
    {
        var data = await CloudSaveAPI.LoadFarmData();
        if (data != null)
        {
            Debug.Log($"🌾 玩家: {data.playerName}, 金幣: {data.gold}");
        }
        else
        {
            Debug.LogWarning("⚠️ 無法載入資料（data == null）");
        }
    }
}
