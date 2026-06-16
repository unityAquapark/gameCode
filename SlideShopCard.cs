using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlideShopCard : MonoBehaviour
{
    [Header("UI карточки")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _incomeText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _lockedText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _slideIcon;

    private SlideConfig _config;
    private SlideShop _shop;
    private static readonly System.Collections.Generic.Dictionary<string, float> UnlockThresholds
        = new System.Collections.Generic.Dictionary<string, float>
    {
        { "Турбо горка",  1500f   },
        { "Волна",        10000f  },
        { "Экстрим",      50000f  },
        { "Легенда",      250000f }
    };

    public void Setup(SlideConfig config, SlideShop shop)
    {
        _config = config;
        _shop = shop;
        _buyButton.onClick.AddListener(() => _shop.SelectSlide(_config));
        Refresh();
    }

    public void Refresh()
    {
        _nameText.text = _config.slideName;
        _incomeText.text = $"Доход:\n   мин.  {_config.baseIncomePerTick} $/тик\n   макс. {_config.maxIncomePerTick} $/тик";
        _costText.text = $"Стоимость: {_config.buildCost}$";
        if (_slideIcon != null && _config.slideIcon != null)
            _slideIcon.sprite = _config.slideIcon;
        var unlocked = IsUnlocked();
        var canAfford = GameEconomy.Instance.TotalBalance >= _config.buildCost;
        _buyButton.interactable = unlocked && canAfford;
        if (!unlocked)
        {
            float threshold = GetUnlockThreshold();
            _lockedText.gameObject.SetActive(true);
            _lockedText.text = $"Нужно {threshold}$";
        }
        else if (!canAfford)
        {
            _lockedText.gameObject.SetActive(true);
            _lockedText.text = "Недостаточно денег";
        }
        else
        {
            _lockedText.gameObject.SetActive(false);
        }
    }

    private bool IsUnlocked()
    {
        var threshold = GetUnlockThreshold();
        return GameEconomy.Instance.TotalBalance >= threshold ||
               !UnlockThresholds.ContainsKey(_config.slideName);
    }

    private float GetUnlockThreshold()
    {
        return UnlockThresholds.TryGetValue(_config.slideName, out float t) ? t : 0f;
    }
}