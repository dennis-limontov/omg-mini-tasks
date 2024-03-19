using App.Scripts.Modules.Grid;
using System;
using UnityEngine;

namespace App.Scripts.Scenes.SceneMatrix.Features.FigureProvider.Parser
{
    public class ParserFigureDummy : IFigureParser
    {
        public Grid<bool> ParseFile(string text)
        {
            string[] gridInfo = text.Split(Environment.NewLine);

            int height, width;
            if (!int.TryParse(gridInfo[0], out width))
            {
                throw new ExceptionParseFigure("Wrong width format!");
            }
            if (!int.TryParse(gridInfo[1], out height))
            {
                throw new ExceptionParseFigure("Wrong height format!");
            }

            int[] indexes;
            try
            {
                indexes = Array.ConvertAll(gridInfo[2].Split(' '), int.Parse);
            }
            catch (Exception)
            {
                throw new ExceptionParseFigure("Wrong indexes format!");
            }

            var grid = new Grid<bool>(new Vector2Int(width, height));

            for (int i = 0; i < indexes.Length; i++)
            {
                int indexHeight = indexes[i] / width, indexWidth = indexes[i] % width;
                if ((indexHeight < 0) || (indexHeight >= height)
                    || (indexWidth < 0) || (indexWidth >= width))
                {
                    throw new ExceptionParseFigure($"Wrong cell index: {indexes[i]}.");
                }
                grid[indexWidth, indexHeight] = true;
            }

            return grid;
        }
    }
}