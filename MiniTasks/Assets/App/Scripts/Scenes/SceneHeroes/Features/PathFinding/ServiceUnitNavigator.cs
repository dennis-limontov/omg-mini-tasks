using System;
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
        private Queue<PathPoint> way = new Queue<PathPoint>();
        private List<Vector2Int> visited = new List<Vector2Int>();

        #region Units Possible Steps
        private List<Vector2Int> StepForSwordMan(Vector2Int current)
        {
            return new List<Vector2Int>()
            {
                new Vector2Int(current.x - 1, current.y),
                new Vector2Int(current.x, current.y + 1),
                new Vector2Int(current.x + 1, current.y),
                new Vector2Int(current.x, current.y - 1),
            };
        }

        private List<Vector2Int> StepForHorseMan(Vector2Int current, Grid<int> gridMatrix)
        {
            List<Vector2Int> stepList = new List<Vector2Int>();
            
            int index = 1;
            while ((current.x - index >= 0) && (current.y + index < gridMatrix.Height)
                && IsStepPossible(UnitType.HorseMan, (CellObstacleType)gridMatrix[current.x - index, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x - index, current.y + index));
                index++;
            }
            index = 1;
            while ((current.x + index < gridMatrix.Width) && (current.y + index < gridMatrix.Height)
                && IsStepPossible(UnitType.HorseMan, (CellObstacleType)gridMatrix[current.x + index, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x + index, current.y + index));
                index++;
            }
            index = 1;
            while ((current.x + index < gridMatrix.Width) && (current.y - index >= 0)
                && IsStepPossible(UnitType.HorseMan, (CellObstacleType)gridMatrix[current.x - index, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x + index, current.y - index));
                index++;
            }
            index = 1;
            while ((current.x - index >= 0) && (current.y - index >= 0)
                && IsStepPossible(UnitType.HorseMan, (CellObstacleType)gridMatrix[current.x - index, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x - index, current.y - index));
                index++;
            }

            return stepList;
        }

        private List<Vector2Int> StepForAngel(Vector2Int current, Grid<int> gridMatrix)
        {
            List<Vector2Int> stepList = new List<Vector2Int>();

            int index = 1;
            while ((current.x - index >= 0) && IsStepPossible(UnitType.Angel, (CellObstacleType)gridMatrix[current.x - index, current.y]))
            {
                stepList.Add(new Vector2Int(current.x - index, current.y));
                index++;
            }
            index = 1;
            while ((current.y + index < gridMatrix.Height) && IsStepPossible(UnitType.Angel, (CellObstacleType)gridMatrix[current.x, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x, current.y + index));
                index++;
            }
            index = 1;
            while ((current.x + index < gridMatrix.Width) && IsStepPossible(UnitType.Angel, (CellObstacleType)gridMatrix[current.x + index, current.y]))
            {
                stepList.Add(new Vector2Int(current.x + index, current.y));
                index++;
            }
            index = 1;
            while ((current.y - index >= 0) && IsStepPossible(UnitType.Angel, (CellObstacleType)gridMatrix[current.x, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x, current.y - index));
                index++;
            }

            return stepList;
        }

        private List<Vector2Int> StepForBarbarian(Vector2Int current, Grid<int> gridMatrix)
        {
            List<Vector2Int> stepList = new List<Vector2Int>();

            int index = 1;
            while ((current.x - index >= 0) && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x - index, current.y]))
            {
                stepList.Add(new Vector2Int(current.x - index, current.y));
                index++;
            }
            index = 1;
            while ((current.x - index >= 0) && (current.y + index < gridMatrix.Height)
                && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x - index, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x - index, current.y + index));
                index++;
            }
            index = 1;
            while ((current.y + index < gridMatrix.Height) && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x, current.y + index));
                index++;
            }
            index = 1;
            while ((current.x + index < gridMatrix.Width) && (current.y + index < gridMatrix.Height)
                && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x + index, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x + index, current.y + index));
                index++;
            }
            index = 1;
            while ((current.x + index < gridMatrix.Width) && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x + index, current.y]))
            {
                stepList.Add(new Vector2Int(current.x + index, current.y));
                index++;
            }
            index = 1;
            while ((current.x + index < gridMatrix.Width) && (current.y - index >= 0)
                && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x + index, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x + index, current.y - index));
                index++;
            }
            index = 1;
            while ((current.y - index >= 0) && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x, current.y - index));
                index++;
            }
            index = 1;
            while ((current.x - index >= 0) && (current.y - index >= 0)
                && IsStepPossible(UnitType.Barbarian, (CellObstacleType)gridMatrix[current.x - index, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x - index, current.y - index));
                index++;
            }
            
            return stepList;
        }

        private List<Vector2Int> StepForPoor(Vector2Int current, Grid<int> gridMatrix)
        {
            List<Vector2Int> stepList = new List<Vector2Int>
            {
                new Vector2Int(current.x - 1, current.y + 1),
                new Vector2Int(current.x + 1, current.y + 1),
                new Vector2Int(current.x + 1, current.y - 1),
                new Vector2Int(current.x - 1, current.y - 1)
            };

            int index = 1;
            while ((current.y + index < gridMatrix.Height) && IsStepPossible(UnitType.Poor, (CellObstacleType)gridMatrix[current.x, current.y + index]))
            {
                stepList.Add(new Vector2Int(current.x, current.y + index));
                index++;
            }
            index = 1;
            while ((current.y - index >= 0) && IsStepPossible(UnitType.Poor, (CellObstacleType)gridMatrix[current.x, current.y - index]))
            {
                stepList.Add(new Vector2Int(current.x, current.y - index));
                index++;
            }

            return stepList;
        }

        private List<Vector2Int> StepForShaman(Vector2Int current)
        {
            return new List<Vector2Int>()
            {
                new Vector2Int(current.x - 2, current.y + 1),
                new Vector2Int(current.x - 1, current.y + 2),
                new Vector2Int(current.x + 1, current.y + 2),
                new Vector2Int(current.x + 2, current.y + 1),
                new Vector2Int(current.x + 2, current.y - 1),
                new Vector2Int(current.x + 1, current.y - 2),
                new Vector2Int(current.x - 1, current.y - 2),
                new Vector2Int(current.x - 2, current.y - 1)
            };
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
        #endregion

        public List<Vector2Int> FindPath(UnitType unitType, Vector2Int from, Vector2Int to, Grid<int> gridMatrix)
        {
            PathPoint wayPeek = new PathPoint(from, null);
            PathPoint localPeek = null;
            Vector2Int wayPeekPoint = wayPeek.Coordinate;
            List<Vector2Int> possibleNextPoints = null;
            way.Clear();
            visited.Clear();
            visited.Add(from);

            while (wayPeekPoint != to)
            {
                switch (unitType)
                {
                    case UnitType.SwordMan:
                        possibleNextPoints = StepForSwordMan(wayPeekPoint);
                        break;
                    case UnitType.HorseMan:
                        possibleNextPoints = StepForHorseMan(wayPeekPoint, gridMatrix);
                        break;
                    case UnitType.Angel:
                        possibleNextPoints = StepForAngel(wayPeekPoint, gridMatrix);
                        break;
                    case UnitType.Barbarian:
                        possibleNextPoints = StepForBarbarian(wayPeekPoint, gridMatrix);
                        break;
                    case UnitType.Poor:
                        possibleNextPoints = StepForPoor(wayPeekPoint, gridMatrix);
                        break;
                    case UnitType.Shaman:
                        possibleNextPoints = StepForShaman(wayPeekPoint);
                        break;
                    default:
                        return null;
                }

                localPeek = wayPeek;
                wayPeek = null;
                for (int j = 0; j < possibleNextPoints.Count; j++)
                {
                    if ((possibleNextPoints[j].x >= 0) && (possibleNextPoints[j].x < gridMatrix.Width)
                        && (possibleNextPoints[j].y >= 0) && (possibleNextPoints[j].y < gridMatrix.Height)
                        && IsStepPossible(unitType, (CellObstacleType)gridMatrix[possibleNextPoints[j]])
                        && !visited.Contains(possibleNextPoints[j]))
                    {
                        PathPoint nextPathPoint = new PathPoint(possibleNextPoints[j], localPeek);
                        if (possibleNextPoints[j] == to)
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
    }
}