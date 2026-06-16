using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopText : MonoBehaviour
{
    [SerializeField] private SlideConfig slideConfig;
    [SerializeField] private TMP_Text slideNameText;
    [SerializeField] private TMP_Text incomePerTickText;
    [SerializeField] private TMP_Text buttonText;

    void Start()
    {
        slideNameText.text = slideConfig.slideName;
        buttonText.text = "Построить \"" + slideConfig.slideName + "\"";
        incomePerTickText.text = "Доход за тик: " + slideConfig.baseIncomePerTick + " / " + slideConfig.maxIncomePerTick;
    }
}
