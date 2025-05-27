using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [SerializeField]
    private int currentMoney = 1000;

    public int CurrentMoney => currentMoney;

    // 當金錢變動時觸發的事件（UI 可訂閱）
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
        DontDestroyOnLoad(gameObject); // 若希望場景切換時仍保留
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
            Debug.Log($"💸 扣款 {amount}，剩餘:{currentMoney}");
            OnMoneyChanged?.Invoke(currentMoney); // 觸發事件
            return true;
        }

        Debug.Log("❌ 錢不夠");
        return false;
    }

    public void Earn(int amount)
    {
        currentMoney += amount;
        Debug.Log($"💰 收到 {amount}，現在擁有:{currentMoney}");
        OnMoneyChanged?.Invoke(currentMoney); // 觸發事件
    }
}
