using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LevelLoader : MonoBehaviour {

    [Header("Settings")]
    public string levelsFolder = "Levels";

    // Load level from Resources folder
    public LevelData LoadLevelFromResources(string fileName) {
        string path = $"{levelsFolder}/{fileName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null) {
            Debug.LogError($"Cannot find level file: {path}");

            return null;
        }

        return ParseJson(jsonFile.text);
    }

    // Load level from StreamingAssets (for external files)
    public LevelData LoadLevelFromStreamingAssets(string fileName) {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(path)) {
            Debug.LogError($"Cannot find level file: {path}");

            return null;
        }

        string jsonContent = File.ReadAllText(path);

        return ParseJson(jsonContent);
    }

    // Parse JSON string to LevelData
    LevelData ParseJson(string jsonString) {
        try {
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonString);
            // Debug.Log($"Level loaded: {levelData.gridRowCount}x{levelData.gridColCount}");
    
            return levelData;
        }
        catch (System.Exception e) {
            Debug.LogError($"Failed to parse JSON: {e.Message}");
    
            return null;
        }
    }
    

    // Helper method to get all unknown grid positions from regions
    public HashSet<Position> GetUnknownPositions(LevelData levelData) {
        HashSet<Position> unknownSet = new HashSet<Position>();

        if (levelData.unknownRegions != null) {
            foreach (var region in levelData.unknownRegions) {
                foreach (var pos in region.ToPositions()) {
                    unknownSet.Add(pos);
                }
            }
        }

        return unknownSet;
    }

    // Helper method to get all mine positions
    public HashSet<Position> GetMinePositions(LevelData levelData) {
        HashSet<Position> mineSet = new HashSet<Position>();

        if (levelData.mineGrids != null) {
            foreach (var pos in levelData.mineGrids) {
                mineSet.Add(pos);
            }
        }

        return mineSet;
    }

    public HashSet<Position> GetNumberPositions(LevelData levelData) {
        HashSet<Position> numberSet = new HashSet<Position>();

        if (levelData.placeableGrids != null) {
            foreach (var pos in levelData.placeableGrids) {
                numberSet.Add(pos);
            }
        }

        return numberSet;
    }

}
