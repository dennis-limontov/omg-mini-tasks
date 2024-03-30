using UnityEngine;

public class DirectionInfo
{
    public Vector2Int Step { get; }
    public int CellCount { get; }
    public bool FoundObstacle { get; set; }

    public DirectionInfo(Vector2Int step, int cellCount)
    {
        Step = step;
        CellCount = cellCount;
        FoundObstacle = false;
    }

    public DirectionInfo(Vector2Int step) : this(step, -1) { }
}