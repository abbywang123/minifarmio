using System.IO;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{
    public static WarehouseManager Instance { get; private set; }
    public Inventory inventory;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        inventory = gameObject.AddComponent<Inventory>();
        inventory.SetCapacity(200); // 可視遊戲調整

        Load(); // 讀舊存檔
    }

    void OnApplicationQuit() => Save();

    void Save()
    {
        var dto = JsonUtility.ToJson(new InventoryDTO(inventory.Slots));
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "warehouse.json"), dto);
    }

    void Load()
    {
        var path = Path.Combine(Application.persistentDataPath, "warehouse.json");
        if (!File.Exists(path)) return;

        var dto = JsonUtility.FromJson<InventoryDTO>(File.ReadAllText(path));
        inventory.FromDTO(dto);
    }
}
