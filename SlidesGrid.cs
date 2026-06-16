using UnityEngine;
using UnityEngine.Rendering;
using System;

public class SlidesGrid : MonoBehaviour
{
    [SerializeField] private GridData _gridData;
    [SerializeField] private bool _isOn;

    public GridData GridData => _gridData;
    public bool IsOn => _isOn;
    public CellSlot[,] SlidеsGrid { get; private set; }
    public event Action<CellSlot> OnCellClicked;
    void Start()
    {
        if (IsOn)
        {
            Debug.Log($"Координаты сетки: {transform.position}");
            GenerateGrid();
        }
    }

    private void GenerateGrid()
    {
        var spacing = 3.5f;
        var totalSize = (GridData.CellCount + 1) * spacing;
        var startPos = new Vector3(
            transform.position.x - totalSize / 2 + spacing / 2,
            transform.position.y + 0.01f,
            transform.position.z + totalSize / 2 - spacing / 2
        );
        SlidеsGrid = new CellSlot[GridData.CellCount, GridData.CellCount];
        for (var i = 0; i < SlidеsGrid.GetLength(0); i++)
        {
            for (var j = 0; j < SlidеsGrid.GetLength(1); j++)
            {
                var cellGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
                cellGO.name = $"Cell {i} {j}";
                cellGO.tag = "Cell.Free";
                cellGO.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                cellGO.GetComponent<Renderer>().material.color = Color.green;
                cellGO.transform.localScale = new Vector3(0.15f, 1, 0.15f);
                cellGO.transform.SetParent(transform, false);
                var x = startPos.x + i * spacing + spacing / 2;
                var z = startPos.z - j * spacing - spacing / 2;
                cellGO.transform.position = new Vector3(x, startPos.y, z);
                var slot = cellGO.AddComponent<CellSlot>();
                slot.Init(i, j, this);
                var clickHandler = cellGO.AddComponent<CellClickHandler>();
                clickHandler.Setup(slot, this);
                SlidеsGrid[i, j] = slot;
            }
        }
    }

    public void NotifyCellClicked(CellSlot slot)
    {
        OnCellClicked?.Invoke(slot);
    }

    public CellSlot GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= GridData.CellCount || y >= GridData.CellCount)
            return null;

        return SlidеsGrid[x, y];
    }
}