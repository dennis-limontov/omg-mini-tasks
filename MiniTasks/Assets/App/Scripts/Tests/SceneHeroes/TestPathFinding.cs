using System.IO;
using App.Scripts.Modules.Grid;
using App.Scripts.Scenes.SceneHeroes.Features.Grid;
using App.Scripts.Scenes.SceneHeroes.Features.Grid.LevelInfo.Serializable;
using App.Scripts.Scenes.SceneHeroes.Features.PathFinding;
using NUnit.Framework;
using UnityEngine;

public class TestPathFinding
{
    private const string PathTest = "Assets/App/Scripts/Tests/SceneHeroes/TestCases/{0}.json";

    [Test]
    [TestCase("levelGridInfo_0")]
    [TestCase("levelGridInfo_1")]
    public void TestPathFindingSimplePasses(string testData)
    {
        var serviceUnitNavigator = new ServiceUnitNavigator();
    
        var testCaseText = File.ReadAllText(string.Format(PathTest, testData));

        var testCase = JsonUtility.FromJson<LevelInfoTarget>(testCaseText);
        
        var grid = new Grid<int>(testCase.gridSize.ToVector2Int());

        foreach (var obstacle in testCase.Obstacles)
        {
            grid[obstacle.Place.ToVector2Int()] = obstacle.ObstacleType;
        }
    
        var path = serviceUnitNavigator.FindPath(testCase.UnitType, testCase.PlaceUnit.ToVector2Int(), 
            testCase.target.ToVector2Int(), grid);

        if (testCase.targetStepCount < 0 && path is null)
        {
            return;   
        }

        Assert.AreEqual(testCase.targetStepCount, path.Count,"step count invalid");
    }


}
