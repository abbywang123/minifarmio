using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase I { get; private set; }

    [SerializeField] private List<ItemData> items;
    private Dictionary<string, ItemData> itemDict;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;

        itemDict = new Dictionary<string, ItemData>();
        foreach (var item in items)
        {
            if (item != null && !string.IsNullOrEmpty(item.id))
                itemDict[item.id] = item;
        }
    }

    public ItemData Get(string id)
    {
        if (itemDict.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"ItemDatabase：查無 id = {id}");
        return null;
    }
}
