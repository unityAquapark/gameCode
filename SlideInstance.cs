using UnityEngine;
using System;

public class SlideInstance : MonoBehaviour
{
    public SlideConfig Config { get; private set; }
    public int IncomeLevel { get; private set; } = 1;
    public int TickIntervalLevel { get; private set; } = 1;
    public bool HasAutoCollect { get; private set; } = false;
    public float SlideBalance { get; private set; } = 0f;
    private int _totalUpgradeSpent = 0;
    public int TotalUpgradeSpent => _totalUpgradeSpent;
    public GameObject _3dModel { get; set; }
    private float _tickTimer = 0f;
    public event Action<SlideInstance> OnBalanceChanged;
    public event Action<SlideInstance> OnUpgraded;
    public event Action<SlideInstance> OnCollected;

    public void Initialize(SlideConfig config)
    {
        Config = config;
        _tickTimer = 0f;
    }

    private void Update()
    {
        if (Config == null) return;
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= GetCurrentTickInterval())
        {
            _tickTimer = 0f;
            AddIncomeToBalance();
        }
    }

    private void AddIncomeToBalance()
    {
        SlideBalance += GetCurrentIncome();
        OnBalanceChanged?.Invoke(this);
        if (HasAutoCollect)
            CollectToGameBalance();
    }

    public void CollectToGameBalance()
    {
        if (SlideBalance <= 0f) return;
        GameEconomy.Instance.AddMoney(SlideBalance);
        SlideBalance = 0f;
        OnBalanceChanged?.Invoke(this);
        OnCollected?.Invoke(this);
    }

    public bool UpgradeIncome()
    {
        return TryUpgrade(
            currentLevel: IncomeLevel,
            levels: Config.upgradeData.incomeLevels,
            onSuccess: () => IncomeLevel++
        );
    }

    public bool UpgradeTickInterval()
    {
        return TryUpgrade(
            currentLevel: TickIntervalLevel,
            levels: Config.upgradeData.tickIntervalLevels,
            onSuccess: () => TickIntervalLevel++
        );
    }

    public bool BuyAutoCollect()
    {
        if (HasAutoCollect)
        {
            Debug.Log("Автосбор уже куплен");
            return false;
        }
        var cost = Config.upgradeData.autoCollectCost;
        if (!GameEconomy.Instance.TrySpendMoney(cost))
        {
            Debug.Log("Недостаточно денег для покупки автосбора");
            return false;
        }
        _totalUpgradeSpent += cost;
        HasAutoCollect = true;
        SaveManager.Instance?.ScheduleSave();
        OnUpgraded?.Invoke(this);
        return true;
    }

    private bool TryUpgrade(int currentLevel, UpgradeLevelData[] levels, Action onSuccess)
    {
        var maxLevel = Config.upgradeData.maxLevel;
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Максимальный уровень достигнут");
            return false;
        }
        var nextIndex = currentLevel;
        var cost = levels[nextIndex].cost;
        if (!GameEconomy.Instance.TrySpendMoney(cost))
        {
            Debug.Log("Недостаточно денег");
            return false;
        }
        _totalUpgradeSpent += cost;
        onSuccess();
        SaveManager.Instance?.ScheduleSave();
        OnUpgraded?.Invoke(this);
        return true;
    }

    public void RestoreState(SlotSaveData data)
    {
        IncomeLevel = data.incomeLevel;
        TickIntervalLevel = data.tickIntervalLevel;
        HasAutoCollect = data.hasAutoCollect;
        _totalUpgradeSpent = data.totalUpgradeSpent;
        SlideBalance = data.slideBalance;
        OnUpgraded?.Invoke(this);
        OnBalanceChanged?.Invoke(this);
    }

    public float GetCurrentIncome()
    {
        var t = (float)(IncomeLevel - 1) / (Config.upgradeData.maxLevel - 1);
        return Mathf.Lerp(Config.baseIncomePerTick, Config.maxIncomePerTick, t);
    }

    public float GetCurrentTickInterval()
    {
        var t = (float)(TickIntervalLevel - 1) / (Config.upgradeData.maxLevel - 1);
        return Mathf.Lerp(Config.baseTickInterval, Config.minTickInterval, t);
    }

    public int GetIncomeUpgradeCost()
    {
        if (IncomeLevel >= Config.upgradeData.maxLevel) return 0;
        return Config.upgradeData.incomeLevels[IncomeLevel].cost;
    }

    public int GetTickUpgradeCost()
    {
        if (TickIntervalLevel >= Config.upgradeData.maxLevel) return 0;
        return Config.upgradeData.tickIntervalLevels[TickIntervalLevel].cost;
    }
}