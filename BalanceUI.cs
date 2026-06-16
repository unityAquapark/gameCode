using UnityEngine;
using TMPro;

public class BalanceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _balanceText;

    private void Start()
    {
        if (GameEconomy.Instance == null)
        {
            Debug.LogError("GameEconomy не найден на сцене!");
            return;
        }
        UpdateBalanceText(GameEconomy.Instance.TotalBalance);
        GameEconomy.Instance.OnBalanceChanged += UpdateBalanceText;
    }

    private void OnDestroy()
    {
        if (GameEconomy.Instance != null)
            GameEconomy.Instance.OnBalanceChanged -= UpdateBalanceText;
    }

    private void UpdateBalanceText(float newBalance)
    {
        _balanceText.text = $"Баланс: {newBalance:F0}$";
    }
}