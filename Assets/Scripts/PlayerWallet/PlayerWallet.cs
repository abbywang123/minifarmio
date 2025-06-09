using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    private const string MoneyKey = "PlayerMoney"; // 用來儲存金錢的鍵值

    [SerializeField]
    private int currentMoney = 1000;

    public int CurrentMoney => currentMoney;

    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        // Singleton 設置
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadMoney(); // 在遊戲啟動時讀取金錢
    }

    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    public bool Spend(int amount)
    {
        if (CanAfford(amount))
        {
            currentMoney -= amount;
            SaveMoney(); // 花錢後儲存
            Debug.Log($"💸 扣款 {amount}，剩餘:{currentMoney}");
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }

        Debug.Log("❌ 錢不夠");
        return false;
    }

    public void Earn(int amount)
    {
        currentMoney += amount;
        SaveMoney(); // 收錢後儲存
        Debug.Log($"💰 收到 {amount}，現在擁有:{currentMoney}");
        OnMoneyChanged?.Invoke(currentMoney);
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, currentMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        currentMoney = PlayerPrefs.GetInt(MoneyKey, 1000); // 若尚未儲存過則用 1000
    }

    public void ResetMoney()
    {
        PlayerPrefs.DeleteKey(MoneyKey);
        currentMoney = 1000;
        SaveMoney();
        OnMoneyChanged?.Invoke(currentMoney);
    }
}
