using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SlideShop : MonoBehaviour
{
    public static SlideShop Instance { get; private set; }

    [Header("UI магазина")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Transform _slotsContainer;

    [Header("Горки в магазине")]
    [SerializeField] private List<SlideConfig> _availableSlides;

    [Header("Префаб карточки горки")]
    [SerializeField] private GameObject _slideCardPrefab;

    public bool IsShopOpen { get; private set; } = false;
    public bool IsWaitingForCell { get; private set; } = false;
    private SlideConfig _pendingConfig = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(CloseShop);
        _shopPanel.SetActive(false);
        GenerateSlideCards();
        GameEconomy.Instance.OnBalanceChanged += _ => RefreshAllCards();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (IsWaitingForCell && Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPendingSlide();
        }
        else if (IsShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        IsShopOpen = true;
        _shopPanel.SetActive(true);
        RefreshAllCards();
    }

    public void CloseShop()
    {
        IsShopOpen = false;
        _shopPanel.SetActive(false);
    }

    public void SelectSlide(SlideConfig config)
    {
        if (!GameEconomy.Instance.TrySpendMoney(config.buildCost))
        {
            Debug.Log("Недостаточно денег");
            return;
        }
        _pendingConfig = config;
        IsWaitingForCell = true;
        CloseShop();
        Debug.Log($"Выбрана горка '{config.slideName}'. Нажми на клетку для постройки. Esc — отмена.");
    }

    public SlideConfig ConsumePendingConfig()
    {
        if (!IsWaitingForCell) return null;
        var config = _pendingConfig;
        _pendingConfig = null;
        IsWaitingForCell = false;
        return config;
    }

    private void CancelPendingSlide()
    {
        if (_pendingConfig == null) return;
        GameEconomy.Instance.AddMoney(_pendingConfig.buildCost);
        Debug.Log($"Отмена постройки '{_pendingConfig.slideName}'. " +
                  $"Возврат: {_pendingConfig.buildCost}$");
        _pendingConfig = null;
        IsWaitingForCell = false;
    }

    private void GenerateSlideCards()
    {
        foreach (Transform child in _slotsContainer)
            Destroy(child.gameObject);
        foreach (var config in _availableSlides)
        {
            var cardGO = Instantiate(_slideCardPrefab, _slotsContainer);
            var card = cardGO.GetComponent<SlideShopCard>();
            card.Setup(config, this);
        }
    }

    private void RefreshAllCards()
    {
        foreach (var card in _slotsContainer.GetComponentsInChildren<SlideShopCard>())
            card.Refresh();
    }
}