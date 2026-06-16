using NUnit.Framework.Constraints;
using UnityEngine;

public class CellSlot : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;
    public SlideInstance PlacedSlide { get; private set; } = null;

    public int GridX { get; private set; }
    public int GridY { get; private set; }

    public SlidesGrid Grid { get; private set; }

    public void Init(int x, int y, SlidesGrid grid)
    {
        GridX = x;
        GridY = y;
        Grid = grid;
    }

    public bool TryPlaceSlide(SlideConfig config)
    {
        if (IsOccupied)
        {
            Debug.Log($"Клетка [{GridX},{GridY}] уже занята");
            return false;
        }
        if (!GameEconomy.Instance.TrySpendMoney(config.buildCost))
        {
            Debug.Log("Недостаточно денег для постройки горки");
            return false;
        }
        var spawnPos = transform.position + Vector3.up * 0.01f;
        GameObject slideGO;
        if (config.slidePrefab != null)
        {
            slideGO = Instantiate(config.slidePrefab, spawnPos, Quaternion.identity, transform);
        }
        else
        {
            slideGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slideGO.transform.SetParent(transform);
            slideGO.transform.position = spawnPos;
            slideGO.GetComponent<Renderer>().enabled = false;
        }
        slideGO.name = $"Slide_{config.slideName}_{GridX}_{GridY}";
        PlacedSlide = slideGO.AddComponent<SlideInstance>();
        PlacedSlide.Initialize(config);
        var slideClick = slideGO.AddComponent<SlideClickHandler>();
        slideClick.Setup(this);
        IsOccupied = true;
        GetComponent<Renderer>().material.color = Color.yellow;
        tag = "Cell.Occupied";
        return true;
    }

    public void PlaceSlideAlreadyPaid(SlideConfig config)
    {
        if (IsOccupied) return;
        var spawnPos = transform.position + Vector3.up * 0.001f;
        GameObject slideGO;
        var _3dModel = Instantiate(config.slidePrefab, spawnPos, Quaternion.identity);
        _3dModel.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f);
        slideGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slideGO.transform.SetParent(transform);
        slideGO.transform.position = spawnPos;
        slideGO.name = $"Slide_{config.slideName}_{GridX}_{GridY}";
        PlacedSlide = slideGO.AddComponent<SlideInstance>();
        PlacedSlide._3dModel = _3dModel;
        PlacedSlide.Initialize(config);
        var slideClick = slideGO.AddComponent<SlideClickHandler>();
        slideClick.Setup(this);
        IsOccupied = true;
        GetComponent<Renderer>().material.color = Color.yellow;
        tag = "Cell.Occupied";
        slideGO.GetComponent<Renderer>().enabled = false;
        Debug.Log($"Горка '{config.slideName}' построена на [{GridX},{GridY}]");
    }

    public void RemoveSlide()
    {
        if (!IsOccupied) return;
        var upgradeRefund = Mathf.FloorToInt(PlacedSlide.TotalUpgradeSpent * 0.5f);
        var buildRefund = Mathf.FloorToInt(PlacedSlide.Config.buildCost * 0.5f);
        var totalRefund = upgradeRefund + buildRefund;
        GameEconomy.Instance.AddMoney(totalRefund);
        Debug.Log($"Горка удалена. Возврат: {totalRefund}$ " +
                  $"(постройка: {buildRefund}$ + улучшения: {upgradeRefund}$)");
        Destroy(PlacedSlide._3dModel);
        Destroy(PlacedSlide.gameObject);
        PlacedSlide = null;
        IsOccupied = false;
        GetComponent<Renderer>().material.color = Color.green;
        tag = "Cell.Free";
    }
}