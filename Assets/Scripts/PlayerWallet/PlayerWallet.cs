using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public int gold = 100;

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"💰 支出 {amount} 金幣，目前剩餘 {gold} 金幣。");
            return true;
        }
        Debug.LogWarning("⚠️ 金幣不足！");
        return false;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"💰 獲得 {amount} 金幣，目前共 {gold} 金幣。");
    }

    public int GetGold()
    {
        return gold;
    }
}
