using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FarmUIController : MonoBehaviour
{
    public Button openInventoryButton;
    public Button openShopButton;

    void Start()
    {
        openInventoryButton.onClick.AddListener(OpenInventoryScene);
    }

    void OpenInventoryScene()
    {
        SceneManager.LoadScene("Inventory");
    }

    
}
