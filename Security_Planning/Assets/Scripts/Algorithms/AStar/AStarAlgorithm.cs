using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using Debug = UnityEngine.Debug;

public static class AStarAlgorithm
{
    public static Path<TNode, TEdge> AStar<TNode, TEdge>(TNode startNode, TNode endNode,
        Heuristics<TNode> heuristics, Func<TNode, bool> nodeFilter = null,
        Func<TEdge, bool> edgeFilter = null, Func<TEdge, float> computeCost = null)
        where TNode : IAStarNode<TNode>
        where TEdge : IAStarEdge<TNode>
    {
        var closedSet = new List<TNode>();
        var openSet = new List<TNode>
        {
            startNode 
        };

        // Key - where I came, Value-First - from where I came, Value-Second - which edge I used.
        var cameFrom = new Dictionary<TNode, Tuple<TNode, TEdge>>();
        
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
            //Debug.Log("AStar current node = " + currentNode);

            if (currentNode.Equals(endNode))
            {
                return ReconstructPath(cameFrom, endNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (TEdge edge in currentNode.Edges)
            {
                //Debug.Log("AStar current edge = " + edge);
                TNode neighbor = edge.Neighbor;
                if (closedSet.Contains(neighbor) || 
                    (nodeFilter != null && nodeFilter(neighbor)) || 
                    (edgeFilter != null && edgeFilter(edge)))
                    continue;
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }

                float cost = computeCost == null ? edge.Cost : computeCost(edge);
                float potentialGScore = gScores[currentNode] + cost;
                if (potentialGScore < gScores[neighbor])
                {
                    cameFrom[neighbor] = new Tuple<TNode, TEdge>(currentNode, edge);
                    gScores[neighbor] = potentialGScore;
                    fScores[neighbor] = potentialGScore + heuristics.ComputeHeuristics(neighbor, endNode);
                }
            }
        }
        return new Path<TNode, TEdge>(null, float.MaxValue);
    }

    private static Path<TNode, TEdge> ReconstructPath<TNode, TEdge>(Dictionary<TNode, Tuple<TNode, TEdge>> pathDictionary, TNode endNode)
        where TNode : IAStarNode<TNode>
        where TEdge : IAStarEdge<TNode>
    {
        var result = new Path<TNode, TEdge>();
        TNode current = endNode;
        while (pathDictionary.Keys.Contains(current))
        {
            Tuple<TNode, TEdge> currentTuple = pathDictionary[current];
            result.AddEdgeToBeginning(currentTuple.Second);
            current = currentTuple.First;
        }
        return result;
    }
}
