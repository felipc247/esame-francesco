using DesignPatterns.Generics;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CellType { Empty, Road, Tree, Tower }

public class TowerGridManager : Singleton<TowerGridManager>
{
    public GridData CurrentGridData;

    [Header("Griglia")]
    public int Width => (CurrentGridData) ? CurrentGridData.Width : 0;
    public int Height => (CurrentGridData) ? CurrentGridData.Height : 0;
    public float CellSize => CurrentGridData ? CurrentGridData.CellSize : 1f;

    [Header("Impostazioni manuali")]
    public List<Vector2Int> RoadCells => (CurrentGridData) ? CurrentGridData.RoadCells : new List<Vector2Int>();
    public List<Vector2Int> TreeCells => (CurrentGridData) ? CurrentGridData.TreeCells : new List<Vector2Int>();

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
        grid = new CellType[Width, Height];

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                grid[x, y] = CellType.Empty;

        foreach (var c in RoadCells)
            if (InBounds(c)) grid[c.x, c.y] = CellType.Road;

        foreach (var c in TreeCells)
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

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchWorldPos = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2Int cell = WorldToCell(touchWorldPos);
            TryPlaceTower(cell);
        }
    }

    private void CreateVisualGrid()
    {
        ClearVisualGrid();

        gridVisual = new GameObject[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 cellPos = transform.position + new Vector3(x * CellSize, y * CellSize);
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

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
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
            Vector3 spawnPos = transform.position + new Vector3(cell.x * CellSize + CellSize / 2f, cell.y * CellSize + CellSize / 2f);
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
        c.x >= 0 && c.x < Width && c.y >= 0 && c.y < Height;

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - transform.position.x) / CellSize);
        int y = Mathf.FloorToInt((worldPos.y - transform.position.y) / CellSize);
        return new Vector2Int(x, y);
    }

    private bool wasFocused;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // avoid drawing the grid if not selected in the hierarchy
        if (Selection.activeGameObject != gameObject)
        {
            wasFocused = false;
            return;
        }
        else
        {
            // if it gained focus update the grid, in case it was modified
            if (!wasFocused)
            {
                InitGrid();
            }
            wasFocused = true;
        }

#endif
        if (grid == null) InitGrid();

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                Vector3 cellCenter = transform.position + new Vector3(x + 0.5f, y + 0.5f) * CellSize;
                switch (grid[x, y])
                {
                    case CellType.Empty: Gizmos.color = Color.green; break;
                    case CellType.Road: Gizmos.color = Color.gray; break;
                    case CellType.Tree: Gizmos.color = new Color(0.4f, 0.2f, 0f); break;
                    case CellType.Tower: Gizmos.color = Color.blue; break;
                }
                Gizmos.DrawWireCube(cellCenter, Vector3.one * (CellSize * 0.9f));
            }
    }
}
