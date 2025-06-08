using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MultiplayerFarmSaver : NetworkBehaviour
{
    // ✅ 點擊返回按鈕觸發：離開並儲存
    public void OnClickExitFarm()
    {
        _ = SaveAllAndExit(); // fire-and-forget async
    }

    private async Task SaveAllAndExit()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("📝 Host 離開前儲存農場與背包...");

            await SaveFarmDataAsync();       // ✅ 儲存格子資料
            await SaveInventoryAsync();      // ✅ 儲存背包與金幣資料
        }

        SceneManager.LoadScene("LobbyScene");
    }

    // ✅ 儲存格子資料
    public async Task SaveFarmDataAsync()
    {
        FarmData farm = new FarmData();
        farm.farmland = new List<FarmlandTile>();

        foreach (var tile in FindObjectsOfType<TileNetworkSync>())
        {
            farm.farmland.Add(new FarmlandTile
            {
                x = tile.X,
                y = tile.Y,
                cropId = tile.CropId,
                growDays = tile.GrowDays,
                isTilled = true
            });
        }

        farm.playerName = PlayerPrefs.GetString("playerName", "Host玩家");
        farm.gold = PlayerPrefs.GetInt("gold", 999); // 金幣由 PlayerPrefs 取得

        await CloudSaveAPI.SaveFarmData(farm);
        Debug.Log("✅ Host 的農場格子資料已儲存！");
    }

    // ✅ 儲存背包資料（從 PlayerInventorySync）
    public async Task SaveInventoryAsync()
    {
        var player = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (player == null)
        {
            Debug.LogWarning("⚠️ 找不到本地玩家 NetworkObject，無法儲存背包");
            return;
        }

        var inventory = player.GetComponent<PlayerInventorySync>();
        if (inventory == null)
        {
            Debug.LogWarning("⚠️ 找不到 PlayerInventorySync");
            return;
        }

        // 轉成 List<ItemSlot>
        List<ItemSlot> itemList = new();
        foreach (var slot in inventory.syncedInventory)
        {
            itemList.Add(new ItemSlot
            {
                itemId = slot.itemId.ToString(),
                count = slot.count
            });
        }

        FarmData data = await CloudSaveAPI.LoadFarmData();
        data.inventory = itemList;
        data.gold = PlayerPrefs.GetInt("gold", 1000); // 也可以再次更新金幣

        await CloudSaveAPI.SaveFarmData(data);
        Debug.Log("✅ 背包資料與金幣已儲存！");
    }
}
