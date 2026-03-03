using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class NumberPlacement {

    public Position position;
    public int value;

    public NumberPlacement(Position position, int value) {
        this.position = position;
        this.value = value;
    }

}

public class SolutionCheck : MonoBehaviour {

    public List<Position> solutionList = new List<Position>();

    public bool IsWin(LevelData levelData, Dictionary<Position, int> placements) {
        foreach (var pos in placements.Keys) {
            if (!levelData.placeableGrids.Contains(pos)) {
                Debug.LogWarning($"Position ({pos.x}, {pos.y}) is not in placeable list!");

                return false;
            }
        }

        if (placements.Count != levelData.placeableGrids.Count) {
            Debug.LogWarning(
                $"placements incorrect! Expected {levelData.placeableGrids.Count} , actual count: {placements.Count}.");

            return false;
        }

        List<Position> unknownList = new List<Position>();

        foreach (var region in levelData.unknownRegions) {
            unknownList.AddRange(region.ToPositions());
        }

        bool[] mineFlags = new bool[unknownList.Count];

        return DFS(mineFlags, 0, 0, unknownList, placements, levelData.totalMines);
    }

    private bool DFS(bool[] mineFlags, int index, int minesPlaced,
        List<Position> unknownList, Dictionary<Position, int> placements, int totalMines) {
        if (minesPlaced > totalMines) return false;
        if (minesPlaced + (unknownList.Count - index) < totalMines) return false;

        // reach the last possible grid
        if (index == unknownList.Count) {
            return minesPlaced == totalMines &&
                   CheckAllPlacements(mineFlags, unknownList, placements);
        }

        // try not to place a mine in current position
        if (DFS(mineFlags, index + 1, minesPlaced, unknownList, placements, totalMines)) {
            return true;
        }

        // place a mine in current position
        mineFlags[index] = true;
        bool result = DFS(mineFlags, index + 1, minesPlaced + 1, unknownList, placements, totalMines);
        mineFlags[index] = false; // backtrace

        return result;
    }

    private bool CheckAllPlacements(bool[] mineFlags, List<Position> unknownList,
        Dictionary<Position, int> placements) {
        HashSet<Position> mineSet = new HashSet<Position>();

        for (int i = 0; i < mineFlags.Length; i++) {
            if (mineFlags[i]) {
                mineSet.Add(unknownList[i]);
            }
        }

        foreach (var kvp in placements) {
            Position pos = kvp.Key;
            int expectedMines = kvp.Value;
            int actualMines = CountMinesAround(pos, mineSet);

            if (actualMines != expectedMines) {
                return false;
            }
        }

        // Record current solution
        if (solutionList.Count > 0) {
            solutionList.Clear();
        }

        foreach (var minePos in mineSet) {
            Debug.Log($"mine in Grid({minePos.x},{minePos.y})");
            solutionList.Add(minePos);
        }

        return true;
    }

    private int CountMinesAround(Position center, HashSet<Position> mineSet) {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                if (dx == 0 && dy == 0) continue;

                Position neighbor = new Position(center.x + dx, center.y + dy);

                if (mineSet.Contains(neighbor)) {
                    count++;
                }
            }
        }

        return count;
    }

}
