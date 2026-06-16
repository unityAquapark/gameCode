using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlideUI : MonoBehaviour
{
    [Header("Текущая горка")]
    private SlideInstance _currentSlide;

    [Header("Кнопки улучшений")]
    public Button upgradeIncomeButton;
    public Button upgradeTickButton;
    public Button buyAutoCollectButton;
    public Button collectButton;

    [Header("Удаление горки")]
    public Button deleteButton;           
    public GameObject confirmPanel;      
    public Button confirmDeleteButton;    
    public Button cancelDeleteButton;     
    public TMP_Text refundPreviewText;    

    [Header("Текстовые поля")]
    public TMP_Text slideNameText;
    public TMP_Text balanceText;
    public TMP_Text incomeLevelText;
    public TMP_Text tickLevelText;
    public TMP_Text autoCollectStatusText;
    public TMP_Text upgradeIncomeCostText;
    public TMP_Text upgradeTickCostText;
    public TMP_Text autoCollectCostText;

    public void Bind(SlideInstance slide)
    {
        if (_currentSlide != null)
        {
            _currentSlide.OnBalanceChanged -= RefreshUI;
            _currentSlide.OnUpgraded -= RefreshUI;
            _currentSlide.OnCollected -= RefreshUI;
        }
        _currentSlide = slide;
        _currentSlide.OnBalanceChanged += RefreshUI;
        _currentSlide.OnUpgraded += RefreshUI;
        _currentSlide.OnCollected += RefreshUI;
        upgradeIncomeButton.onClick.RemoveAllListeners();
        upgradeTickButton.onClick.RemoveAllListeners();
        buyAutoCollectButton.onClick.RemoveAllListeners();
        collectButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
        upgradeIncomeButton.onClick.AddListener(() => _currentSlide.UpgradeIncome());
        upgradeTickButton.onClick.AddListener(() => _currentSlide.UpgradeTickInterval());
        buyAutoCollectButton.onClick.AddListener(() => _currentSlide.BuyAutoCollect());
        collectButton.onClick.AddListener(() => _currentSlide.CollectToGameBalance());
        deleteButton.onClick.AddListener(ShowDeleteConfirm);
        confirmDeleteButton.onClick.RemoveAllListeners();
        cancelDeleteButton.onClick.RemoveAllListeners();
        confirmDeleteButton.onClick.AddListener(ConfirmDelete);
        cancelDeleteButton.onClick.AddListener(HideDeleteConfirm);
        HideDeleteConfirm();
        RefreshUI(_currentSlide);
    }

    private void ShowDeleteConfirm()
    {
        var upgradeRefund = Mathf.FloorToInt(_currentSlide.TotalUpgradeSpent * 0.5f);
        var buildRefund = Mathf.FloorToInt(_currentSlide.Config.buildCost * 0.5f);
        var totalRefund = upgradeRefund + buildRefund;
        refundPreviewText.text = $"Вернётся: {totalRefund}$";
        confirmPanel.SetActive(true);
    }

    private void HideDeleteConfirm()
    {
        confirmPanel.SetActive(false);
    }

    private void ConfirmDelete()
    {
        var cell = _currentSlide.GetComponentInParent<CellSlot>();
        if (cell != null)
        {
            cell.RemoveSlide();
        }
        HideDeleteConfirm();
        gameObject.SetActive(false);
    }

    private void RefreshUI(SlideInstance slide)
    {
        var maxIncome = slide.IncomeLevel >= slide.Config.upgradeData.maxLevel;
        var maxTick = slide.TickIntervalLevel >= slide.Config.upgradeData.maxLevel;
        slideNameText.text = slide.Config.slideName;
        balanceText.text = $"Баланс: {slide.SlideBalance:F0}$";
        incomeLevelText.text = $"Доход: ур. {slide.IncomeLevel} (+{slide.GetCurrentIncome():F0}$/тик)";
        tickLevelText.text = $"Скорость: ур. {slide.TickIntervalLevel} ({slide.GetCurrentTickInterval():F1}с)";
        autoCollectStatusText.text = slide.HasAutoCollect ? "Автосбор: ВКЛ ✓" : "Автосбор: ВЫКЛ";
        upgradeIncomeCostText.text = maxIncome ? "МАКС" : $"{slide.GetIncomeUpgradeCost()}$";
        upgradeTickCostText.text = maxTick ? "МАКС" : $"{slide.GetTickUpgradeCost()}$";
        autoCollectCostText.text = slide.HasAutoCollect ? "Куплено" : $"{slide.Config.upgradeData.autoCollectCost}$";
        upgradeIncomeButton.interactable = !maxIncome;
        upgradeTickButton.interactable = !maxTick;
        buyAutoCollectButton.interactable = !slide.HasAutoCollect;
        collectButton.gameObject.SetActive(!slide.HasAutoCollect);
    }
}