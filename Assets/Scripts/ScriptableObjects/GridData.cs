using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GridData : ScriptableObject
{
    [Header("Griglia")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    [Header("Impostazioni manuali")]
    [SerializeField] private List<Vector2Int> roadCells = new();
    [SerializeField] private List<Vector2Int> treeCells = new();

    public void Initialize(int width, int height, float cellSize,
        List<Vector2Int> roadCells, List<Vector2Int> treeCells)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.roadCells = new List<Vector2Int>(roadCells);
        this.treeCells = new List<Vector2Int>(treeCells);
    }

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    public List<Vector2Int> RoadCells => roadCells;
    public List<Vector2Int> TreeCells => treeCells;
}
