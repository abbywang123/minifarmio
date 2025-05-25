using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmName);
    }

    private void OnConfirmName()
    {
        string playerName = nameInput.text;
        Debug.Log("暱稱輸入：" + playerName);
        namePanel.SetActive(false); // 隱藏輸入介面
        // 可在這裡儲存暱稱到 PlayerPrefs 或上傳至後端
    }
}
