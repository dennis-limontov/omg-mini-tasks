using App.Scripts.Modules.Grid;
using UnityEngine;

namespace App.Scripts.Scenes.SceneMatrix.Features.FigureRotator.Services
{
    public class FigureRotatorDummy : IFigureRotator
    {
        public Grid<bool> RotateFigure(Grid<bool> grid, int rotateCount)
        {
            Grid<bool> localGrid;
            Grid<bool> rotatedGrid = grid;
            for (int k = 0; k < Mathf.Abs(rotateCount % 4); k++)
            {
                localGrid = new Grid<bool>(new Vector2Int(rotatedGrid.Height, rotatedGrid.Width));
                for (int i = 0; i < rotatedGrid.Width; i++)
                {
                    for (int j = 0; j < rotatedGrid.Height; j++)
                    {
                        if (rotateCount > 0)
                        {
                            localGrid[j, rotatedGrid.Width - 1 - i] = rotatedGrid[i, j];
                        }
                        else
                        {
                            localGrid[rotatedGrid.Height - 1 - j, i] = rotatedGrid[i, j];
                        }
                    }
                }
                rotatedGrid = localGrid;
            }
            
            return rotatedGrid;
        }
    }
}