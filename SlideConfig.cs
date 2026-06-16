using UnityEngine;

[CreateAssetMenu(fileName = "SlideConfig", menuName = "Slides/SlideConfig")]
public class SlideConfig : ScriptableObject
{
    [Header("Основные параметры")]
    public string slideName = "Водная горка";
    public Sprite slideIcon;
    public GameObject slidePrefab;

    [Header("Начальные значения")]
    public float baseIncomePerTick;
    public float maxIncomePerTick;
    public float baseTickInterval;
    public float minTickInterval;

    [Header("Стоимость постройки")]
    public int buildCost = 100;

    [Header("Данные улучшений")]
    public SlideUpgradeData upgradeData;
}