using UnityEngine;

namespace App.Scripts.Scenes.SceneHeroes.Features.PathFinding
{
    public class PathPoint
    {
        public Vector2Int Coordinate { get; set; }

        public PathPoint PrevPoint { get; set; }

        public PathPoint(Vector2Int coordinate, PathPoint prevPoint)
        {
            Coordinate = coordinate;
            PrevPoint = prevPoint;
        }
    }
}