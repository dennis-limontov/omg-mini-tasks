using System.Collections.Generic;
using App.Scripts.Modules.Grid;
using App.Scripts.Scenes.SceneHeroes.Features.Grid;
using App.Scripts.Scenes.SceneHeroes.Features.Grid.LevelInfo.Config;
using App.Scripts.Scenes.SceneHeroes.Features.Grid.LevelInfo.Serializable;
using UnityEngine;

namespace App.Scripts.Scenes.SceneHeroes.Features.PathFinding
{
    public class ServiceUnitNavigator : IServiceUnitNavigator
    {
        private readonly Dictionary<UnitType, DirectionInfo[]> directions = new Dictionary<UnitType, DirectionInfo[]>
        {
            {
                UnitType.SwordMan, new DirectionInfo[]
                {
                    new DirectionInfo(new Vector2Int(-1, 0), 1),
                    new DirectionInfo(new Vector2Int(0, 1), 1),
                    new DirectionInfo(new Vector2Int(1, 0), 1),
                    new DirectionInfo(new Vector2Int(0, -1), 1),
                }
            },
            {
                UnitType.HorseMan, new DirectionInfo[]
                {
                    new DirectionInfo(new Vector2Int(-1, -1)),
                    new DirectionInfo(new Vector2Int(-1, 1)),
                    new DirectionInfo(new Vector2Int(1, 1)),
                    new DirectionInfo(new Vector2Int(1, -1)),
                }
            },
            {
                UnitType.Angel, new DirectionInfo[]
                {
                    new DirectionInfo(new Vector2Int(-1, 0)),
                    new DirectionInfo(new Vector2Int(0, 1)),
                    new DirectionInfo(new Vector2Int(1, 0)),
                    new DirectionInfo(new Vector2Int(0, -1)),
                }
            },
            {
                UnitType.Barbarian, new DirectionInfo[]
                {
                    new DirectionInfo(new Vector2Int(-1, -1)),
                    new DirectionInfo(new Vector2Int(-1, 1)),
                    new DirectionInfo(new Vector2Int(1, 1)),
                    new DirectionInfo(new Vector2Int(1, -1)),
                    new DirectionInfo(new Vector2Int(-1, 0)),
                    new DirectionInfo(new Vector2Int(0, 1)),
                    new DirectionInfo(new Vector2Int(1, 0)),
                    new DirectionInfo(new Vector2Int(0, -1)),
                }
            },
            {
                UnitType.Poor, new DirectionInfo[]
                {
                    new DirectionInfo(new Vector2Int(0, -1)),
                    new DirectionInfo(new Vector2Int(0, 1)),
                    new DirectionInfo(new Vector2Int(-1, -1), 1),
                    new DirectionInfo(new Vector2Int(-1, 1), 1),
                    new DirectionInfo(new Vector2Int(1, 1), 1),
                    new DirectionInfo(new Vector2Int(1, -1), 1)
                }
            },
            {
                UnitType.Shaman, new DirectionInfo[]
                {
                    new DirectionInfo(new Vector2Int(-2, 1), 1),
                    new DirectionInfo(new Vector2Int(-1, 2), 1),
                    new DirectionInfo(new Vector2Int(1, 2), 1),
                    new DirectionInfo(new Vector2Int(2, 1), 1),
                    new DirectionInfo(new Vector2Int(2, -1), 1),
                    new DirectionInfo(new Vector2Int(1, -2), 1),
                    new DirectionInfo(new Vector2Int(-1, -2), 1),
                    new DirectionInfo(new Vector2Int(-2, -1), 1)
                }
            },
        };

        private void FindSteps(List<Vector2Int> steps, UnitType unitType, Vector2Int current, Grid<int> gridMatrix)
        {
            bool hasPath = true;
            DirectionInfo[] directionsForUnit = directions[unitType];
            foreach (DirectionInfo directionInfo in directionsForUnit)
            {
                directionInfo.FoundObstacle = false;
            }
            steps.Clear();

            for (int i = 1; hasPath; i++)
            {
                hasPath = false;
                foreach (DirectionInfo directionInfo in directionsForUnit)
                {
                    if (directionInfo.FoundObstacle) continue;
                    
                    Vector2Int possibleStep = current + directionInfo.Step * i;
                    if (((directionInfo.CellCount == -1) || (directionInfo.CellCount >= i))
                        && gridMatrix.IsValid(possibleStep)
                        && IsStepPossible(unitType, (CellObstacleType)gridMatrix[possibleStep]))
                    {
                        steps.Add(possibleStep);
                    }
                    else
                    {
                        directionInfo.FoundObstacle = true;
                    }

                    if (!directionInfo.FoundObstacle)
                    {
                        hasPath = true;
                    }
                }
            }
        }

        public List<Vector2Int> FindPath(UnitType unitType, Vector2Int from, Vector2Int to, Grid<int> gridMatrix)
        {
            Queue<PathPoint> way = new Queue<PathPoint>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>() { from };
            PathPoint wayPeek = new PathPoint(from, null);
            PathPoint localPeek;
            Vector2Int wayPeekPoint = wayPeek.Coordinate;
            List<Vector2Int> possibleNextPoints = new List<Vector2Int>();

            while (wayPeekPoint != to)
            {
                FindSteps(possibleNextPoints, unitType, wayPeekPoint, gridMatrix);

                localPeek = wayPeek;
                wayPeek = null;
                foreach (Vector2Int possibleNextPoint in possibleNextPoints)
                {
                    if (!visited.Contains(possibleNextPoint))
                    {
                        PathPoint nextPathPoint = new PathPoint(possibleNextPoint, localPeek);
                        if (possibleNextPoint == to)
                        {
                            wayPeek = nextPathPoint;
                            break;
                        }
                        way.Enqueue(nextPathPoint);
                        visited.Add(nextPathPoint.Coordinate);
                    }
                }

                if (wayPeek == null)
                {
                    if (!way.TryDequeue(out wayPeek)) return null;
                }
                wayPeekPoint = wayPeek.Coordinate;
            }

            List<Vector2Int> finalWay = new List<Vector2Int>();
            while (wayPeek != null)
            {
                finalWay.Insert(0, wayPeek.Coordinate);
                wayPeek = wayPeek.PrevPoint;
            }

            return finalWay;
        }

        private bool IsStepPossible(UnitType unitType, CellObstacleType obstacleType)
        {
            switch (unitType)
            {
                case UnitType.SwordMan:
                case UnitType.HorseMan:
                case UnitType.Poor:
                case UnitType.Shaman:
                    return (obstacleType == CellObstacleType.None);
                case UnitType.Angel:
                    return (obstacleType != CellObstacleType.Stone);
                case UnitType.Barbarian:
                    return (obstacleType != CellObstacleType.Water);
                default:
                    return false;
            }
        }
    }
}