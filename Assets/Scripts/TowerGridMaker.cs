using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TowerGridMaker : MonoBehaviour
{
    [Header("Griglia")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    [Header("Griglia corrente")]
    public GridData CurrentData;
    
    [Header("Editor Brush")]
    public CellType brushType = CellType.Road;

    [Header("Impostazioni manuali")]
    public List<Vector2Int> roadCells = new();
    public List<Vector2Int> treeCells = new();


    [SerializeField, HideInInspector] private GridData _previousData = null;
    [SerializeField, HideInInspector] private string _previousSaveFolder = string.Empty;

    public static string PreviousSaveFolder = string.Empty;

    public static readonly string DefaultGridDataName = "NewGridData";

    private CellType[,] grid;

    private void OnValidate()
    {
        if (CurrentData)
        {
            // if previous data is set and different from current data
            if (_previousData)
            {
                if (_previousData != CurrentData)
                {
                    _previousData = CurrentData;
                    LoadData();
                }
                else
                {
                    // Update CurrentData with the new values from the user
                    CurrentData.Initialize(width, height, cellSize, roadCells, treeCells);
                }
            }
            else
            {
                if (_previousData != CurrentData) _previousData = CurrentData;
                LoadData();
            }
        }
        InitGrid();
    }

    private void LoadData()
    {
        width = CurrentData.Width;
        height = CurrentData.Height;
        cellSize = CurrentData.CellSize;
        roadCells = new List<Vector2Int>(CurrentData.RoadCells);
        treeCells = new List<Vector2Int>(CurrentData.TreeCells);
    }

    public void UnloadData()
    {
        if (!CurrentData) return;
    }

    private void InitGrid()
    {
        grid = new CellType[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = CellType.Empty;

        foreach (var c in roadCells)
            if (InBounds(c)) grid[c.x, c.y] = CellType.Road;

        foreach (var c in treeCells)
            if (InBounds(c)) grid[c.x, c.y] = CellType.Tree;
    }

    private bool InBounds(Vector2Int c) =>
        c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - transform.position.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public void ToggleCell(Vector2Int cell)
    {
        if (!InBounds(cell)) return;

        switch (brushType)
        {
            case CellType.Road:
                if (roadCells.Contains(cell)) roadCells.Remove(cell);
                else
                {
                    // no overlaps
                    if (treeCells.Contains(cell)) treeCells.Remove(cell);
                    roadCells.Add(cell);
                }
                break;

            case CellType.Tree:
                if (treeCells.Contains(cell)) treeCells.Remove(cell);
                else
                {
                    // no overlaps
                    if (roadCells.Contains(cell)) roadCells.Remove(cell);
                    treeCells.Add(cell);
                }
                break;
            case CellType.Empty:
                if (roadCells.Contains(cell)) roadCells.Remove(cell);
                if (treeCells.Contains(cell)) treeCells.Remove(cell);
                break;
        }

        OnValidate();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // avoid drawing the grid if not selected in the hierarchy
        if (Selection.activeGameObject != gameObject) return;
#endif

        if (grid == null) InitGrid();

        try
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellCenter = transform.position + new Vector3(x + 0.5f, y + 0.5f) * cellSize;
                    switch (grid[x, y])
                    {
                        case CellType.Empty: Gizmos.color = Color.green; break;
                        case CellType.Road: Gizmos.color = Color.gray; break;
                        case CellType.Tree: Gizmos.color = new Color(0.4f, 0.2f, 0f); break;
                        case CellType.Tower: Gizmos.color = Color.blue; break;
                    }
                    Gizmos.DrawWireCube(cellCenter, Vector3.one * (cellSize * 0.9f));
                }
        }
        catch (System.IndexOutOfRangeException)
        {
            // avoid errors for grid changing while drawing
        }

    }
}
