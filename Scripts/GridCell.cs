using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IPointerClickHandler {

    [Header("Grid Status")]
    public int x;
    public int y;
    public bool canBeMarked;
    public bool hasMine;
    public bool hasX;
    public bool canPlaceNumber;
    public bool hasNumber;

    [Header("UI References")]
    public Image backgroundImage;
    public TextMeshProUGUI gridText;
    public GameObject mineIcon;
    public GameObject xIcon;
    public Outline outline;

    public void Init(int posX, int posY) {
        x = posX;
        y = posY;
        canBeMarked = false;
        canPlaceNumber = false;
        hasMine = false;
        hasNumber = false;
        
        backgroundImage = GetComponent<Image>();
        
        outline = GetComponentInChildren<Outline>();
        gridText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (mineIcon != null) mineIcon.SetActive(false);
        if (xIcon != null) xIcon.SetActive(false);
        if (gridText != null) gridText.text = " ";

        UpdateUI();

        gameObject.name = $"Cell_{x}_{y}";
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            OnLeftClick(); // mark as mine
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            // TODO
        }
    }

    public void OnLeftClick() {
        if (!canBeMarked) {
            Debug.Log($"Grid({x},{y}) can't be marked!");
            return;
        }

        if (!hasMine && !hasX) {
            MarkAsMine();
        }
        else if (hasMine) {
            MarkAsX();
        }
        else {
            ClearMark();
        }

        
    }

    public void MarkAsMine() {
        hasMine = true;
        hasX = false;
        UpdateUI();
    }

    public void MarkAsX() {
        hasMine = false;
        hasX = true;
        UpdateUI();
    }

    public void ClearMark() {
        hasMine = false;
        hasX = false;
        UpdateUI();
    }

    public void SetCanPlaceNumber(bool isNumberGrid = true) {
        canPlaceNumber = isNumberGrid;

        if (canPlaceNumber) {
            canBeMarked = false;
            gridText.text = "?";
            // backgroundImage.color = new Color(1f, 0.95f, 0.7f);
            // outline.effectColor = new Color(0.9f, 0.6f, 0.3f);
        }
    }

    public void SetCanBeMarked(bool isMarkable = true) {
        canBeMarked = isMarkable;

        if (canBeMarked) {
            canPlaceNumber = false;
            backgroundImage.color = new Color(0.8f, 0.8f, 0.8f);
        }
    }
    
    public void PlaceNumber(NumberBlock numberBlock) {
        hasNumber = true;

        UpdateUI();
        Debug.Log($"Grid({x},{y}) is now {numberBlock.numberValue}");
    }

    public void RemoveNumber() {
        hasNumber = false;
        UpdateUI();
    }

    void UpdateUI() {
        if (backgroundImage == null || canPlaceNumber)
            return;

        mineIcon.SetActive(hasMine);
        xIcon.SetActive(hasX);
    }

    public void Reset() {
        hasMine = false;
        hasX = false;
        hasNumber = false;
        
        UpdateUI();
    }

}
