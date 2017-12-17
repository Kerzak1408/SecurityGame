using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;
using System;

public static class FloodFillAlgorithm
{
    public static void FloodFill<TCell>(int i, int j, TCell[,] grid, FloodFillResult result,
        Func<TCell[,], IntegerTuple, bool> isTileCertainlyOutside,
        Func<TCell[,], IntegerTuple, IntegerTuple, bool> canGetFromTo)
    {
        FloodFill(i, j, grid, result, new List<IntegerTuple>(), isTileCertainlyOutside, canGetFromTo);
    }

    private static void FloodFill<TCell>(int i, int j, TCell[,] grid, FloodFillResult result,
        List<IntegerTuple> visited,
        Func<TCell[,], IntegerTuple, bool> isTileCertainlyOutside,
        Func<TCell[,], IntegerTuple, IntegerTuple, bool> canGetFromTo)
    {
        Debug.Log("Flood fill step (" + i + ", " + j + ")");
        var currentIndices = new IntegerTuple(i, j);
        visited.Add(currentIndices);
        if (isTileCertainlyOutside(grid, currentIndices))
        {
            result.IsRoom = false;
        }
        result.Coordinates.Add(new IntegerTuple(i, j));

        IntegerTuple[] neighbors =
        {
            new IntegerTuple(i - 1, j),
            new IntegerTuple(i + 1, j),
            new IntegerTuple(i, j - 1),
            new IntegerTuple(i, j + 1)
        };
        foreach (IntegerTuple neighbor in neighbors)
        {
            if (!visited.Contains(neighbor) && canGetFromTo(grid, currentIndices, neighbor))
            {
                FloodFill(neighbor.First, neighbor.Second, grid, result, visited, isTileCertainlyOutside, canGetFromTo);
            }
        }
    }
}
