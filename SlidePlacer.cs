using UnityEngine;

public class SlidePlacer : MonoBehaviour
{
    [SerializeField] private SlidesGrid _grid;
    [SerializeField] private SlideUI _slideUI;
    [SerializeField] private GameObject _slideUIPanel;

    private void Start()
    {
        _grid.OnCellClicked += HandleCellClicked;
    }

    private void OnDestroy()
    {
        _grid.OnCellClicked -= HandleCellClicked;
    }

    private void HandleCellClicked(CellSlot slot)
    {
        if (SlideShop.Instance.IsShopOpen) return;
        if (slot.IsOccupied)
        {
            if (!SlideShop.Instance.IsWaitingForCell)
                OpenSlideUI(slot.PlacedSlide);
            return;
        }
        var config = SlideShop.Instance.ConsumePendingConfig();
        if (config != null)
        {
            slot.PlaceSlideAlreadyPaid(config);
        }
        else
        {
            Debug.Log("Сначала выбери горку в магазине");
        }
    }

    private void OpenSlideUI(SlideInstance slide)
    {
        _slideUIPanel.SetActive(true);
        _slideUI.Bind(slide);
    }
}