using System;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public float totalBalance;
        public List<SlotSaveData> placedSlides = new List<SlotSaveData>();
    }
}

[Serializable]
public class SlotSaveData
{
    public int gridX;
    public int gridY;
    public string slideConfigName;
    public int incomeLevel;
    public int tickIntervalLevel;
    public bool hasAutoCollect;
    public int totalUpgradeSpent;
    public float slideBalance;
}