using DesignPatterns.Generics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerGridManager : Singleton<TowerGridManager>
{
    public enum CellType { Empty, Road, Tree, Tower }

    [Header("Griglia")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    [Header("Impostazioni manuali")]
    public List<Vector2Int> roadCells = new();
    public List<Vector2Int> treeCells = new();

    [Header("Editor Brush")]
    public CellType brushType = CellType.Road;

    [Header("Prefab e Torre")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public Transform towerParent;

    private CellType[,] grid;
    private GameObject[,] gridVisual;

    private TurretButton selectedTurretButton;
    private Camera mainCamera;

    private Dictionary<TurretController, Vector2Int> towersPositions;

    private void OnValidate()
    {
        InitGrid();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        towersPositions = new();
        InitGrid();
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

    private void Update()
    {
        if (selectedTurretButton == null) return;

        UpdateCellHighlights();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int cell = WorldToCell(mouseWorldPos);
            TryPlaceTower(cell);
        }
    }

    private void CreateVisualGrid()
    {
        ClearVisualGrid();

        gridVisual = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellPos = transform.position + new Vector3(x * cellSize, y * cellSize);
                GameObject cell = Instantiate(cellPrefab, cellPos, Quaternion.identity, gridParent);
                gridVisual[x, y] = cell;
            }
        }
    }

    private void ClearVisualGrid()
    {
        if (gridVisual == null) return;

        foreach (var go in gridVisual)
            if (go != null) Destroy(go);

        gridVisual = null;
    }

    private void UpdateCellHighlights()
    {
        if (gridVisual == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var rend = gridVisual[x, y].GetComponentInChildren<SpriteRenderer>();
                if (grid[x, y] == CellType.Empty)
                    rend.color = new Color(0, 1, 0, 0.5f);
                else
                    rend.color = new Color(1, 0, 0, 0.5f);
            }
        }
    }

    public void SelectTurret(TurretButton turretButton)
    {
        selectedTurretButton = turretButton;
        CreateVisualGrid();
    }

    public void CancelTurretSelection()
    {
        selectedTurretButton = null;
        ClearVisualGrid();
    }

    private void TryPlaceTower(Vector2Int cell)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!InBounds(cell)) return;
        if (grid[cell.x, cell.y] != CellType.Empty) return;

        if (GameManager.Instance.SpendCoins(selectedTurretButton.Cost))
        {
            Vector3 spawnPos = transform.position + new Vector3(cell.x * cellSize + cellSize / 2f, cell.y * cellSize + cellSize / 2f);
            TurretController turret = Instantiate(selectedTurretButton.TurretPrefab, spawnPos, Quaternion.identity, towerParent);
            turret.Initialize(selectedTurretButton.BaseTurret);
            grid[cell.x, cell.y] = CellType.Tower;
            towersPositions[turret] = new(cell.x, cell.y);
        }
    }

    public void RemoveTower(TurretController turret)
    {
        if (!towersPositions.ContainsKey(turret)) return;

        Vector2Int position = towersPositions[turret];
        grid[position.x, position.y] = CellType.Empty;
        towersPositions.Remove(turret);
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
                else roadCells.Add(cell);
                break;

            case CellType.Tree:
                if (treeCells.Contains(cell)) treeCells.Remove(cell);
                else treeCells.Add(cell);
                break;
        }

        OnValidate();
    }

    private void OnDrawGizmos()
    {
        if (grid == null) InitGrid();

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
}
