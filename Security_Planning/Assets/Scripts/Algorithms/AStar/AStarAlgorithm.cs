using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class AStarAlgorithm
{
    public static List<TNode> AStar<TNode>(TNode startNode, TNode endNode, Heuristics<TNode> heuristics, Action<string> log, Func<TNode, bool> nodeFilter)
        where TNode : IAStarNode<TNode>
    {
        var closedSet = new List<TNode>();
        var openSet = new List<TNode>
        {
            startNode 
        };

        var cameFrom = new Dictionary<TNode, TNode>();
        
        var gScores = new LazyDictionary<TNode, float>(float.MaxValue);
        gScores[startNode] = 0;

        var fScores = new LazyDictionary<TNode, float>(float.MaxValue);
        fScores[startNode] = heuristics.ComputeHeuristics(startNode, endNode);

        while (openSet.Count != 0)
        {
            TNode currentNode = openSet.First();
            //log("AStar, exploiting node: " + currentNode);
            float minFValue = fScores[currentNode];
            foreach (TNode node in openSet)
            {
                var fValue = fScores[node];
                if (fValue < minFValue)
                {
                    currentNode = node;
                    minFValue = fValue;
                }
            }

            if (currentNode.Equals(endNode))
            {
                return ReconstructPath(cameFrom, endNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (IAStarEdge<TNode> edge in currentNode.Edges)
            {
                TNode neighbor = edge.Neighbor;
                if (closedSet.Contains(neighbor) || nodeFilter(neighbor)) continue;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }

                float potentialGScore = gScores[currentNode] + edge.Cost;
                if (potentialGScore < gScores[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gScores[neighbor] = potentialGScore;
                    fScores[neighbor] = potentialGScore + heuristics.ComputeHeuristics(neighbor, endNode);
                }
            }
        }
        return null;
    }

    private static List<TNode> ReconstructPath<TNode>(Dictionary<TNode, TNode> pathDictionary, TNode endNode) where TNode : IAStarNode<TNode>
    {
        var result = new List<TNode>();
        TNode current = endNode;
        while (pathDictionary.Keys.Contains(current))
        {
            result.Insert(0, current);
            current = pathDictionary[current];
        }
        result.Insert(0, current);
        return result;
    }
}
