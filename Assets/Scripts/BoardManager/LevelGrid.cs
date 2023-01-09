using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using CC = ComponentCache;

public class LevelGrid : ScriptableObject
{
    public List<List<List<GameObject>>> _levelGrid { get; private set; }

    public static LevelGrid CreateLevelGrid(List<List<List<GameObject>>> levelGrid)
    {
        var levelGridInstance = ScriptableObject.CreateInstance<LevelGrid>();

        levelGridInstance._levelGrid = levelGrid;

        return levelGridInstance;
    }

    public void forEachCell(Func<int, int, List<GameObject>, object> expression)
    {
        for (int i = 0; i < _levelGrid.Count; i++)
        {
            for (int j = 0; j < _levelGrid[i].Count; j++)
            {
                expression(i, j, _levelGrid[i][j]);
            }
        }
    }

    public void rawMoveObject(int newX, int newY, GameObject gameObjectToMove)
    {
        int oldX = CC.GetGP(gameObjectToMove).BoardX;
        int oldY = CC.GetGP(gameObjectToMove).BoardY;
        
        rawMoveObject(oldX, oldY, newX, newY, gameObjectToMove);
    }
    
    public void rawMoveObject(int oldX, int oldY, int newX, int newY, GameObject gameObjectToMove)
    {
        if (oldX == newX && oldY == newY)
        {
            Debug.Log("Moving object to same place, skipping");
            return;
        }

        if (!InBounds(newX, newY))
        {
            Debug.Log("Moving out of bounds! x:" + newX + " y:"+ newY);
            // Don't skip, we should error here.
        }
        _levelGrid[oldX][oldY].Remove(gameObjectToMove);
        _levelGrid[newX][newY].Add(gameObjectToMove);

        CC.GetGP(gameObjectToMove).BoardX = newX;
        CC.GetGP(gameObjectToMove).BoardY = newY;
    }

    public bool IsTileOccupied(int x, int y)
    {
        foreach (var gameObject in _levelGrid[x][y])
        {
            if (CC.GetGP(gameObject).DoesOccupyTile())
            {
                return true;
            }
        }

        return false;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < _levelGrid.Count && y < _levelGrid[x].Count;
    }

    public List<GameObject> GetTile(int x, int y)
    {
        return _levelGrid[x][y];
    }
}
