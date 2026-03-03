using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GridManager : MonoBehaviour {

    [Header("Grid Settings")]
    public int rows;
    public int columns;
    public GameObject cellPrefab;

    [Header("UI References")]
    public Transform gridPanel;
    public GridLayoutGroup gridLayout;

    // 2-dimension array
    private GridCell[,] cells;
    public bool isGridGenerated { get; private set; } = false;

    public System.Action OnGridGenerated;
    
    void Start() {
        SetGridLayoutGroup();
        // GenerateGrid();
        
    }

    void GenerateGrid() {
        cells = new GridCell[rows, columns];

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < columns; col++) {
                GameObject newCell = Instantiate(cellPrefab, gridPanel);

                GridCell gridCell = newCell.GetComponent<GridCell>();

                if (gridCell == null) {
                    gridCell = newCell.AddComponent<GridCell>();
                }

                gridCell.backgroundImage = newCell.GetComponent<Image>();

                gridCell.Init(row, col);

                cells[row, col] = gridCell;
            }
        }

        isGridGenerated = true;
        OnGridGenerated?.Invoke();
        Debug.Log($"cells size:{cells.Length}");
    }

    void SetGridLayoutGroup() {
        if (gridLayout != null) {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;

            RectTransform panelRect = gridPanel.GetComponent<RectTransform>();

            float panelWidth = (columns * gridLayout.cellSize.x)
                               + ((columns - 1) * gridLayout.spacing.x)
                               + gridLayout.padding.left
                               + gridLayout.padding.right;

            float panelHeight = (rows * gridLayout.cellSize.y)
                                + ((rows - 1) * gridLayout.spacing.y)
                                + gridLayout.padding.top
                                + gridLayout.padding.bottom;

            panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        }
    }

    public GridCell GetCell(int x, int y) {
        if (!isGridGenerated) {
            Debug.Log("Grid has not been generated！");

            return null;
        }

        if (x >= 0 && x < rows && y >= 0 && y < columns) {
            return cells[x, y];
        }

        Debug.LogWarning("[GetCell] Invalid cell position!");

        return null;
    }
    
    public void GenerateGridFromLevel(LevelData levelData, LevelLoader levelLoader) {
        rows = levelData.gridRowCount;
        columns = levelData.gridColCount;

        // Get unknown and mine positions
        HashSet<Position> unknownPositions = levelLoader.GetUnknownPositions(levelData);
        HashSet<Position> minePositions = levelLoader.GetMinePositions(levelData);
        HashSet<Position> numberPositions = levelLoader.GetNumberPositions(levelData);

        // Clear existing grid if any
        if (cells != null) {
            for (int x = 0; x < rows; x++) {
                for (int y = 0; y < columns; y++) {
                    if (cells[x, y] != null)
                        Destroy(cells[x, y].gameObject);
                }
            }
        }

        // Setup grid layout
        if (gridLayout != null) {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
        }

        // Calculate panel size
        RectTransform panelRect = gridPanel.GetComponent<RectTransform>();

        float panelWidth = (columns * gridLayout.cellSize.x)
                           + ((columns - 1) * gridLayout.spacing.x)
                           + gridLayout.padding.left
                           + gridLayout.padding.right;

        float panelHeight = (rows * gridLayout.cellSize.y)
                            + ((rows - 1) * gridLayout.spacing.y)
                            + gridLayout.padding.top
                            + gridLayout.padding.bottom;

        panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        
        cells = new GridCell[rows, columns];

        // Create cells
        for (int x = 0; x < rows; x++) {
            for (int y = 0; y < columns; y++) {
                GameObject newCell = Instantiate(cellPrefab, gridPanel);
                GridCell cell = newCell.GetComponent<GridCell>();

                if (cell == null)
                    cell = newCell.AddComponent<GridCell>();

                cell.Init(x, y);
                
                Position currentPos = new Position(x, y);

                if (unknownPositions.Contains(currentPos)) {
                    cell.SetCanBeMarked(true);
                }
                if (numberPositions.Contains(currentPos)) {
                    cell.SetCanPlaceNumber(true);
                }
                
                // For test: mark mine grid in the beginning
                // if (minePositions.Contains(currentPos)) {
                //     cell.OnLeftClick();
                // }

                cells[x, y] = cell;
            }
        }
        
        // Store total mines for validation
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null) {
            gameManager.SetTotalMines(levelData.totalMines);
        }

        isGridGenerated = true;
        OnGridGenerated?.Invoke();
        
        Debug.Log($"Grid generated from level data: {rows}x{columns}");
    }

    public void ResetAllCells() {
        foreach (GridCell cell in cells) {
            if (cell != null) {
                cell.Reset();
            }
        }
    }
    
    void OnDestroy() {
        StopAllCoroutines();
    }

}
