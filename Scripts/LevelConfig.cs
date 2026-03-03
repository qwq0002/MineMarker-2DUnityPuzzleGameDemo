using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NumberConfig
{
    public int value;
    public int count;
}


[Serializable]
public struct Position
{
    public int x;
    public int y;
    
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public override bool Equals(object obj) {
        if (obj is Position other) {
            return x == other.x && y == other.y;
        }
        return false;
    }
    
    public override int GetHashCode() {
        return x * 10000 + y;
    }
}

[Serializable]
public class GridRegion
{
    public int startX;
    public int startY;
    public int endX;
    public int endY;
    
    // Convert region to list of positions
    public List<Position> ToPositions()
    {
        List<Position> positions = new List<Position>();
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                positions.Add(new Position(x, y));
            }
        }
        return positions;
    }
}

[Serializable]
public class LevelData
{
    public int gridRowCount;
    public int gridColCount;
    public int totalMines;
    
    public List<Position> mineGrids;
    public List<GridRegion> unknownRegions;
    public List<Position> placeableGrids;
    public List<NumberConfig> availableNumbers;
}




