using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NumberManager : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject numberBlockPrefab;

    [Header("UI References")]
    public Transform numberContainer; // NumberContainer

    private Dictionary<int, int> availableNumbers = new Dictionary<int, int>();
    private List<NumberBlock> activeNumberBlocks = new List<NumberBlock>();

    public void SetupNumbers(List<NumberConfig> numberConfigs) {
        ClearNumbers();

        Debug.Log($"Initialize numbers，count: {numberConfigs?.Count ?? 0}");

        if (numberConfigs == null || numberConfigs.Count == 0) {
            Debug.LogError("numberConfigs is null!");

            return;
        }

        foreach (var config in numberConfigs) {
            if (config == null) {
                Debug.LogError("config is null!");

                continue;
            }

            availableNumbers[config.value] = config.count;
            Debug.Log($"generating {config.value}，count: {config.count}");

            for (int i = 0; i < config.count; i++) {
                CreateNumberBlock(config.value);
            }
        }
        
    }

    void CreateNumberBlock(int value) {
        if (numberBlockPrefab == null || numberContainer == null) {
            Debug.LogError("NumberBlockPrefab or NumberContainer is null！");

            return;
        }

        GameObject blockObj = Instantiate(numberBlockPrefab, numberContainer);
        NumberBlock numBlock = blockObj.GetComponent<NumberBlock>();

        if (numBlock != null) {
            numBlock.Initialize(value);
            activeNumberBlocks.Add(numBlock);
        }
    }

    public void OnNumberPlaced(int value) {
        if (availableNumbers.ContainsKey(value)) {
            availableNumbers[value]--;
        }
    }

    public bool HasNumber(int value) {
        return availableNumbers.ContainsKey(value) && availableNumbers[value] > 0;
    }

    void ClearNumbers() {
        Debug.Log("ClearNumbers");

        if (activeNumberBlocks != null) {
            for (int i = activeNumberBlocks.Count - 1; i >= 0; i--) {
                if (activeNumberBlocks[i] != null && activeNumberBlocks[i].gameObject != null) {
                    Destroy(activeNumberBlocks[i].gameObject);
                }
            }

            activeNumberBlocks.Clear();
        }
        else {
            activeNumberBlocks = new List<NumberBlock>();
        }

        if (availableNumbers != null) {
            availableNumbers.Clear();
        }
        else {
            availableNumbers = new Dictionary<int, int>();
        }

        Debug.Log("ResetNumbers is done!");
    }

    public void ResetAllNumbers() {
        foreach (var block in activeNumberBlocks) {
            block.Reset();
        }
    }

}
