using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;

public class EuclideanHeuristics<TNode> : Heuristics<TNode> where TNode : IAStarNode<TNode>
{
    private GameObject[,] grid;
    private int pathLengthIndex;

    public EuclideanHeuristics(GameObject[,] grid, int pathLengthIndex)
    {
        this.grid = grid;
        this.pathLengthIndex = pathLengthIndex;
    }

    public override PriorityCost ComputeHeuristics(TNode from, TNode to, int priorityCostLength)
    {
        Vector3 fromPosition = new Vector3(from.Position.First, from.Position.Second);
        Vector3 toPosition = new Vector3(to.Position.First, to.Position.Second);
        var result = new PriorityCost(priorityCostLength);
        result[pathLengthIndex] = Vector3.Distance(fromPosition, toPosition);
        return result;
    }
}
