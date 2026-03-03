using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {

    [FormerlySerializedAs("gridGenerator")]
    [Header("Managers")]
    public GridManager gridManager;
    public NumberManager numberManager;
    public LevelLoader levelLoader;
    public NumberPlacementTracker numberPlacementTracker;
    public SolutionCheck solutionCheck;

    [Header("UI")]
    public Button resetButton;
    public Button backButton;
    public Button checkButton;
    public Button showSolutionButton;

    public TextMeshProUGUI levelInfoText;
    public TextMeshProUGUI totalMinesText;

    public TextMeshProUGUI resultText;
    public TextMeshProUGUI mineCountText;

    [Header("Level Settings")]
    public string currentLevelFile = "level_1";
    public int currentLevelIndex = 1;
    public bool useResources = true; // true for Resources, false for StreamingAssets

    private LevelData currentLevelData;
    private int totalMines;
    private int currentMineCount;

    void Start() {
        if (resetButton != null) {
            resetButton.onClick.AddListener(OnResetButtonClick);
        }

        if (backButton != null) {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        if (checkButton != null) {
            checkButton.onClick.AddListener(CheckSolution);
            checkButton.gameObject.SetActive(false);
        }

        if (showSolutionButton != null) {
            showSolutionButton.gameObject.SetActive(false);
        }

        // Subscribe to grid generation event
        if (gridManager != null)
            gridManager.OnGridGenerated += OnGridGenerated;


        if (levelLoader == null) {
            levelLoader = GetComponent<LevelLoader>();

            if (levelLoader == null) {
                Debug.LogError("LevelLoader is not assigned!");
            }
        }

        if (solutionCheck == null) {
            solutionCheck = GetComponent<SolutionCheck>();
        }

        LoadLevel(currentLevelFile);
    }

    void LoadLevel(string fileName) {
        Debug.Log($"Loading level: {fileName}");

        // Load level data
        if (useResources) {
            currentLevelData = levelLoader.LoadLevelFromResources(fileName);
        }
        else {
            currentLevelData = levelLoader.LoadLevelFromStreamingAssets(fileName + ".json");
        }

        if (currentLevelData == null) {
            Debug.LogError("Failed to load level!");

            return;
        }

        // Generate grid from level data
        if (gridManager != null) {
            gridManager.GenerateGridFromLevel(currentLevelData, levelLoader);
        }

        // Setup numbers
        if (numberManager != null && currentLevelData.availableNumbers != null) {
            numberManager.SetupNumbers(currentLevelData.availableNumbers);
        }
        
        if (numberPlacementTracker != null)
            numberPlacementTracker.Initialize(currentLevelData);
    }
    
    void CheckSolution() {
        Debug.Log("Checking solution...");
        
        if (solutionCheck.IsWin(currentLevelData, numberPlacementTracker.numberPlacements)) {
            // TODO: mark one possible solution
            showSolutionButton.gameObject.SetActive(true);
            Debug.Log("恭喜过关！");
        }
        else {
            Debug.Log("答案错误！");
        }
        
    }

    void OnGridGenerated() {
        Debug.Log("Grid generated!");
    }

    public void SetTotalMines(int count) {
        totalMines = count;
        currentMineCount = 0;

        // Update UI
        if (levelInfoText != null) {
            levelInfoText.text = $"Level {currentLevelIndex}";
        }

        if (totalMinesText != null) {
            totalMinesText.text = $"Mines : {totalMines}";
        }
    }
    
    void OnResetButtonClick() {
        if (numberPlacementTracker != null) {
            numberPlacementTracker.Reset();
            Debug.Log("Numbers reset!");
        }
        
        if (numberManager != null) {
            numberManager.ResetAllNumbers();
            Debug.Log("Numbers reset!");
        }

        if (gridManager != null) {
            gridManager.ResetAllCells();
            Debug.Log("Cells reset!");
        }

        if (showSolutionButton != null) {
            showSolutionButton.gameObject.SetActive(false);
        }
    }

    void OnBackButtonClick() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnShowSolutionButtonDown() {
        gridManager.ResetAllCells();
        foreach (var pos in solutionCheck.solutionList) {
            GridCell mineGrid = gridManager.GetCell(pos.x, pos.y);
            mineGrid.MarkAsMine();
        }
    }

    public void OnShowSolutionButtonUp() {
        gridManager.ResetAllCells();
        foreach (var pos in solutionCheck.solutionList) {
            GridCell mineGrid = gridManager.GetCell(pos.x, pos.y);
            mineGrid.ClearMark();
        }
    }

    void OnDestroy() {
        if (gridManager != null)
            gridManager.OnGridGenerated -= OnGridGenerated;
    }

}
