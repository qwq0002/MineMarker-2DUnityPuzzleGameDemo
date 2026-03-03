using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class NumberBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

    [Header("Number Status")]
    public int numberValue;

    [Header("UI References")]
    public TextMeshProUGUI numberText;
    public CanvasGroup canvasGroup;
    public Image backgroundImage;

    public static System.Action<GridCell, int> OnNumberPlaced;
    public static System.Action<GridCell, int> OnNumberRemoved;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private GridCell parentCell;

    private Vector2 originalPosition;
    private Transform originalParent;
    private bool isDragging = false;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (numberText == null)
            numberText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize(int value) {
        numberValue = value;
        parentCell = null;

        if (numberText != null)
            numberText.text = value.ToString();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (isDragging) return;

        // Click to return to hand if on a cell
        if (parentCell != null && eventData.button == PointerEventData.InputButton.Left) {
            Debug.Log($"Click to return number {numberValue} to hand");
            ReturnToHand();
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log($"Start drag number {numberValue}");

        // Store current state
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Move to root canvas for free dragging
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isDragging) return;

        // Follow mouse
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isDragging) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if dropped on a cell
        bool placedOnCell = false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results) {
            GridCell cell = result.gameObject.GetComponent<GridCell>();

            if (cell != null && cell.canPlaceNumber) {
                if (!cell.hasNumber) {
                    PlaceOnCell(cell);
                    placedOnCell = true;

                    break;
                }
                else {
                    // Cell has a number - try to swap
                    ReplaceNumberInCell(cell);
                    placedOnCell = true;

                    break;
                }
            }
        }

        if (!placedOnCell) {
            // Return to hand
            ReturnToHand();
        }

        isDragging = false;
    }

    void PlaceOnCell(GridCell cell) {
        // Set as child of the cell
        transform.SetParent(cell.transform, false);

        PlaceInCellCenter(cell);

        // Clear last parent cell
        if (parentCell != null) {
            parentCell.RemoveNumber();
        }

        parentCell = cell;
        cell.PlaceNumber(this);
        
        OnNumberPlaced?.Invoke(cell, this.numberValue);
    }

    private void PlaceInCellCenter(GridCell cell) {
        RectTransform cellRect = cell.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(
            cellRect.rect.width * 0.5f,
            -cellRect.rect.height * 0.5f
        );
    }

    void ReplaceNumberInCell(GridCell targetCell) {
        NumberBlock targetBlock = targetCell.GetComponentInChildren<NumberBlock>();

        if (targetBlock == null) {
            Debug.LogWarning("Cell has number flag but no NumberBlock found!");

            return;
        }

        Debug.Log($"Swapping number {numberValue} with number {targetBlock.numberValue}");
        
        // If we're dragging from a cell, detach first
        if (parentCell != null) {
            OnNumberRemoved?.Invoke(parentCell, this.numberValue);
            parentCell.RemoveNumber();
        }

        // Remove target number block from its cell
        targetCell.RemoveNumber();
        // Move target block back to hand
        targetBlock.ReturnToHand();

        // Place current block on target cell
        transform.SetParent(targetCell.transform, false);
        parentCell = targetCell;
        PlaceInCellCenter(targetCell);
        targetCell.PlaceNumber(this);

        Debug.Log($"Swap complete: {numberValue} now on ({targetCell.x},{targetCell.y})");
    }

    void ReturnToHand() {
        NumberManager numberManager = FindObjectOfType<NumberManager>();

        if (numberManager != null) {
            transform.SetParent(numberManager.numberContainer, false);
            rectTransform.anchoredPosition = Vector2.zero;

            if (parentCell != null) {
                OnNumberRemoved?.Invoke(parentCell, this.numberValue);
                parentCell.RemoveNumber();
            }

            parentCell = null;
        }
        else {
            // Fallback to original parent
            transform.SetParent(originalParent, false);
            rectTransform.anchoredPosition = originalPosition;
        }

        Debug.Log($"Number {numberValue} returned to hand");
    }

    public void Reset() {
        if (canvasGroup != null) {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (parentCell != null) {
            // Return to hand
            ReturnToHand();
        }

        isDragging = false;
        parentCell = null;
    }

}
