using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CellClickHandler : MonoBehaviour
{
    private CellSlot _slot;
    private SlidesGrid _grid;

    public void Setup(CellSlot slot, SlidesGrid grid)
    {
        _slot = slot;
        _grid = grid;
    }

    private void OnMouseDown()
    {
        _grid.NotifyCellClicked(_slot);
    }
}