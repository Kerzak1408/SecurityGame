using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EuclideanHeuristics<TNode> : Heuristics<TNode> where TNode : IAStarNode<TNode>
{
    private GameObject[,] grid;

    public EuclideanHeuristics(GameObject[,] grid)
    {
        this.grid = grid;
    }

    public override float ComputeHeuristics(TNode from, TNode to)
    {
        Vector3 fromPosition = new Vector3(from.Position.First, from.Position.Second);
        Vector3 toPosition = new Vector3(to.Position.First, to.Position.Second);
        return Vector3.Distance(fromPosition, toPosition);
    }
}
