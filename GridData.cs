using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Grid/GridData")]
public class GridData : ScriptableObject
{
    [SerializeField] private int _cellCount;
    public int CellCount => _cellCount;
}
