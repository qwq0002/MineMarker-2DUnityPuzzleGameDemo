using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NumberPlacementTracker : MonoBehaviour {

    [Header("References")]
    public Button checkButton;
    public Button showSolutionButton;

    public Dictionary<Position, int> numberPlacements = new Dictionary<Position, int>();

    private LevelData currentLevel;
    private int totalPlaceable = 0;

    void Start() {
        NumberBlock.OnNumberPlaced += OnNumberPlaced;
        NumberBlock.OnNumberRemoved += OnNumberRemoved;
    }

    public void Initialize(LevelData levelData) {
        currentLevel = levelData;
        totalPlaceable = levelData.placeableGrids.Count;
        numberPlacements.Clear();

        if (checkButton != null)
            checkButton.gameObject.SetActive(false);
        if (showSolutionButton != null)
            showSolutionButton.gameObject.SetActive(false);
    }

    private void OnNumberPlaced(GridCell cell, int number) {
        Debug.Log($"{number} placed on Grid({cell.x},{cell.y}).");

        if (currentLevel == null) return;

        Position pos = new Position(cell.x, cell.y);
        numberPlacements[pos] = number;
        UpdateButtonState();
    }

    private void OnNumberRemoved(GridCell cell, int number) {
        Debug.Log($"{number} removed from Grid({cell.x},{cell.y}).");

        if (currentLevel == null) return;

        Position pos = new Position(cell.x, cell.y);

        if (numberPlacements.Remove(pos)) {
            UpdateButtonState();
        }
    }

    private void UpdateButtonState() {
        Debug.Log($"[UpdateButtonState] numberPlacements count: {numberPlacements.Count}, totalPlaceable: {totalPlaceable}");

        if (checkButton != null) {
            bool allFilled = (numberPlacements.Count == totalPlaceable);

            if (checkButton.gameObject.activeSelf != allFilled) {
                checkButton.gameObject.SetActive(allFilled);
                Debug.Log($"Placement complete: {allFilled}");
            }

            if (!allFilled) {
                showSolutionButton.GameObject().SetActive(false);
            }
        }
    }

    public void Reset() {
        numberPlacements.Clear();

        if (checkButton != null)
            checkButton.gameObject.SetActive(false);
    }

    void OnDestroy() {
        NumberBlock.OnNumberPlaced -= OnNumberPlaced;
        NumberBlock.OnNumberRemoved -= OnNumberRemoved;
    }

}
