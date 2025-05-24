using UnityEngine;
using UnityEngine.UIElements;

public class WarehouseUI : MonoBehaviour
{
    private VisualElement root;
    private ScrollView slotContainer;

    void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        slotContainer = root.Q<ScrollView>("slotContainer"); // 名稱要和 .uxml 對應
        RefreshUI();
    }

    void RefreshUI()
    {
        // TODO: 實作顯示格子邏輯
    }
}
