using UnityEngine;
using YG;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Ссылки на системы")]
    [SerializeField] private SlidesGrid _grid;
    [SerializeField] private GameEconomy _economy;

    [Header("Все доступные конфиги горок")]
    [SerializeField] private List<SlideConfig> _allSlideConfigs;

    [Header("Настройки дебаунса")]
    [Tooltip("Задержка перед сохранением в секундах после последнего действия игрока")]
    [SerializeField] private float _saveDelay;

    [Tooltip("Принудительное сохранение раз в N секунд, даже если дебаунс не сработал")]
    [SerializeField] private float _forceSaveInterval;

    private bool _loaded = false;
    private bool _saveScheduled = false;
    private float _lastForceSaveTime = 0f;
    private Coroutine _saveCoroutine = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += OnSDKReady;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= OnSDKReady;
    }

    private void OnSDKReady()
    {
        if (_loaded) return;
        _loaded = true;
        LoadGame();
    }

    private void Update()
    {
        if (_saveScheduled && Time.time - _lastForceSaveTime >= _forceSaveInterval)
        {
            ExecuteSave();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && _saveScheduled)
        {
            ExecuteSave();
        }
    }

    public void ScheduleSave()
    {
        if (!_loaded) return;
        _saveScheduled = true;
        if (_saveCoroutine != null)
            StopCoroutine(_saveCoroutine);
        _saveCoroutine = StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_saveDelay);
        ExecuteSave();
    }

    private void ExecuteSave()
    {
        if (_saveCoroutine != null)
        {
            StopCoroutine(_saveCoroutine);
            _saveCoroutine = null;
        }
        _saveScheduled = false;
        _lastForceSaveTime = Time.time;
        YG2.saves.totalBalance = _economy.TotalBalance;
        YG2.saves.placedSlides.Clear();
        for (var x = 0; x < _grid.GridData.CellCount; x++)
        {
            for (var y = 0; y < _grid.GridData.CellCount; y++)
            {
                var slot = _grid.SlidеsGrid[x, y];
                if (slot == null || !slot.IsOccupied) continue;
                var slide = slot.PlacedSlide;
                YG2.saves.placedSlides.Add(new SlotSaveData
                {
                    gridX = x,
                    gridY = y,
                    slideConfigName = slide.Config.slideName,
                    incomeLevel = slide.IncomeLevel,
                    tickIntervalLevel = slide.TickIntervalLevel,
                    hasAutoCollect = slide.HasAutoCollect,
                    totalUpgradeSpent = slide.TotalUpgradeSpent,
                    slideBalance = slide.SlideBalance
                });
            }
        }
        YG2.SaveProgress();
        Debug.Log($"[SaveManager] Сохранено. Баланс: {YG2.saves.totalBalance:F0}$ | Горок: {YG2.saves.placedSlides.Count}");
    }

    private void LoadGame()
    {
        _economy.SetBalance(YG2.saves.totalBalance);
        foreach (var data in YG2.saves.placedSlides)
        {
            var config = FindConfig(data.slideConfigName);
            if (config == null)
            {
                Debug.LogWarning($"[SaveManager] Конфиг '{data.slideConfigName}' не найден!");
                continue;
            }
            var slot = _grid.GetCell(data.gridX, data.gridY);
            if (slot == null || slot.IsOccupied) continue;
            slot.PlaceSlideAlreadyPaid(config);
            if (slot.PlacedSlide != null)
                slot.PlacedSlide.RestoreState(data);
        }
        Debug.Log($"[SaveManager] Загружено. Баланс: {YG2.saves.totalBalance:F0}$ | Горок: {YG2.saves.placedSlides.Count}");
    }

    private SlideConfig FindConfig(string name)
    {
        foreach (var config in _allSlideConfigs)
            if (config.slideName == name) return config;
        return null;
    }

    private void OnApplicationQuit()
    {
        if (_saveScheduled)
            ExecuteSave();
    }
}