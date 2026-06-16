using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SlideClickHandler : MonoBehaviour
{
    private CellSlot _parentCell;

    public void Setup(CellSlot cell)
    {
        _parentCell = cell;
    }

    private void OnMouseDown()
    {
        if (_parentCell != null)
            _parentCell.Grid.NotifyCellClicked(_parentCell);
    }
}