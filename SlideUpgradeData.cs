using UnityEngine;

[System.Serializable]
public class UpgradeLevelData
{
    public float value;
    public int cost;
}

[CreateAssetMenu(fileName = "SlideUpgradeData", menuName = "Slides/SlideUpgradeData")]
public class SlideUpgradeData : ScriptableObject
{
    [Header("Максимальный уровень улучшений")]
    public int maxLevel = 5;

    [Header("Доход за тик (по уровням 1-5)")]
    public UpgradeLevelData[] incomeLevels = new UpgradeLevelData[5];

    [Header("Интервал тика в секундах (по уровням 1-5)")]
    public UpgradeLevelData[] tickIntervalLevels = new UpgradeLevelData[5];

    [Header("Автосбор (покупается один раз)")]
    public int autoCollectCost;
}