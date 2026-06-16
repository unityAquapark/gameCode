using UnityEngine;
using System;

public class GameEconomy : MonoBehaviour
{
    public static GameEconomy Instance { get; private set; }
    public float TotalBalance { get; private set; } = 100f;
    public event Action<float> OnBalanceChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddMoney(float amount)
    {
        TotalBalance += amount;
        OnBalanceChanged?.Invoke(TotalBalance);
        SaveManager.Instance?.ScheduleSave();
    }

    public bool TrySpendMoney(float amount)
    {
        if (TotalBalance < amount)
            return false;
        TotalBalance -= amount;
        OnBalanceChanged?.Invoke(TotalBalance);
        SaveManager.Instance?.ScheduleSave();
        return true;

    }

    public void SetBalance(float amount)
    {
        TotalBalance = amount;
        OnBalanceChanged?.Invoke(TotalBalance);
    }
}