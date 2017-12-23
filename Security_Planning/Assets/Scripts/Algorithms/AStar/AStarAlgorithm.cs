using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class AStarAlgorithm
{
    public static List<IAStarNode> AStar(IAStarNode startNode, IAStarNode endNode, Func<IAStarNode, IAStarNode, float> heuristics)
    {
        var closedSet = new List<IAStarNode>();
        var openSet = new SortedList<float, IAStarNode>
        {
            { 0, startNode }
        };

        var cameFrom = new Dictionary<IAStarNode, IAStarNode>();
        
        var gScores = new LazyDictionary<IAStarNode, float>(float.MaxValue);
        gScores[startNode] = 0;

        var fScores = new LazyDictionary<IAStarNode, float>(float.MaxValue);
        gScores[startNode] = heuristics(startNode, endNode);

        while (openSet.Count != 0)
        {
            KeyValuePair<float, IAStarNode> first = openSet.First();
            IAStarNode currentNode = first.Value;

            if (currentNode == endNode)
            {
                return ReconstructPath(cameFrom, endNode);
            }

            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            foreach (IAStarEdge edge in currentNode.Edges)
            {
                IAStarNode neighbor = edge.Neighbor;
                if (closedSet.Contains(neighbor)) continue;

                float potentialGScore = gScores[currentNode] + edge.Cost;
                if (potentialGScore < gScores[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gScores[neighbor] = potentialGScore;
                    fScores[neighbor] = potentialGScore + heuristics(neighbor, endNode);
                }
            }
        }
        return null;
    }

    private static List<IAStarNode> ReconstructPath(Dictionary<IAStarNode, IAStarNode> pathDictionary, IAStarNode endNode)
    {
        var result = new List<IAStarNode>();
        IAStarNode current = endNode;
        while (pathDictionary.Keys.Contains(current))
        {
            result.Insert(0, current);
            current = pathDictionary[current];
        }
        return result;
    }
}
