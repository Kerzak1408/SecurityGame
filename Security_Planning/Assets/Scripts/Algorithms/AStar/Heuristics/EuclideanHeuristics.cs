using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EuclideanHeuristics : Heuristics<TileNode>
{
    private GameObject[,] grid;

    public EuclideanHeuristics(GameObject[,] grid)
    {
        this.grid = grid;
    }

    public override float ComputeHeuristics(TileNode from, TileNode to)
    {
        Vector3 fromPosition = grid.Get(from.Position).transform.position;
        Vector3 toPosition = grid.Get(to.Position).transform.position;
        return Vector3.Distance(fromPosition, toPosition);
    }
}
