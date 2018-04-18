using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;

public class EuclideanHeuristics<TNode> : Heuristics<TNode> where TNode : IAStarNode<TNode>
{
    private GameObject[,] grid;

    public EuclideanHeuristics(GameObject[,] grid)
    {
        this.grid = grid;
    }

    public override PriorityCost ComputeHeuristics(TNode from, TNode to, int priorityCostLength)
    {
        Vector3 fromPosition = new Vector3(from.Position.First, from.Position.Second);
        Vector3 toPosition = new Vector3(to.Position.First, to.Position.Second);
        var result = new PriorityCost(priorityCostLength, Vector3.Distance(fromPosition, toPosition));
        if (priorityCostLength > 1)
        {
            result[1] = 0;
        }
        return result;
    }
}
